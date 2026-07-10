# Warzone Handoff Packaging

## Rule

Handoff archives must preserve repository-relative paths using forward slashes.

Required top-level entries:

- `Assets/_Project`
- `Packages`
- `ProjectSettings`
- `Docs`
- `scripts`

Do not package:

- `Library`
- `Temp`
- `Obj`
- `Logs`
- `UserSettings`
- build output directories
- `artifacts/logs`
- `artifacts/test-results`
- historical zip files

## Recommended Script

Use:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/create_handoff_package.ps1 -Milestone M13
```

The script should:

- print the current `git rev-parse HEAD`
- stage only the allowed top-level directories
- normalize zip entry names to `/`
- write a small `package_manifest.txt`

## Verification

Use:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_handoff_package.ps1
```

The check should confirm:

- no zip entries contain backslashes
- required top-level prefixes exist
- `package_manifest.txt` exists
