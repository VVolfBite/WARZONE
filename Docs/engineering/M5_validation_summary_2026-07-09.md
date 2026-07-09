# Warzone M5 Validation Summary - 2026-07-09

## 1. Goal

M5 focuses on Unity/Sandbox engineering closure, not new combat mechanics.

Main goals:

- unify the sandbox entry path
- stabilize view/input wiring for the existing M1-M4 combat slice
- prepare a repeatable Unity scene creation path
- strengthen validation scripts so compile, test-source compile, test execution, and Unity compile are reported separately

## 2. New Modules

Sandbox runtime consolidation:

- `BattleSandboxMode`
- `BattleSandboxLauncher`
- `BattleSandboxScenarioRegistry`
- `BattleSandboxRuntimeContext`
- `BattleSandboxCommandQueries`
- `BattleSandboxViewPresenter`
- `BattleSandboxInputController`

M5 sandbox path:

- `M5IntegratedSandboxScenario`
- `M5IntegratedSandboxScenarioFactory`
- `M5IntegratedSandboxBootstrap`
- `M5IntegratedSandboxDebugPanel`

Sandbox views:

- `SandboxMemberView`
- `SandboxEnemyView`
- `SandboxTacticalNodeView`
- `SandboxObstacleView`
- `SandboxBuildingView`
- `SandboxSelectionMarkerView`
- `SandboxFireLineView`

Editor tooling:

- `Assets/_Project/Editor/SandboxTools/SandboxSceneCreateMenu.cs`

Validation tooling:

- `scripts/check_warzone_all.ps1`
- `scripts/check_unity_editmode.ps1`

## 3. M5 Sandbox How To Run

Preferred path:

1. Open a sandbox-capable scene or create one with the editor menu.
2. Create an empty `GameObject` or use the generated launcher object.
3. Attach `Warzone.Sandbox.BattleSandbox.BattleSandboxLauncher`.
4. Set mode to `M5IntegratedSandbox`.
5. Enter Play Mode.

Controls:

- left click: select squad or member
- right click: move selected squad
- `D`: defend area at cursor
- `S`: search nearest incomplete search point
- `E`: extract to nearest extraction point
- `P` or `Space`: pause/resume
- `R`: reset scenario
- `C`: toggle fire line debug

Pause policy:

- pause stops simulation ticks
- commands are still allowed while paused
- queued commands execute after resume

## 4. Sandbox Launcher / Mode

`BattleSandboxLauncher` is the preferred entry going forward.

Supported modes:

- `M1MemberMovement`
- `M2CombatSlice`
- `M3TacticalMission`
- `M4SpatialCombat`
- `M5IntegratedSandbox`

Current policy:

- M1-M4 bootstrap classes remain as compatibility entries
- M5 uses the unified launcher plus the shared runtime context/input/view path

## 5. View / Input Consolidation

M5 centralizes Unity-side responsibilities:

- `BattleSandboxViewPresenter` owns view creation, update, and cleanup
- `BattleSandboxInputController` owns selection, commands, pause, reset, and fire-line toggles
- view scripts only display snapshot state and stable ids
- command input goes through `TacticalCommandService`

This reduces duplicated logic across the per-milestone bootstrap scripts and fixes the main M4 engineering issue: bootstrap-level input, view spawning, and debug code were all mixed together.

## 6. Unity Scene Creation

Unity scene asset creation is now prepared through an editor menu.

Menu:

- `Warzone/Sandbox/Create M5 Integrated Sandbox Scene`

Behavior:

- creates a new scene
- adds default scene objects
- adds `M5 Sandbox Launcher`
- attaches `BattleSandboxLauncher`
- sets mode to `M5IntegratedSandbox`
- saves to `Assets/Scenes/Sandbox/Sandbox_M5_Integrated.unity`

If Unity CLI is unavailable, this menu path is the supported way to create the scene in-editor.

Manual fallback:

1. Create an empty scene.
2. Add camera and light if needed.
3. Add an empty `GameObject`.
4. Attach `BattleSandboxLauncher`.
5. Set mode to `M5IntegratedSandbox`.

## 7. Validation Commands And Results

Commands run in this environment:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_text_boundaries.ps1
powershell -ExecutionPolicy Bypass -File scripts/check_domain_compile.ps1
powershell -ExecutionPolicy Bypass -File scripts/check_warzone_all.ps1
powershell -ExecutionPolicy Bypass -File scripts/check_unity_editmode.ps1
git status --short --branch
git diff --stat
git diff --name-status
```

Observed results:

- `check_text_boundaries.ps1`
  - `UNITYENGINE_BOUNDARY: OK`
  - `LEGACY_NAMESPACE_BOUNDARY: OK`
- `check_domain_compile.ps1`
  - `DOMAIN_COMPILE: OK`
  - `ARCH_TEST_SOURCE_COMPILE: OK`
  - `COMBAT_TEST_SOURCE_COMPILE: OK`
  - `SANDBOX_SOURCE_COMPILE: SKIPPED (UnityEngine assemblies not available for offline framework csc validation)`
- `check_unity_editmode.ps1`
  - `UNITY_CLI: SKIPPED - Unity.exe not found`
  - `UNITY_EDITMODE_TEST_EXECUTION: SKIPPED - Unity.exe not found`
  - `UNITY_EDITOR_COMPILE: SKIPPED - Unity.exe not found`
- `check_warzone_all.ps1`
  - `DOTNET_TOOLCHAIN: SKIPPED - dotnet/msbuild not found`
  - `WARZONE_VALIDATION: OK`

## 8. Actual Status Separation

Actual status for this machine:

- domain compile: verified
- test source compile: verified
- actual test execution: not verified
- Unity Editor compile: not verified
- sandbox manual run status: not verified

Required wording when Unity was not actually compiled:

`Unity Editor compile not verified`

## 9. Known Issues

- M1-M4 compatibility bootstraps are still present to preserve prior validation paths
- actual Unity EditMode execution still depends on a machine with Unity CLI installed
- sandbox view/presenter coverage is still compile-oriented here; visual correctness still needs Unity-side verification
- this milestone does not add new combat mechanics beyond the existing M4 rule set

## 10. Next Step Suggestion

The next sensible step is not another combat mechanic. It is Unity-side verification:

- create the M5 scene with the editor menu
- run EditMode tests through Unity CLI or the editor
- confirm the launcher, reset flow, and presenter cleanup visually
- then decide whether to harden the M5 sandbox into a stable sample scene or move to the next gameplay milestone
