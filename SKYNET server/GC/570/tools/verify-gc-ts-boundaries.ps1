$ErrorActionPreference = "Stop"

$root = Resolve-Path "$PSScriptRoot\.."
$businessRoots = @(
    Join-Path $root "modules"
)

$forbiddenPattern = '\b(decode|send|body|messageType|now|accountId|steamId|personaName|log)\s*\('
$violations = New-Object System.Collections.Generic.List[string]

foreach ($businessRoot in $businessRoots) {
    if (!(Test-Path -LiteralPath $businessRoot)) {
        continue
    }

    foreach ($file in Get-ChildItem -LiteralPath $businessRoot -Recurse -Filter "*.ts") {
        $matches = Select-String -LiteralPath $file.FullName -Pattern $forbiddenPattern -AllMatches
        foreach ($match in $matches) {
            $line = $match.Line.Trim()
            if ($line -match '\bctx\.(send|clock\.now|logger\.info|encode|decode)\s*\(') {
                continue
            }

            $violations.Add("$($file.FullName):$($match.LineNumber): $line")
        }
    }
}

if ($violations.Count -gt 0) {
    $violations | ForEach-Object { Write-Error $_ }
    throw "GC TS boundary check failed: business modules must use ctx APIs instead of host globals."
}

Write-Host "GC TS boundary check passed."
