# Unity Error Report Template

Use this template when the first Unity import or compile pass fails.

## Required fields

- branch
- commit SHA
- Unity version
- operation:
  - import
  - compile
  - scene open
  - Play Mode
- first error text
- full stack trace, if available
- affected file path
- asmdef or package involved

## Example structure

```text
branch: main
commit: <sha>
unity_version: <version>
operation: import
first_error: <exact error text>
file: Assets/_Project/Sandbox/...
category: asmdef resolution
```

## What to include

- the first compile error, not the last cascade error
- any `asmdef` resolution failures
- missing package names
- scene/bootstrap errors
- Play Mode runtime errors

## What not to include

- unrelated warnings
- later cascade errors
- assumptions about cause without the raw error text

