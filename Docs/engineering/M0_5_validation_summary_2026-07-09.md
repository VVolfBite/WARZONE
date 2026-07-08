# M0.5 Validation Summary - 2026-07-09

## Scope

This summary records the verification completed for the M0.5 architecture closure pass.

## Completed Verification

### Source / text checks

- `Assets/_Project` uses the target module layout:
  - `Core`
  - `Content`
  - `Combat`
  - `Campaign`
  - `Application`
  - `Runtime`
  - `Sandbox`
  - `Editor`
  - `Tests`
- legacy top-level directories were removed:
  - `Foundation`
  - `Meta`
  - `Adapters`
  - `Controls`
  - `Presentation`
  - `Framework`
- Runtime / Sandbox legacy namespaces were cleared from source:
  - `Warzone.Adapters`
  - `Warzone.Controls`
  - `Warzone.Presentation`
- domain layers do not contain `UnityEngine`:
  - `Core`
  - `Content/Definitions`
  - `Content/Catalog`
  - `Content/Queries`
  - `Content/Validation`
  - `Combat`
  - `Campaign`
  - `Application`

### Offline compile validation

Compiler used:

- `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`

Compiled scope:

- `Assets/_Project/Core`
- `Assets/_Project/Content/Definitions`
- `Assets/_Project/Content/Catalog`
- `Assets/_Project/Content/Validation`
- `Assets/_Project/Content/Queries`
- `Assets/_Project/Combat`
- `Assets/_Project/Campaign`
- `Assets/_Project/Application`

Result:

- passed

### Git / delivery state

- commit created:
  - `ec7585b3e16331c1c293c283bdf0f0c3e7532277`
- pushed to:
  - `origin/main`

## Not Verified

Unity Editor compile and Unity EditMode tests were not verified in this environment.

Reason:

- no available `Unity.exe` / Unity CLI

Required follow-up in a Unity-capable environment:

- open project
- let Unity import / recompile
- run EditMode tests
- fix any real Unity-side compile or serialization issues

