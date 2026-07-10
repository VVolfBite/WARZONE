# M11 Validation Summary

## Current Baseline

- Base commit at M11 start: `0fc4e7f5f913ba00ef72cd118217155e9a5157c2`
- Branch: `main`
- Unity Editor compile: not verified

## Goal

Extend Campaign from a one-time settlement loop into a repeatable world progression loop with:

- site state drift
- repeated entry decay
- outpost support
- time-based world ticks
- current site state feeding mission launch plans

## What Changed

- `CampaignSiteState` now tracks:
  - site type
  - searched / cleared / occupied / exhausted state
  - threat level and cap
  - loot remaining and resource richness
  - visit count
  - last updated / last visited time
  - outpost id and tags
- `CampaignState` now tracks outposts
- `CampaignOutpostState` and `CampaignOutpostSystem` provide a minimal local outpost model
- `CampaignTimeSystem` and `CampaignWorldTickSystem` advance world time and apply daily/hourly drift
- `MissionLaunchPlanFactory` reads the current site state instead of only the static site definition
- `MissionRewardResolver` isolates reward profile mapping from settlement code
- `StartingCampaignFactory` now seeds multiple sites and a minimal resource reserve for an outpost

## Validation Summary

### Domain compile

- `DOMAIN_COMPILE: OK`
- `ARCH_TEST_SOURCE_COMPILE: OK`
- `COMBAT_TEST_SOURCE_COMPILE: OK`
- `CAMPAIGN_TEST_SOURCE_COMPILE: OK`
- `APPLICATION_TEST_SOURCE_COMPILE: OK`
- `SANDBOX_TEST_SOURCE_COMPILE: OK`

### Text boundaries

- `UNITYENGINE_BOUNDARY: OK`
- `LEGACY_NAMESPACE_BOUNDARY: OK`

### Unity / actual test execution

- `Unity.exe` was not available in this environment
- `check_unity_editmode.ps1` reported:
  - `UNITY_CLI: SKIPPED - Unity.exe not found`
  - `UNITY_EDITMODE_TEST_EXECUTION: SKIPPED - Unity.exe not found`
  - `UNITY_EDITOR_COMPILE: SKIPPED - Unity.exe not found`

### Static project check

- `check_unity_project_static.ps1: OK`

## Known Issues

- Unity Editor compile is still not verified in this environment
- Actual NUnit execution is still not verified
- `CampaignWorldTickSystem` is intentionally conservative and does not attempt full world simulation

## Notes

- M11 keeps the boundary strict: Campaign owns long-term world state, Application bridges into Combat, and Combat remains free of Campaign references.
- This is not a full economy or world-map system. It is a repeatable site/outpost loop that can be verified in pure code.
