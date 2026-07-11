[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$InputPath,

    [uint32]$AppId = 570,

    [string]$OutputRoot = "",

    [switch]$Overwrite
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($OutputRoot)) {
    $gcRoot = Split-Path -Parent $PSScriptRoot
    $OutputRoot = Join-Path $gcRoot $AppId
}

$inputFullPath = [System.IO.Path]::GetFullPath($InputPath)
$outputFullPath = [System.IO.Path]::GetFullPath($OutputRoot)
$fixtureRoot = Join-Path $outputFullPath "fixtures\nethook"
$captureRoot = Join-Path $outputFullPath "captures"

New-Item -ItemType Directory -Path $fixtureRoot -Force | Out-Null
New-Item -ItemType Directory -Path $captureRoot -Force | Out-Null

function Get-PropertyValue {
    param(
        [Parameter(Mandatory = $true)]$Object,
        [Parameter(Mandatory = $true)][string[]]$Names
    )

    if ($null -eq $Object) {
        return $null
    }

    foreach ($name in $Names) {
        $property = $Object.PSObject.Properties[$name]
        if ($null -ne $property) {
            return $property.Value
        }
    }

    return $null
}

function Convert-ToUInt32OrNull {
    param($Value)

    if ($null -eq $Value) {
        return $null
    }

    if ($Value -is [int] -or $Value -is [long] -or $Value -is [uint32] -or $Value -is [uint64]) {
        return [uint32]$Value
    }

    $text = [string]$Value
    if ($text -match "([0-9]{1,6})") {
        return [uint32]$Matches[1]
    }

    return $null
}

function Get-MessageTypeFromName {
    param([string]$Name)

    $patterns = @(
        "(?i)(?:emsg|msg|message|gc)[^0-9]{0,12}([0-9]{2,6})",
        "(?<![0-9])([0-9]{4,5})(?![0-9])",
        "(?<![0-9])([0-9]{2})(?![0-9])"
    )

    foreach ($pattern in $patterns) {
        if ($Name -match $pattern) {
            $value = [uint32]$Matches[1]
            if ($value -gt 0 -and $value -lt 1000000) {
                return $value
            }
        }
    }

    return $null
}

function Get-DirectionFromPath {
    param([string]$Path)

    $value = $Path.ToLowerInvariant()
    if ($value -in @("out", "sent", "send", "client", "clienttogc")) {
        return "client"
    }

    if ($value -in @("in", "recv", "received", "server", "gctoclient")) {
        return "server"
    }

    if ($value -match "\\(out|sent|send|client|clienttogc)\\" -or $value -match "(^|[_\-.])(out|sent|send|clienttogc)([_\-.]|$)") {
        return "client"
    }

    if ($value -match "\\(in|recv|received|server|gctoclient)\\" -or $value -match "(^|[_\-.])(in|recv|received|gctoclient)([_\-.]|$)") {
        return "server"
    }

    return "unknown"
}

function Convert-HexToBytes {
    param([string]$Hex)

    $clean = ($Hex -replace "[^0-9a-fA-F]", "")
    if ([string]::IsNullOrWhiteSpace($clean)) {
        return ,[byte[]]::new(0)
    }

    if (($clean.Length % 2) -ne 0) {
        throw "Odd-length hex payload."
    }

    $bytes = [byte[]]::new($clean.Length / 2)
    for ($i = 0; $i -lt $bytes.Length; $i++) {
        $bytes[$i] = [Convert]::ToByte($clean.Substring($i * 2, 2), 16)
    }

    return ,$bytes
}

function Get-BodyFromRawPacket {
    param(
        [byte[]]$Raw,
        [Nullable[uint32]]$ExpectedMessageType
    )

    if ($Raw.Length -lt 8) {
        return ,$Raw
    }

    $embedded = [BitConverter]::ToUInt32($Raw, 0) -band 0x7fffffff
    $headerLength = [BitConverter]::ToUInt32($Raw, 4)
    $offset = 8 + [int64]$headerLength

    if ($offset -le $Raw.Length -and $embedded -gt 0 -and $embedded -lt 1000000) {
        if ($null -eq $ExpectedMessageType -or $embedded -eq [uint32]$ExpectedMessageType) {
            $bodyLength = $Raw.Length - [int]$offset
            $body = [byte[]]::new($bodyLength)
            if ($bodyLength -gt 0) {
                [Array]::Copy($Raw, [int]$offset, $body, 0, $bodyLength)
            }

            return ,$body
        }
    }

    return ,$Raw
}

function Copy-ByteRange {
    param(
        [byte[]]$Source,
        [int]$Offset,
        [int]$Length
    )

    if ($Length -le 0) {
        return ,[byte[]]::new(0)
    }

    if ($Offset -lt 0 -or $Length -lt 0 -or ($Offset + $Length) -gt $Source.Length) {
        throw "Byte range is outside the source buffer."
    }

    $result = [byte[]]::new($Length)
    [Array]::Copy($Source, $Offset, $result, 0, $Length)
    return ,$result
}

function Read-Varint {
    param(
        [byte[]]$Source,
        [ref]$Index
    )

    [uint64]$value = 0
    $shift = 0
    while ($Index.Value -lt $Source.Length -and $shift -lt 64) {
        $current = [byte]$Source[$Index.Value]
        $Index.Value = $Index.Value + 1
        $value = $value -bor (([uint64]($current -band 0x7f)) -shl $shift)
        if (($current -band 0x80) -eq 0) {
            return $value
        }

        $shift += 7
    }

    throw "Invalid protobuf varint."
}

function Skip-ProtobufField {
    param(
        [byte[]]$Source,
        [ref]$Index,
        [int]$WireType
    )

    switch ($WireType) {
        0 {
            [void](Read-Varint $Source $Index)
            return
        }
        1 {
            $Index.Value = $Index.Value + 8
        }
        2 {
            $length = [int](Read-Varint $Source $Index)
            $Index.Value = $Index.Value + $length
        }
        5 {
            $Index.Value = $Index.Value + 4
        }
        default {
            throw "Unsupported protobuf wire type: $WireType"
        }
    }

    if ($Index.Value -gt $Source.Length) {
        throw "Protobuf field extends past the end of the buffer."
    }
}

function Read-LengthDelimitedBytes {
    param(
        [byte[]]$Source,
        [ref]$Index
    )

    $length = [int](Read-Varint $Source $Index)
    $value = Copy-ByteRange $Source $Index.Value $length
    $Index.Value = $Index.Value + $length
    return ,$value
}

function Read-CMsgGCClient {
    param([byte[]]$Source)

    $index = 0
    $appId = $null
    $rawMsgType = $null
    $payload = [byte[]]::new(0)
    $steamId = $null
    $gcName = ""

    while ($index -lt $Source.Length) {
        $tag = Read-Varint $Source ([ref]$index)
        $fieldNumber = [int]($tag -shr 3)
        $wireType = [int]($tag -band 7)

        switch ($fieldNumber) {
            1 {
                if ($wireType -ne 0) { Skip-ProtobufField $Source ([ref]$index) $wireType; break }
                $appId = [uint32](Read-Varint $Source ([ref]$index))
            }
            2 {
                if ($wireType -ne 0) { Skip-ProtobufField $Source ([ref]$index) $wireType; break }
                $rawMsgType = [uint32](Read-Varint $Source ([ref]$index))
            }
            3 {
                if ($wireType -ne 2) { Skip-ProtobufField $Source ([ref]$index) $wireType; break }
                $payload = Read-LengthDelimitedBytes $Source ([ref]$index)
            }
            4 {
                if ($wireType -ne 1) { Skip-ProtobufField $Source ([ref]$index) $wireType; break }
                if (($index + 8) -gt $Source.Length) { throw "CMsgGCClient steamid extends past the end of the buffer." }
                $steamId = [BitConverter]::ToUInt64($Source, $index)
                $index += 8
            }
            5 {
                if ($wireType -ne 2) { Skip-ProtobufField $Source ([ref]$index) $wireType; break }
                $gcNameBytes = Read-LengthDelimitedBytes $Source ([ref]$index)
                $gcName = [System.Text.Encoding]::UTF8.GetString($gcNameBytes)
            }
            default {
                Skip-ProtobufField $Source ([ref]$index) $wireType
            }
        }
    }

    if ($null -eq $appId -or $null -eq $rawMsgType) {
        return $null
    }

    return [pscustomobject]@{
        AppId = [uint32]$appId
        RawMessageType = [uint32]$rawMsgType
        MessageType = ([uint32]$rawMsgType -band [uint32]2147483647)
        Protobuf = (([uint32]$rawMsgType -band [uint32]2147483648) -ne 0)
        Payload = $payload
        SteamId = $steamId
        GcName = $gcName
    }
}

function Read-GcProtoHeaderJobIds {
    param([byte[]]$Header)

    $index = 0
    $sourceJobId = $null
    $targetJobId = $null

    while ($index -lt $Header.Length) {
        $tag = Read-Varint $Header ([ref]$index)
        $fieldNumber = [int]($tag -shr 3)
        $wireType = [int]($tag -band 7)

        if (($fieldNumber -eq 10 -or $fieldNumber -eq 11) -and $wireType -eq 1) {
            if (($index + 8) -gt $Header.Length) {
                throw "GC protobuf header job id extends past the end of the buffer."
            }

            $value = [BitConverter]::ToUInt64($Header, $index)
            $index += 8
            if ($fieldNumber -eq 10) {
                $sourceJobId = $value
            }
            else {
                $targetJobId = $value
            }

            continue
        }

        Skip-ProtobufField $Header ([ref]$index) $wireType
    }

    return [pscustomobject]@{
        SourceJobId = $sourceJobId
        TargetJobId = $targetJobId
    }
}

function Get-GCBodyFromWrappedPayload {
    param(
        [uint32]$RawMessageType,
        [byte[]]$Payload
    )

    $messageType = $RawMessageType -band [uint32]2147483647
    $protobuf = (($RawMessageType -band [uint32]2147483648) -ne 0)
    $headerLength = 0
    $sourceJobId = $null
    $targetJobId = $null

    if ($protobuf -and $Payload.Length -ge 8) {
        $embeddedMsg = [BitConverter]::ToUInt32($Payload, 0) -band [uint32]2147483647
        $candidateHeaderLength = [BitConverter]::ToInt32($Payload, 4)
        $offset = 8 + $candidateHeaderLength

        if ($embeddedMsg -eq $messageType -and $candidateHeaderLength -ge 0 -and $offset -le $Payload.Length) {
            $headerLength = $candidateHeaderLength
            $header = Copy-ByteRange $Payload 8 $headerLength
            $jobIds = Read-GcProtoHeaderJobIds $header
            $sourceJobId = $jobIds.SourceJobId
            $targetJobId = $jobIds.TargetJobId
            $body = Copy-ByteRange $Payload $offset ($Payload.Length - $offset)

            return [pscustomobject]@{
                Body = $body
                MessageType = [uint32]$messageType
                RawMessageType = [uint32]$RawMessageType
                Protobuf = $true
                HeaderLength = $headerLength
                SourceJobId = $sourceJobId
                TargetJobId = $targetJobId
            }
        }
    }

    if (-not $protobuf -and $Payload.Length -ge 18) {
        $headerVersion = [BitConverter]::ToUInt16($Payload, 0)
        if ($headerVersion -eq 1) {
            $targetJobId = [BitConverter]::ToUInt64($Payload, 2)
            $sourceJobId = [BitConverter]::ToUInt64($Payload, 10)
            $body = Copy-ByteRange $Payload 18 ($Payload.Length - 18)

            return [pscustomobject]@{
                Body = $body
                MessageType = [uint32]$messageType
                RawMessageType = [uint32]$RawMessageType
                Protobuf = $false
                HeaderLength = 18
                SourceJobId = $sourceJobId
                TargetJobId = $targetJobId
            }
        }
    }

    return [pscustomobject]@{
        Body = $Payload
        MessageType = [uint32]$messageType
        RawMessageType = [uint32]$RawMessageType
        Protobuf = $protobuf
        HeaderLength = 0
        SourceJobId = $sourceJobId
        TargetJobId = $targetJobId
    }
}

function Get-JsonItems {
    param([string]$Path)

    $json = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    if ($json -is [System.Array]) {
        return $json
    }

    foreach ($name in @("messages", "Messages", "records", "Records", "packets", "Packets")) {
        $value = Get-PropertyValue $json @($name)
        if ($value -is [System.Array]) {
            return $value
        }
    }

    return @($json)
}

function Get-PayloadBytesFromJson {
    param($Item)

    $base64 = Get-PropertyValue $Item @("BodyBase64", "PayloadBase64", "DataBase64", "body_base64", "payload_base64", "data_base64", "bodyBase64", "payloadBase64")
    if (-not [string]::IsNullOrWhiteSpace([string]$base64)) {
        return ,[Convert]::FromBase64String([string]$base64)
    }

    $hex = Get-PropertyValue $Item @("BodyHex", "PayloadHex", "DataHex", "body_hex", "payload_hex", "data_hex", "bodyHex", "payloadHex")
    if (-not [string]::IsNullOrWhiteSpace([string]$hex)) {
        return Convert-HexToBytes ([string]$hex)
    }

    return $null
}

function Save-Record {
    param(
        [System.Collections.Generic.List[object]]$Records,
        [uint32]$MessageType,
        [string]$Direction,
        [byte[]]$RawPayload,
        [string]$SourceFile,
        [int]$Index,
        [Nullable[uint32]]$OuterMessageType = $null,
        [Nullable[uint32]]$RawMessageType = $null,
        [bool]$Protobuf = $false,
        [Nullable[uint64]]$SteamId = $null,
        [string]$GcName = "",
        [Nullable[uint64]]$SourceJobId = $null,
        [Nullable[uint64]]$TargetJobId = $null,
        [int]$HeaderLength = 0
    )

    $safeDirection = if ([string]::IsNullOrWhiteSpace($Direction)) { "unknown" } else { $Direction }
    $fixtureName = "{0}_{1}_{2:D4}.bin" -f $safeDirection, $MessageType, $Index
    $fixturePath = Join-Path $fixtureRoot $fixtureName
    if ((Test-Path -LiteralPath $fixturePath) -and -not $Overwrite) {
        throw "Fixture already exists: $fixturePath. Use -Overwrite to replace it."
    }

    if ($null -eq $RawPayload) {
        $payload = [byte[]]::new(0)
    }
    else {
        $payload = [byte[]]$RawPayload
    }
    [System.IO.File]::WriteAllBytes($fixturePath, $payload)
    $relativeFixture = "fixtures/nethook/$fixtureName"

    $Records.Add([pscustomobject]@{
        app_id = $AppId
        message_type = $MessageType
        direction = $safeDirection
        fixture = $relativeFixture
        size = $payload.Length
        source_file = $SourceFile
        outer_message_type = $OuterMessageType
        raw_message_type = $RawMessageType
        protobuf = $Protobuf
        steam_id = $SteamId
        gc_name = $GcName
        source_job_id = $SourceJobId
        target_job_id = $TargetJobId
        header_length = $HeaderLength
    })
}

$records = [System.Collections.Generic.List[object]]::new()
$nextIndexByKey = @{}
$files = Get-ChildItem -LiteralPath $inputFullPath -Recurse -File

foreach ($file in $files) {
    $extension = $file.Extension.ToLowerInvariant()
    if ($extension -notin @(".bin", ".dat", ".msg", ".payload", ".json")) {
        continue
    }

    $relativeSource = Resolve-Path -LiteralPath $file.FullName -Relative
    $direction = Get-DirectionFromPath $file.FullName

    if ($extension -eq ".json") {
        foreach ($item in Get-JsonItems $file.FullName) {
            $itemAppId = Convert-ToUInt32OrNull (Get-PropertyValue $item @("AppId", "app_id", "appid", "GameAppId"))
            if ($null -ne $itemAppId -and $itemAppId -ne $AppId) {
                continue
            }

            $msgType = Convert-ToUInt32OrNull (Get-PropertyValue $item @("MessageType", "MsgType", "EMsg", "EMsgID", "message_type", "msg_type", "msg"))
            if ($null -eq $msgType) {
                $msgType = Get-MessageTypeFromName $file.Name
            }

            $payload = Get-PayloadBytesFromJson $item
            if ($null -eq $msgType -or $null -eq $payload) {
                continue
            }

            $itemDirection = Get-PropertyValue $item @("Direction", "direction", "Flow", "flow")
            if (-not [string]::IsNullOrWhiteSpace([string]$itemDirection)) {
                $direction = Get-DirectionFromPath ([string]$itemDirection)
            }

            $key = "$direction/$msgType"
            if (-not $nextIndexByKey.ContainsKey($key)) {
                $nextIndexByKey[$key] = 0
            }

            $nextIndexByKey[$key] = $nextIndexByKey[$key] + 1
            $body = Get-BodyFromRawPacket $payload $msgType
            Save-Record $records $msgType $direction $body $relativeSource $nextIndexByKey[$key]
        }

        continue
    }

    $msgType = Get-MessageTypeFromName $file.Name
    $raw = [System.IO.File]::ReadAllBytes($file.FullName)
    if ($null -eq $msgType -and $raw.Length -ge 4) {
        $embedded = [BitConverter]::ToUInt32($raw, 0) -band 0x7fffffff
        if ($embedded -gt 0 -and $embedded -lt 1000000) {
            $msgType = [uint32]$embedded
        }
    }

    if ($null -eq $msgType) {
        continue
    }

    $key = "$direction/$msgType"
    if (-not $nextIndexByKey.ContainsKey($key)) {
        $nextIndexByKey[$key] = 0
    }

    $body = Get-BodyFromRawPacket $raw $msgType
    if ($msgType -eq 5452 -or $msgType -eq 5453) {
        $gcClient = Read-CMsgGCClient $body
        if ($null -ne $gcClient -and $gcClient.AppId -eq $AppId) {
            $inner = Get-GCBodyFromWrappedPayload $gcClient.RawMessageType $gcClient.Payload
            $innerDirection = if ($msgType -eq 5452) { "client" } else { "server" }
            $innerKey = "$innerDirection/$($inner.MessageType)"
            if (-not $nextIndexByKey.ContainsKey($innerKey)) {
                $nextIndexByKey[$innerKey] = 0
            }

            $nextIndexByKey[$innerKey] = $nextIndexByKey[$innerKey] + 1
            Save-Record `
                -Records $records `
                -MessageType $inner.MessageType `
                -Direction $innerDirection `
                -RawPayload $inner.Body `
                -SourceFile $relativeSource `
                -Index $nextIndexByKey[$innerKey] `
                -OuterMessageType $msgType `
                -RawMessageType $inner.RawMessageType `
                -Protobuf $inner.Protobuf `
                -SteamId $gcClient.SteamId `
                -GcName $gcClient.GcName `
                -SourceJobId $inner.SourceJobId `
                -TargetJobId $inner.TargetJobId `
                -HeaderLength $inner.HeaderLength
            continue
        }
    }

    $nextIndexByKey[$key] = $nextIndexByKey[$key] + 1
    Save-Record $records $msgType $direction $body $relativeSource $nextIndexByKey[$key]
}

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$capturePath = Join-Path $captureRoot "nethook_$timestamp.json"
$records | Sort-Object direction, message_type, fixture | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $capturePath -Encoding UTF8

$routePath = Join-Path $outputFullPath "routes.generated.lua"
$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add("-- Generated by Import-NetHook.ps1. Review before including from main.lua.")
$lines.Add("OBSERVED = OBSERVED or {}")
$lines.Add("OBSERVED.client = OBSERVED.client or {}")
$lines.Add("OBSERVED.server = OBSERVED.server or {}")
$lines.Add("OBSERVED.unknown = OBSERVED.unknown or {}")
$lines.Add("")

foreach ($group in ($records | Sort-Object direction, message_type, fixture | Group-Object direction, message_type)) {
    $directionName = [string]$group.Group[0].direction
    $messageType = [uint32]$group.Group[0].message_type
    $fixtureList = ($group.Group | ForEach-Object { '"' + $_.fixture.Replace('\', '/') + '"' }) -join ", "
    $lines.Add(("OBSERVED.{0}[{1}] = {{ {2} }}" -f $directionName, $messageType, $fixtureList))
}

$lines.Add("")
$lines.Add("function observed_fixture(direction, message_type, index)")
$lines.Add("    local bucket = OBSERVED[direction]")
$lines.Add("    if bucket == nil or bucket[message_type] == nil then return nil end")
$lines.Add("    local selected = bucket[message_type][index or 1]")
$lines.Add("    if selected == nil then return nil end")
$lines.Add("    return runtime.Fixture(selected)")
$lines.Add("end")

Set-Content -LiteralPath $routePath -Value $lines -Encoding UTF8

Write-Host ("Imported {0} NetHook payloads for AppID {1}" -f $records.Count, $AppId)
Write-Host "Capture index: $capturePath"
Write-Host "Lua index: $routePath"
