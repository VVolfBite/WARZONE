param(
    [string]$PackagePath = $null
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

if ([string]::IsNullOrWhiteSpace($PackagePath)) {
    $latest = Get-ChildItem -Path $root -Filter "Warzone_M14_source_*.zip" -File -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($null -eq $latest) {
        Write-Output "HANDOFF_PACKAGE_CHECK: SKIPPED (no M14 handoff package found)"
        exit 0
    }

    $PackagePath = $latest.FullName
}

if (-not (Test-Path $PackagePath)) {
    Write-Output "HANDOFF_PACKAGE_CHECK: FAILED (package not found: $PackagePath)"
    exit 1
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
$zip = [System.IO.Compression.ZipFile]::OpenRead($PackagePath)
try {
    $entries = $zip.Entries
    $hasBackslash = $false
    $requiredPrefixes = @(
        "Assets/_Project/",
        "Packages/",
        "ProjectSettings/",
        "Docs/",
        "scripts/"
    )

    foreach ($entry in $entries) {
        if ($entry.FullName.Contains("\\")) {
            $hasBackslash = $true
            break
        }
    }

    if ($hasBackslash) {
        Write-Output "HANDOFF_PACKAGE_CHECK: FAILED (zip entries contain backslashes)"
        exit 1
    }

    foreach ($prefix in $requiredPrefixes) {
        $found = $false
        foreach ($entry in $entries) {
            if ($entry.FullName.StartsWith($prefix)) {
                $found = $true
                break
            }
        }

        if (-not $found) {
            Write-Output "HANDOFF_PACKAGE_CHECK: FAILED (missing prefix $prefix)"
            exit 1
        }
    }

    $manifestFound = $false
    foreach ($entry in $entries) {
        if ($entry.FullName -eq "package_manifest.txt") {
            $manifestFound = $true
            break
        }
    }

    if (-not $manifestFound) {
        Write-Output "HANDOFF_PACKAGE_CHECK: FAILED (missing package_manifest.txt)"
        exit 1
    }
}
finally {
    $zip.Dispose()
}

Write-Output "HANDOFF_PACKAGE_CHECK: OK"
