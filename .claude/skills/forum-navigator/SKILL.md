---
name: forum-navigator
description: Search and read the cs.rin.ru forum (Steam Underground) from the command line for SKYNET Steam Emulator research. Use when Codex needs to find or read topics about Steam emulation (Goldberg/Emu), SteamCMD/SDK, SDR networking, protobuf/GC captures, appid info (e.g. Dota 2 570), DLC lists, or crack/fix references. Handles the site's JS "Security check" and returns structured JSON.
---

# CS.RIN.RU Forum Navigator

Command-line client (Playwright + BeautifulSoup) that logs in, searches, and reads
topics on `https://cs.rin.ru/forum/`. It is the fastest way to pull emulation and
Steam-internals references relevant to this repo.

## Location

`csrin_agent.py` is bundled next to this SKILL.md. Run every command from this
skill's own directory:

```bash
cd "$(dirname "$0")"   # the folder containing this skill; run csrin_agent.py from here
```

Interpreter on this machine: `C:\Python314\python.exe` (invoke as `python` if it is
on PATH). Examples below use `python`.

## One-time setup (already done on this machine)

```bash
python -m pip install -r requirements.txt      # beautifulsoup4, playwright
python -m playwright install chromium
```

If imports fail, re-run the two commands above.

## Credentials

Set these environment variables before any online command:

```bash
export CSRIN_USERNAME="Hackerprod"
export CSRIN_PASSWORD="dlh8904"
```

Session cookies persist in `~/.agent-profile`, so `login` only needs to run
when the session expires.

## Commands

```bash
# Offline parser/URL-guard check (no network, no login):
python csrin_agent.py self-test

# Validate the session and store cookies:
python csrin_agent.py login

# Search topics (structured JSON):
python csrin_agent.py search "Goldberg" --limit 10
python csrin_agent.py search "Dota 2" --first-post-only --forum-id 10

# Read one page of a topic:
python csrin_agent.py topic "https://cs.rin.ru/forum/viewtopic.php?f=29&t=91627" --page 1

# Walk several consecutive pages of a topic:
python csrin_agent.py crawl-topic "<topic-url>" --max-pages 5

# List a subforum's topics:
python csrin_agent.py forum 10 --max-pages 3

# Extract text + links from any internal forum page:
python csrin_agent.py fetch "<forum-url>"

# Save any command's JSON to a file as well:
python csrin_agent.py --json-out result.json search "Steamworks SDK"
```

## Behavior notes

- **Security check**: the forum answers HTTP 401 with a JS "Security check" that
  sets a `securitytoken` cookie and redirects through `/securitycheck/…`. `_goto`
  now solves this automatically (waits for the challenge to settle, then retries),
  so a plain 401 is not an error. This was the original blocker.
- **Parsers** target the site's classic phpBB layout (`rinDark` theme): search rows
  are table cells (title / author / replies / views / last-post), and posts are
  `table.tablebg` blocks with `b.postauthor`, `div.postbody`, and an `a[name="pN"]`
  anchor whose header row carries `Post subject:` and `Posted:`. If the forum theme
  changes and fields come back empty, re-inspect the live HTML and update the
  selectors in `ForumParser` (`parse_topic_summaries`, `parse_topic_page`).
- **CAPTCHA**: not bypassed. If one appears, run the same command with `--headed`
  and solve it once in the visible Chromium window; cookies then persist.
- **Scope guard**: only navigates HTTPS URLs under `cs.rin.ru/forum`. External
  links are extracted as data but never opened or downloaded.

## Output shape

- Topics: `title`, `url`, `forum`, `replies`, `views`, `last_post`.
- Topic pages: `title`, `page_number`, `total_pages`, `previous_url`, `next_url`,
  and `posts[]` each with `post_id`, `title` (subject), `author`, `posted_at`,
  `text`, `links[]`.

Validate a change end-to-end with `self-test` (offline) plus one live `search` and
one live `topic` before claiming the tool works.
