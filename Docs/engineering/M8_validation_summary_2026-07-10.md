# M8 Validation Summary

Recorded package source commit SHA: `f5c94aafaf167f552cda4ebb4dffd9c0e2927ceb`

## 1. Goal

M8 extends the M7 combat slice in three directions:

- move mission-result authority from `RecentEvents` into stable mission runtime state
- add first-order building, window, entrance, and interior-node tactics
- prepare a clearer Unity first-open smoke-check path for the user's local machine

## 2. M8 Fixes For Prior Risks

### MissionResult / RecentEvents

- loot, completed search points, extracted members, killed enemies, and building entry now write into `BattleMissionRuntimeState`
- `MissionObjectiveSystem` reads stable runtime state plus entity state
- `BattleResultSystem` reads stable runtime state instead of inferring loot from `RecentEvents`
- `RecentEvents` remains debug history only

### Unity first-open checklist

- added `Docs/engineering/unity_first_open_checklist.md`
- added `scripts/check_unity_project_static.ps1`
- static checks cover asmdef JSON, asmdef references, Editor script placement, InputSystem package presence, and sandbox menu entries

## 3. Building / Window / Interior Rules

- buildings remain node-based abstractions, not full indoor navigation meshes
- entrance and doorway nodes are entry-capable tactical nodes
- window nodes allow outward vision / fire and expose the occupant to return fire with cover reduction
- interior nodes are inside-building positions and do not freely see or fire outside unless the unit occupies a window / doorway / entrance context
- search building behavior routes one member to a building search node and assigns the rest to support positions

## 4. M8 Sandbox How To Run

On a Unity machine:

1. Open Unity Editor
2. Run `Warzone/Sandbox/Create M8 Building Tactics Sandbox Scene`
3. Open `Assets/Scenes/Sandbox/Sandbox_M8_BuildingTactics.unity`
4. Enter Play Mode

Launcher mode:

- `BattleSandboxMode.M8BuildingTactics`

Controls:

- `LMB` select
- `RMB` move
- `D` defend area
- `S` search point
- `E` extract
- `G` enter nearest building
- `H` defend nearest building
- `J` search nearest building
- `N/B/F/L` keep M7 environment debug controls

## 5. Mission State And BattleResult

Stable state now includes:

- completed search point ids
- entered building ids
- extracted member ids
- killed enemy ids
- loot discovered count and loot source node ids

`BattleResult` is generated from:

- `BattleMissionRuntimeState`
- current member alive / extracted state
- current enemy alive / dead state
- evaluated objective results

It is no longer generated from `RecentEvents` history.

## 6. Validation Commands And Results

### Text boundary

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_text_boundaries.ps1
```

### Domain compile

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_domain_compile.ps1
```

Expected M8 report fields:

- `DOMAIN_COMPILE`
- `ARCH_TEST_SOURCE_COMPILE`
- `COMBAT_TEST_SOURCE_COMPILE`
- `SANDBOX_TEST_SOURCE_COMPILE`

### Unity static project check

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_unity_project_static.ps1
```

Expected M8 report fields:

- `ASMDEF_JSON:*`
- `EDITOR_SCRIPT_PLACEMENT`
- `SANDBOX_SCENE_MENU`
- `INPUT_SYSTEM_PACKAGE`
- `HANDOFF_PATH_NOTE`

### Unity EditMode / compile check

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_unity_editmode.ps1
```

Current environment expectation without Unity CLI:

- `UNITY_EDITMODE_TEST_EXECUTION: SKIPPED - Unity.exe not found`
- `UNITY_EDITOR_COMPILE: SKIPPED - Unity.exe not found`

## 7. Validation State By Layer

- Domain compile: pending final run
- Test source compile: pending final run
- Actual NUnit / Unity EditMode execution: not verified in this environment
- Unity Editor compile: not verified in this environment
- Static Unity project check: pending final run

## 8. Known Issues

- Unity CLI is still unavailable here, so actual Unity compile and EditMode execution remain unverified
- building tactics are first-order node assignments, not room-scale pathfinding
- M8 still does not include indoor breaching AI, structural destruction, or smoke diffusion

## 9. Next Step

The next practical closure after M8 is live Unity verification:

1. create the M8 scene from the menu
2. enter Play Mode
3. verify `G/H/J` building commands
4. verify window visibility / fire behavior
5. verify mission completion and extraction produce the expected `BattleResult`
