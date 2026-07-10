# M16 Design Freeze Review

## 1. Scope

M16 is a freeze audit, not a feature milestone. The goal is to confirm that the current code-only stack still matches the intended game shape before the first Unity import / compile recovery pass.

## 2. Original Design Anchors

The project should continue to behave like this:

- small-scale tactical RTS
- squad as the command object
- members as real combat entities
- combat centered on move, defend, search, extract, fire, casualties, environment, and buildings
- campaign centered on roster, squads, resources, base, sites, outposts, and saves
- no large-scale economy sim
- no hard countdown loop
- no complex morale / psychology simulation
- no full interior simulation
- no full base construction tree
- no vehicle-first progression
- no merchant-first progression

## 3. Current Implementation Mapping

- Combat:
  - members
  - squads
  - tactical nodes
  - obstacles
  - buildings
  - environment
  - pressure / retreat
  - battle results
- Campaign:
  - roster
  - squads
  - inventory
  - resources
  - base
  - sites
  - outposts
  - time
  - save / load
- Application:
  - mission launch plan
  - battle state creation
  - settlement translation
  - save / load
  - world progression
- Content:
  - static catalog
  - demo content
  - validation
- Sandbox:
  - compatibility bootstrap entries
  - launcher
  - views
  - input

## 4. Possible Design Drift

- Overly base-management focused: Watch
  - base and outpost systems exist, but they are still small and campaign-supportive
- Overly equipment-grind focused: Watch
  - inventory and weapon loss exist, but they are bounded by mission result and catalog data
- Overly complex tactical simulation focused: Watch
  - buildings, environment, pressure, and retreat are useful, but still first-order abstractions
- Overly survival-colony focused: OK
  - there is no workforce simulation, housing loop, or colony economy
- Overly Unity-scene-debug-project focused: Watch
  - Unity validation is still missing, so the project can drift into being "code only" too long
- Too many combat rules with too little player decision space: Watch
  - command inputs are present, but Unity presentation is still missing
- Too many long-term systems for the demo target: Watch
  - the foundation is valid, but freeze is the correct next step

## 5. Current Biggest Risks

- Unity Editor compile is still not verified
- actual NUnit / EditMode execution is still not verified
- Sandbox Play Mode is not verified
- legacy M1-M8 bootstrap paths may still carry Unity API risk
- player-facing feedback is still thin
- `DemoMission01` is not yet a real scene
- Content is centralized but not yet paired with a real Unity authoring workflow
- `BattleSession` and `CombatResolver` remain legacy

## 6. Freeze Decision

Pure-code foundation should freeze after M16.

Next phase should be Unity first import / compile recovery.

Do not add M17 gameplay systems before Unity validation.

## 7. Recommended Next Phase

- U1: Unity first import / compile fix
- U2: `Demo_Mission_01` scene engineering
- U3: basic UI / feedback / result panel

