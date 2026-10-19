#!/usr/bin/env python3
"""Compare our golden layouts with Valve's Proton lsteamclient Windows vtables and the real
steamclient64 slot counts. Usage: python compare_proton.py [repo_root]"""
import glob, os, re, sys

root = sys.argv[1] if len(sys.argv) > 1 else os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
proton = os.path.join(root, ".tmp", "ProtonLsteamclient", "lsteamclient")
golden = os.path.join(root, "DeveloperTools", "InterfaceLayoutCheck", "golden")
counts = os.path.join(root, "DeveloperTools", "SteamClientVtableProbe", "steamclient64_slot_counts.txt")

def norm(n):
    n = re.sub(r"__V\d+$", "", n)
    n = re.sub(r"_[23]$", "", n)
    return "Destructor" if n.lower() in ("destructor", "~dtor") else n

# Proton: version -> (iface, [names])
prot = {}
for f in glob.glob(os.path.join(proton, "win*.c")):
    txt = open(f, encoding="utf-8", errors="replace").read()
    for m in re.finditer(r"__ASM_VTABLE\((win(ISteam\w+?)_((?:STEAM|Steam|STEAMGAMESERVER)\w*)),(.*?)\n\s*\);", txt, re.S):
        full, iface, ver, body = m.groups()
        pref = full + "_"
        names = [x[len(pref):] if x.startswith(pref) else x for x in re.findall(r"VTABLE_ADD_FUNC\((\w+)\)", body)]
        prot[ver] = (iface, names)

real = {}
for l in open(counts):
    m = re.match(r"CAdapterSteam(\w+?)(\d{3}) .*slots=(\d+)", l)
    if m:
        real[(m.group(1), m.group(2))] = int(m.group(3))

def real_count(iface, ver):
    fam = iface[len("ISteam"):]
    m = re.search(r"(\d{3})$", ver)
    return real.get((fam, m.group(1))) if m else None

def load_golden(v):
    out = []
    for l in open(os.path.join(golden, v + ".txt"), encoding="utf-8").read().splitlines():
        if l.strip():
            out.append(norm(l.split("|")[1]))
    return out

def canon(n):
    n = n.lower()
    for suf in ("_deprecated", "_old", "old", "_"):
        while n.endswith(suf) and len(n) > len(suf):
            n = n[:-len(suf)]
    n = re.sub(r"_\d$", "", n)
    n = re.sub(r"(float|int32|int64|double|bool|string|cursor|page|001)$", "", n)
    return n

def same(a, b):
    if a == b:
        return True
    ca, cb = canon(a), canon(b)
    return ca == cb or ca.startswith(cb) or cb.startswith(ca)

ndiff = 0
naming_only = 0
common = 0
for v in sorted(prot):
    if not os.path.exists(os.path.join(golden, v + ".txt")):
        continue
    common += 1
    iface, pn = prot[v]
    pn = [norm(x) for x in pn]
    g = load_golden(v)
    rc = real_count(iface, v)
    msgs = []
    if len(g) != len(pn):
        msgs.append(f"COUNT ours={len(g)} proton={len(pn)}")
    if rc is not None and rc != len(g):
        msgs.append(f"REAL-COUNT ours={len(g)} real={rc}")
    if rc is not None and rc != len(pn):
        msgs.append(f"proton={len(pn)} != real={rc}")
    for i in range(min(len(g), len(pn))):
        if g[i] != pn[i]:
            if same(g[i], pn[i]):
                naming_only += 1
            else:
                msgs.append(f"  [{i}] ours={g[i]} proton={pn[i]}")
    if len(g) != len(pn):
        a, b = (g, pn)
        if len(pn) == len(g) + 1 and pn[-1] == "Destructor" and rc == len(g):
            msgs.append("note: Proton has a trailing C++ destructor the real client lacks (Proton artifact)")
        for i in range(min(len(a), len(b)), max(len(a), len(b))):
            msgs.append(f"  [{i}] ours={g[i] if i < len(g) else '-'} proton={pn[i] if i < len(pn) else '-'}")
    if msgs:
        ndiff += 1
        print(f"{v}:")
        for m_ in msgs:
            print("   " + m_.strip() if not m_.startswith("  [") else m_)
print(f"compared {common} versions, {ndiff} with differences, {naming_only} naming-only slot differences suppressed")

# Second pass: every golden vs real client slot count (independent of Proton coverage).
print("--- golden vs real steamclient64 slot counts ---")
realkeys = {(f.lower() + n): c for (f, n), c in real.items()}
nreal = 0
for fn in sorted(os.listdir(golden)):
    v = fn[:-4]
    key = re.sub(r"_INTERFACE_VERSION_?|_INTERFACE_V_?|^steam", "", v, flags=re.I).lower()
    key = key.replace("_", "")
    cnt = len(load_golden(v))
    for k, c in realkeys.items():
        if k == key:
            nreal += 1
            if c != cnt:
                print(f"{v}: ours={cnt} real={c}")
print(f"checked {nreal} goldens against real client")
