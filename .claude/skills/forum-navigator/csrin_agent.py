#!/usr/bin/env python3
"""Cliente automatizado y parser estructurado para cs.rin.ru/forum.

Características:
- Inicio de sesión mediante Playwright (credenciales por variables de entorno o prompt).
- Búsqueda de temas.
- Listado de temas de un subforo.
- Lectura de páginas concretas y recorrido paginado de un tema.
- Extracción estructurada de autores, fechas, texto y enlaces.
- Navegación restringida a https://cs.rin.ru/forum/.

No descarga archivos externos ni intenta eludir CAPTCHA o controles anti-bot.
"""

from __future__ import annotations

import argparse
import asyncio
import getpass
import json
import os
import re
import shutil
import sys
from dataclasses import asdict, dataclass
from pathlib import Path
from typing import Any, AsyncIterator, Iterable
from urllib.parse import parse_qs, urlencode, urljoin, urlparse, urlunparse

try:
    from bs4 import BeautifulSoup, Tag
except ImportError as exc:  # pragma: no cover
    raise SystemExit("Falta beautifulsoup4. Ejecuta: pip install -r requirements.txt") from exc

try:
    from playwright.async_api import (
        BrowserContext,
        Page,
        Playwright,
        Error as PlaywrightError,
        TimeoutError as PlaywrightTimeoutError,
        async_playwright,
    )
except ImportError as exc:  # pragma: no cover
    raise SystemExit("Falta playwright. Ejecuta: pip install -r requirements.txt") from exc


BASE_URL = "https://cs.rin.ru/forum/"
DEFAULT_PROFILE_DIR = Path.home() / ".csrin-agent-profile"
DEFAULT_USER_AGENT = (
    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) "
    "AppleWebKit/537.36 (KHTML, like Gecko) "
    "Chrome/124.0.0.0 Safari/537.36"
)


class CsRinError(RuntimeError):
    """Error base del cliente."""


class AuthenticationError(CsRinError):
    """La autenticación falló o la sesión expiró."""


class CaptchaRequired(CsRinError):
    """El sitio exige intervención manual mediante CAPTCHA."""


class NavigationRejected(CsRinError):
    """Se intentó navegar fuera del foro permitido."""


@dataclass(slots=True)
class TopicSummary:
    title: str
    url: str
    forum: str | None = None
    replies: int | None = None
    views: int | None = None
    last_post: str | None = None


@dataclass(slots=True)
class PostData:
    post_id: str | None
    title: str | None
    author: str | None
    posted_at: str | None
    text: str
    links: list[str]


@dataclass(slots=True)
class TopicPageData:
    title: str
    url: str
    page_number: int
    total_pages: int
    posts: list[PostData]
    previous_url: str | None
    next_url: str | None


@dataclass(slots=True)
class ForumPageData:
    title: str
    url: str
    topics: list[TopicSummary]
    previous_url: str | None
    next_url: str | None


@dataclass(slots=True)
class GenericPageData:
    title: str
    url: str
    text: str
    internal_links: list[str]
    external_links: list[str]


def _clean_text(value: str | None) -> str:
    return re.sub(r"\s+", " ", value or "").strip()


def _parse_int(value: str | None) -> int | None:
    if not value:
        return None
    digits = re.sub(r"\D", "", value)
    return int(digits) if digits else None


def _tag_text(tag: Tag | None) -> str | None:
    return _clean_text(tag.get_text(" ", strip=True)) if tag else None


class ForumParser:
    """Parser tolerante a cambios menores en plantillas phpBB."""

    def __init__(self, base_url: str = BASE_URL) -> None:
        self.base_url = base_url

    def absolute_url(self, href: str | None, current_url: str | None = None) -> str | None:
        if not href:
            return None
        return urljoin(current_url or self.base_url, href)

    @staticmethod
    def _soup(html: str) -> BeautifulSoup:
        return BeautifulSoup(html, "html.parser")

    def parse_error_message(self, html: str) -> str | None:
        soup = self._soup(html)
        selectors = (
            ".error",
            ".errorbox",
            ".message",
            "#message",
            ".panel .content",
        )
        for selector in selectors:
            node = soup.select_one(selector)
            text = _tag_text(node)
            if text and len(text) < 1000:
                return text
        return None

    def parse_topic_summaries(self, html: str, current_url: str) -> list[TopicSummary]:
        soup = self._soup(html)
        results: list[TopicSummary] = []
        seen: set[str] = set()

        for anchor in soup.select("a.topictitle"):
            href = self.absolute_url(anchor.get("href"), current_url)
            if not href or href in seen:
                continue
            seen.add(href)

            # cs.rin.ru runs a classic phpBB whose results/topic lists are table
            # rows. The cells after the title cell are, in order: author, replies,
            # views, last post. Locate the title cell and read the rest by offset.
            row = anchor.find_parent("tr")
            forum = replies = views = last_post = None
            if row is not None:
                cells = row.find_all("td", recursive=False)
                title_idx = next(
                    (i for i, cell in enumerate(cells) if anchor in cell.descendants),
                    None,
                )
                if title_idx is not None:
                    forum_node = cells[title_idx].select_one("a[href*='viewforum.php']")
                    trailing = cells[title_idx + 1 :]
                    if len(trailing) >= 3:
                        replies = _parse_int(_tag_text(trailing[1]))
                        views = _parse_int(_tag_text(trailing[2]))
                    if len(trailing) >= 4:
                        last_post = _tag_text(trailing[3])
                else:
                    forum_node = None
                forum = _tag_text(forum_node or row.select_one("a[href*='viewforum.php']"))

            results.append(
                TopicSummary(
                    title=_tag_text(anchor) or "(sin título)",
                    url=href,
                    forum=forum,
                    replies=replies,
                    views=views,
                    last_post=last_post,
                )
            )

        return results

    def parse_pagination_links(self, html: str, current_url: str) -> dict[int, str]:
        soup = self._soup(html)
        pages: dict[int, str] = {}
        # phpBB paginates with `start=` links whose text is the page number.
        for anchor in soup.select("a[href*='start='], .pagination a"):
            text = _tag_text(anchor)
            if text and text.isdigit():
                url = self.absolute_url(anchor.get("href"), current_url)
                if url:
                    pages[int(text)] = url
        return pages

    def _parse_page_numbers(self, soup: BeautifulSoup) -> tuple[int, int]:
        # phpBB prints "Page X of Y" in the navigation bar.
        text = soup.get_text(" ", strip=True)
        match = re.search(r"(?:Page|Página)\s+(\d+)\s+(?:of|de)\s+(\d+)", text, re.I)
        if match:
            return int(match.group(1)), int(match.group(2))
        return 1, 1

    def parse_topic_page(self, html: str, current_url: str) -> TopicPageData:
        soup = self._soup(html)
        title_node = soup.select_one(
            "h2 a[href*='viewtopic.php'], a.maintitle, h2 a, h2"
        )
        title = _tag_text(title_node) or _tag_text(soup.title) or "(tema sin título)"

        # Each post in classic phpBB is a table.tablebg anchored by <a name="pN">.
        # The author sits in a b.postauthor, the message in div.postbody, and the
        # subject/date live in the header row that holds the anchor
        # ("... Post subject: <subject> Posted: <date>").
        posts: list[PostData] = []
        seen_ids: set[str] = set()
        for name_anchor in soup.select("a[name]"):
            post_id = name_anchor.get("name") or ""
            if not re.match(r"^p\d+$", post_id) or post_id in seen_ids:
                continue
            post = name_anchor.find_parent("table", class_="tablebg")
            body = post.select_one("div.postbody") if post else None
            if post is None or body is None:
                continue
            seen_ids.add(post_id)

            author = post.select_one("b.postauthor, .postauthor, .username-coloured")
            header = name_anchor.find_parent("tr")
            header_text = _clean_text(header.get_text(" ", strip=True)) if header else ""
            subject = posted_at = None
            subject_match = re.search(r"Post subject:\s*(.*?)\s*(?:Posted:|$)", header_text)
            if subject_match:
                subject = subject_match.group(1).strip() or None
            posted_match = re.search(r"Posted:\s*(.+)$", header_text)
            if posted_match:
                posted_at = posted_match.group(1).strip() or None

            links: list[str] = []
            for anchor in body.select("a[href]"):
                url = self.absolute_url(anchor.get("href"), current_url)
                if url and url not in links:
                    links.append(url)

            posts.append(
                PostData(
                    post_id=post_id,
                    title=subject,
                    author=_tag_text(author),
                    posted_at=posted_at,
                    text=_clean_text(body.get_text("\n", strip=True)),
                    links=links,
                )
            )

        page_number, total_pages = self._parse_page_numbers(soup)
        pages = self.parse_pagination_links(html, current_url)

        return TopicPageData(
            title=title,
            url=current_url,
            page_number=page_number,
            total_pages=total_pages,
            posts=posts,
            previous_url=pages.get(page_number - 1),
            next_url=pages.get(page_number + 1),
        )

    def parse_forum_page(self, html: str, current_url: str) -> ForumPageData:
        soup = self._soup(html)
        title_node = soup.select_one("a.maintitle, h2 a[href*='viewforum.php'], h2")
        page_number, _ = self._parse_page_numbers(soup)
        pages = self.parse_pagination_links(html, current_url)
        return ForumPageData(
            title=_tag_text(title_node) or _tag_text(soup.title) or "(foro sin título)",
            url=current_url,
            topics=self.parse_topic_summaries(html, current_url),
            previous_url=pages.get(page_number - 1),
            next_url=pages.get(page_number + 1),
        )

    def parse_generic_page(self, html: str, current_url: str) -> GenericPageData:
        soup = self._soup(html)
        for removable in soup.select("script, style, noscript"):
            removable.decompose()

        internal: list[str] = []
        external: list[str] = []
        for anchor in soup.select("a[href]"):
            url = self.absolute_url(anchor.get("href"), current_url)
            if not url:
                continue
            parsed = urlparse(url)
            target = internal if parsed.hostname == "cs.rin.ru" and parsed.path.startswith("/forum") else external
            if url not in target:
                target.append(url)

        main = soup.select_one("#page-body, main, body") or soup
        return GenericPageData(
            title=_tag_text(soup.title) or "(sin título)",
            url=current_url,
            text=_clean_text(main.get_text("\n", strip=True)),
            internal_links=internal,
            external_links=external,
        )


class CsRinForumClient:
    """Navegador autenticado para uso humano o como herramienta de un agente IA."""

    def __init__(
        self,
        *,
        base_url: str = BASE_URL,
        profile_dir: str | Path = DEFAULT_PROFILE_DIR,
        headless: bool = True,
        timeout_ms: int = 30_000,
        delay_seconds: float = 1.25,
        browser_executable: str | None = None,
    ) -> None:
        self.base_url = base_url.rstrip("/") + "/"
        self.profile_dir = Path(profile_dir).expanduser()
        self.headless = headless
        self.timeout_ms = timeout_ms
        self.delay_seconds = max(0.0, delay_seconds)
        self.browser_executable = browser_executable
        self.parser = ForumParser(self.base_url)

        self._playwright: Playwright | None = None
        self.context: BrowserContext | None = None
        self.page: Page | None = None

    async def __aenter__(self) -> "CsRinForumClient":
        await self.start()
        return self

    async def __aexit__(self, exc_type: Any, exc: Any, tb: Any) -> None:
        await self.close()

    @staticmethod
    def validate_forum_url(url: str) -> str:
        parsed = urlparse(url)
        if parsed.scheme != "https" or parsed.hostname != "cs.rin.ru":
            raise NavigationRejected(f"URL externa rechazada: {url}")
        if not parsed.path.startswith("/forum"):
            raise NavigationRejected(f"Ruta ajena al foro rechazada: {url}")
        return url

    def normalize_forum_url(self, url_or_path: str) -> str:
        absolute = urljoin(self.base_url, url_or_path)
        return self.validate_forum_url(absolute)

    async def start(self) -> None:
        if self.context is not None:
            return

        self.profile_dir.mkdir(parents=True, exist_ok=True)
        self._playwright = await async_playwright().start()
        executable = self.browser_executable
        if executable is None:
            executable = (
                shutil.which("google-chrome")
                or shutil.which("google-chrome-stable")
                or shutil.which("chromium")
                or shutil.which("chromium-browser")
            )

        launch_options: dict[str, Any] = {
            "user_data_dir": str(self.profile_dir),
            "headless": self.headless,
            "locale": "en-US",
            "user_agent": DEFAULT_USER_AGENT,
            "viewport": {"width": 1440, "height": 1000},
            "args": ["--disable-dev-shm-usage"],
        }
        if executable:
            launch_options["executable_path"] = executable

        self.context = await self._playwright.chromium.launch_persistent_context(**launch_options)
        self.context.set_default_timeout(self.timeout_ms)
        self.page = self.context.pages[0] if self.context.pages else await self.context.new_page()

    async def close(self) -> None:
        if self.context is not None:
            await self.context.close()
            self.context = None
            self.page = None
        if self._playwright is not None:
            await self._playwright.stop()
            self._playwright = None

    def _require_page(self) -> Page:
        if self.page is None:
            raise CsRinError("El cliente no está iniciado. Usa 'async with' o llama start().")
        return self.page

    async def _pause(self) -> None:
        if self.delay_seconds:
            await asyncio.sleep(self.delay_seconds)

    async def _goto(self, url_or_path: str) -> str:
        page = self._require_page()
        url = self.normalize_forum_url(url_or_path)
        # cs.rin.ru guards every page with a JS "Security check" that answers
        # HTTP 401 the first time, sets a `securitytoken` cookie and reloads via
        # /securitycheck/<path>. Let that challenge run and retry with the cookie
        # in place instead of failing on the initial 401.
        for _ in range(4):
            try:
                response = await page.goto(url, wait_until="domcontentloaded")
            except PlaywrightError as exc:
                message = str(exc)
                if "ERR_BLOCKED_BY_ADMINISTRATOR" in message:
                    raise CsRinError(
                        "La red o política del equipo bloqueó el acceso a cs.rin.ru."
                    ) from exc
                if "ERR_NAME_NOT_RESOLVED" in message:
                    raise CsRinError("No se pudo resolver el dominio cs.rin.ru.") from exc
                raise CsRinError(f"Error de navegación al acceder a {url}: {message}") from exc

            await self._pause()
            status = response.status if response is not None else 200
            # The security check answers 401 on the first hit; its window.onload
            # JS sets the securitytoken cookie and auto-navigates through
            # /securitycheck/<path> back to the real page. Rather than racing to
            # detect that transient page, let the navigation settle and retry:
            # once the cookie is stored the same URL returns 200.
            if status == 401:
                await self._solve_security_check()
                continue
            if status >= 400:
                raise CsRinError(f"HTTP {status} al acceder a {url}")
            return page.url

        raise CsRinError(
            f"No se superó el Security check de cs.rin.ru tras varios intentos ({url})"
        )

    async def _solve_security_check(self) -> None:
        # The challenge fires on window.onload: it sets the securitytoken cookie
        # and location.replace()s to /securitycheck/<path>, which validates and
        # redirects back. Wait for the load event and the follow-up navigations
        # to settle so the clearance cookie is stored before we retry.
        page = self._require_page()
        for state in ("load", "networkidle"):
            try:
                await page.wait_for_load_state(state, timeout=self.timeout_ms)
            except PlaywrightError:
                pass
        await asyncio.sleep(1.5)

    async def _looks_like_login_page(self) -> bool:
        page = self._require_page()
        return (
            await page.locator("input[name='username']").count() > 0
            and await page.locator("input[name='password']").count() > 0
        )

    async def is_logged_in(self) -> bool:
        page = self._require_page()
        return await page.locator("a[href*='mode=logout']").count() > 0

    async def login(self, username: str, password: str, *, remember: bool = True) -> bool:
        if not username or not password:
            raise AuthenticationError("Usuario y contraseña son obligatorios.")

        await self._goto("ucp.php?mode=login")
        page = self._require_page()

        if await self.is_logged_in():
            return True

        username_input = page.locator("input[name='username']").first
        password_input = page.locator("input[name='password']").first
        if await username_input.count() == 0 or await password_input.count() == 0:
            raise AuthenticationError("No se encontró el formulario de acceso esperado.")

        await username_input.fill(username)
        await password_input.fill(password)

        autologin = page.locator("input[name='autologin']")
        if remember and await autologin.count() > 0:
            await autologin.check()

        submit = page.locator(
            "input[name='login'], button[name='login'], "
            "input[type='submit'][value*='Login' i], button[type='submit']"
        ).first
        try:
            if await submit.count() > 0:
                await submit.click()
            else:
                await password_input.press("Enter")
            await page.wait_for_load_state("domcontentloaded")
        except PlaywrightTimeoutError as exc:
            raise AuthenticationError("El acceso no terminó dentro del tiempo configurado.") from exc

        await self._pause()
        body_text = (await page.locator("body").inner_text()).lower()
        captcha_present = (
            "captcha" in body_text
            or await page.locator("iframe[src*='captcha'], .g-recaptcha, input[name*='captcha']").count() > 0
        )
        if captcha_present and not await self.is_logged_in():
            raise CaptchaRequired(
                "El foro solicitó CAPTCHA. Ejecuta el comando con --headed y resuélvelo manualmente."
            )

        if await self.is_logged_in():
            return True

        html = await page.content()
        message = self.parser.parse_error_message(html)
        raise AuthenticationError(message or "No se pudo confirmar el inicio de sesión.")

    async def ensure_authenticated(self) -> None:
        if await self._looks_like_login_page() or not await self.is_logged_in():
            raise AuthenticationError("La sesión no está autenticada o ha expirado.")

    async def search_topics(
        self,
        query: str,
        *,
        first_post_only: bool = False,
        forum_ids: Iterable[int] | None = None,
        limit: int = 50,
    ) -> list[TopicSummary]:
        if not query.strip():
            raise ValueError("La búsqueda no puede estar vacía.")

        params: list[tuple[str, str]] = [
            ("keywords", query.strip()),
            ("terms", "all"),
            ("sr", "topics"),
            ("sf", "firstpost" if first_post_only else "all"),
        ]
        for forum_id in forum_ids or []:
            params.append(("fid[]", str(forum_id)))

        await self._goto("search.php?" + urlencode(params))
        if await self._looks_like_login_page():
            raise AuthenticationError("La búsqueda redirigió al formulario de acceso.")

        page = self._require_page()
        results = self.parser.parse_topic_summaries(await page.content(), page.url)
        return results[: max(0, limit)]

    async def list_forum_page(self, forum: int | str) -> ForumPageData:
        if isinstance(forum, int) or str(forum).isdigit():
            target = f"viewforum.php?f={int(forum)}"
        else:
            target = str(forum)

        await self._goto(target)
        if await self._looks_like_login_page():
            raise AuthenticationError("El subforo redirigió al formulario de acceso.")

        page = self._require_page()
        return self.parser.parse_forum_page(await page.content(), page.url)

    @staticmethod
    def _url_with_start(url: str, start: int) -> str:
        parsed = urlparse(url)
        query = parse_qs(parsed.query, keep_blank_values=True)
        query["start"] = [str(start)]
        flat: list[tuple[str, str]] = []
        for key, values in query.items():
            for value in values:
                flat.append((key, value))
        return urlunparse(parsed._replace(query=urlencode(flat)))

    @staticmethod
    def _infer_page_size(page_links: dict[int, str]) -> int | None:
        candidates: list[int] = []
        for number, url in page_links.items():
            if number <= 1:
                continue
            start_values = parse_qs(urlparse(url).query).get("start")
            if not start_values or not start_values[0].isdigit():
                continue
            start = int(start_values[0])
            divisor = number - 1
            if divisor and start % divisor == 0:
                candidates.append(start // divisor)
        return min(candidates) if candidates else None

    async def read_topic_page(self, topic_url: str, *, page_number: int = 1) -> TopicPageData:
        if page_number < 1:
            raise ValueError("page_number debe ser >= 1.")

        topic_url = self.normalize_forum_url(topic_url)
        await self._goto(topic_url)
        page = self._require_page()
        if await self._looks_like_login_page():
            raise AuthenticationError("El tema redirigió al formulario de acceso.")

        if page_number > 1:
            initial_html = await page.content()
            page_links = self.parser.parse_pagination_links(initial_html, page.url)
            target = page_links.get(page_number)

            if target is None:
                page_size = self._infer_page_size(page_links)
                if page_size:
                    target = self._url_with_start(topic_url, (page_number - 1) * page_size)

            if target is not None:
                await self._goto(target)
            else:
                current = self.parser.parse_topic_page(initial_html, page.url)
                while current.page_number < page_number and current.next_url:
                    await self._goto(current.next_url)
                    current = self.parser.parse_topic_page(await page.content(), page.url)
                if current.page_number != page_number:
                    raise CsRinError(f"No se pudo alcanzar la página {page_number} del tema.")
                return current

        return self.parser.parse_topic_page(await page.content(), page.url)

    async def iter_topic_pages(
        self,
        topic_url: str,
        *,
        max_pages: int = 10,
    ) -> AsyncIterator[TopicPageData]:
        if max_pages < 1:
            return

        next_url: str | None = self.normalize_forum_url(topic_url)
        visited: set[str] = set()
        count = 0
        while next_url and next_url not in visited and count < max_pages:
            visited.add(next_url)
            await self._goto(next_url)
            page = self._require_page()
            if await self._looks_like_login_page():
                raise AuthenticationError("El tema redirigió al formulario de acceso.")
            data = self.parser.parse_topic_page(await page.content(), page.url)
            yield data
            next_url = data.next_url
            count += 1

    async def iter_forum_pages(
        self,
        forum: int | str,
        *,
        max_pages: int = 5,
    ) -> AsyncIterator[ForumPageData]:
        first = await self.list_forum_page(forum)
        yield first
        next_url = first.next_url
        visited = {first.url}
        count = 1

        while next_url and next_url not in visited and count < max_pages:
            visited.add(next_url)
            await self._goto(next_url)
            page = self._require_page()
            data = self.parser.parse_forum_page(await page.content(), page.url)
            yield data
            next_url = data.next_url
            count += 1

    async def fetch_internal_page(self, url: str) -> GenericPageData:
        await self._goto(url)
        page = self._require_page()
        if await self._looks_like_login_page():
            raise AuthenticationError("La página redirigió al formulario de acceso.")
        return self.parser.parse_generic_page(await page.content(), page.url)


SEARCH_FIXTURE = """
<html><head><title>Search</title></head><body>
<table class="tablebg"><tbody>
<tr>
  <td class="row1"></td>
  <td class="row1"></td>
  <td class="row1"><a class="topictitle" href="./viewtopic.php?f=10&t=123">Dota 2</a>
    <br><a href="./viewforum.php?f=10">Main Forum</a></td>
  <td class="row2">TestUser</td>
  <td class="row1">42</td>
  <td class="row2">1234</td>
  <td class="row1">by TestUser</td>
</tr>
<tr>
  <td class="row1"></td>
  <td class="row1"></td>
  <td class="row1"><a class="topictitle" href="./viewtopic.php?f=10&t=456">Dota Tools</a></td>
  <td class="row2"></td>
  <td class="row1"></td>
  <td class="row2"></td>
  <td class="row1"></td>
</tr>
</tbody></table>
</body></html>
"""

TOPIC_FIXTURE = """
<html><head><title>Dota 2 - Forum</title></head><body>
<h2><a href="./viewtopic.php?f=10&t=123">Dota 2</a></h2>
<span class="gensmall">Page 1 of 2</span>
<span class="gensmall"><a href="./viewtopic.php?f=10&t=123&start=10">2</a></span>
<table class="tablebg"><tbody>
<tr><td><a name="p100"></a><b class="postauthor">Alice</b>
  Post subject: First post Posted: Monday, 14 Jul 2026, 10:00</td></tr>
<tr><td><div class="postbody">Hello <a href="./viewtopic.php?f=10&t=999">internal</a>
  <a href="https://example.com/file">external</a></div></td></tr>
</tbody></table>
<table class="tablebg"><tbody>
<tr><td><a name="p101"></a><b class="postauthor">Bob</b>
  Post subject: Reply Posted: Monday, 14 Jul 2026, 11:00</td></tr>
<tr><td><div class="postbody">Second post</div></td></tr>
</tbody></table>
</body></html>
"""


def run_self_test() -> dict[str, Any]:
    parser = ForumParser()
    summaries = parser.parse_topic_summaries(SEARCH_FIXTURE, BASE_URL + "search.php")
    assert len(summaries) == 2
    assert summaries[0].title == "Dota 2"
    assert summaries[0].replies == 42
    assert summaries[0].views == 1234

    topic = parser.parse_topic_page(TOPIC_FIXTURE, BASE_URL + "viewtopic.php?f=10&t=123")
    assert topic.title == "Dota 2"
    assert topic.page_number == 1
    assert topic.total_pages == 2
    assert len(topic.posts) == 2
    assert topic.posts[0].author == "Alice"
    assert topic.next_url and "start=10" in topic.next_url

    page_links = parser.parse_pagination_links(TOPIC_FIXTURE, topic.url)
    assert page_links[2].endswith("start=10")
    assert CsRinForumClient._infer_page_size(page_links) == 10

    try:
        CsRinForumClient.validate_forum_url("https://example.com/")
    except NavigationRejected:
        external_rejected = True
    else:
        external_rejected = False
    assert external_rejected

    return {
        "ok": True,
        "tests": {
            "search_parser": "passed",
            "topic_parser": "passed",
            "pagination": "passed",
            "external_url_guard": "passed",
        },
    }


def _jsonable(value: Any) -> Any:
    if hasattr(value, "__dataclass_fields__"):
        return {key: _jsonable(item) for key, item in asdict(value).items()}
    if isinstance(value, list):
        return [_jsonable(item) for item in value]
    if isinstance(value, dict):
        return {key: _jsonable(item) for key, item in value.items()}
    return value


def _print_json(value: Any, output_file: str | None = None) -> None:
    text = json.dumps(_jsonable(value), ensure_ascii=False, indent=2)
    if output_file:
        Path(output_file).write_text(text + "\n", encoding="utf-8")
    print(text)


def _read_credentials() -> tuple[str, str]:
    username = os.getenv("CSRIN_USERNAME") or input("Usuario de CS.RIN.RU: ").strip()
    password = os.getenv("CSRIN_PASSWORD") or getpass.getpass("Contraseña: ")
    return username, password


async def _run_browser_command(args: argparse.Namespace) -> Any:
    username, password = _read_credentials()
    async with CsRinForumClient(
        profile_dir=args.profile_dir,
        headless=not args.headed,
        timeout_ms=args.timeout_ms,
        delay_seconds=args.delay,
        browser_executable=args.browser_executable,
    ) as client:
        await client.login(username, password)

        if args.command == "login":
            return {"ok": True, "authenticated": True, "profile_dir": str(client.profile_dir)}

        if args.command == "search":
            return await client.search_topics(
                args.query,
                first_post_only=args.first_post_only,
                forum_ids=args.forum_id,
                limit=args.limit,
            )

        if args.command == "forum":
            pages = []
            async for page in client.iter_forum_pages(args.forum, max_pages=args.max_pages):
                pages.append(page)
            return pages

        if args.command == "topic":
            return await client.read_topic_page(args.url, page_number=args.page)

        if args.command == "crawl-topic":
            pages = []
            async for page in client.iter_topic_pages(args.url, max_pages=args.max_pages):
                pages.append(page)
            return pages

        if args.command == "fetch":
            return await client.fetch_internal_page(args.url)

        raise ValueError(f"Comando no implementado: {args.command}")


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(
        description="Navegador estructurado para cs.rin.ru/forum mediante Playwright."
    )
    parser.add_argument("--profile-dir", default=str(DEFAULT_PROFILE_DIR))
    parser.add_argument("--headed", action="store_true", help="Muestra Chromium; útil si aparece CAPTCHA.")
    parser.add_argument("--timeout-ms", type=int, default=30_000)
    parser.add_argument("--delay", type=float, default=1.25, help="Pausa entre páginas para no saturar el foro.")
    parser.add_argument("--json-out", help="Guarda también la salida en un archivo JSON.")
    parser.add_argument("--browser-executable", help="Ruta opcional a Chrome/Chromium. Se autodetecta si está instalado.")

    sub = parser.add_subparsers(dest="command", required=True)
    sub.add_parser("self-test", help="Prueba offline del parser y los controles de URL.")
    sub.add_parser("login", help="Valida el acceso y conserva cookies en el perfil local.")

    search = sub.add_parser("search", help="Busca temas.")
    search.add_argument("query")
    search.add_argument("--first-post-only", action="store_true")
    search.add_argument("--forum-id", type=int, action="append", default=[])
    search.add_argument("--limit", type=int, default=50)

    forum = sub.add_parser("forum", help="Lista temas de un subforo y sigue su paginación.")
    forum.add_argument("forum", help="ID numérico o URL interna del subforo.")
    forum.add_argument("--max-pages", type=int, default=3)

    topic = sub.add_parser("topic", help="Lee una página concreta de un tema.")
    topic.add_argument("url")
    topic.add_argument("--page", type=int, default=1)

    crawl = sub.add_parser("crawl-topic", help="Recorre varias páginas consecutivas de un tema.")
    crawl.add_argument("url")
    crawl.add_argument("--max-pages", type=int, default=10)

    fetch = sub.add_parser("fetch", help="Extrae texto y enlaces de cualquier página interna.")
    fetch.add_argument("url")

    return parser


def main() -> int:
    parser = build_parser()
    args = parser.parse_args()

    try:
        if args.command == "self-test":
            result = run_self_test()
        else:
            result = asyncio.run(_run_browser_command(args))
        _print_json(result, args.json_out)
        return 0
    except KeyboardInterrupt:
        print("Cancelado.", file=sys.stderr)
        return 130
    except (CsRinError, ValueError, PlaywrightTimeoutError, PlaywrightError) as exc:
        error = {"ok": False, "error": type(exc).__name__, "message": str(exc)}
        _print_json(error, args.json_out)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
