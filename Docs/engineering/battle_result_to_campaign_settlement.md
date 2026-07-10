# BattleResult to CampaignSettlement

This document describes the current M15 battle-result translation boundary.

## Boundary

- `Combat.BattleResult` stays inside Combat.
- `Application.Services.MissionSettlementService` reads `BattleResult`.
- `MissionSettlementService` emits `CampaignSettlement` data.
- `CampaignSettlementSystem` applies the settlement to long-term Campaign state.

## Member outcomes

Current settlement categories:

- dead
- extracted and wounded
- extracted and unwounded
- living but not extracted

Rules:

- dead members do not recover wounds
- extracted unwounded members do not get wound settlements
- extracted wounded members get light wounds on success or moderate wounds on failure
- living members who did not extract get severe wounds in the simplified pre-missing-state model

## Equipment outcomes

Current equipment categories:

- returned
- damaged
- lost

Rules:

- dead or unextracted members lose their weapons
- extracted unwounded members return weapons undamaged
- extracted wounded members return damaged weapons only when the base has no workshop
- workshop capability prevents that damage in the simplified model

## Experience

- extracted survivors gain experience
- dead members gain none
- training capability adds a small bonus

## Simplifications

- no Missing state yet
- no corpse recovery
- no battle-save support
- no campaign-side inference from `RecentEvents`

