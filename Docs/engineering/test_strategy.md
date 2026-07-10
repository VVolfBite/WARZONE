# Warzone Test Strategy

## 1. Validation Layers

Warzone validation is reported in separate layers. They are not interchangeable.

1. Domain compile
   - Plain C# source compile for `Core`, `Content`, `Combat`, `Campaign`, and `Application`.
   - Verifies type wiring and dependency boundaries in domain code.

2. Test source compile
   - Test sources compile against `nunit.framework.dll`.
   - Reported separately for:
     - `ARCH_TEST_SOURCE_COMPILE`
     - `COMBAT_TEST_SOURCE_COMPILE`
     - `SANDBOX_TEST_SOURCE_COMPILE`

3. Actual test execution
   - NUnit or Unity EditMode tests actually run.
   - This is behavioral verification, not just syntax verification.

4. Unity Editor compile
   - Unity imports the project and compiles asmdefs.
   - This is the only reliable signal for Unity-side runtime/editor integration.

5. Sandbox manual run status
   - Whether a sandbox entry was actually opened in Unity and visually checked.
   - This remains separate from automated compile and automated tests.

Final milestone reports must state these separately.

## 2. Test Assembly Boundaries

- `Warzone.Tests.Combat` may reference only:
  - `Warzone.Core`
  - `Warzone.Content`
  - `Warzone.Combat`
- `Warzone.Tests.Combat` must not reference:
  - `Warzone.Sandbox`
  - `Warzone.Runtime`
  - `Unity.InputSystem`
- `Warzone.Tests.Sandbox` is the separate assembly for sandbox scenario and registry tests.
- Unity-facing tests must not be mixed into pure Combat tests.

## 3. Preferred Order

1. Unity EditMode tests, if Unity CLI is available
2. Test source compile with `dotnet` or `msbuild`, if available
3. Framework `csc.exe` domain compile and test source compile
4. Text boundary checks
5. Manual sandbox verification notes

## 4. Repository Scripts

### Domain and test source compile

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_domain_compile.ps1
```

Current behavior:

- compiles domain sources into `Temp\Warzone.DomainValidation.dll`
- compiles `Architecture` tests against local NUnit
- compiles `Combat` tests against local NUnit without pulling in Sandbox sources
- compiles `Sandbox` tests separately against the pure C# scenario and registry sources they need

### Text boundaries

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_text_boundaries.ps1
```

Current behavior:

- checks `UnityEngine` does not appear in domain layers
- checks legacy namespaces do not reappear outside architecture tests

### All-in-one validation pass

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_warzone_all.ps1
```

Current behavior:

- runs text-boundary checks
- runs domain compile and test-source compile checks
- attempts Unity EditMode execution if Unity CLI is available
- reports explicit `OK`, `FAIL`, or `SKIPPED` states

### Unity EditMode entry

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_unity_editmode.ps1
```

Current behavior:

- looks for `Unity.exe`
- if missing, reports a stable `SKIPPED`
- if found, runs batchmode EditMode tests and writes:
  - `artifacts/logs/unity_editmode_M6.log`
  - `artifacts/test-results/editmode_M6.xml`

## 5. Current Environment Constraints

- Unity CLI is not currently available in this environment
- `dotnet` is not currently available in this environment
- validation currently relies on:
  - legacy .NET Framework `csc.exe`
  - local `nunit.framework.dll` from Unity package cache

This is enough for domain compile and test source compile, but not enough for actual Unity test execution.

Every milestone report must explicitly distinguish:

- domain compile
- test source compile
- actual test execution
- Unity Editor compile
- sandbox manual run status
## M10 Test Boundary Notes

- `Campaign.Tests` stays pure Campaign and does not reference Combat or Sandbox
- `Application.Tests` may reference Combat and Campaign because it validates the bridge
- `Sandbox.Tests` is separate and is the only place where Sandbox-facing Unity code should be tested
- validation output should distinguish domain compile, test source compile, actual test execution, and Unity Editor compile

## M11 Test Boundary Notes

- `CampaignWorldProgressionTests` stays pure Campaign and covers site state, world tick, and outpost rules
- `Application` tests cover mission planning, reward resolution, and settlement bridging
- `MissionLaunchPlanFactory` should remain side-effect free
- `MissionSettlementService` should translate combat results into campaign settlements, not mutate world progression rules directly
- `CampaignSiteState` repeat-entry and decay rules should be validated without Unity
- validation output should continue to separate domain compile, test source compile, actual test execution, Unity Editor compile, and sandbox manual run status

## M12 Test Boundary Notes

- save/load tests stay in `Tests/Application` and `Tests/Campaign`
- save DTOs must not depend on `Combat.BattleState`
- `CampaignSaveMapper` should be testable without Unity
- `JsonSaveGameSerializer` should round-trip the DTO snapshot without touching the filesystem
- `SaveGameService` should use an in-memory repository in tests unless a file repository is explicitly introduced later
