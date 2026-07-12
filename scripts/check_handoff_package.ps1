param(
    [string]$PackagePath = $null,
    [string]$Milestone = $null
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

if ([string]::IsNullOrWhiteSpace($PackagePath)) {
    $packageFilter = if ([string]::IsNullOrWhiteSpace($Milestone)) { "Warzone_*_source_*.zip" } else { "Warzone_${Milestone}_source_*.zip" }
    $latest = Get-ChildItem -Path $root -Filter $packageFilter -File -ErrorAction SilentlyContinue | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($null -eq $latest) {
        Write-Output "HANDOFF_PACKAGE_CHECK: SKIPPED (no handoff package found)"
        exit 0
    }

    $PackagePath = $latest.FullName
}

if ([string]::IsNullOrWhiteSpace($Milestone)) {
    $fileName = [System.IO.Path]::GetFileName($PackagePath)
    if ($fileName -match '^Warzone_([^_]+)_source_') {
        $Milestone = $Matches[1]
    }
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

    $summaryPath = "Docs/engineering/M16_validation_summary_2026-07-11.md"
    if ($Milestone -eq "U1") {
        $summaryPath = "Docs/engineering/U1_playtest_feedback_fix_summary_2026-07-11.md"
    }

    $summaryFound = $false
    foreach ($entry in $entries) {
        if ($entry.FullName -eq $summaryPath) {
            $summaryFound = $true
            break
        }
    }

    if (-not $summaryFound) {
        Write-Output "HANDOFF_PACKAGE_CHECK: FAILED (missing validation summary: $summaryPath)"
        exit 1
    }
}
finally {
    $zip.Dispose()
}

Write-Output "HANDOFF_PACKAGE_CHECK: OK"
