$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

$domainTargets = @(
    'Assets/_Project/Core',
    'Assets/_Project/Content/Definitions',
    'Assets/_Project/Content/Catalog',
    'Assets/_Project/Content/Queries',
    'Assets/_Project/Content/Validation',
    'Assets/_Project/Combat',
    'Assets/_Project/Campaign',
    'Assets/_Project/Application'
)

$unityHits = rg -n "UnityEngine" $domainTargets
if ($LASTEXITCODE -eq 0) {
    Write-Output "UNITYENGINE_BOUNDARY: FAILED"
    $unityHits
    exit 1
}

Write-Output "UNITYENGINE_BOUNDARY: OK"

$contentCombatHits = rg -n "Warzone.Combat" Assets/_Project/Content
if ($LASTEXITCODE -eq 0) {
    Write-Output "CONTENT_COMBAT_BOUNDARY: FAILED"
    $contentCombatHits
    exit 1
}

Write-Output "CONTENT_COMBAT_BOUNDARY: OK"

$campaignCombatHits = rg -n "Warzone.Combat" Assets/_Project/Campaign
if ($LASTEXITCODE -eq 0) {
    Write-Output "CAMPAIGN_COMBAT_BOUNDARY: FAILED"
    $campaignCombatHits
    exit 1
}

Write-Output "CAMPAIGN_COMBAT_BOUNDARY: OK"

$legacyNamespaceHits = rg -n "Warzone.Adapters|Warzone.Controls|Warzone.Presentation" Assets/_Project
if ($LASTEXITCODE -eq 0) {
    $nonArchitectureHits = $legacyNamespaceHits | Where-Object { $_ -notmatch "Assets/_Project\\Tests\\Architecture\\AssemblyDefinitionTests.cs" }
    if ($nonArchitectureHits.Count -gt 0) {
        Write-Output "LEGACY_NAMESPACE_BOUNDARY: FAILED"
        $nonArchitectureHits
        exit 1
    }
}

Write-Output "LEGACY_NAMESPACE_BOUNDARY: OK"
