param(
    [string]$Configuration = "Debug",
    [string]$ServerProject = "$PSScriptRoot\..\..\..\SKYNET server.csproj",
    [string]$AssemblyPath = "$PSScriptRoot\..\..\..\bin\$Configuration\net8.0\SKYNET server.dll",
    [string]$OutputPath = "$PSScriptRoot\..\generated\dota.ts",
    [string]$ExtraMessageIdsPath = "$PSScriptRoot\..\contracts\extra-message-ids.json",
    [string]$RoutesPath = "$PSScriptRoot\..\contracts\routes.json"
)

$ErrorActionPreference = "Stop"

if (!(Test-Path -LiteralPath $AssemblyPath)) {
    dotnet build $ServerProject -c $Configuration --no-restore /nodeReuse:false
}

dotnet run --project "$PSScriptRoot\GcTsContractGenerator\GcTsContractGenerator.csproj" -- `
    --assembly $AssemblyPath `
    --output $OutputPath `
    --extra-message-ids $ExtraMessageIdsPath `
    --routes $RoutesPath
