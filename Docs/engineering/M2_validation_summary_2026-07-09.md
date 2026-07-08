# Warzone M2 Validation Summary - 2026-07-09

## 1. Goal

M2 delivers a minimal combat slice on top of the M1 member-level movement skeleton:

- Core tactical coordinate type moved to `Warzone.Core.Math.Vec2`
- minimal content-backed weapon and enemy definitions
- member perception, target selection, fire, damage, death
- M2 Sandbox bootstrap for a playable combat slice

This is still not the full combat redesign.

## 2. New / Updated Modules

### Core

- `Assets/_Project/Core/Math/Vec2.cs`
- `Assets/_Project/Core/Math/Vec3.cs`
- `Assets/_Project/Core/Math/MathUtil.cs`

### Content

- extended `WeaponDefinition`
- extended `UnitDefinition`
- new `EnemyDefinition`
- new `WeaponCategory`, `AmmoCategory`, `FireMode`
- extended `ContentCatalog` with weapon and enemy lookup

### Combat

- public tactical positions moved from `System.Numerics.Vector2` to `Warzone.Core.Math.Vec2`
- `BattleMemberState` expanded with weapon / target / cooldown / perception fields
- `BattleEnemyState` expanded into a real combat entity
- `BattleState` now stores pending damage and recent event history
- new systems:
  - `PerceptionSystem`
  - `TargetSelectionSystem`
  - `FireSystem`
  - `DamageSystem`
  - `DeathCleanupSystem`
- snapshots now include enemies and recent events

### Application

- `Assets/_Project/Application/Services/BattleService.cs`
- `Assets/_Project/Application/Services/TacticalCommandService.cs`

### Sandbox

- `M2CombatSliceSandboxBootstrap`
- `M2CombatSliceScenarioFactory`
- `M2CombatDebugPanel`
- `SandboxCombatContentCatalogFactory`

## 3. M2 Sandbox How To Run

Unity Editor compile was not available in this environment, so the M2 path is prepared as a manual bootstrap.

Manual setup:

1. Open or create a test scene.
2. Create an empty `GameObject`.
3. Add `M2CombatSliceSandboxBootstrap`.
4. Press Play.

Controls:

- left click: select squad marker or member
- right click: issue squad move command
- `P` or `Space`: pause / resume
- `R`: rebuild scenario

## 4. Current Combat Slice

### Perception

- `PerceptionSystem` scans alive enemies by distance only
- no occlusion, no smoke, no night visibility, no physics raycast

### Target Selection

- `TargetSelectionSystem` chooses the nearest alive visible enemy
- dead enemies are ignored

### Fire

- `FireSystem` checks:
  - alive member
  - current target exists
  - target still alive
  - target within attack range
  - cooldown ready
- valid fire creates a `PendingDamageRequest`
- current implementation is deterministic hit, not random spread

### Damage

- `DamageSystem` applies pending damage to enemies
- produces damage events
- marks enemy dead when HP reaches zero

### Death

- `DeathCleanupSystem` clears stale member targets after enemy death

### Snapshot

- `BattleSnapshot` now includes:
  - squads
  - members
  - enemies
  - recent events

## 5. Explicitly Still Out Of Scope

- cover
- smoke
- night combat
- building occupation / garrison
- mission settlement
- enemy AI beyond simple static target entities
- base economy
- vehicles

## 6. Validation Commands And Results

### Baseline

- `git status --short --branch`
- `git log -1 --oneline`
- `git remote -v`

Result:

- branch confirmed on `main`
- base M1 commit confirmed as `094429d`

### Text checks

- `rg -n "UnityEngine" Assets/_Project/Core Assets/_Project/Content/Definitions Assets/_Project/Content/Catalog Assets/_Project/Content/Queries Assets/_Project/Content/Validation Assets/_Project/Combat Assets/_Project/Campaign Assets/_Project/Application`
- result: no hits in domain layers

- `rg -n "Warzone\.Adapters|Warzone\.Controls|Warzone\.Presentation" Assets/_Project`
- result: legacy namespace strings remain only in architecture tests as expected assertions, not revived in Runtime/Sandbox code

- `rg -n "System\.Numerics" Assets/_Project/Combat Assets/_Project/Sandbox/BattleSandbox Assets/_Project/Application Assets/_Project/Tests/Combat Assets/_Project/Core Assets/_Project/Content`
- result: no active M2 tactical coordinate usage left in those source areas

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

- passed

### Test runner

- Unity EditMode runner not available here
- NUnit test assembly execution not verified in this environment

## 7. Unity Editor Compile

Unity Editor compile not verified

## 8. Known Issues

- M2 Sandbox was prepared as bootstrap code only; no dedicated scene asset was authored here
- M1 and M2 Unity-side compile/import was not validated by the Editor in this environment
- old `BattleSession` / `CombatResolver` remain legacy prototype code
- enemy behavior is intentionally passive in M2; enemies are visible and can die, but do not yet run a full combat AI loop
