# InterfaceLayoutGen

Generates the vtable specs that [`InterfaceLayoutCheck`](../InterfaceLayoutCheck/README.md) compares
`steam_api` against, by parsing Valve's real Steamworks headers instead of trusting hand-written lists.

One script: `gen_header_spec.py` (Python 3, standard library only).

```powershell
python DeveloperTools/InterfaceLayoutGen/gen_header_spec.py                       # generate every spec and calibrate
python DeveloperTools/InterfaceLayoutGen/gen_header_spec.py --only SteamUser019   # one version, verbose
python DeveloperTools/InterfaceLayoutGen/gen_header_spec.py --no-write            # calibrate only, write nothing
```

Run it from the repository root.

## Output

`DeveloperTools/InterfaceLayoutCheck/header-spec/<Version>.txt`, one line per vtable slot:

```
index|MethodName|paramCount|returnType|paramTypes
```

Each file starts with `#` comments naming the source header(s), any C++ overload groups
(`# overload <name> slots a,b`), warnings when sources disagree, and a `# note:` when the spec comes from
a newer SDK than the usual source.

After generating, it calibrates: the ordered method names are compared with the trusted
`InterfaceLayoutCheck/golden/*.txt`, ignoring the leading `this` pointer and trailing `__V...` suffixes.

## Sources (all under the repository root)

Reference material is not committed; put it under `.tmp/` (git-ignored):

| Path | Content |
|------|---------|
| `.tmp/goldberg_emulator_ref/sdk_includes/` | Per-version headers `isteam<name><NNN>.h`; preferred when present. |
| `.tmp/ProtonLsteamclient/lsteamclient/steamworks_sdk_<NNN>/` | Valve's own Proton repository, SDK 0.99u to 1.65. |
| `.tmp/SmokeAPI/res/steamworks/<sdk>/headers/steam/` | One header per interface per SDK release. |
| `Tools/steamworks_sdk_164/sdk/public/steam/` | SDK 1.64. |
| `.tmp/goldberg_emulator/gse_fork/sdk/steam/` | Last resort, only for versions no other source defines. |

A version string comes from `#define <STEM>_INTERFACE_VERSION... "<string>"`; the class is the one in the
same header. Every source that defines a version is parsed and disagreements in the method order are
reported (the preferred source wins).

## Parsing rules that matter

- Comments are stripped and `#if/#ifdef/#elif/#else` are evaluated for a Windows PC build (`_WIN32`,
  `STEAM_WIN32`, `_MSC_VER` defined; PS3/POSIX/Linux undefined).
- Every `virtual` declaration takes a slot, including hidden `STEAM_PRIVATE_API(...)` methods and a
  virtual destructor (`ISteamHTMLSurface` slot 0).
- MSVC overload rule: for a group of same-named virtuals, the slot positions stay where declared and the
  group's members are assigned to them in reverse declaration order. This is what real Windows binaries
  do, and was confirmed against the real client (see
  [`SteamClientVtableProbe`](../SteamClientVtableProbe/README.md)).
- `STEAM_FLAT_NAME(X)` becomes the method name; other annotation macros are dropped.
- Each parsed class is cross-checked: the number of `virtual` tokens must equal the parsed method count,
  otherwise it reports `PARSE-ERROR`.

## Policy: existing specs do not change

A version that any source already defines keeps exactly its previously chosen source, so regenerating
never reshuffles older specs. The one exception is when the newest SDK (`proton165`) defines the same
version string with a method list that strictly extends the chosen one (additive trailing methods, for
example `STEAMAPPS_INTERFACE_VERSION009` in SDK 1.65); the spec is then regenerated from it and says so.
