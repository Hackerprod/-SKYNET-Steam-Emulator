#!/usr/bin/env python3
"""Lists callbacks that exist in older SDK header sets but not in the newest one (SDK 1.65).

Names and IDs only (regex over the raw headers, base ids from the k_i*Callbacks enums), grouped by the
oldest SDK that declares them. A game built against that SDK may still register them.

usage: old_sdk_callbacks.py [--root <lsteamclient dir>] [--latest steamworks_sdk_165]
"""
import argparse
import glob
import os
import re

HERE = os.path.dirname(os.path.abspath(__file__))
DEFAULT_ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".tmp", "ProtonLsteamclient", "lsteamclient"))

BASE_RE = re.compile(r"\b(k_[iI]\w+Callbacks)\s*=\s*(\d+)")
MACRO_RE = re.compile(r"STEAM_CALLBACK_BEGIN\(\s*(\w+)\s*,\s*(k_[iI]\w+Callbacks)\s*(?:\+\s*(\d+))?\s*\)")
STRUCT_RE = re.compile(r"struct\s+(\w+)\s*\{\s*enum\s*\{\s*k_iCallback\s*=\s*(k_[iI]\w+Callbacks)\s*(?:\+\s*(\d+))?\s*\}")


def strip_comments(text):
    text = re.sub(r"/\*.*?\*/", "", text, flags=re.S)
    return re.sub(r"//[^\n]*", "", text)


def scan(sdk_dir):
    bases, found = {}, {}
    texts = []
    for path in glob.glob(os.path.join(sdk_dir, "*.h")):
        with open(path, "r", errors="replace") as f:
            texts.append(strip_comments(f.read()))
    for t in texts:
        for m in BASE_RE.finditer(t):
            bases.setdefault(m.group(1), int(m.group(2)))
    for t in texts:
        for rx in (MACRO_RE, STRUCT_RE):
            for m in rx.finditer(t):
                base = bases.get(m.group(2))
                if base is not None:
                    found[m.group(1)] = base + int(m.group(3) or 0)
    return found


def sdk_key(name):
    num = re.match(r"steamworks_sdk_(\d+)(.*)", name)
    return (int(num.group(1)), num.group(2))


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--root", default=DEFAULT_ROOT)
    ap.add_argument("--latest", default="steamworks_sdk_165")
    args = ap.parse_args()

    dirs = sorted((os.path.basename(d) for d in glob.glob(os.path.join(args.root, "steamworks_sdk_*"))), key=sdk_key)
    latest = scan(os.path.join(args.root, args.latest))
    latest_ids = {}
    for n, i in latest.items():
        latest_ids.setdefault(i, []).append(n)

    first_seen, last_seen, ids = {}, {}, {}
    for d in dirs:
        for name, cid in scan(os.path.join(args.root, d)).items():
            first_seen.setdefault(name, d)
            last_seen[name] = d
            ids[name] = cid

    removed = sorted((n for n in first_seen if n not in latest), key=lambda n: (sdk_key(first_seen[n]), ids[n], n))
    print("# %d callbacks in the newest SDK (%s); %d callbacks declared in older SDKs only" % (len(latest), args.latest, len(removed)))
    group = None
    for n in removed:
        if first_seen[n] != group:
            group = first_seen[n]
            print("\n[oldest %s]" % group.replace("steamworks_sdk_", ""))
        reused = latest_ids.get(ids[n])
        print("  %s  id=%d  last=%s%s" % (n, ids[n], last_seen[n].replace("steamworks_sdk_", ""),
                                         ("  (id reused by %s in newest)" % ",".join(reused)) if reused else ""))


if __name__ == "__main__":
    main()
