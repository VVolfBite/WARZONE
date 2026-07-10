# M14 Validation Summary

## 1. Goal

M14 fixes the `Content -> Combat` boundary leak and adds long-term aftermath systems:

- member progression
- wound recovery
- weapon loss / return / damage
- small base capability effects on recovery and growth

## 2. Boundary fix

- `EnvironmentalZoneDefinition` now uses `EnvironmentalZoneDefinitionType`
- `DemoContentFactory` uses the Content-side enum only
- `Application` maps Content zone types to Combat runtime zone types
- `Content` does not reference `Combat`

## 3. Long-term systems

- `CampaignProgressionSystem`
- `CampaignRecoverySystem`
- `CampaignEquipmentSettlementSystem`
- `MissionPreparationService` marks deployed weapon instances when a mission starts
- `MissionSettlementService` emits experience, wound, and equipment settlements

## 4. Save/load impact

Save DTOs now carry:

- member experience, level, kills, missions completed
- wound severity, recovery days, recovering flag
- weapon availability, loss, damage, durability, mission id

Combat battle state remains unsaved.

## 5. Validation status

- Domain compile: OK
- Test source compile: OK
- Actual test execution: not verified
- Unity Editor compile: not verified
- Static Unity project check: OK
- Handoff package path check: pending package creation

## 6. Known limitations

- no Unity scene work
- no Play Mode verification
- no combat-in-save support
- no full equipment management tree
- no full base-building layer

## 7. Commit SHA

ed202d2c354c8ae63b22a13497ff034e49efc915
