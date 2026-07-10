# M16 Code Risk Audit

## 1. Boundary Risks

- Content references Combat: No
- Content references Campaign: No
- Combat references Campaign: No
- Combat references Application: No
- Campaign references Combat: No
- Runtime references Sandbox: No
- Save DTO references Combat BattleState: No
- Tests pollute boundaries: Mostly contained; Application tests intentionally bridge Combat and Campaign, while Combat and Campaign test assemblies stay narrow

## 2. Duplicate Truth Risks

### Member alive / wounded / available

- Source of truth: `CampaignMemberState`
- Derived views: `CampaignCasualtySettlement`, `CampaignWoundSettlement`, save DTOs, debug panels
- Risk: moderate
  - the state is spread across settlement, progression, and recovery systems, so the settlement path must remain explicit

### Weapon available / lost / damaged

- Source of truth: `CampaignWeaponInstanceState`
- Derived views: `CampaignEquipmentSettlement`, save DTOs, launch-plan validation
- Risk: moderate
  - launch validation must stay stricter than load-time reconstruction

### Site searched / exhausted / cleared / occupied

- Source of truth: `CampaignSiteState`
- Derived views: `CampaignSiteSettlement`, `MissionSiteContext`, world-tick rules, save DTOs
- Risk: moderate
  - site state has several related flags, so the risk is ambiguity rather than missing data

### Mission objective completion

- Source of truth: `BattleResult` and `BattleMissionStatusSnapshot` for combat; `CampaignMissionHistoryRecord` for campaign history
- Derived views: settlement, analytics, debug output
- Risk: low

### Loot count

- Source of truth: `BattleResult.LootResult`
- Derived views: campaign resources, item stacks, mission history
- Risk: low

### Current mission

- Source of truth: `CampaignState.CurrentMission`
- Derived views: `CurrentMissionId`, save DTOs, UI/debug
- Risk: moderate
  - the id mirror is useful, but `CurrentMission` remains the object source of truth

### Battle wounded members

- Source of truth: `BattleResult.WoundResult`
- Derived views: settlement wound translation
- Risk: low

### Campaign time

- Source of truth: `CampaignState.CampaignTime`
- Derived views: world tick, site drift, save DTOs, debug
- Risk: low

## 3. ID Boundary Risks

- `CampaignMemberId`: string
- `BattleMemberId` / `BattleEntityId`: int wrapper
- `WeaponInstanceId`: string
- `WeaponDefinitionId`: string
- `SiteId`: string
- `MissionId`: string
- `OutpostId`: string

Assessment:

- the string/int split is acceptable for now because Application owns the bridge
- `MissionLaunchPlan` is the authoritative mapping layer between battle ids and campaign ids
- do not let Combat start depending on Campaign ids directly

## 4. Legacy Risks

- `BattleSession`: legacy
- `CombatResolver`: legacy
- other compatibility bootstrap paths: keep as-is until Unity validation tells us which are still needed

Current rule:

- do not extend legacy paths
- do not migrate more behavior into them
- do not delete them before Unity validation

## 5. Unity Import Risks

- asmdef reference mismatches
- InputSystem package resolution
- Editor menu placement
- Sandbox `MonoBehaviour` compile issues
- missing scene asset entries
- package manifest incompatibilities
- `noEngineReferences` mistakes
- script API version mismatch

## 6. Deferred Until Unity Validation

- exact runtime behavior of Sandbox launcher bootstrap
- actual EditMode / PlayMode execution
- scene creation paths
- first import console noise

