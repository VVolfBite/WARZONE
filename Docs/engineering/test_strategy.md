# Warzone Test Strategy

## 1. Validation Layers

Warzone validation is split into four different checks. They are not interchangeable.

1. Source compile:
   - domain assemblies compile as plain C# sources
   - this verifies type wiring and dependency boundaries

2. Test source compile:
   - test sources compile against `nunit.framework.dll`
   - this verifies the tests themselves are syntactically valid in the current environment

3. Test execution:
   - tests actually run through Unity EditMode or another supported runner
   - this verifies behavior, not just type wiring

4. Unity Editor compile:
   - Unity imports the project and compiles all assemblies
   - this verifies Unity-side integration, asmdef wiring, and editor/runtime references

Final reports must state these separately.

## 2. Preferred Order

1. Unity EditMode tests, if Unity CLI is available
2. Test source compile with a newer compiler, if `dotnet` or `msbuild` is available
3. Domain compile and text-boundary checks
4. Manual sandbox bootstrap verification notes

## 3. Repository Scripts

### Domain and test source compile

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_domain_compile.ps1
```

Current behavior:

- compiles domain sources into `Temp\Warzone.DomainValidation.dll`
- compiles `Architecture` test sources against local NUnit
- compiles `Combat` test sources against local NUnit and the M4 sandbox scenario factory sources needed by factory tests

### Text boundaries

```powershell
powershell -ExecutionPolicy Bypass -File scripts/check_text_boundaries.ps1
```

Current behavior:

- checks `UnityEngine` does not appear in domain layers
- checks legacy namespaces do not reappear outside architecture tests

## 4. Current Environment Constraints

- Unity CLI is not currently available in this environment
- `dotnet` is not currently available in this environment
- validation currently relies on:
  - legacy .NET Framework `csc.exe`
  - local `nunit.framework.dll` from Unity package cache

This is enough for domain compile and test-source compile, but not enough for actual Unity test execution.
