# M10 Validation Summary

- Current branch: `main`
- Current commit SHA: see final response / `git rev-parse HEAD` after packaging

## 1. Goal

M10 extends the campaign loop with resource packages, inventory flow, base maintenance, and settlement fixes.

## 2. M9 Fixes Addressed

- `MissionLaunchPlanFactory` no longer mutates `CampaignState`
- mission start is explicit through `MissionPreparationService.StartMission`
- site search completion is derived from objective completion
- squad availability is computed per squad from member outcomes
- zero loot does not create a resource entry

## 3. Resource / Base Model

- strategic resources live in `CampaignResourceLedgerState`
- item stacks and weapon instances live in `CampaignInventoryState`
- the starting base is represented by `CampaignBaseState`
- module capabilities and maintenance costs are represented by `CampaignBaseModuleState`

## 4. Settlement Flow

1. `BattleResult`
2. `MissionSettlementService`
3. `CampaignSettlement`
4. `CampaignSettlementSystem`
5. `CampaignState`

## 5. Campaign Starter State

`StartingCampaignFactory` creates:

- 4 members
- 1 squad
- initial weapons and items
- initial strategic resources
- a discovered site
- a main base with basic modules

## 6. Validation Plan

- domain compile
- test source compile
- actual test execution if runner is available
- Unity Editor compile not verified in this environment
- static Unity project check

## 7. Validation Results

- `scripts/check_text_boundaries.ps1`: OK
- `scripts/check_domain_compile.ps1`: OK
- `scripts/check_warzone_all.ps1`: OK
- `scripts/check_unity_project_static.ps1`: OK
- `scripts/check_unity_editmode.ps1`: SKIPPED because `Unity.exe` was not found
- actual NUnit / Unity EditMode execution: not verified

## 8. Known Issues

- Unity Editor compile not verified
- M10 does not include full base construction or merchants
