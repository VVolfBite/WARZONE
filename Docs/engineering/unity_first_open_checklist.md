# Unity First Open Checklist

## 1. Before Opening Unity

- Confirm branch: `main`
- Confirm expected commit SHA from handoff or remote report
- Confirm the handoff package preserves repository-root paths:
  - `Assets/_Project`
  - `Packages`
  - `ProjectSettings`
  - `Docs`
  - `scripts`

## 2. First Open

1. Open the project in Unity Editor
2. Wait for the initial import to settle
3. Open the Console immediately
4. Record the first compile error, not just the last cascade error

## 3. If asmdef Errors Appear

Check these first:

- `Assets/_Project/*/*.asmdef`
- `Assets/_Project/Tests/*/*.asmdef`
- `Assets/_Project/Sandbox/Warzone.Sandbox.asmdef`
- `Assets/_Project/Editor/Warzone.Editor.asmdef`

Also run:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_unity_project_static.ps1
```

## 4. If InputSystem Errors Appear

Check:

- `Packages/manifest.json` contains `com.unity.inputsystem`
- `Assets/_Project/Sandbox/Warzone.Sandbox.asmdef` references `Unity.InputSystem`
- `Assets/_Project/Tests/Sandbox/Warzone.Tests.Sandbox.asmdef` references `Unity.InputSystem` only if required

## 5. Sandbox Scene Creation

In Unity Editor, run:

- `Warzone/Sandbox/Create M8 Building Tactics Sandbox Scene`

Expected scene path:

- `Assets/Scenes/Sandbox/Sandbox_M8_BuildingTactics.unity`

If the first import is clean, create the M8 scene first, then the M7 scene. Do not start with the heaviest environment scene.

## 6. Manual Smoke Check

1. Open `Assets/Scenes/Sandbox/Sandbox_M8_BuildingTactics.unity`
2. Enter Play Mode
3. Verify launcher mode is `M8BuildingTactics`
4. Verify selection, move, defend, search, extract
5. Verify `G/H/J` building commands
6. Verify the debug panel shows building status, mission status, and recent events

## 7. Recommended Play Order

1. simple M1 or M2 compatibility entry
2. M5 integrated sandbox
3. M7 environment sandbox
4. M8 building sandbox

Do not start with the most complex scene path if the import has not yet been validated.

## 8. Error Capture Format

When reporting back, include:

- branch
- commit SHA
- Unity version
- first Console error
- full stack trace if available
- whether the error happens on import, scene open, or Play Mode

## 9. Useful Logs To Return

- Unity Console error text
- `Editor.log` excerpt around the first compile failure
- output of:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_unity_project_static.ps1
powershell -ExecutionPolicy Bypass -File scripts/check_unity_editmode.ps1
```

## 10. If Reporting To A Remote LLM

Return:

- commit SHA
- exact failing file path
- exact error text
- whether this is:
  - asmdef resolution
  - missing package
  - compile error
  - scene/bootstrap error
  - Play Mode runtime error

## 11. M15 Freeze Context

After M15, the first Unity open should be treated as a compile-risk check, not a feature review.

Verify in this order:

1. project import completes
2. Console has no asmdef resolution failures
3. `Warzone.Content` still has no `Warzone.Combat` references
4. `Warzone.Combat` still has no `Warzone.Campaign` or `Warzone.Application` references
5. `Warzone.Application` still bridges the campaign/combat boundary only through its service layer
6. run the static project checks before trying scenes

If any of those fail, capture the first error and stop. Do not continue into gameplay verification until the assembly graph is healthy.

## 12. M16 Import Focus

After M16, the checklist should be treated as an import recovery plan.

Required first-pass checks:

1. capture `UnityVersion`
2. confirm `Packages/manifest.json`
3. run static project checks
4. verify asmdef graph
5. only then inspect sandbox scenes or launcher wiring

## 13. U1 Playtest Feedback Checks

After U1, prioritize these manual sandbox checks:

1. Hold `P` pauses only while held.
2. `Space` toggles persistent pause.
3. Hold `Shift` shows the current squad command plan overlay.
4. Clicking a member selects the squad and does not create a member micro-control feel.
5. Right-click movement records a squad command and uses ground targeting.
6. Right-clicking over members, obstacles, nodes, or buildings still resolves the command target to ground.
7. Walls and building blockers stop movement and emit `MovementBlocked`.
8. Low cover with `BlocksMovement=false` does not stop movement.
9. The debug panel shows selected squad, current order, desired position, recent commands, and raycast diagnostics.
