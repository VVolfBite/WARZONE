# Warzone M1 Validation Summary - 2026-07-09

## Scope

This round delivers the M1 member-level squad movement skeleton:

- new `BattleState` root state
- `BattleMemberState` as the real combat carrier
- squad-level `MoveSquadCommand`
- command -> planning -> formation -> movement simulation chain
- snapshot output for Runtime / Sandbox read access
- Sandbox-only M1 bootstrap for member visualization and movement debug

This does **not** deliver the full combat redesign.

## Validation Performed

### 1. Repository baseline

Commands:

- `git status --short --branch`
- `git log -1 --oneline`
- `git remote -v`

Observed baseline:

- branch: `main`
- base commit before M1 work: `ec7585b M0 architecture module closure`

### 2. Offline domain compile

Unity CLI was not available in this environment, so domain validation used:

```powershell
$files = @(Get-ChildItem Assets/_Project/Core -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Content/Definitions -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Content/Catalog -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Content/Validation -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Content/Queries -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Combat -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Campaign -Recurse -Filter *.cs | % FullName) +
         @(Get-ChildItem Assets/_Project/Application -Recurse -Filter *.cs | % FullName)

& 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe' `
  /nologo `
  /target:library `
  /out:Temp\Warzone.DomainValidation.dll `
  /r:System.Numerics.dll `
  $files
```

Result:

- passed

Covered modules:

- `Core`
- `Content/Definitions`
- `Content/Catalog`
- `Content/Validation`
- `Content/Queries`
- `Combat`
- `Campaign`
- `Application`

### 3. Boundary text checks

Commands:

- `rg -n "UnityEngine" Assets/_Project/Core Assets/_Project/Content/Definitions Assets/_Project/Content/Catalog Assets/_Project/Content/Queries Assets/_Project/Content/Validation Assets/_Project/Combat Assets/_Project/Campaign Assets/_Project/Application`
- `rg -n "Warzone\.Adapters|Warzone\.Controls|Warzone\.Presentation" Assets/_Project/Runtime Assets/_Project/Sandbox`

Results:

- no `UnityEngine` references found in domain-layer source
- no legacy Runtime / Sandbox namespace strings found in Runtime or Sandbox source

### 4. Test coverage added

New or expanded source tests:

- `Assets/_Project/Tests/Combat/BattleSimulationTests.cs`
- `Assets/_Project/Tests/Architecture/AssemblyDefinitionTests.cs`

Covered assertions:

- battle state factory creates squad + members
- missing squad move command rejection
- existing squad move command acceptance
- one intent per alive member
- distinct formation target positions
- movement progression toward targets
- snapshot contains squad/member positions
- architecture source / asmdef boundaries still hold

### 5. Unity Editor compile

Unity Editor compile not verified.

Reason:

- Unity CLI / `Unity.exe` was not available in this execution environment
- no Editor import or EditMode runner was executed here

## Sandbox M1 Entry

The new M1 validation entry is:

- `Assets/_Project/Sandbox/BattleSandbox/M1MemberSquadSandboxBootstrap.cs`

Current hookup model:

- add `M1MemberSquadSandboxBootstrap` to an empty `GameObject` in a test scene
- press play
- left click selects the squad marker or a member
- right click issues `MoveSquadCommand`
- `P` or `Space` toggles pause

This path is intentionally Sandbox-only and does not replace the legacy sample demo flow yet.

## Known Limits

- old `BattleSession` and `CombatResolver` remain legacy prototype code
- no damage, cover, smoke, AI, or mission settlement expansion was added to the M1 path
- no Unity scene asset was authored in this environment
- Unity-generated `.meta` files for new source assets will be created by the Editor on import if absent
