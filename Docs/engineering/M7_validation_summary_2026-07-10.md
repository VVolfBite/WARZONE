# M7 Validation Summary

Recorded package source commit SHA: `1868aa6ab40a9c8da4fae9b18a1f8f5e42907909`

## 1. Goal

M7 extends the combat slice in three directions:

- fix the event-buffer lifecycle so `PressureSystem` no longer clears global frame events
- add first-order environmental combat state and rules
- add an M7 sandbox entry for smoke, night, light, fire, and toxic-zone interaction

## 2. M7 Fixes For Prior Risks

### EventBuffer / PressureSystem

- `PressureSystem` now observes `battleState.EventBuffer.Events`
- it no longer calls `Drain()`
- `BattleSimulation` now owns frame-event cleanup
- recent debug/event history remains visible through `BattleState.RecentEvents`
- snapshots still show fired / damage / killed events after pressure processing

### Handoff path

Packaging for M7 continues to use repository-root paths:

- `Assets/_Project`
- `Packages`
- `ProjectSettings`
- `Docs`
- `scripts`

### M7 scene creation menu

Editor menu entry to be used on a Unity machine:

- `Warzone/Sandbox/Create M7 Environment Combat Sandbox Scene`

Expected save path:

- `Assets/Scenes/Sandbox/Sandbox_M7_EnvironmentCombat.unity`

## 3. Environment System Summary

### Smoke

- represented as circular environment zones
- dense smoke blocks visual acquisition
- smoke does not behave like a physical wall in `FireLineRule`
- the current slice prevents aimed fire when the target is hidden by smoke

### Night / Darkness

- `BattleEnvironmentState.IsNight` enables the night penalty layer
- effective detection range is multiplied by `GlobalVisibilityMultiplier`
- darkness zones can further reduce effective visibility

### Light

- light zones make targets easier to detect during night conditions
- they do not replace the full lighting model; they are a tactical simplification

### Fire

- fire zones apply deterministic environmental damage over time
- fire zones can also add pressure
- no spread or propagation is implemented

### Toxic

- toxic zones apply lightweight damage and pressure
- no long-term sickness or infection system is implemented

## 4. M7 Sandbox How To Run

On a Unity machine:

1. Open Unity Editor
2. Run `Warzone/Sandbox/Create M7 Environment Combat Sandbox Scene`
3. Open `Assets/Scenes/Sandbox/Sandbox_M7_EnvironmentCombat.unity`
4. Enter Play Mode

Launcher mode:

- `BattleSandboxMode.M7EnvironmentCombat`

Debug controls:

- `N` toggle night/day
- `V` toggle player night vision
- `B` place smoke zone at cursor hit point
- `F` place fire zone at cursor hit point
- `L` place light zone at cursor hit point
- plus existing move / defend / search / extract / pause / reset controls

## 5. Validation Commands And Results

### Text boundary

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_text_boundaries.ps1
```

Expected milestone result:

- `UNITYENGINE_BOUNDARY: OK`
- `LEGACY_NAMESPACE_BOUNDARY: OK`

### Domain compile

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_domain_compile.ps1
```

Current result in this environment after M7 changes:

- `DOMAIN_COMPILE: OK`

### Test source compile

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_domain_compile.ps1
```

Current result in this environment after M7 changes:

- `ARCH_TEST_SOURCE_COMPILE: OK`
- `COMBAT_TEST_SOURCE_COMPILE: OK`
- `SANDBOX_TEST_SOURCE_COMPILE: OK`

### All-in-one validation

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_warzone_all.ps1
```

Expected result shape:

- `DOMAIN_COMPILE`
- `ARCH_TEST_SOURCE_COMPILE`
- `COMBAT_TEST_SOURCE_COMPILE`
- `SANDBOX_TEST_SOURCE_COMPILE`
- `UNITY_EDITMODE_TEST_EXECUTION`
- `UNITY_EDITOR_COMPILE`

### Actual test execution

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_unity_editmode.ps1
```

Current environment status:

- `UNITY_EDITMODE_TEST_EXECUTION: SKIPPED - Unity.exe not found`

### Unity compile

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_unity_editmode.ps1
```

Current environment status:

- `UNITY_EDITOR_COMPILE: SKIPPED - Unity.exe not found`

## 6. Validation State By Layer

- Domain compile: verified
- Test source compile: verified
- Actual NUnit / Unity EditMode execution: not verified in this environment
- Unity Editor compile: not verified in this environment
- Sandbox manual run: not verified in this environment

## 7. Known Issues

- Unity CLI is still unavailable here, so Unity-side compile and EditMode execution remain unverified
- M7 sandbox MonoBehaviour code is only covered here by source inspection, test-source compile, and asmdef boundary checks
- environment-zone visuals and input mappings still need live Unity validation
- this milestone is not a weather system, not a full lighting model, and not a physically simulated smoke implementation

## 8. Next Step

The next practical closure is Unity-side verification on a machine with `Unity.exe`:

1. create the M7 sandbox scene from the Editor menu
2. enter Play Mode and verify zone spawning
3. confirm smoke blocks acquisition, night reduces detection, and light mitigates the penalty
4. run EditMode tests and capture Unity compile status
