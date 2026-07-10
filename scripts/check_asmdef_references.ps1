$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

function Read-AsmdefText {
    param([string]$RelativePath)

    $path = Join-Path $root $RelativePath
    if (-not (Test-Path $path)) {
        throw "Missing asmdef: $RelativePath"
    }

    return Get-Content $path -Raw
}

function Assert-Contains {
    param(
        [string]$Text,
        [string]$Needle,
        [string]$Label
    )

    if ($Text -notmatch [regex]::Escape($Needle)) {
        Write-Output "${Label}: FAILED (missing $Needle)"
        exit 1
    }
}

function Assert-DoesNotContain {
    param(
        [string]$Text,
        [string]$Needle,
        [string]$Label
    )

    if ($Text -match [regex]::Escape($Needle)) {
        Write-Output "${Label}: FAILED (unexpected $Needle)"
        exit 1
    }
}

try {
    $content = Read-AsmdefText "Assets/_Project/Content/Warzone.Content.asmdef"
    Assert-Contains $content '"name": "Warzone.Content"' "ASMDEF_CONTENT"
    Assert-Contains $content '"noEngineReferences": true' "ASMDEF_CONTENT"
    Assert-Contains $content '"Warzone.Core"' "ASMDEF_CONTENT"
    Assert-DoesNotContain $content '"Warzone.Combat"' "ASMDEF_CONTENT"
    Assert-DoesNotContain $content '"Warzone.Campaign"' "ASMDEF_CONTENT"
    Assert-DoesNotContain $content '"Warzone.Application"' "ASMDEF_CONTENT"
    Assert-DoesNotContain $content '"Warzone.Runtime"' "ASMDEF_CONTENT"
    Assert-DoesNotContain $content '"Warzone.Sandbox"' "ASMDEF_CONTENT"

    $contentAuthoring = Read-AsmdefText "Assets/_Project/Content/Authoring/Warzone.Content.Authoring.asmdef"
    Assert-Contains $contentAuthoring '"name": "Warzone.Content.Authoring"' "ASMDEF_CONTENT_AUTHORING"
    Assert-Contains $contentAuthoring '"Warzone.Core"' "ASMDEF_CONTENT_AUTHORING"
    Assert-Contains $contentAuthoring '"Warzone.Content"' "ASMDEF_CONTENT_AUTHORING"
    Assert-DoesNotContain $contentAuthoring '"Warzone.Combat"' "ASMDEF_CONTENT_AUTHORING"
    Assert-DoesNotContain $contentAuthoring '"Warzone.Campaign"' "ASMDEF_CONTENT_AUTHORING"
    Assert-DoesNotContain $contentAuthoring '"Warzone.Application"' "ASMDEF_CONTENT_AUTHORING"
    Assert-DoesNotContain $contentAuthoring '"Warzone.Runtime"' "ASMDEF_CONTENT_AUTHORING"
    Assert-DoesNotContain $contentAuthoring '"Warzone.Sandbox"' "ASMDEF_CONTENT_AUTHORING"

    $combat = Read-AsmdefText "Assets/_Project/Combat/Warzone.Combat.asmdef"
    Assert-Contains $combat '"Warzone.Core"' "ASMDEF_COMBAT"
    Assert-Contains $combat '"Warzone.Content"' "ASMDEF_COMBAT"
    Assert-DoesNotContain $combat '"Warzone.Campaign"' "ASMDEF_COMBAT"
    Assert-DoesNotContain $combat '"Warzone.Application"' "ASMDEF_COMBAT"
    Assert-DoesNotContain $combat '"Warzone.Runtime"' "ASMDEF_COMBAT"
    Assert-DoesNotContain $combat '"Warzone.Sandbox"' "ASMDEF_COMBAT"

    $campaign = Read-AsmdefText "Assets/_Project/Campaign/Warzone.Campaign.asmdef"
    Assert-Contains $campaign '"Warzone.Core"' "ASMDEF_CAMPAIGN"
    Assert-Contains $campaign '"Warzone.Content"' "ASMDEF_CAMPAIGN"
    Assert-DoesNotContain $campaign '"Warzone.Combat"' "ASMDEF_CAMPAIGN"
    Assert-DoesNotContain $campaign '"Warzone.Application"' "ASMDEF_CAMPAIGN"
    Assert-DoesNotContain $campaign '"Warzone.Runtime"' "ASMDEF_CAMPAIGN"
    Assert-DoesNotContain $campaign '"Warzone.Sandbox"' "ASMDEF_CAMPAIGN"

    $application = Read-AsmdefText "Assets/_Project/Application/Warzone.Application.asmdef"
    Assert-Contains $application '"Warzone.Core"' "ASMDEF_APPLICATION"
    Assert-Contains $application '"Warzone.Campaign"' "ASMDEF_APPLICATION"
    Assert-Contains $application '"Warzone.Combat"' "ASMDEF_APPLICATION"
    Assert-Contains $application '"Warzone.Content"' "ASMDEF_APPLICATION"
    Assert-DoesNotContain $application '"Warzone.Runtime"' "ASMDEF_APPLICATION"
    Assert-DoesNotContain $application '"Warzone.Sandbox"' "ASMDEF_APPLICATION"

    $runtime = Read-AsmdefText "Assets/_Project/Runtime/Warzone.Runtime.asmdef"
    Assert-DoesNotContain $runtime '"Warzone.Sandbox"' "ASMDEF_RUNTIME"

    $sandbox = Read-AsmdefText "Assets/_Project/Sandbox/Warzone.Sandbox.asmdef"
    Assert-Contains $sandbox '"Warzone.Runtime"' "ASMDEF_SANDBOX"
    Assert-Contains $sandbox '"Warzone.Application"' "ASMDEF_SANDBOX"
    Assert-Contains $sandbox '"Warzone.Combat"' "ASMDEF_SANDBOX"
    Assert-Contains $sandbox '"Warzone.Campaign"' "ASMDEF_SANDBOX"
    Assert-Contains $sandbox '"Warzone.Content"' "ASMDEF_SANDBOX"
    Assert-Contains $sandbox '"Warzone.Core"' "ASMDEF_SANDBOX"
    Assert-Contains $sandbox '"Unity.InputSystem"' "ASMDEF_SANDBOX"

    $combatTests = Read-AsmdefText "Assets/_Project/Tests/Combat/Warzone.Tests.Combat.asmdef"
    Assert-DoesNotContain $combatTests '"Warzone.Sandbox"' "ASMDEF_TEST_COMBAT"
    Assert-DoesNotContain $combatTests '"Warzone.Runtime"' "ASMDEF_TEST_COMBAT"

    $campaignTests = Read-AsmdefText "Assets/_Project/Tests/Campaign/Warzone.Tests.Campaign.asmdef"
    Assert-DoesNotContain $campaignTests '"Warzone.Combat"' "ASMDEF_TEST_CAMPAIGN"
    Assert-DoesNotContain $campaignTests '"Warzone.Runtime"' "ASMDEF_TEST_CAMPAIGN"
    Assert-DoesNotContain $campaignTests '"Warzone.Sandbox"' "ASMDEF_TEST_CAMPAIGN"

    $applicationTests = Read-AsmdefText "Assets/_Project/Tests/Application/Warzone.Tests.Application.asmdef"
    Assert-Contains $applicationTests '"Warzone.Combat"' "ASMDEF_TEST_APPLICATION"
    Assert-Contains $applicationTests '"Warzone.Campaign"' "ASMDEF_TEST_APPLICATION"
    Assert-Contains $applicationTests '"Warzone.Application"' "ASMDEF_TEST_APPLICATION"
    Assert-DoesNotContain $applicationTests '"Warzone.Runtime"' "ASMDEF_TEST_APPLICATION"
    Assert-DoesNotContain $applicationTests '"Warzone.Sandbox"' "ASMDEF_TEST_APPLICATION"

    $contentTests = Read-AsmdefText "Assets/_Project/Tests/Content/Warzone.Tests.Content.asmdef"
    Assert-Contains $contentTests '"Warzone.Content"' "ASMDEF_TEST_CONTENT"
    Assert-DoesNotContain $contentTests '"Warzone.Combat"' "ASMDEF_TEST_CONTENT"
    Assert-DoesNotContain $contentTests '"Warzone.Campaign"' "ASMDEF_TEST_CONTENT"
    Assert-DoesNotContain $contentTests '"Warzone.Application"' "ASMDEF_TEST_CONTENT"
    Assert-DoesNotContain $contentTests '"Warzone.Runtime"' "ASMDEF_TEST_CONTENT"
    Assert-DoesNotContain $contentTests '"Warzone.Sandbox"' "ASMDEF_TEST_CONTENT"

    $sandboxTests = Read-AsmdefText "Assets/_Project/Tests/Sandbox/Warzone.Tests.Sandbox.asmdef"
    Assert-Contains $sandboxTests '"Warzone.Sandbox"' "ASMDEF_TEST_SANDBOX"

    Write-Output "ASMDEF_BOUNDARY: OK"
}
catch {
    Write-Output "ASMDEF_BOUNDARY: FAILED ($($_.Exception.Message))"
    exit 1
}
