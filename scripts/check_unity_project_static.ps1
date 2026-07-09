$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

function Write-CheckResult {
    param(
        [string]$Label,
        [bool]$Passed,
        [string]$Detail
    )

    if ($Passed) {
        Write-Output "${Label}: OK"
    } else {
        Write-Output "${Label}: FAILED - $Detail"
        $script:HasFailure = $true
    }
}

function Get-AsmdefNames {
    $map = @{}
    Get-ChildItem Assets -Recurse -Filter *.asmdef | ForEach-Object {
        $json = Get-Content $_.FullName -Raw | ConvertFrom-Json
        $map[$json.name] = $_.FullName
    }

    return $map
}

$HasFailure = $false
$asmdefFiles = Get-ChildItem Assets -Recurse -Filter *.asmdef
$asmdefNames = Get-AsmdefNames
$knownUnityReferences = @(
    "Unity.InputSystem",
    "TestAssemblies"
)

foreach ($asmdefFile in $asmdefFiles) {
    try {
        $json = Get-Content $asmdefFile.FullName -Raw | ConvertFrom-Json
        Write-CheckResult "ASMDEF_JSON:$($asmdefFile.Name)" $true ""

        foreach ($reference in @($json.references)) {
            if ($asmdefNames.ContainsKey($reference) -or $knownUnityReferences -contains $reference) {
                continue
            }

            Write-CheckResult "ASMDEF_REFERENCE:$($json.name)" $false "Unknown reference $reference"
        }
    } catch {
        Write-CheckResult "ASMDEF_JSON:$($asmdefFile.Name)" $false $_.Exception.Message
    }
}

$editorScriptFailures = @()
Get-ChildItem Assets -Recurse -Filter *.cs | ForEach-Object {
    $text = Get-Content $_.FullName -Raw
    if ($text.Contains("using UnityEditor;") -or $text.Contains("namespace Warzone.Editor")) {
        if ($_.FullName -notmatch "\\Editor\\") {
            $editorScriptFailures += $_.FullName
        }
    }
}
Write-CheckResult "EDITOR_SCRIPT_PLACEMENT" ($editorScriptFailures.Count -eq 0) (($editorScriptFailures -join ", "))

$sceneMenuPath = Join-Path $root "Assets\_Project\Editor\SandboxTools\SandboxSceneCreateMenu.cs"
if (Test-Path $sceneMenuPath) {
    $sceneMenuText = Get-Content $sceneMenuPath -Raw
    $hasMenus = $sceneMenuText.Contains("Create M5 Integrated Sandbox Scene") -and
        $sceneMenuText.Contains("Create M6 Pressure Retreat Sandbox Scene") -and
        $sceneMenuText.Contains("Create M7 Environment Combat Sandbox Scene") -and
        $sceneMenuText.Contains("Create M8 Building Tactics Sandbox Scene")
    Write-CheckResult "SANDBOX_SCENE_MENU" $hasMenus "Missing one or more M5-M8 scene menu entries"
} else {
    Write-CheckResult "SANDBOX_SCENE_MENU" $false "SandboxSceneCreateMenu.cs not found"
}

$manifestPath = Join-Path $root "Packages\manifest.json"
if (Test-Path $manifestPath) {
    $manifestText = Get-Content $manifestPath -Raw
    Write-CheckResult "INPUT_SYSTEM_PACKAGE" ($manifestText.Contains("com.unity.inputsystem")) "Packages/manifest.json missing com.unity.inputsystem"
} else {
    Write-CheckResult "INPUT_SYSTEM_PACKAGE" $false "Packages/manifest.json not found"
}

$checklistPath = Join-Path $root "Docs\engineering\unity_first_open_checklist.md"
if (Test-Path $checklistPath) {
    $checklistText = Get-Content $checklistPath -Raw
    Write-CheckResult "HANDOFF_PATH_NOTE" ($checklistText.Contains("Assets/_Project")) "unity_first_open_checklist.md should mention Assets/_Project handoff path"
} else {
    Write-CheckResult "HANDOFF_PATH_NOTE" $false "unity_first_open_checklist.md not found"
}

if ($HasFailure) {
    exit 1
}
