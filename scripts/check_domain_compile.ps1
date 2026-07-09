$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

function Get-FrameworkCscPath {
    $path = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
    if (Test-Path $path) {
        return $path
    }

    return $null
}

function Get-NUnitPath {
    $path = Join-Path $root "Library\PackageCache\com.unity.ext.nunit@44f7d31723bd\net472\unity-custom\nunit.framework.dll"
    if (Test-Path $path) {
        return $path
    }

    return $null
}

function Get-DomainFiles {
    return @(
        @(Get-ChildItem Assets/_Project/Core -Recurse -Filter *.cs | Select-Object -ExpandProperty FullName)
        @(Get-ChildItem Assets/_Project/Content/Definitions -Recurse -Filter *.cs | Select-Object -ExpandProperty FullName)
        @(Get-ChildItem Assets/_Project/Content/Catalog -Recurse -Filter *.cs | Select-Object -ExpandProperty FullName)
        @(Get-ChildItem Assets/_Project/Content/Validation -Recurse -Filter *.cs | Select-Object -ExpandProperty FullName)
        @(Get-ChildItem Assets/_Project/Content/Queries -Recurse -Filter *.cs | Select-Object -ExpandProperty FullName)
        @(Get-ChildItem Assets/_Project/Combat -Recurse -Filter *.cs | Select-Object -ExpandProperty FullName)
        @(Get-ChildItem Assets/_Project/Campaign -Recurse -Filter *.cs | Select-Object -ExpandProperty FullName)
        @(Get-ChildItem Assets/_Project/Application -Recurse -Filter *.cs | Select-Object -ExpandProperty FullName)
    ) | ForEach-Object { $_ }
}

function Get-CombatTestExtraFiles {
    return @(
        (Join-Path $root 'Assets/_Project/Sandbox/BattleSandbox/SandboxCombatContentCatalogFactory.cs')
        (Join-Path $root 'Assets/_Project/Sandbox/BattleSandbox/BattleSandboxMode.cs')
        (Join-Path $root 'Assets/_Project/Sandbox/BattleSandbox/BattleSandboxScenarioRegistry.cs')
        (Join-Path $root 'Assets/_Project/Sandbox/BattleSandbox/BattleSandboxCommandQueries.cs')
        (Join-Path $root 'Assets/_Project/Sandbox/BattleSandbox/M4SpatialCombatScenario.cs')
        (Join-Path $root 'Assets/_Project/Sandbox/BattleSandbox/M4SpatialCombatScenarioFactory.cs')
        (Join-Path $root 'Assets/_Project/Sandbox/BattleSandbox/M5IntegratedSandboxScenario.cs')
        (Join-Path $root 'Assets/_Project/Sandbox/BattleSandbox/M5IntegratedSandboxScenarioFactory.cs')
    )
}

function Invoke-Compile {
    param(
        [string]$CompilerPath,
        [string]$OutputPath,
        [string[]]$Files,
        [string[]]$References = @()
    )

    $args = @("/nologo", "/target:library", "/out:$OutputPath")
    foreach ($reference in $References) {
        $args += "/r:$reference"
    }

    $args += $Files
    & $CompilerPath $args
    return $LASTEXITCODE
}

$compilerPath = Get-FrameworkCscPath
if (-not $compilerPath) {
    Write-Output "DOMAIN_COMPILE: SKIPPED (framework csc.exe not found)"
    exit 0
}

if (-not (Test-Path Temp)) {
    New-Item -ItemType Directory -Path Temp | Out-Null
}

$domainFiles = Get-DomainFiles
$domainOutput = Join-Path $root 'Temp\Warzone.DomainValidation.dll'
$domainExitCode = Invoke-Compile -CompilerPath $compilerPath -OutputPath $domainOutput -Files $domainFiles
if ($domainExitCode -eq 0) {
    Write-Output "DOMAIN_COMPILE: OK"
} else {
    Write-Output "DOMAIN_COMPILE: FAILED"
    exit $domainExitCode
}

$nunitPath = Get-NUnitPath
if (-not $nunitPath) {
    Write-Output "ARCH_TEST_SOURCE_COMPILE: SKIPPED (nunit.framework.dll not found)"
    Write-Output "COMBAT_TEST_SOURCE_COMPILE: SKIPPED (nunit.framework.dll not found)"
    Write-Output "SANDBOX_SOURCE_COMPILE: SKIPPED (UnityEngine assemblies not available for offline framework csc validation)"
    exit 0
}

$hasFailure = $false
$architectureTestFiles = @(Get-ChildItem Assets/_Project/Tests/Architecture -Recurse -Filter *.cs | Select-Object -ExpandProperty FullName)
$architectureOutput = Join-Path $root 'Temp\Warzone.ArchitectureTestsValidation.dll'
$architectureExitCode = Invoke-Compile -CompilerPath $compilerPath -OutputPath $architectureOutput -Files $architectureTestFiles -References @($nunitPath)
if ($architectureExitCode -eq 0) {
    Write-Output "ARCH_TEST_SOURCE_COMPILE: OK"
} else {
    Write-Output "ARCH_TEST_SOURCE_COMPILE: FAILED"
    $hasFailure = $true
}

$combatTestFiles = @(Get-ChildItem Assets/_Project/Tests/Combat -Recurse -Filter *.cs | Select-Object -ExpandProperty FullName)
$combatExtraFiles = Get-CombatTestExtraFiles
$combatOutput = Join-Path $root 'Temp\Warzone.CombatTestsValidation.dll'
$combatExitCode = Invoke-Compile -CompilerPath $compilerPath -OutputPath $combatOutput -Files ($combatTestFiles + $combatExtraFiles) -References @($domainOutput, $nunitPath)
if ($combatExitCode -eq 0) {
    Write-Output "COMBAT_TEST_SOURCE_COMPILE: OK"
} else {
    Write-Output "COMBAT_TEST_SOURCE_COMPILE: FAILED"
    $hasFailure = $true
}

Write-Output "SANDBOX_SOURCE_COMPILE: SKIPPED (UnityEngine assemblies not available for offline framework csc validation)"
if ($hasFailure) {
    exit 1
}
