# M9 Validation Summary

- Date: 2026-07-10
- Branch: `main`
- Commit SHA: see final response from the current turn

## 1. Goal

M9 closes the code-only campaign mission loop:

`CampaignState -> MissionLaunchPlan -> BattleState -> BattleResult -> Settlement -> CampaignState`

This milestone does not add a Unity scene, PlayMode work, or a full economy.

## 2. Why No Unity Demo Scene

The user requested a pure code-verifiable campaign loop for this milestone.
Unity scene engineering is intentionally deferred so the boundary work can be validated without depending on local editor availability.

## 3. Major Additions

- Minimal Campaign long-term state
- Mission launch planning in Application
- Battle state creation from mission plans
- Battle result settlement back into Campaign
- Campaign / Application test coverage

## 4. Campaign Loop

1. `CampaignState` stores roster, inventory, sites, mission history, and current mission.
2. `MissionLaunchPlanFactory` validates squad/site availability and builds `MissionLaunchPlan`.
3. `BattleStateFromMissionFactory` creates a Combat `BattleState`.
4. Combat simulation produces `BattleResult`.
5. `MissionSettlementService` converts the result into `CampaignSettlement`.
6. `CampaignSettlementSystem` applies the settlement back into `CampaignState`.

## 5. CampaignState Structure

Current long-term state includes:

- roster
- squads
- inventory
- sites
- current mission
- mission history
- resource ledger
- campaign time

## 6. MissionLaunchPlan

The launch plan carries only the data needed to start a mission:

- mission id
- site id and site context
- selected squads
- member loadouts
- weapon assignments
- objective definitions
- random seed

It does not expose Combat internals back into Campaign.

## 7. Settlement Rules

- dead battle members are marked dead in Campaign
- extracted members remain available or are marked according to the settlement rule
- loot discovered in battle is written into Campaign inventory
- cleared or searched sites are updated in Campaign site state
- mission history is appended

## 8. Validation Results

### Source compile

- `DOMAIN_COMPILE: OK`
- `ARCH_TEST_SOURCE_COMPILE: OK`
- `COMBAT_TEST_SOURCE_COMPILE: OK`
- `CAMPAIGN_TEST_SOURCE_COMPILE: OK`
- `APPLICATION_TEST_SOURCE_COMPILE: OK`
- `SANDBOX_TEST_SOURCE_COMPILE: OK`

### Actual execution

- `Unity EditMode tests`: not verified
- `Unity PlayMode tests`: not run

### Unity

- `Unity Editor compile not verified`
- static project validation: OK

## 9. Known Issues

- Unity is still not available in this environment, so editor compile and actual NUnit execution remain unverified.
- M9 is a skeleton loop, not the full campaign economy or save/load system.

## 10. Next Step

Use this loop as the base for:

- save/load
- campaign economy
- mission choice UI
- mission result presentation

