param(
    [Parameter(Mandatory = $true)]
    [uint32]$AppId,
    [string]$Configuration = "Debug",
    [string]$ServerProject,
    [string]$AssemblyPath,
    [string]$OutputPath,
    [string]$ExtraMessageIdsPath,
    [string]$RoutesPath
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($ServerProject)) {
    $ServerProject = Join-Path $PSScriptRoot "..\..\SKYNET server.csproj"
}

if ([string]::IsNullOrWhiteSpace($AssemblyPath)) {
    $AssemblyPath = Join-Path $PSScriptRoot "..\..\bin\$Configuration\net8.0\SKYNET server.dll"
}

function Resolve-AppRelativePath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$AppRoot,
        [Parameter(Mandatory = $true)]
        [string]$RelativePath,
        [Parameter(Mandatory = $true)]
        [string]$FieldName
    )

    if ([System.IO.Path]::IsPathRooted($RelativePath)) {
        throw "GC manifest typeScript.$FieldName must be relative: $RelativePath"
    }

    $rootFullPath = [System.IO.Path]::GetFullPath($AppRoot).TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar)
    $fullPath = [System.IO.Path]::GetFullPath((Join-Path $rootFullPath $RelativePath))
    if (!$fullPath.StartsWith($rootFullPath + [System.IO.Path]::DirectorySeparatorChar, [System.StringComparison]::OrdinalIgnoreCase) -and
        ![string]::Equals($fullPath, $rootFullPath, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "GC manifest typeScript.$FieldName escapes the app root: $RelativePath"
    }

    return $fullPath
}

$appRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot "..\$AppId"))
if (!(Test-Path -LiteralPath $appRoot -PathType Container)) {
    throw "GC app folder was not found: $appRoot"
}

$manifestPath = Join-Path $appRoot "gc.json"
$manifest = $null
if (Test-Path -LiteralPath $manifestPath -PathType Leaf) {
    $manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
    if ($null -ne $manifest.appId -and [uint32]$manifest.appId -ne $AppId) {
        throw "GC manifest appId $($manifest.appId) does not match requested app id ${AppId}: $manifestPath"
    }
}

$typeScript = if ($null -ne $manifest -and $null -ne $manifest.typeScript) { $manifest.typeScript } else { $null }
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $relative = if ($null -ne $typeScript -and ![string]::IsNullOrWhiteSpace($typeScript.generatedContracts)) { $typeScript.generatedContracts } else { "generated/contracts.ts" }
    $OutputPath = Resolve-AppRelativePath $appRoot $relative "generatedContracts"
}

if ([string]::IsNullOrWhiteSpace($ExtraMessageIdsPath)) {
    $relative = if ($null -ne $typeScript -and ![string]::IsNullOrWhiteSpace($typeScript.extraMessageIds)) { $typeScript.extraMessageIds } else { "contracts/extra-message-ids.json" }
    $ExtraMessageIdsPath = Resolve-AppRelativePath $appRoot $relative "extraMessageIds"
}

if ([string]::IsNullOrWhiteSpace($RoutesPath)) {
    $relative = if ($null -ne $typeScript -and ![string]::IsNullOrWhiteSpace($typeScript.routes)) { $typeScript.routes } else { "contracts/routes.json" }
    $RoutesPath = Resolve-AppRelativePath $appRoot $relative "routes"
}

if (!(Test-Path -LiteralPath $AssemblyPath -PathType Leaf)) {
    dotnet build $ServerProject -c $Configuration --no-restore /nodeReuse:false
}

$generatorProject = Join-Path $PSScriptRoot "GcTsContractGenerator\GcTsContractGenerator.csproj"
dotnet run --project $generatorProject -- `
    --app-id $AppId `
    --generator "GC/tools/generate-contracts.ps1" `
    --app-root $appRoot `
    --manifest $manifestPath `
    --assembly $AssemblyPath `
    --output $OutputPath `
    --extra-message-ids $ExtraMessageIdsPath `
    --routes $RoutesPath
