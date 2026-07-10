# M13 Validation Summary

## 1. Goal
Consolidate content definitions into `ContentCatalog` and route the following systems through catalog lookups:
- `StartingCampaignFactory`
- `MissionLaunchPlanFactory`
- `BattleStateFromMissionFactory`
- `MissionRewardResolver`

## 2. Handoff packaging fix
Added:
- `scripts/create_handoff_package.ps1`
- `scripts/check_handoff_package.ps1`

The archive must use forward slashes inside the zip:
- `Assets/_Project/...`

## 3. ContentCatalog structure
`ContentCatalog` now manages:
- weapons
- enemies
- missions
- sites
- items
- resource packages
- outposts
- loot profiles
- environmental zones
- vision equipment
- base modules

## 4. DemoContentFactory contents
`DemoContentFactory.CreateDemoCatalog()` provides demo ids for:
- `rifle`, `pistol`, `enemy_claws`
- `infected_basic`, `infected_runner`, `hostile_scavenger`
- `food`, `medicine`, `ammo`, `fuel`, `building_material`, `modification_material`, `generic_loot`
- `medical_clinic`, `supply_depot`, `residential_block`, `industrial_yard`
- `medical_loot`, `supply_loot`, `generic_loot`
- `basic_outpost`, `watch_post`
- smoke/fire/toxic/light/darkness zones
- vision equipment and base modules

## 5. LootProfile / MissionRewardResolver
Reward resolution now prefers:
1. explicit battle loot / mission reward
2. mission reward profile
3. loot profile definition
4. site-type fallback
5. generic fallback

Zero loot returns no reward.

## 6. StartingCampaignFactory
The starting campaign now comes from demo content when no catalog is passed in.
It reads:
- starting weapon definitions
- base modules
- site definitions
- outpost definitions

## 7. Validation rules
`ContentCatalogValidator` checks:
- duplicate ids
- mission objective and reward references
- site default outpost references
- outpost and base module costs
- item / resource / weapon / environment / vision / base module ids

## 8. Test and validation status
To be filled after running compile and validation scripts.

## 9. Known issues
- Unity Editor compile not verified
- NUnit / Unity EditMode execution not verified

## 10. Next step
Run domain and test-source compile, then verify handoff package paths.

## 11. Current commit baseline
`ef8d0b66512db25491fbf7dc4ea75d9a0290cea0`
