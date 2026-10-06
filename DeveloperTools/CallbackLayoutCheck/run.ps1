# Builds the checker for x64 and x86 and verifies steam_api/obj/Release/steam_api.dll against both expected files.
# Pass -Regenerate to rebuild expected/*.txt from the SDK headers first (needs MSVC BuildTools + python).
param([switch]$Regenerate)
$ErrorActionPreference = 'Stop'
$tool = $PSScriptRoot
$dll = Join-Path $tool '..\..\steam_api\obj\Release\steam_api.dll'
if ($Regenerate) { python (Join-Path $tool 'gen_expected.py'); if ($LASTEXITCODE -ne 0) { exit 3 } }
$rc = 0
foreach ($b in 'x64', 'x86') {
    dotnet build (Join-Path $tool 'CallbackLayoutCheck.csproj') -c Release "-p:Bitness=$b" -v q --nologo | Out-Null
    if ($LASTEXITCODE -ne 0) { Write-Host "build failed ($b)"; exit 3 }
    & (Join-Path $tool "bin\$b\CallbackLayoutCheck.exe") verify $dll $tool
    if ($LASTEXITCODE -ne 0) { $rc = 1 }
}
exit $rc
