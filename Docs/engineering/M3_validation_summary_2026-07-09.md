# Warzone M3 Validation Summary - 2026-07-09

## 1. Goal

M3 extends the M2 combat slice into a tactical mission slice:

- tactical nodes for cover, defensive positions, search, extraction, and enemy ingress
- defend, search, and extract squad commands
- search and extraction progression
- minimum enemy awareness, movement, and attack behavior
- bidirectional damage and death
- M3 Sandbox bootstrap for a one-click tactical mission slice

This is still not the final combat redesign.

## 2. New / Updated Modules

### Content

- `MissionObjectiveType`
- `MissionObjectiveDefinition`
- extended `MissionDefinition`
- extended `EnemyDefinition` with attack stats

### Combat

- extended `TacticalNodeState`
- new `TacticalNodeType`
- new commands:
  - `SearchPointCommand`
  - `ExtractSquadCommand`
- new queries:
  - `FindNearestTacticalNodeQuery`
  - `FindAvailableCoverNodeQuery`
  - `FindSearchPointQuery`
  - `FindExtractionPointQuery`
- new systems:
  - `SearchSystem`
  - `ExtractionSystem`
  - `EnemyAwarenessSystem`
  - `EnemyBehaviorSystem`
  - `EnemyFireSystem`
- extended:
  - `BattleMemberState`
  - `BattleEnemyState`
  - `DamageSystem`
  - `DeathCleanupSystem`
  - `BattleSimulation`
- new snapshots:
  - `TacticalNodeSnapshot`
  - `BattleMissionStatusSnapshot`

### Application

- extended `TacticalCommandService` with defend/search/extract command entry points

### Sandbox

- `M3TacticalMissionSandboxBootstrap`
- `M3TacticalMissionScenarioFactory`
- `M3TacticalMissionDebugPanel`
- `M3TacticalNodeView`
- `M3SearchPointView`
- `M3ExtractionPointView`

## 3. M3 Sandbox How To Run

Unity Editor compile was not available in this environment, so the M3 path is prepared as a manual bootstrap.

Manual setup:

1. Open or create a test scene.
2. Create an empty `GameObject`.
3. Add `M3TacticalMissionSandboxBootstrap`.
4. Press Play.

Controls:

- left click: select squad marker or member
- right click: issue squad move command
- `D`: defend current mouse point
- `S`: issue search command to the nearest incomplete search point
- `E`: issue extract command to the nearest extraction point
- `P` or `Space`: pause / resume
- `R`: rebuild scenario

## 4. Tactical Mission Slice

### Tactical nodes

- nodes are pure combat state, not Unity objects
- current node types:
  - `Cover`
  - `DefensivePosition`
  - `SearchPoint`
  - `ExtractionPoint`
  - `EnemyIngress`
  - `RallyPoint`

### Defend

- `DefendAreaCommand` prefers nearby cover / defensive nodes
- if nodes are available, members receive `TakeCover`
- if nodes are not available, the system falls back to formation positions

### Search

- `SearchPointCommand` assigns one active member to the search point
- `SearchSystem` progresses search only when the member is inside the node radius
- completion produces:
  - `SearchCompleted`
  - `LootDiscovered`

### Extraction

- `ExtractSquadCommand` sends active members to the extraction point
- `ExtractionSystem` marks members extracted inside the extraction radius
- extracted members stop moving, firing, perception, and targeting
- when all surviving members are extracted, the system produces `SquadExtracted`

### Enemy awareness

- enemies detect the nearest living, non-extracted member by distance only
- no occlusion, no smoke, no night-vision rules

### Enemy attack

- if the enemy has a target but is outside attack range, it moves directly toward the target
- once inside range and off cooldown, it produces a damage request against the member

### Member death

- `DamageSystem` now supports damage against members and enemies
- dead members stop moving, targeting, firing, searching, and extracting
- `DeathCleanupSystem` clears stale targets

## 5. Explicitly Still Out Of Scope

- full cover simulation
- building interiors / room-by-room occupation
- smoke
- night battle
- vehicles
- base economy
- campaign settlement

## 6. Validation Commands And Results

### Baseline

- `git status --short --branch`
- `git log -1 --oneline`
- `git remote -v`
- `git diff --stat`

Result:

- branch confirmed on `main`
- base M2 commit confirmed as `90a0dcf`

### Text checks

- `rg -n "UnityEngine" Assets/_Project/Core Assets/_Project/Content/Definitions Assets/_Project/Content/Catalog Assets/_Project/Content/Queries Assets/_Project/Content/Validation Assets/_Project/Combat Assets/_Project/Campaign Assets/_Project/Application`
- result: no hits in domain layers

- `rg -n "Warzone.Adapters|Warzone.Controls|Warzone.Presentation" Assets/_Project`
- result: legacy namespace strings remain only in architecture tests as expected assertions

### Offline domain compile

Executed:

```powershell
$files = @(Get-ChildItem Assets/_Project/Core -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Content/Definitions -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Content/Catalog -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Content/Validation -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Content/Queries -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Combat -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Campaign -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Application -Recurse -Filter *.cs | % FullName)

& 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe' `
  /nologo `
  /target:library `
  /out:Temp\Warzone.DomainValidation.dll `
  $files
```

Result:

- passed (`DOMAIN_COMPILE_OK`)

### Test runner

- Unity EditMode runner not available here
- `Architecture` test sources compile against local `nunit.framework.dll`
- `Combat` test sources were also checked, but the legacy .NET Framework `csc.exe` in this environment cannot parse some existing higher-version C# syntax already present in older combat tests
- NUnit test execution not verified in this environment

## 7. Unity Editor Compile

Unity Editor compile not verified

## 8. Known Issues

- M3 Sandbox is bootstrap-based; no dedicated `Sandbox_M3_TacticalMission.unity` scene asset was authored here
- Unity-side compile/import was not validated by the Editor in this environment
- tactical nodes are still first-order abstractions, not full building / window / cover-volume simulation
- enemy behavior is intentionally simple and direct
- legacy `BattleSession` / `CombatResolver` remain in place and are still not part of the new combat path
