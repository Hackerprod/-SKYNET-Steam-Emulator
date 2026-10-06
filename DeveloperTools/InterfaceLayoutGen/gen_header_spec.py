#!/usr/bin/env python3
"""
gen_header_spec.py - derive Steamworks interface vtable specs from the real Valve headers.

Usage (from repo root):
    python DeveloperTools/InterfaceLayoutGen/gen_header_spec.py                      # generate all + calibrate
    python DeveloperTools/InterfaceLayoutGen/gen_header_spec.py --only SteamUser019  # one version, verbose
    python DeveloperTools/InterfaceLayoutGen/gen_header_spec.py --no-write           # calibrate only

Sources (all under the repo root):
  * .tmp/goldberg_emulator_ref/sdk_includes/isteam<name><NNN>.h  per-version headers (class ISteam<Name><NNN>);
    preferred when present, version string derived from the family prefix of the SDK defines.
  * .tmp/SmokeAPI/res/steamworks/<sdk>/headers/steam/isteam*.h  one header per interface per SDK release
  * Tools/steamworks_sdk_164/sdk/public/steam/isteam*.h          SDK 1.64
  * .tmp/goldberg_emulator_ref/sdk_includes/isteam<name>.h       (current-version headers, treated as an SDK dir)
  * .tmp/goldberg_emulator/gse_fork/sdk/steam/isteam*.h          gbe_fork sdk (per-version + current headers). LAST RESORT:
    used only for versions no other source defines (so previously generated specs can never change); family prefixes
    come from the defines of the other sources first (setdefault), then from these headers' own defines
    (e.g. STEAMUSERITEMS_INTERFACE_VERSION -> STEAMUSERITEMS_INTERFACE_VERSION001, as Goldberg's getter spells it).
A version string comes from `#define <STEM>_INTERFACE_VERSION[...] "<string>"`; the class is the one in the
same header named like <STEM>. Every source defining a version is parsed; disagreements in the ordered
method names are reported (the preferred source wins).

Parsing rules: comments stripped; #if/#ifdef/#elif/#else evaluated for a Windows PC build (_WIN32, STEAM_WIN32,
_MSC_VER defined; _PS3/POSIX/__linux__/... undefined); STEAM_PRIVATE_API(...) unwrapped (hidden methods keep
their slots); other STEAM_* attribute macros (STEAM_FLAT_NAME, STEAM_OUT_*, ...) dropped; every `virtual`
declaration takes a slot. MSVC overload rule: for each group of same-named virtuals the slot positions stay
where declared and the group's members are assigned to them in REVERSE declaration order.
Details: annotation macros (ALL_CAPS(...)) are dropped; STEAMWORKS_STRUCT_RETURN_N(...) (goldberg headers) is expanded
to a virtual decl; a STEAM_FLAT_NAME(X) becomes the spec MethodName (C++ overload groups are listed as
"# overload <cppName> slots a,b" comment lines); a virtual destructor takes a slot (ISteamHTMLSurface slot 0);
`_SERVER`/`_PS3` are NOT defined (PC build), the SDR macros from steamnetworkingtypes.h ARE. Source choice: per-version
goldberg header first, else newest SDK; sources that disagree are listed as "# WARNING" lines in the spec. Every parsed
class is cross-checked: count of `virtual` tokens must equal the parsed method count (else PARSE-ERROR).

Output: DeveloperTools/InterfaceLayoutCheck/header-spec/<Version>.txt with lines
    index|MethodName|paramCount|returnType|paramTypes
preceded by a '#' comment naming the source header(s). Calibration compares the ordered names against the
trusted DeveloperTools/InterfaceLayoutCheck/golden/*.txt (trailing __V... and the leading IntPtr this ignored).
"""
import argparse
import collections
import glob
import os
import re
import sys

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
GOLD_DIR = os.path.join(ROOT, "DeveloperTools", "InterfaceLayoutCheck", "golden")
SPEC_DIR = os.path.join(ROOT, "DeveloperTools", "InterfaceLayoutCheck", "header-spec")
GB_DIR = os.path.join(ROOT, ".tmp", "goldberg_emulator_ref", "sdk_includes")
GSE_DIR = os.path.join(ROOT, ".tmp", "goldberg_emulator", "gse_fork", "sdk", "steam")  # gbe_fork sdk folder (last-resort source)

DEFINES = {"_WIN32": 1, "WIN32": 1, "STEAM_WIN32": 1, "_MSC_VER": 1930, "__cplusplus": 201703,
           "VALVE_CALLBACK_PACK_SMALL": 1,
           # defined by steamnetworkingtypes.h in the Steamworks SDK (always included before isteamnetworking*.h)
           "STEAMNETWORKINGSOCKETS_STEAM": 1, "STEAMNETWORKINGSOCKETS_STEAMCLIENT": 1,
           "STEAMNETWORKINGSOCKETS_ENABLE_SDR": 1, "STEAMNETWORKINGSOCKETS_STEAMAPI": 1}


# ----------------------------------------------------------------------------- text helpers
def strip_comments(t):
    out, i, n = [], 0, len(t)
    while i < n:
        c = t[i]
        if c == "/" and i + 1 < n and t[i + 1] == "/":
            j = t.find("\n", i)
            i = n if j < 0 else j
        elif c == "/" and i + 1 < n and t[i + 1] == "*":
            j = t.find("*/", i + 2)
            seg = t[i:n if j < 0 else j + 2]
            out.append("\n" * seg.count("\n"))
            i = n if j < 0 else j + 2
        elif c == '"' or c == "'":
            j = i + 1
            while j < n and t[j] != c:
                j += 2 if t[j] == "\\" else 1
            out.append(t[i:j + 1])
            i = j + 1
        else:
            out.append(c)
            i += 1
    return "".join(out)


def eval_cond(expr):
    e = re.sub(r"\bdefined\s*\(\s*(\w+)\s*\)", lambda m: "1" if m.group(1) in DEFINES else "0", expr)
    e = re.sub(r"\bdefined\s+(\w+)", lambda m: "1" if m.group(1) in DEFINES else "0", e)
    e = re.sub(r"\b[A-Za-z_]\w*\b", lambda m: str(DEFINES.get(m.group(0), 0)), e)
    e = e.replace("&&", " and ").replace("||", " or ")
    e = re.sub(r"!(?!=)", " not ", e)
    e = re.sub(r"(\d)[uUlL]+\b", r"\1", e)
    try:
        return bool(eval(e, {"__builtins__": {}}, {}))
    except Exception:
        raise ValueError("cannot evaluate #if " + expr)


def preprocess(t):
    """Evaluate conditionals; drops directive lines except active #define lines (kept for version scraping)."""
    lines = t.replace("\r\n", "\n").split("\n")
    merged, buf = [], ""
    for ln in lines:  # join continuations
        if ln.endswith("\\"):
            buf += ln[:-1] + " "
        else:
            merged.append(buf + ln)
            buf = ""
    out = []
    stack = []  # (parent_active, taken, active)
    active = True
    for ln in merged:
        m = re.match(r"\s*#\s*(\w+)\s*(.*)", ln)
        if not m:
            out.append(ln if active else "")
            continue
        d, rest = m.group(1), m.group(2).strip()
        if d in ("if", "ifdef", "ifndef"):
            if d == "ifdef":
                c = rest.split()[0] in DEFINES
            elif d == "ifndef":
                c = rest.split()[0] not in DEFINES
            else:
                c = eval_cond(rest) if active else False
            stack.append((active, c and active, c and active))
            active = active and c
        elif d == "elif":
            pa, taken, _ = stack.pop()
            c = (not taken) and pa and eval_cond(rest)
            stack.append((pa, taken or c, c))
            active = c
        elif d == "else":
            pa, taken, _ = stack.pop()
            c = pa and not taken
            stack.append((pa, True, c))
            active = c
        elif d == "endif":
            pa, _, _ = stack.pop()
            active = pa
        elif d == "define" and active:
            out.append(ln)
        else:
            out.append("")
    return "\n".join(out)


def match_close(s, i, op, cl):
    """s[i]==op; return index of the matching closer."""
    d = 0
    for j in range(i, len(s)):
        if s[j] == op:
            d += 1
        elif s[j] == cl:
            d -= 1
            if d == 0:
                return j
    raise ValueError("unbalanced " + op)


def drop_macros(s, unwrap=("STEAM_PRIVATE_API",)):
    """Unwrap STEAM_PRIVATE_API(...); delete other STEAM_*( ... ) attribute macros."""
    # any ALL-CAPS identifier followed by '(' is an annotation macro (STEAM_*, CALL_RESULT, METHOD_DESC, OUT_ARRAY_COUNT, ...)
    pat = re.compile(r"\b([A-Z][A-Z0-9_]{2,})\s*\(")
    pos = 0
    while True:
        m = pat.search(s, pos)
        if not m:
            return s
        o = m.end() - 1
        c = match_close(s, o, "(", ")")
        if m.group(1) in unwrap:
            s = s[:m.start()] + s[o + 1:c] + s[c + 1:]
            pos = m.start()
        else:
            s = s[:m.start()] + " " + s[c + 1:]
            pos = m.start()


def split_top(s, sep=","):
    parts, d, cur = [], 0, []
    for ch in s:
        if ch in "([{<":
            d += 1
        elif ch in ")]}>":
            d -= 1
        if ch == sep and d == 0:
            parts.append("".join(cur))
            cur = []
        else:
            cur.append(ch)
    parts.append("".join(cur))
    return parts


def norm(s):
    return re.sub(r"\s+", " ", s).strip()


def strip_default(p):
    d, cut = 0, None
    for i, ch in enumerate(p):
        if ch in "([<":
            d += 1
        elif ch in ")]>":
            d -= 1
        elif ch == "=" and d == 0:
            cut = i
            break
    return p if cut is None else p[:cut]


# ----------------------------------------------------------------------------- class parsing
Method = collections.namedtuple("Method", "name ret params pcount flat")

CLASS_RE = re.compile(r"\bclass\s+(?:[A-Z][A-Z0-9_]*\s+)*(I[A-Za-z0-9_]+)\s*(?::\s*(?:public\s+)?(\w+)\s*)?\{")
_file_cache = {}


def load(path):
    if path not in _file_cache:
        raw = open(path, encoding="utf-8", errors="ignore").read()
        _file_cache[path] = preprocess(strip_comments(raw))
    return _file_cache[path]


def classes_in(text):
    res = {}
    for m in CLASS_RE.finditer(text):
        o = m.end() - 1
        c = match_close(text, o, "{", "}")
        res[m.group(1)] = (text[o + 1:c], m.group(2))
    return res


def expand_struct_returns(s):
    """goldberg per-version headers: STEAMWORKS_STRUCT_RETURN_N(ret, name, t1, n1, ...) == virtual ret name(t1 n1, ...) = 0;"""
    pat = re.compile(r"\bSTEAMWORKS_STRUCT_RETURN_\d\s*\(")
    while True:
        m = pat.search(s)
        if not m:
            return s
        o = m.end() - 1
        c = match_close(s, o, "(", ")")
        a = [x.strip() for x in split_top(s[o + 1:c])]
        params = ", ".join(a[i] + " " + a[i + 1] for i in range(2, len(a), 2))
        s = s[:m.start()] + " virtual %s %s( %s ) = 0; " % (a[0], a[1], params) + s[c + 1:]


def parse_body(body):
    body = expand_struct_returns(body)
    body = re.sub(r"STEAM_FLAT_NAME\s*\(\s*(\w+)\s*\)", r" __FLAT_\1__ ", body)
    body = drop_macros(body)
    body = re.sub(r"\b(public|protected|private)\s*:(?!:)", ";", body)
    decls, d, cur = [], 0, []
    for ch in body:
        if ch in "({":
            d += 1
        elif ch in ")}":
            d -= 1
        cur.append(ch)
        if (ch == ";" and d == 0) or (ch == "}" and d == 0):
            decls.append("".join(cur))
            cur = []
    decls.append("".join(cur))
    methods = []
    for dcl in decls:
        dcl = norm(dcl)
        fm = re.search(r"__FLAT_(\w+)__", dcl)
        flat = fm.group(1) if fm else None
        dcl = norm(re.sub(r"__FLAT_\w+__", " ", dcl))
        m = re.match(r"(?:\w+\s+)*?virtual\s+(.*)$", dcl)
        if not m or not re.search(r"\bvirtual\b", dcl.split("(")[0]):
            continue
        rest = m.group(1)
        o = rest.find("(")
        if o < 0:
            raise ValueError("virtual without parens: " + dcl)
        c = match_close(rest, o, "(", ")")
        head, params = rest[:o].rstrip(), rest[o + 1:c]
        nm = re.search(r"(~?\w+|operator\s*\S+)$", head)
        name = nm.group(1)
        ret = norm(head[:nm.start()])
        plist = [norm(strip_default(p)) for p in split_top(params)]
        plist = [p for p in plist if p]
        if plist == ["void"]:
            plist = []
        methods.append(Method(name, ret, ", ".join(plist), len(plist), flat))
    return methods


def apply_overload_rule(methods):
    pos = collections.defaultdict(list)
    for i, m in enumerate(methods):
        pos[m.name].append(i)
    out = list(methods)
    for name, idx in pos.items():
        if len(idx) > 1:
            for slot, mm in zip(idx, reversed([methods[i] for i in idx])):
                out[slot] = mm
    return out


def class_layout(text, cls, path):
    cl = classes_in(text)
    body, base = cl[cls]
    methods = []
    if base and base in cl:
        methods = parse_body(cl[base][0])
    elif base:
        raise ValueError("%s: unresolved base class %s" % (cls, base))
    own = parse_body(body)
    nv = len(re.findall(r"\bvirtual\b", expand_struct_returns(body)))
    if nv != len(own):
        raise ValueError("%s: %d 'virtual' tokens but %d parsed methods" % (cls, nv, len(own)))
    methods += own
    return apply_overload_rule(methods)


# ----------------------------------------------------------------------------- source discovery
def sdk_dirs():
    ds = []
    for d in sorted(glob.glob(os.path.join(ROOT, ".tmp", "SmokeAPI", "res", "steamworks", "*", "headers", "steam"))):
        ds.append((d, "sdk" + os.path.basename(os.path.dirname(os.path.dirname(d)))))
    ds.append((os.path.join(ROOT, "Tools", "steamworks_sdk_164", "sdk", "public", "steam"), "sdk164"))
    ds.append((GB_DIR, "goldberg-current"))
    ds.append((GSE_DIR, "gse-current"))
    return ds


DEF_RE = re.compile(r'#\s*define\s+(\w*INTERFACE_V\w*)\s+"([^"]+)"')


def discover():
    """returns (sources: version -> [(path, class, tag)], family_prefix: classlower -> version prefix)"""
    sources = collections.defaultdict(list)
    family = {}
    warns = []
    for d, tag in sdk_dirs():
        for f in sorted(glob.glob(os.path.join(d, "isteam*.h")) + glob.glob(os.path.join(d, "steamnetworkingfakeip.h"))):
            text = load(f)
            cls = classes_in(text)
            lower = {c.lower()[1:]: c for c in cls}
            for m in DEF_RE.finditer(text):
                macro, ver = m.group(1), m.group(2)
                stem = re.sub(r"_?INTERFACE_V.*$", "", macro).lower()
                c = lower.get(stem)
                if c is None:  # version-suffixed classes sharing one header (ISteamNetworkingSocketsSerialized002...)
                    c = lower.get(stem + re.sub(r"^\D*", "", ver)[-3:])
                if c is None:
                    warns.append("no class for %s (%s) in %s" % (macro, ver, f))
                    continue
                sources[ver].append((f, c, tag))
                pre = re.sub(r"\d+$", "", ver)
                if pre != ver:
                    family.setdefault(re.sub(r"\d+$", "", c.lower()), pre)
    # per-version goldberg headers: class ISteamUser017 -> family ISteamUser
    for f in sorted(glob.glob(os.path.join(GB_DIR, "isteam*.h"))):
        text = load(f)
        for c in classes_in(text):
            m = re.match(r"(I[A-Za-z]+?)(\d{3})$", c)
            if not m:
                continue
            pre = family.get(m.group(1).lower())
            if pre is None:
                warns.append("goldberg class %s has no known version family" % c)
                continue
            sources[pre + m.group(2)].append((f, c, "goldberg-ver"))
    # gbe_fork (gse_fork/sdk/steam) per-version headers: same naming, but ONLY a last-resort source
    for f in sorted(glob.glob(os.path.join(GSE_DIR, "isteam*.h"))):
        text = load(f)
        for c in classes_in(text):
            m = re.match(r"(I[A-Za-z]+?)(\d{3})$", c)
            if not m:
                continue
            pre = family.get(m.group(1).lower())
            if pre is None:
                warns.append("gse_fork class %s has no known version family" % c)
                continue
            sources[pre + m.group(2)].append((f, c, "gse-ver"))
    # gse_fork headers are used only for versions no other source defines, so existing specs never change
    for ver in list(sources):
        if any(not x[2].startswith("gse-") for x in sources[ver]):
            sources[ver] = [x for x in sources[ver] if not x[2].startswith("gse-")]
    return sources, warns


# ----------------------------------------------------------------------------- golden handling
def read_golden(path):
    names = []
    for ln in open(path, encoding="utf-8").read().splitlines():
        p = ln.split("|")
        if len(p) < 2 or ln.startswith("ERROR"):
            return None
        names.append(re.sub(r"__.*$", "", p[1]))
    return names


def disp(m):
    return m.flat or m.name


def slot_key(m):
    """Name identity used to compare sources: ignores emulator-style disambiguation suffixes and the dtor's class name."""
    if m.name.startswith("~"):
        return "~"
    k = re.sub(r"^deprecated", "", re.sub(r"_", "", m.name.lower()))
    return re.sub(r"(old|deprecated|001)$", "", k)


def classify(variants):
    """variants: ordered {key-tuple: [sources]}, first = chosen. Returns (counter, notes)."""
    items = list(variants.items())
    base = items[0][0]
    cnt, notes = collections.Counter(), []
    for k, v in items[1:]:
        if base[:len(k)] == k:
            kind = "older-prefix"          # other source is a strict prefix of the chosen layout (methods appended later)
        elif k[:len(base)] == base:
            kind = "LONGER-THAN-CHOSEN"    # chosen source lacks trailing methods another source has
        else:
            kind = "CONFLICT"              # different slot order
        cnt[kind] += 1
        if kind != "older-prefix":
            d = next(i for i in range(min(len(base), len(k))) if base[i] != k[i]) if kind == "CONFLICT" else len(base)
            tags = ",".join(sorted({x[2] for x in v}, key=lambda t: (len(t), t)))
            notes.append("%s other-sources=[%s] n=%d (chosen n=%d) first-diff@%d other=%s chosen=%s" % (
                kind, tags[:50], len(k), len(base), d, k[d] if d < len(k) else None, base[d] if d < len(base) else None))
    return cnt, notes


def src_rank(src):
    tag = src[2]
    if tag == "goldberg-ver":
        return (0, 0, "")
    if tag == "goldberg-current":
        return (2, 0, "")
    if tag == "gse-ver":
        return (3, 0, "")
    if tag == "gse-current":
        return (3, 1, "")
    num = re.match(r"sdk(\d+)(\w*)", tag)
    return (1, -int(num.group(1)), num.group(2))


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--only")
    ap.add_argument("--no-write", action="store_true")
    args = ap.parse_args()

    sources, warns = discover()
    goldens = {os.path.splitext(os.path.basename(f))[0].lower(): f for f in glob.glob(os.path.join(GOLD_DIR, "*.txt"))}
    if not args.no_write:
        os.makedirs(SPEC_DIR, exist_ok=True)
        for f in glob.glob(os.path.join(SPEC_DIR, "*.txt")):
            os.remove(f)

    layouts, errors, disagree = {}, {}, {}
    for ver in sorted(sources):
        if args.only and ver.lower() != args.only.lower():
            continue
        srcs = sorted(sources[ver], key=src_rank)  # per-version goldberg header first, then newest SDK first
        parsed = []  # (src, methods)
        for path, cls, tag in srcs:
            try:
                parsed.append(((path, cls, tag), class_layout(load(path), cls, path)))
            except Exception as ex:  # noqa
                errors.setdefault(ver, []).append("%s %s: %s" % (path, cls, ex))
        if not parsed:
            continue
        variants = collections.OrderedDict()
        for (path, cls, tag), ms in parsed:
            variants.setdefault(tuple(slot_key(m) for m in ms), []).append((path, cls, tag))
        if len(variants) > 1:
            disagree[ver] = classify(variants)
        layouts[ver] = (parsed[0][1], [p[0] for p in parsed])

    rel = lambda p: os.path.relpath(p, ROOT).replace("\\", "/")
    for ver, (methods, srcs) in layouts.items():
        if args.only:
            for i, m in enumerate(methods):
                print("%d|%s|%d|%s|%s" % (i, disp(m), m.pcount, m.ret, m.params))
        if args.no_write:
            continue
        hdr = ["# source: " + rel(srcs[0][0]) + " (class " + srcs[0][1] + ")"]
        others = sorted({rel(x[0]) for x in srcs[1:]} - {rel(srcs[0][0])})
        if others:
            hdr.append("# also: " + ", ".join(others[:6]) + (" (+%d more)" % (len(others) - 6) if len(others) > 6 else ""))
        for note in disagree.get(ver, (None, []))[1]:
            hdr.append("# WARNING sources disagree: " + note)
        groups = collections.defaultdict(list)
        for i, m in enumerate(methods):
            groups[m.name].append(i)
        for name, idx in groups.items():
            if len(idx) > 1:  # informational for tooling: C++ overload group, members stored in reverse order
                hdr.append("# overload %s slots %s" % (name, ",".join(map(str, idx))))
        with open(os.path.join(SPEC_DIR, ver + ".txt"), "w", newline="\n") as fh:
            fh.write("\n".join(hdr) + "\n")
            for i, m in enumerate(methods):
                fh.write("%d|%s|%d|%s|%s\n" % (i, disp(m), m.pcount, m.ret, m.params))

    # calibration: golden names must equal header names; names of members of a C++ overload group may differ
    # (the emulator invents disambiguators such as _old / Float that the header cannot know) as long as they
    # start with the C++ name or are the flat name.
    def eq(gname, m, in_group):
        if gname == disp(m):
            return True
        g = gname.lower().replace("_", "")
        return g.startswith(m.name.lower().replace("_", "")) or g.startswith(disp(m).lower().replace("_", ""))

    matched, mism, lenient = [], [], []
    for ver, (methods, srcs) in layouts.items():
        g = goldens.get(ver.lower())
        if not g:
            continue
        gn = read_golden(g)
        names = [disp(m) for m in methods]
        cnt = collections.Counter(m.name for m in methods)
        if gn == names:
            matched.append((ver, gn, names))
        elif gn is not None and len(gn) == len(methods) and all(eq(x, m, cnt[m.name] > 1) for x, m in zip(gn, methods)):
            matched.append((ver, gn, names))
            lenient.append(ver)
        else:
            mism.append((ver, gn, names))
    print("generated %d specs (%d versions with disagreeing sources, %d with parse errors)" %
          (len(layouts), len(disagree), len(errors)))
    print("calibration: %d matched / %d total (goldens covered by headers; %d matched only via overload-name leniency: %s)" % (
        len(matched), len(matched) + len(mism), len(lenient), lenient))
    print("goldens without header:", sorted(os.path.splitext(os.path.basename(f))[0] for k, f in goldens.items()
                                           if k not in {v.lower() for v in layouts}))
    for ver, gn, names in mism:
        i = next((i for i in range(min(len(gn), len(names))) if gn[i] != names[i]), min(len(gn), len(names)))
        print("MISMATCH %s: golden %d slots, header %d slots; first diff at %d: golden=%s header=%s" % (
            ver, len(gn), len(names), i, gn[i] if i < len(gn) else None, names[i] if i < len(names) else None))
    for ver, e in errors.items():
        print("PARSE-ERROR", ver, e[:2])
    for w in warns:
        print("WARN", w)
    cls_count = collections.Counter()
    for ver, (cnt, notes) in disagree.items():
        cls_count.update(cnt)
        for n in notes:
            print("DISAGREE %s: %s" % (ver, n))
    print("source disagreements by kind:", dict(cls_count))
    return 1 if (mism or errors) else 0


if __name__ == "__main__":
    sys.exit(main())
