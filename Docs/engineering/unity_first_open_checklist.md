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

## 6. Manual Smoke Check

1. Open `Assets/Scenes/Sandbox/Sandbox_M8_BuildingTactics.unity`
2. Enter Play Mode
3. Verify launcher mode is `M8BuildingTactics`
4. Verify selection, move, defend, search, extract
5. Verify `G/H/J` building commands
6. Verify the debug panel shows building status, mission status, and recent events

## 7. Error Capture Format

When reporting back, include:

- branch
- commit SHA
- Unity version
- first Console error
- full stack trace if available
- whether the error happens on import, scene open, or Play Mode

## 8. Useful Logs To Return

- Unity Console error text
- `Editor.log` excerpt around the first compile failure
- output of:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_unity_project_static.ps1
powershell -ExecutionPolicy Bypass -File scripts/check_unity_editmode.ps1
```

## 9. If Reporting To A Remote LLM

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
