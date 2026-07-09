# M6 Validation Summary

## 1. Goal

M6 closes three items in parallel:

- reduce M5 Unity/Sandbox integration risk
- clean test boundaries so `Combat.Tests` no longer depends on `Sandbox`
- add a first-order pressure / suppression / retreat layer to Combat and expose it through an M6 sandbox entry

## 2. M5 Risk Fixes

- `Warzone.Sandbox.asmdef` now explicitly references `Unity.InputSystem`
- `BattleSandboxViewPresenter` now rebuilds from `BattleSnapshot` on the active M5/M6 path
- `DestroyImmediate` is no longer used unconditionally during runtime view cleanup
- `BattleSandboxLauncher` now disables known bootstrap entries before adding the selected mode entry
- handoff packaging for this milestone uses repository-root paths:
  - `Assets/_Project`
  - `Packages`
  - `ProjectSettings`
  - `Docs`
  - `scripts`

## 3. Test Boundary Cleanup

- `Warzone.Tests.Combat.asmdef` now references only:
  - `Warzone.Core`
  - `Warzone.Content`
  - `Warzone.Combat`
- `M5IntegratedSandboxTests` moved out of `Tests/Combat`
- added `Assets/_Project/Tests/Sandbox`
- added `Warzone.Tests.Sandbox.asmdef`
- `check_domain_compile.ps1` now reports:
  - `ARCH_TEST_SOURCE_COMPILE`
  - `COMBAT_TEST_SOURCE_COMPILE`
  - `SANDBOX_TEST_SOURCE_COMPILE`

## 4. Pressure / Suppression / Retreat Slice

### State

`BattleMemberState` now carries:

- `Pressure`
- `MaxPressure`
- `Suppression`
- `IsSuppressed`
- `IsBroken`
- `IsRetreating`
- `RetreatTargetPosition`
- `LastDamageSourceEnemyId`
- `RecentIncomingFireSeconds`

### Rules

- `PressureGainRule`
- `PressureRecoveryRule`
- `SuppressionRule`
- `BreakRetreatRule`

### Systems

- `PressureSystem`
- `RetreatSystem`

### Current behavior

- incoming fire and damage raise pressure
- pressure slowly recovers when incoming fire has stopped
- suppressed members move slower and fire more slowly
- broken members switch to retreat behavior
- retreat prefers `RallyPoint`, then `ExtractionPoint`, then a fallback point away from the nearest enemy
- retreating members stop accepting normal squad assignments until the order is cleared

## 5. M6 Sandbox Entry

Launcher mode:

- `BattleSandboxMode.M6PressureRetreat`

New sandbox files:

- `M6PressureRetreatSandboxBootstrap`
- `M6PressureRetreatScenarioFactory`
- `M6PressureRetreatScenario`
- `M6PressureRetreatDebugPanel`

Runtime behavior:

- keeps the M5 launcher / presenter / input pattern
- exposes debug pressure controls:
  - `T` apply pressure
  - `Y` clear pressure
  - `G` simulate incoming fire

## 6. Validation Commands And Results

### Text boundary

Command:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_text_boundaries.ps1
```

Result:

- `UNITYENGINE_BOUNDARY: OK`
- `LEGACY_NAMESPACE_BOUNDARY: OK`

### Domain compile

Command:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_domain_compile.ps1
```

Result:

- `DOMAIN_COMPILE: OK`

### Test source compile

Command:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_domain_compile.ps1
```

Result:

- `ARCH_TEST_SOURCE_COMPILE: OK`
- `COMBAT_TEST_SOURCE_COMPILE: OK`
- `SANDBOX_TEST_SOURCE_COMPILE: OK`

### All-in-one validation

Command:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_warzone_all.ps1
```

Result:

- `DOTNET_TOOLCHAIN: SKIPPED - dotnet/msbuild not found`
- `WARZONE_VALIDATION: OK`

### Actual test execution

Command:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_unity_editmode.ps1
```

Result:

- `UNITY_EDITMODE_TEST_EXECUTION: SKIPPED - Unity.exe not found`

### Unity compile

Command:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_unity_editmode.ps1
```

Result:

- `UNITY_EDITOR_COMPILE: SKIPPED - Unity.exe not found`

## 7. Current Status By Validation Layer

- Domain compile: verified
- Test source compile: verified
- Actual NUnit / Unity EditMode execution: not verified in this environment
- Unity Editor compile: not verified in this environment
- Sandbox manual run: not verified in this environment

## 8. Known Issues

- Unity CLI is unavailable on this machine, so Unity-side compile and EditMode execution remain unverified
- runtime MonoBehaviour code under `Sandbox` is only covered by source inspection and asmdef boundary checks here
- the top-level engineering docs outside this milestone contain unrelated untracked files and were intentionally left alone

## 9. Next Step

The next useful closure is Unity-side validation on a machine with `Unity.exe` available:

1. create or open the M5/M6 sandbox scene through the Editor menu
2. run EditMode tests
3. confirm launcher mode switching, reset flow, and pressure debug controls in Play Mode
