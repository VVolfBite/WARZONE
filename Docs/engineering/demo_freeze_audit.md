# Demo Freeze Audit

M15 is a freeze point for the pure-code foundation before the first real Unity compile recovery pass.

## Main modules

- `ContentCatalog` and `DemoContentFactory`
- `CampaignState` and the campaign systems
- `Application` mission, settlement, save, and world services
- `Combat` battle state and result systems
- `Sandbox` launcher and compatibility entry points

## Legacy modules

- `BattleSession`
- `CombatResolver`

These remain legacy and are not being extended.

## Risk areas before Unity import

- asmdef reference boundaries
- Editor script placement
- `Unity.InputSystem` references in Sandbox only
- scene/bootstrap wiring
- manual smoke checks after import

## Do not add before Unity validation

- merchants
- vehicles
- large equipment trees
- new enemy ecologies
- full base construction
- cloud save

## First Unity validation order

1. import the project
2. check Console for asmdef errors
3. run static project checks
4. verify `Warzone.Content` does not reference `Warzone.Combat`
5. verify `Warzone.Combat` does not reference `Warzone.Campaign` or `Warzone.Application`
6. only then open sandbox scenes

## M16 / next-step recommendation

- fix Unity import or compile issues
- verify Sandbox menus and launcher wiring
- only after that consider a formal demo scene

