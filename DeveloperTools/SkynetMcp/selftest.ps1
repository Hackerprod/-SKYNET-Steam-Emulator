# Offline check of skynet-mcp: stdio JSON-RPC handshake (initialize, tools/list, tools/call) against a synthetic
# capture. Starts no game and injects nothing. Build skynet-gc and skynet-mcp first (see DeveloperTools\SkynetGc\README.md).
#   powershell -File DeveloperTools\SkynetMcp\selftest.ps1 [-Cfg Debug]
param([string]$Cfg = "Debug")
$ErrorActionPreference = "Stop"
$dev = Split-Path -Parent $PSScriptRoot
$gc = Join-Path $dev "SkynetGc\bin\$Cfg\net8.0-windows\skynet-gc.exe"
$mcp = Join-Path $PSScriptRoot "bin\$Cfg\net8.0\skynet-mcp.exe"
$work = Join-Path ([IO.Path]::GetTempPath()) ("skynet-mcp-selftest-" + [guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory $work | Out-Null

& $gc iface selftest --keep $work | Out-Null
$cap = Join-Path $work "ok"
if (-not (Test-Path (Join-Path $cap "iface.jsonl"))) { throw "synthetic capture missing" }

# A one-record GC capture (ClientHello, empty body) for the gc_* tools.
$hdr = [BitConverter]::GetBytes([uint32](4006 + 2147483648)) + [BitConverter]::GetBytes([uint32]0)
$gcDir = Join-Path $work "gc"
New-Item -ItemType Directory $gcDir | Out-Null
'{"t":1,"dir":"send","type":4006,"proto":true,"size":8,"b64":"' + [Convert]::ToBase64String($hdr) + '"}' | Set-Content (Join-Path $gcDir "gc.jsonl") -Encoding ASCII

$psi = New-Object Diagnostics.ProcessStartInfo $mcp
$psi.UseShellExecute = $false
$psi.RedirectStandardInput = $true; $psi.RedirectStandardOutput = $true; $psi.RedirectStandardError = $true
$psi.StandardOutputEncoding = [Text.Encoding]::UTF8
$psi.EnvironmentVariables["SKYNET_GC_EXE"] = $gc
$p = [Diagnostics.Process]::Start($psi)
$errTask = $p.StandardError.ReadToEndAsync()
$script:id = 0
$fail = 0
function Send($method, $params, [bool]$notify = $false) {
    $m = @{ jsonrpc = "2.0"; method = $method }
    if ($params -ne $null) { $m.params = $params }
    if (-not $notify) { $script:id++; $m.id = $script:id }
    $p.StandardInput.WriteLine(($m | ConvertTo-Json -Depth 10 -Compress))
    $p.StandardInput.Flush()
    if ($notify) { return $null }
    $t = $p.StandardOutput.ReadLineAsync()
    if (-not $t.Wait(120000)) { throw "timeout waiting for $method" }
    return ($t.Result | ConvertFrom-Json)
}
function Check($name, $cond, $detail = "") {
    if ($cond) { Write-Host "ok   $name" } else { Write-Host "FAIL $name $detail"; $script:fail++ }
}
function Call($tool, $arguments) {
    $r = Send "tools/call" @{ name = $tool; arguments = $arguments }
    if ($r.error) { return "ERROR: " + $r.error.message }
    return ($r.result.content | ForEach-Object { $_.text }) -join "`n"
}

try {
    $init = Send "initialize" @{ protocolVersion = "2024-11-05"; capabilities = @{}; clientInfo = @{ name = "selftest"; version = "1" } }
    Check "initialize" ($init.result.serverInfo -ne $null) ($init | ConvertTo-Json -Depth 5)
    Send "notifications/initialized" $null $true | Out-Null
    $list = Send "tools/list" @{}
    $names = @($list.result.tools | ForEach-Object { $_.name }) | Sort-Object
    $expected = "emu_launch,game_ui,gc_analyze,gc_capture_start,gc_capture_stop,gc_decode,gc_messages,iface_diff,iface_mark,iface_query,iface_start,iface_status,iface_stop,iface_timeline".Split(",")
    Check "tools/list has the 14 tools" (($names -join ",") -eq ($expected -join ",")) ($names -join ",")
    Check "iface_start takes interfaces/methods" (@($list.result.tools | Where-Object { $_.name -eq "iface_start" -and $_.inputSchema.properties.interfaces -and $_.inputSchema.properties.methods }).Count -eq 1)
    Check "VAC sentence in iface_start and gc_capture_start" (@($list.result.tools | Where-Object { $_.name -in "iface_start", "gc_capture_start" -and $_.description -match "VAC risk" }).Count -eq 2)

    # Paging: a capture bigger than one page must be readable page by page without losing a record or the continuation.
    $big = Join-Path $work "big"
    New-Item -ItemType Directory $big | Out-Null
    $rows = @('{"k":"meta","v":1,"provider":"emulator","qfreq":10000000,"q0":0,"ifaces":[],"status":"ok"}')
    1..300 | ForEach-Object { $rows += '{"k":"call","seq":' + $_ + ',"q":' + ($_ * 1000) + ',"tid":1,"i":"SteamFriends017","m":"GetFriendRichPresence","a":["76561197960275660","some_long_key_' + $_ + '_xxxxxxxxxxxxxxxxxxxx"],"r":"value value value value","c":"client.dll+0x5A1B20"}' }
    $rows | Set-Content (Join-Path $big "iface.jsonl") -Encoding ASCII
    foreach ($tool in "iface_timeline", "iface_query") {
        $seen = @{}; $off = 0; $pages = 0; $maxLen = 0; $cut = 0
        while ($pages -lt 200) {
            $args2 = @{ dir = $big; offset = $off }
            if ($tool -eq "iface_query") { $args2.method = "GetFriendRichPresence" }
            $t = Call $tool $args2
            $pages++
            if ($t.Length -gt $maxLen) { $maxLen = $t.Length }
            if ($t -match "truncated=true") { $cut++ }
            foreach ($m in [regex]::Matches($t, "some_long_key_(\d+)_")) { $seen[$m.Groups[1].Value] = 1 }
            $n = [regex]::Match($t, "next_offset=(\d+)\s*$")
            if (-not $n.Success) { break }
            $off = [int]$n.Groups[1].Value
        }
        Check "$tool pages through all 300 records without loss ($pages pages, $cut cut by the size cap)" ($seen.Count -eq 300 -and $cut -gt 0 -and $pages -gt 1) "seen=$($seen.Count) pages=$pages cut=$cut"
        Check "$tool page stays compact" ($maxLen -lt 4400) "max=$maxLen"
    }    $t = Call "iface_status" @{ dir = $cap }
    Check "iface_status" ($t -match "provider=emulator status=ok" -and $t -match "records=42" -and $t -match "SteamUser023: patched=4") $t
    $t = Call "iface_status" @{}
    Check "iface_status (no dir) does not fail" ($t -notmatch "^ERROR") $t
    $t = Call "iface_timeline" @{ dir = $cap; method = "Relationship"; limit = 5 }
    Check "iface_timeline" ($t -match "GetFriendRelationship\(76561197960275660\) = 3 k_EFriendRelationshipFriend") $t
    $t = Call "iface_timeline" @{ dir = $cap; iface = "SteamUser" }
    Check "iface_timeline iface filter (generic interface)" ($t -match "SteamUser023.BLoggedOn\(\) = true" -and $t -notmatch "SteamFriends017") $t
    $t = Call "iface_query" @{ dir = $cap; method = "HasFriend|GetFriendCount" }
    Check "iface_query" ($t -match "HasFriend" -and $t -match "x2") $t
    $t = Call "iface_diff" @{ dir = $cap }
    Check "iface_diff" ($t -match "result: PASS" -and $t -match "mismatch: 0") $t
    $t = Call "iface_mark" @{ dir = $cap; label = "from mcp" }
    Check "iface_mark" ($t -match "^mark n=6") $t
    $t = Call "iface_stop" @{ dir = $cap }
    Check "iface_stop (already stopped capture)" ($t -match "^stopped records=") $t
    $t = Call "iface_start" @{ mode = "bogus" }
    Check "iface_start rejects a bad mode without launching" ($t -match "launch or attach") $t
    $t = Call "game_ui" @{ action = "bogus" }
    Check "game_ui rejects a bad action without touching the screen" ($t -match "action must be") $t
    $t = Call "gc_capture_stop" @{ dir = $gcDir }
    Check "gc_capture_stop without a session" ($t -match "no mcp-session.json") $t
    $t = Call "gc_analyze" @{ dir = $gcDir }
    Check "gc_analyze" ($t -match "files=1 decoded=1" -and $t -match "ClientHello") $t
    $t = Call "gc_messages" @{ dir = $gcDir }
    Check "gc_messages" ($t -match "^1 client ") $t
    $t = Call "gc_messages" @{ dir = $gcDir; sequence = 1 }
    Check "gc_messages sequence" ($t -match "GcMessageType") $t
    $t = Call "gc_decode" @{ type = 4006; body_b64 = "" }
    Check "gc_decode" ($t -notmatch "^ERROR" -and $t -notmatch "^exit") $t
} finally {
    try { $p.StandardInput.Close(); if (-not $p.WaitForExit(5000)) { $p.Kill() } } catch { }
    Remove-Item $work -Recurse -Force -ErrorAction SilentlyContinue
}
if ($fail -eq 0) { Write-Host "skynet-mcp selftest: all passed" } else { Write-Host "skynet-mcp selftest: $fail failed"; exit 1 }
