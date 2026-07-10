# M15 Validation Summary

## Status

- Milestone: M15 demo freeze cleanup
- Commit SHA: f2d9a74b84f7d634d4b0c759b93f9f905fe196dd

## Goal

Freeze the code-only foundation before the first real Unity import/compile recovery pass.

## Fixes made in M15

- extracted members are no longer assumed to be wounded
- extracted members are no longer assumed to have damaged weapons
- weapon launch validation rejects lost, unavailable, and damaged instances
- content / combat / campaign / application boundaries are checked more aggressively

## Wound and settlement behavior

See `Docs/engineering/battle_result_to_campaign_settlement.md`.

## Boundary checks

- text boundary scan
- asmdef reference scan
- domain compile
- source compile for tests
- handoff package path normalization

## Demo freeze summary

See `Docs/engineering/demo_freeze_audit.md`.

## Validation results

- domain compile: OK
- test source compile: OK
- actual test execution: not verified
- Unity Editor compile: not verified
- static Unity project check: OK
- asmdef boundary check: OK
- handoff package path check: pending final package generation

## Known issues

- Unity Editor compile still needs a real Unity pass
- actual NUnit / EditMode execution is not verified in this environment
- CombatResolver remains legacy

## Next step

Open Unity on a clean machine and follow the first-open checklist before adding any new gameplay systems.
