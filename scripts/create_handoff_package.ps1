param(
    [string]$Milestone = "M13",
    [string]$OutputPath = $null
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

function Get-Timestamp {
    return Get-Date -Format "yyyyMMdd_HHmmss"
}

function Copy-PackagedDirectory {
    param(
        [string]$Source,
        [string]$RelativePath,
        [string]$DestinationRoot
    )

    if (-not (Test-Path $Source)) {
        return
    }

    $destination = Join-Path $DestinationRoot ($RelativePath -replace '/', '\')
    $destinationParent = Split-Path -Parent $destination
    if (-not (Test-Path $destinationParent)) {
        New-Item -ItemType Directory -Path $destinationParent -Force | Out-Null
    }

    Copy-Item -Path $Source -Destination $destination -Recurse -Force
}

$timestamp = Get-Timestamp
if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $root ("Warzone_{0}_source_{1}.zip" -f $Milestone, $timestamp)
}

$commitSha = (git rev-parse HEAD).Trim()
Write-Output ("Handoff commit: {0}" -f $commitSha)
Write-Output ("Handoff output: {0}" -f $OutputPath)

$stagingRoot = Join-Path $root ("Temp\handoff_{0}_{1}" -f $Milestone, $timestamp)
if (Test-Path $stagingRoot) {
    Remove-Item -LiteralPath $stagingRoot -Recurse -Force
}
New-Item -ItemType Directory -Path $stagingRoot -Force | Out-Null

$packageRoots = @(
    "Assets/_Project",
    "Packages",
    "ProjectSettings",
    "Docs",
    "scripts"
)

foreach ($packageRoot in $packageRoots) {
    Copy-PackagedDirectory -Source (Join-Path $root $packageRoot) -RelativePath $packageRoot -DestinationRoot $stagingRoot
}

$manifestPath = Join-Path $stagingRoot "package_manifest.txt"
$generatedUtc = (Get-Date).ToUniversalTime().ToString("o")
$manifestLines = @(
    "milestone=$Milestone"
    "commit=$commitSha"
    "generated_utc=$generatedUtc"
    "package_roots=Assets/_Project,Packages,ProjectSettings,Docs,scripts"
)
$manifestLines | Set-Content -LiteralPath $manifestPath -Encoding UTF8

if (Test-Path $OutputPath) {
    Remove-Item -LiteralPath $OutputPath -Force
}

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zipMode = [System.IO.Compression.ZipArchiveMode]::Create
$zipFile = [System.IO.Compression.ZipFile]::Open($OutputPath, $zipMode)
try {
    $files = Get-ChildItem -LiteralPath $stagingRoot -File -Recurse
    foreach ($file in $files) {
        $relativePath = $file.FullName.Substring($stagingRoot.Length).TrimStart('\', '/')
        $entryName = $relativePath -replace '\\', '/'
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zipFile, $file.FullName, $entryName, [System.IO.Compression.CompressionLevel]::Optimal) | Out-Null
    }
}
finally {
    $zipFile.Dispose()
}

Write-Output "PACKAGE_CREATED: OK"
