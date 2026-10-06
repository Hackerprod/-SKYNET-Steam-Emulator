#!/usr/bin/env python3
"""Generates expected/callbacks_<arch>.txt from the real Valve SDK headers.

Pipeline per architecture (x64, x86), using the MSVC toolchain from VS BuildTools:
  1. cl /EP over all_headers.h  -> preprocessed text (macros expanded, #if resolved)
  2. parse every struct holding `k_iCallback` plus the structs listed in structs.txt
  3. emit a C++ probe that includes the same headers and prints
        Name|callbackId|sizeof|field=offset;field=offset
     (non-callback structs print -1 as the id)
  4. compile the probe with the same arch, run it, write the expected file.

usage: gen_expected.py [--sdk <dir>] [--arch x64|x86|both]
"""
import argparse
import os
import re
import subprocess
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
REPO = os.path.abspath(os.path.join(HERE, "..", ".."))
DEFAULT_SDK = os.path.join(REPO, ".tmp", "ProtonLsteamclient", "lsteamclient", "steamworks_sdk_165")
VC_BUILD = r"C:\Program Files (x86)\Microsoft Visual Studio\18\BuildTools\VC\Auxiliary\Build"
OBJ = os.path.join(HERE, "obj")
EXPECTED = os.path.join(HERE, "expected")
VCVARS = {"x64": "vcvars64.bat", "x86": "vcvars32.bat"}

STRUCT_RE = re.compile(r"\b(?:struct|class)\s+([A-Za-z_]\w*)\s*(?:final\s*)?(?::\s*[^{;]+)?\{")
SKIP_PREFIX = ("enum", "static", "typedef", "friend", "using", "template", "union", "struct", "class")


def run_vc(arch, command, cwd):
    cmd_file = os.path.join(OBJ, "run_%s.cmd" % arch)
    with open(cmd_file, "w") as f:
        f.write("@echo off\r\n")
        f.write('call "%s" >nul\r\n' % os.path.join(VC_BUILD, VCVARS[arch]))
        f.write('cd /d "%s"\r\n' % cwd)
        f.write(command + "\r\n")
    return subprocess.run(["cmd", "/c", cmd_file], capture_output=True, text=True)


def match_brace(text, open_index):
    depth = 0
    for i in range(open_index, len(text)):
        c = text[i]
        if c == "{":
            depth += 1
        elif c == "}":
            depth -= 1
            if depth == 0:
                return i
    return -1


def top_level(body):
    out, depth = [], 0
    for c in body:
        if c == "{":
            if depth == 0:
                out.append("{}")
            depth += 1
        elif c == "}":
            depth -= 1
        elif depth == 0:
            out.append(c)
    return "".join(out)


def parse_fields(body):
    fields = []
    text = re.sub(r"\b(public|private|protected)\s*:", ";", top_level(body))
    for stmt in text.split(";"):
        stmt = " ".join(stmt.split())
        if not stmt or "{}" in stmt or "(" in stmt or ":" in stmt or "operator" in stmt:
            continue
        if stmt.startswith(SKIP_PREFIX):
            continue
        for idx, piece in enumerate(p.strip() for p in stmt.split(",")):
            piece = re.sub(r"\[[^\]]*\]", "", piece).replace("*", " ").replace("&", " ").strip()
            tokens = piece.split()
            if not tokens or not re.match(r"^[A-Za-z_]\w*$", tokens[-1]):
                continue
            if idx == 0 and len(tokens) < 2:
                continue
            fields.append(tokens[-1])
    return fields


def parse_structs(pre_text, wanted_noncallback):
    structs = {}
    for m in STRUCT_RE.finditer(pre_text):
        name = m.group(1)
        end = match_brace(pre_text, m.end() - 1)
        if end < 0:
            continue
        body = pre_text[m.end():end]
        is_cb = re.search(r"enum\s*\{\s*k_iCallback\s*=", body) is not None
        if not is_cb and name not in wanted_noncallback:
            continue
        if name in structs:
            continue
        structs[name] = (is_cb, parse_fields(body))
    return structs


def load_structs_list():
    names = []
    with open(os.path.join(HERE, "structs.txt")) as f:
        for line in f:
            line = line.split("#")[0].strip()
            if line:
                names.append(line)
    return names


def build_probe(structs, out_path):
    lines = [
        "#include <cstddef>", "#include <cstdio>", "#include <cstring>", "#include <cstdint>",
        "#include <cwchar>", "#include <cstdlib>", "#include <string>", "#include <vector>",
        "#define private public", "#define protected public",
        '#include "all_headers.h"',
        "int main() {",
    ]
    for name in sorted(structs):
        is_cb, fields = structs[name]
        ident = "(int)%s::k_iCallback" % name if is_cb else "-1"
        lines.append('  std::printf("%s|%%d|%%zu|", %s, sizeof(%s));' % (name, ident, name))
        for i, field in enumerate(fields):
            lines.append('  std::printf("%s%s=%%zu", offsetof(%s, %s));' % (";" if i else "", field, name, field))
        lines.append('  std::printf("\\n");')
    lines.append("  return 0;\n}")
    with open(out_path, "w") as f:
        f.write("\n".join(lines) + "\n")


def generate(arch, sdk):
    os.makedirs(OBJ, exist_ok=True)
    os.makedirs(EXPECTED, exist_ok=True)
    inc = '/I"%s" /I"%s"' % (sdk, HERE)
    pre = os.path.join(OBJ, "pre_%s.i" % arch)
    r = run_vc(arch, 'cl /nologo /EP /Zc:__cplusplus /TP %s all_headers.h > "%s"' % (inc, pre), HERE)
    if not os.path.exists(pre) or os.path.getsize(pre) == 0:
        print(r.stdout, r.stderr)
        raise SystemExit("preprocess failed for " + arch)
    with open(pre, "r", errors="replace") as f:
        pre_text = f.read()

    wanted = {n.split("=")[0].strip() for n in load_structs_list()}
    structs = parse_structs(pre_text, wanted)
    missing = sorted(wanted - set(structs))
    if missing:
        print("warning: structs.txt names not found in headers:", ", ".join(missing))

    probe = os.path.join(OBJ, "probe_%s.cpp" % arch)
    build_probe(structs, probe)
    exe = os.path.join(OBJ, "probe_%s.exe" % arch)
    r = run_vc(arch, 'cl /nologo /EHsc /w /Zc:__cplusplus %s /Fo"%s\\%s_" /Fe"%s" "%s"' % (inc, OBJ, arch, exe, probe), HERE)
    if not os.path.exists(exe):
        print(r.stdout[-6000:], r.stderr[-2000:])
        raise SystemExit("probe compile failed for " + arch)
    out = subprocess.run([exe], capture_output=True, text=True)
    rows = sorted(l for l in out.stdout.splitlines() if l.strip())
    path = os.path.join(EXPECTED, "callbacks_%s.txt" % arch)
    header = [
        "# Generated by gen_expected.py from the SDK 1.65 headers with MSVC (%s). Do not edit." % arch,
        "# Name|callbackId(-1 = not a callback)|sizeof|field=offset;...",
    ]
    with open(path, "w", newline="\n") as f:
        f.write("\n".join(header + rows) + "\n")
    callbacks = sum(1 for n in structs if structs[n][0])
    print("%s: %d callbacks, %d other structs -> %s" % (arch, callbacks, len(structs) - callbacks, path))


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--sdk", default=DEFAULT_SDK)
    ap.add_argument("--arch", default="both", choices=["x64", "x86", "both"])
    args = ap.parse_args()
    for arch in (["x64", "x86"] if args.arch == "both" else [args.arch]):
        generate(arch, args.sdk)


if __name__ == "__main__":
    sys.exit(main())
