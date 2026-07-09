$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

function Get-UnityPath {
    $candidates = @(
        $env:WARZONE_UNITY_EXE,
        "C:\Program Files\Unity\Hub\Editor\6000.0.0f1\Editor\Unity.exe",
        "C:\Program Files\Unity\Hub\Editor\2022.3.0f1\Editor\Unity.exe",
        "C:\Program Files\Unity\Hub\Editor\2022.3.21f1\Editor\Unity.exe",
        "C:\Program Files\Unity\Hub\Editor\2021.3.0f1\Editor\Unity.exe"
    ) | Where-Object { $_ }

    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            return $candidate
        }
    }

    $hubEditors = "C:\Program Files\Unity\Hub\Editor"
    if (Test-Path $hubEditors) {
        $match = Get-ChildItem $hubEditors -Directory | Sort-Object Name -Descending | Select-Object -First 1
        if ($match -and (Test-Path (Join-Path $match.FullName "Editor\Unity.exe"))) {
            return (Join-Path $match.FullName "Editor\Unity.exe")
        }
    }

    return $null
}

$unityPath = Get-UnityPath
if (-not $unityPath) {
    Write-Output "UNITY_CLI: SKIPPED - Unity.exe not found"
    Write-Output "UNITY_EDITMODE_TEST_EXECUTION: SKIPPED - Unity.exe not found"
    Write-Output "UNITY_EDITOR_COMPILE: SKIPPED - Unity.exe not found"
    exit 0
}

$artifactRoot = Join-Path $root "artifacts"
$logDirectory = Join-Path $artifactRoot "logs"
$testDirectory = Join-Path $artifactRoot "test-results"
New-Item -ItemType Directory -Force -Path $logDirectory | Out-Null
New-Item -ItemType Directory -Force -Path $testDirectory | Out-Null

$logPath = Join-Path $logDirectory "unity_editmode_M5.log"
$resultPath = Join-Path $testDirectory "editmode_M5.xml"

& $unityPath `
    -batchmode `
    -quit `
    -projectPath $root `
    -runTests `
    -testPlatform EditMode `
    -testResults $resultPath `
    -logFile $logPath

$exitCode = $LASTEXITCODE
Write-Output "UNITY_CLI: OK - $unityPath"
if ($exitCode -eq 0) {
    Write-Output "UNITY_EDITMODE_TEST_EXECUTION: OK"
    Write-Output "UNITY_EDITOR_COMPILE: OK"
} else {
    Write-Output "UNITY_EDITMODE_TEST_EXECUTION: FAILED"
    Write-Output "UNITY_EDITOR_COMPILE: FAILED"
    exit $exitCode
}
