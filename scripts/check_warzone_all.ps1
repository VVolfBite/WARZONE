$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

function Invoke-CheckedScript {
    param(
        [string]$Label,
        [string]$Path
    )

    if (-not (Test-Path $Path)) {
        Write-Output "${Label}: SKIPPED ($Path not found)"
        return
    }

    try {
        $scriptOutput = & powershell -ExecutionPolicy Bypass -File $Path 2>&1
        if ($scriptOutput) {
            $scriptOutput | ForEach-Object { Write-Output $_ }
        }

        $exitCode = $LASTEXITCODE
        if ($exitCode -ne 0) {
            Write-Output "${Label}: FAIL (exit code $exitCode)"
            return $false
        }

        Write-Output "${Label}: OK"
    } catch {
        Write-Output "${Label}: FAIL ($($_.Exception.Message))"
        return $false
    }

    return $true
}

$dotnetPath = Get-Command dotnet -ErrorAction SilentlyContinue
$msbuildPath = Get-Command msbuild -ErrorAction SilentlyContinue
if ($dotnetPath) {
    Write-Output "DOTNET_TOOLCHAIN: OK - $($dotnetPath.Source)"
} elseif ($msbuildPath) {
    Write-Output "DOTNET_TOOLCHAIN: OK - $($msbuildPath.Source)"
} else {
    Write-Output "DOTNET_TOOLCHAIN: SKIPPED - dotnet/msbuild not found"
}

$allPassed = $true
if (-not (Invoke-CheckedScript -Label "TEXT_BOUNDARY_CHECK" -Path (Join-Path $root "scripts\check_text_boundaries.ps1"))) {
    $allPassed = $false
}

if (-not (Invoke-CheckedScript -Label "ASMDEF_BOUNDARY_CHECK" -Path (Join-Path $root "scripts\check_asmdef_references.ps1"))) {
    $allPassed = $false
}

if (-not (Invoke-CheckedScript -Label "DOMAIN_AND_TEST_SOURCE_CHECK" -Path (Join-Path $root "scripts\check_domain_compile.ps1"))) {
    $allPassed = $false
}

if (-not (Invoke-CheckedScript -Label "UNITY_PROJECT_STATIC_CHECK" -Path (Join-Path $root "scripts\check_unity_project_static.ps1"))) {
    $allPassed = $false
}

if (-not (Invoke-CheckedScript -Label "UNITY_EDITMODE_CHECK" -Path (Join-Path $root "scripts\check_unity_editmode.ps1"))) {
    $allPassed = $false
}

if (-not (Invoke-CheckedScript -Label "HANDOFF_PACKAGE_CHECK" -Path (Join-Path $root "scripts\check_handoff_package.ps1"))) {
    $allPassed = $false
}

if ($allPassed) {
    Write-Output "WARZONE_VALIDATION: OK"
} else {
    Write-Output "WARZONE_VALIDATION: FAIL"
    exit 1
}
