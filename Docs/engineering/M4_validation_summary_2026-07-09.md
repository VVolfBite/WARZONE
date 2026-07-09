# Warzone M4 Validation Summary - 2026-07-09

## 1. Goal

M4 upgrades the M3 tactical mission slice into a spatial combat slice:

- combat obstacles and first-order building abstractions
- line-of-sight and fire-line checks
- cover-based damage reduction
- window / doorway tactical nodes
- mission objective closure into `BattleResult`
- improved repository-level validation scripts

This is still not the full combat redesign.

## 2. New Modules

Combat additions:

- `TacticalObstacleState`, `TacticalObstacleType`, `BuildingState`
- `LineOfSightRule`, `FireLineRule`, `CoverModifierRule`, `DamageModifierRule`
- `MissionObjectiveSystem`, `BattleResultSystem`
- expanded `BattleResult` supporting objective, casualty, loot, and extraction summaries
- expanded snapshots for obstacles, buildings, mission status, and battle result

Sandbox additions:

- `M4SpatialCombatScenarioFactory`
- `M4SpatialCombatSandboxBootstrap`
- `M4ObstacleView`
- `M4BuildingView`
- `M4FireLineView`
- `M4SpatialCombatDebugPanel`

Repository validation additions:

- `scripts/check_domain_compile.ps1`
- `scripts/check_text_boundaries.ps1`
- `Docs/engineering/test_strategy.md`

## 3. M4 Sandbox How To Run

Unity scene asset creation was not verified in this environment.
Use bootstrap wiring:

1. Open any sandbox-capable scene or create an empty scene.
2. Create an empty `GameObject`.
3. Attach `Warzone.Sandbox.BattleSandbox.M4SpatialCombatSandboxBootstrap`.
4. Enter Play Mode.

Current controls:

- left click: select squad or member
- right click: move selected squad
- `D`: defend area at cursor
- `S`: search nearest incomplete search point
- `E`: extract to nearest extraction point
- `P` or `Space`: pause/resume
- `R`: rebuild scenario
- `C`: toggle debug fire lines

## 4. Spatial Combat Rules

Obstacles:

- `LowCover` does not fully block fire, but can reduce incoming damage.
- `HighCover`, `Wall`, and `BuildingBlocker` can block line of sight and fire lines.
- `Window` is modeled as a special obstacle/node combination that preserves a building-facing firing lane.

Line of sight:

- perception checks now require both distance and a clear `LineOfSightRule` result
- enemy awareness uses the same rule and cannot see through blocking obstacles

Fire line:

- firing checks `FireLineRule` before creating damage requests
- blocked shots produce `ShotBlocked`
- cover-capable obstacles near the target carry a deterministic damage multiplier

Cover modifier:

- low cover currently applies 30% damage reduction
- high cover currently applies 50% damage reduction
- this is a first-order deterministic rule, not a full hit-probability simulation

Building/window abstraction:

- buildings are still shells plus tactical nodes, not indoor navigation spaces
- `Window` and `Doorway` nodes can receive defend assignments
- windows keep a simple firing lane around a building edge without full room modeling

## 5. BattleResult Closure

`BattleResultSystem` builds mission status and final result without coupling Combat back to Campaign.

Current objective closure:

- search objective: required search point is completed
- eliminate objective: required enemies are dead
- extract objective: all surviving members are extracted

Current result payload includes:

- completion type: `Success`, `Failure`, or `Partial`
- objective result list
- dead and surviving member ids
- extracted member ids
- killed enemy ids
- loot count discovered during the slice
- elapsed battle time

## 6. Current Non-Goals

Still out of scope in M4:

- full building interiors
- room ownership and stack entry logic
- smoke
- night battle
- vehicles
- base economy
- campaign settlement

## 7. Validation Commands And Results

Environment reality:

- Unity CLI unavailable
- `dotnet` unavailable
- validation used local .NET Framework `csc.exe`

Commands run:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_domain_compile.ps1
powershell -ExecutionPolicy Bypass -File scripts/check_text_boundaries.ps1
git status --short --branch
git diff --stat
git diff --name-status
```

Observed results:

- source compile: `DOMAIN_COMPILE: OK`
- architecture test source compile: `ARCH_TEST_SOURCE_COMPILE: OK`
- combat test source compile: `COMBAT_TEST_SOURCE_COMPILE: OK`
- text boundary check: `UNITYENGINE_BOUNDARY: OK`
- legacy namespace boundary check: `LEGACY_NAMESPACE_BOUNDARY: OK`

## 8. Test Execution And Unity Compile Status

- test execution: not verified in this environment
- Unity Editor compile: not verified in this environment
- Unity EditMode tests: not run

Required wording:

`Unity Editor compile not verified`

## 9. Known Issues

- obstacle data affects perception, firing, and cover damage, but does not yet block movement pathing
- window/building behavior is a first-order abstraction, not a full interior combat system
- no Unity scene asset was authored for M4 in this environment; bootstrap/manual attachment is the supported path
- actual NUnit or Unity EditMode execution is still not verified here
- legacy `BattleSession` and `CombatResolver` remain in place and should not be extended
