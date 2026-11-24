# CallbackLayoutCheck

Verifies that the managed callback and struct definitions in `steam_api` have the same ids, sizes and
field offsets as the real Steamworks SDK. Games read these structs as raw memory, so a wrong size or
offset corrupts data without any error.

Two parts:

1. `gen_expected.py` builds the reference from the real SDK 1.65 headers with the MSVC compiler
   (`expected/callbacks_x64.txt`, `expected/callbacks_x86.txt`).
2. `Program.cs` (the checker) loads the managed `steam_api.dll` by reflection and compares it against
   the reference for its own bitness.

## Run it

```powershell
DeveloperTools\CallbackLayoutCheck\run.ps1               # verify steam_api against the committed reference
DeveloperTools\CallbackLayoutCheck\run.ps1 -Regenerate   # rebuild the reference from the SDK headers first
```

`run.ps1` builds the checker for x64 and x86 (`bin\x64`, `bin\x86`) and runs both against
`steam_api/obj/Release/steam_api.dll`, so build `steam_api` in Release first. Exit code 0 means both
architectures match, 1 means mismatches, 3 means a build or generation failure.

The checker alone:

```
CallbackLayoutCheck verify <steam_api.dll> <toolDir>     # toolDir contains expected/, aliases.txt, waivers.txt
```

## Output

One line per finding, then a summary per architecture:

| Kind | Meaning |
|------|---------|
| `MISSING` | The SDK has the callback or struct, `steam_api` does not. |
| `ID-DIFF` | Same type, different callback id. |
| `SIZE-DIFF` | Same type, different `sizeof`. |
| `OFFSET-DIFF` | A field is at a different offset. |
| `OUR-ONLY` | `steam_api` defines it, the SDK does not (informational). |
| `NOTE` | Informational, for example implemented under another name with the same id. |
| `WAIVED <kind>` | A finding listed in `waivers.txt`; it does not fail the run. |

## Files

| File | Role |
|------|------|
| `Program.cs`, `CallbackLayoutCheck.csproj` | The checker (.NET Framework 4.7.2; `-p:Bitness=x86` builds the 32-bit one). |
| `run.ps1` | Build both architectures and verify. |
| `gen_expected.py` | Generates `expected/*.txt`: preprocesses the headers (`cl /EP`), parses every struct with `k_iCallback` plus those in `structs.txt`, compiles and runs a C++ probe per architecture that prints `Name\|callbackId\|sizeof\|field=offset;...`. Needs the MSVC Build Tools and Python. |
| `all_headers.h` | The headers fed to the preprocessor. |
| `structs.txt` | Non-callback SDK structs to verify as well. `Name=ManagedName` when the managed type is named differently. |
| `aliases.txt` | `ManagedName=HeaderName` for managed types whose name differs from the SDK struct. |
| `waivers.txt` | `<KIND> <Name>: <reason>` for findings deliberately left as they are. Empty means no waivers. |
| `old_sdk_callbacks.py` | Lists callbacks that exist only in older SDK header sets (not in 1.65), grouped by the oldest SDK that declares them. A game built against an older SDK may still register them. |
| `expected/` | Generated reference. Do not edit; regenerate. |

## Requirements for `gen_expected.py`

- Visual Studio Build Tools with the C++ toolchain. The path of `vcvars64.bat`/`vcvars32.bat` is the
  `VC_BUILD` constant at the top of the script; adjust it for your installation.
- The SDK headers. Default: `.tmp/ProtonLsteamclient/lsteamclient/steamworks_sdk_165` (git-ignored
  reference material); override with `--sdk <dir>`. `--arch x64|x86|both` selects the architecture.

`old_sdk_callbacks.py` reads the same `.tmp/ProtonLsteamclient/lsteamclient` tree (`--root`, `--latest`).
