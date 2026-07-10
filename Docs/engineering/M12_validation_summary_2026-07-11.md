# M12 Validation Summary

## Current Baseline

- Base commit at M12 start: `bac15bbf314e15747e5e17d537835c703aa7183c`
- Final commit SHA: `f7c0e763e29cc64820de97c00d572e7578198219`
- Branch: `main`
- Unity Editor compile: not verified

## Goal

Add a code-only save/load loop for Campaign long-term state:

- `StartingCampaignFactory`
- `NewGame`
- `CampaignState`
- `SaveGameSnapshot` / save DTOs
- JSON serialization
- in-memory repository
- load back into `CampaignState`
- continue the campaign loop

## What Changed

- `CampaignTimeSystem` / `CampaignWorldTickSystem` remain the world progression core
- `WorldProgressionService` is the recommended Application entry point for time advancement
- `CampaignResourceLedgerState` now supports atomic `CanSpendAll` / `SpendAll`
- `CampaignOutpostSystem` uses atomic resource spending for establishment and maintenance
- `SaveGameSnapshot` captures version and metadata
- `CampaignSaveMapper` converts between `CampaignState` and save DTOs
- `JsonSaveGameSerializer` round-trips the snapshot as JSON text
- `InMemorySaveGameRepository` stores serialized saves by slot
- `CampaignLifecycleService` provides NewGame and snapshot load helpers
- `SaveGameService` provides slot-based save/load orchestration

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
- Actual NUnit / Unity EditMode execution is still not verified
- Save/load is campaign-only; battle state is still ephemeral and not persisted

## Notes

- Save DTOs are kept in Application, not in Combat or Campaign
- Campaign remains the source of truth for long-term state
- This is a baseline persistence loop, not cloud save, encryption, or migration support
