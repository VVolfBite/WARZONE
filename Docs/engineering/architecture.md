# Warzone 工程架构设计

## 1. 架构目标

Warzone 的工程架构必须服务两个长期目标：第一，能在较早阶段形成可运行 Demo/Sandbox，持续验证战斗手感；第二，不能因为快速原型而把核心状态、Unity 表现、数据资产和长期战役逻辑混成一团。

本项目推荐采用“State + System + Rule + Query + Event”的领域结构，而不是传统的 Manager 中心结构，也不建议一开始全面采用 ECS。状态只保存数据，系统修改状态，规则负责可复用计算，查询负责只读检索，事件负责通知外部表现和统计。

核心原则：

- State 只保存状态，不承担复杂行为。
- System 修改 State。
- Rule 做可复用计算。
- Query 做只读查询。
- Event 负责通知，不直接调用 UI、音频或其他系统。
- Simulation/Flow 决定执行顺序，System 之间不随意互相调用。
- Runtime 负责 Unity 输入、显示、场景、动画、音频和调试，不拥有游戏真相。
- Combat 与 Campaign 是两条核心骨架，互不直接依赖。

## 2. 目标目录结构

```text
Assets/_Project/
├─ Core/
├─ Content/
├─ Combat/
├─ Campaign/
├─ Application/
├─ Runtime/
├─ Sandbox/
├─ Editor/
└─ Tests/
```

## 3. Core

Core 是跨系统基础设施层，不是游戏基类层。它可以提供 ID、轻量数学值对象、随机数、时间、结果类型、事件缓冲、序列化辅助、通用接口等。

Core 不应包含 RTSUnit、Weapon、Squad、Base、Mission、Enemy 等游戏领域概念。这些看起来像基础类，但其实属于具体游戏领域，应放到 Content、Combat 或 Campaign。

Core 不依赖 Unity。Unity 的 Vector3、GameObject、MonoBehaviour、ScriptableObject 不能进入 Core。若需要坐标类型，Core 可以使用轻量 Vec2/Vec3，Runtime 再负责与 UnityEngine.Vector3 转换。

## 4. Content

Content 是静态内容资产层。它保存武器、护甲、敌人、小队模板、成员模板、车辆、基地模块、任务、地图地点、资源包、阵营关系等定义。

Content 分为 Data、Definitions、Catalog、Queries、Validation、Authoring。

Data 是编辑态数据，只表达数据长什么样，不提供调用函数。Definitions 是运行态只读定义。Catalog 是统一索引。Queries 是内容查询视图。Validation 负责校验数据是否合法。Authoring 负责 ScriptableObject、导入、烘焙和编辑器输入。

Content 不写战斗行为。武器造成多少伤害、护甲如何减伤、敌人如何选择目标、小队如何移动，这些都属于 Combat 的 Rules 或 Systems。

## 5. Combat

Combat 负责一次任务内的完整战术模拟。它保存 BattleState、BattleMemberState、BattleSquadState、BattleVehicleState、BattleEnemyState、TacticalNodeState、HazardZoneState、MissionRuntimeState 等即时状态。

Combat 应拆为 State、Commands、Systems、Rules、Queries、Events、Results、Factories、Snapshots。

Combat 不依赖 Campaign，不知道基地、长期资源、商人、世界地图和存档。Combat 不依赖 Unity，不知道 GameObject、NavMeshAgent、Camera、UI 或 Scene。

Combat 可以读取 Content 中的静态定义，例如武器定义、敌人定义、任务定义，但不能修改 Content。

## 6. Campaign

Campaign 负责长期战役状态。它保存 CampaignState、RosterState、MemberState、SquadRosterState、InventoryState、ResourceLedgerState、BaseState、OutpostState、WorldMapState、SiteState、MissionOfferState、StoryPathState 等。

Campaign 负责基地、前哨、资源衰减、成员长期状态、库存、车辆、商人、主线进度、动态任务生成、战斗结果写回等。

Campaign 不依赖 Combat，不知道 BattleState、BattleMemberState、DamageSystem 或 VisibilitySystem。它也不依赖 Unity。

战斗结果通过 Application 中转。Combat 产出 BattleResult，Application 调用 Campaign 的结算流程更新长期状态。

## 7. Application

Application 是流程协调层。它连接 Content、Combat 和 Campaign，负责开始新战役、加载存档、准备任务、创建战斗、接收战斗结果、执行结算、保存、切换流程。

Application 不写具体玩法规则，不写 UI，不操作 Unity 场景。它可以定义流程服务和接口，但具体 Unity 场景加载、文件读写、输入和相机都属于 Runtime。

Application 类必须保持薄。如果 Application 中出现大量伤害判断、资源衰减公式、AI 逻辑或 UI 逻辑，就说明职责放错层。

## 8. Runtime

Runtime 是 Unity 运行层，负责输入、相机、UI、Views、动画、音频、特效、导航、物理查询、场景加载、文件持久化和 Presenters。

Runtime 可以发送命令、读取 Snapshot、消费 Event、绑定 GameObject、调用 Unity API。Runtime 不直接修改 BattleState 或 CampaignState。Runtime 管理的是表现状态和交互状态，例如当前选中对象、相机位置、UI 面板状态和 View 绑定关系，不管理成员真实生命、基地资源或任务是否完成。

## 9. Sandbox

Sandbox 是开发验证平台，不是正式内容。它用于快速运行和观察小队移动、战术暂停、掩体、烟雾、夜战、敌人补充、伤亡、溃退、撤离和结算。

Sandbox 可以依赖 Runtime、Application、Combat、Campaign，但正式系统不能反向依赖 Sandbox。

## 10. Editor

Editor 是 Unity 编辑器工具层，不进入正式游戏包。它负责内容校验、地图节点编辑、任务编辑、战术节点自动生成、存档检查和构建工具。

地图编辑器初版不应是完整地形编辑器，而应是战术节点编辑器。美术或程序生成地图结构后，Editor 工具负责标注 CoverNode、WindowNode、SearchPoint、ExtractionPoint、EnemyIngressPoint、RetreatSafeNode、VehicleParkingNode 等。

## 11. Tests

Tests 按被测模块组织，包含 Core.Tests、Content.Tests、Combat.Tests、Campaign.Tests、Application.Tests、Runtime.EditMode.Tests、Runtime.PlayMode.Tests、Architecture.Tests。

Architecture.Tests 是必需品，用于保证依赖方向不被破坏。模块改名或删除时，对应测试必须同步调整，不允许测试目录长期堆积过期文件。

## 12. M2 Tick Order

M2 expands the new combat path from movement-only into a minimal combat slice.
The current intended order inside `BattleSimulation` is:

1. `CommandSystem`
2. `SquadPlanningSystem`
3. `FormationSystem`
4. `MovementSystem`
5. `PerceptionSystem`
6. `TargetSelectionSystem`
7. `FireSystem`
8. `DamageSystem`
9. `DeathCleanupSystem`
10. `BattleSnapshotFactory`

This remains a small technical slice. It is not the final combat model and it does not replace the legacy prototype resolver path yet.

## 13. M3 Tick Order

M3 extends the slice from open-ground fire into a tactical mission loop with nodes,
search, extraction, and enemy response.
The current intended order inside `BattleSimulation` is:

1. `CommandSystem`
2. `SquadPlanningSystem`
3. `FormationSystem`
4. `MovementSystem`
5. `SearchSystem`
6. `ExtractionSystem`
7. `EnemyAwarenessSystem`
8. `EnemyBehaviorSystem`
9. `PerceptionSystem`
10. `TargetSelectionSystem`
11. `FireSystem`
12. `EnemyFireSystem`
13. `DamageSystem`
14. `DeathCleanupSystem`
15. `BattleSnapshotFactory`

This is still a technical slice, not the final tactical combat architecture.

## 14. M4 Tick Order

M4 extends the slice from tactical nodes into first-order spatial combat rules.
The current intended order inside `BattleSimulation` is:

1. `CommandSystem`
2. `SquadPlanningSystem`
3. `FormationSystem`
4. `MovementSystem`
5. `SearchSystem`
6. `ExtractionSystem`
7. `EnemyAwarenessSystem`
8. `EnemyBehaviorSystem`
9. `PerceptionSystem`
10. `TargetSelectionSystem`
11. `FireSystem`
12. `EnemyFireSystem`
13. `DamageSystem`
14. `DeathCleanupSystem`
15. `BattleResultSystem.UpdateMissionStatus`
16. `BattleResultSystem.UpdateBattleResult`
17. `BattleSnapshotFactory`

M4 also adds first-order spatial combat abstractions:

- `TacticalObstacleState` for low cover, high cover, walls, windows, doors, and building blockers
- `LineOfSightRule` to gate perception on distance plus obstacle blocking
- `FireLineRule` to gate shots on obstacle blocking and expose cover-based damage modifiers
- `BuildingState` plus `Window` / `Doorway` tactical nodes as a simple building-facing abstraction
- `BattleResult` closure for search, eliminate, and extract objectives without coupling Combat back to Campaign

This is still a bounded technical slice. It is not the final combat model, does not
include interior navigation, and does not replace the legacy prototype resolver path.

## 15. M5 Sandbox Integration

M5 does not add a new combat rules layer. It consolidates the Unity sandbox path.

Current direction:

- `BattleSandboxLauncher` is the preferred entry instead of attaching per-milestone bootstrap scripts by hand
- `M1` to `M4` bootstrap classes remain as compatibility entries for prior verification paths
- `M5IntegratedSandboxBootstrap` uses a shared runtime context, input controller, view presenter, and debug panel
- sandbox view objects consume `BattleSnapshot` data only and do not mutate Combat state directly
- command input is routed through `TacticalCommandService`

This keeps the engineering sandbox stable without moving Unity concerns back into
Combat or Application.

## 16. M6 Pressure / Retreat Extension

M6 keeps the M5 Unity integration path and extends the Combat tick with a minimal
pressure, suppression, and retreat layer.

Current intended order inside `BattleSimulation` is:

1. `CommandSystem`
2. `SquadPlanningSystem`
3. `FormationSystem`
4. `MovementSystem`
5. `SearchSystem`
6. `ExtractionSystem`
7. `EnemyAwarenessSystem`
8. `EnemyBehaviorSystem`
9. `PerceptionSystem`
10. `TargetSelectionSystem`
11. `FireSystem`
12. `EnemyFireSystem`
13. `DamageSystem`
14. `DeathCleanupSystem`
15. `PressureSystem`
16. `RetreatSystem`
17. `BattleResultSystem.UpdateMissionStatus`
18. `BattleResultSystem.UpdateBattleResult`
19. `BattleSnapshotFactory`

M6 rules remain first-order abstractions:

- pressure rises from incoming fire, damage, and nearby friendly losses
- suppression applies light movement and fire penalties
- broken members retreat to a rally or extraction point when possible
- retreat is a local member response, not a full-squad chain-collapse model

This is still a bounded combat slice. It does not add smoke, night battle,
vehicles, or Campaign settlement.

## 17. M7 Environment Layer

M7 extends the combat slice with first-order environmental combat effects.

Current intended order inside `BattleSimulation` is:

1. `CommandSystem`
2. `SquadPlanningSystem`
3. `FormationSystem`
4. `MovementSystem`
5. `EnvironmentalZoneSystem`
6. `SearchSystem`
7. `ExtractionSystem`
8. `EnemyAwarenessSystem`
9. `EnemyBehaviorSystem`
10. `PerceptionSystem`
11. `TargetSelectionSystem`
12. `FireSystem`
13. `EnemyFireSystem`
14. `DamageSystem`
15. `DeathCleanupSystem`
16. `PressureSystem`
17. `RetreatSystem`
18. `BattleResultSystem.UpdateMissionStatus`
19. `BattleResultSystem.UpdateBattleResult`
20. `BattleSnapshotFactory`

M7 adds:

- `BattleEnvironmentState`
- `EnvironmentalZoneState` for smoke, fire, toxic, light, and darkness zones
- visibility rules that combine:
  - tactical obstacles
  - smoke line blocking
  - night visibility modifiers
  - light/darkness zone modifiers
- environmental damage and environmental pressure as lightweight combat hazards

This remains a technical slice. It is not a full weather system, not a real-time
lighting model, and not a physically simulated smoke solution.

## 18. M8 Building Tactical Layer

M8 extends the combat slice from environmental combat into first-order building tactics.

Current intended order inside `BattleSimulation` remains:

1. `CommandSystem`
2. `SquadPlanningSystem`
3. `FormationSystem`
4. `MovementSystem`
5. `EnvironmentalZoneSystem`
6. `SearchSystem`
7. `ExtractionSystem`
8. `EnemyAwarenessSystem`
9. `EnemyBehaviorSystem`
10. `PerceptionSystem`
11. `TargetSelectionSystem`
12. `FireSystem`
13. `EnemyFireSystem`
14. `DamageSystem`
15. `DeathCleanupSystem`
16. `PressureSystem`
17. `RetreatSystem`
18. `BattleResultSystem.UpdateMissionStatus`
19. `BattleResultSystem.UpdateBattleResult`
20. `BattleSnapshotFactory`

M8 adds:

- stable `BattleMissionRuntimeState` authority for search, loot, extraction, building entry, and enemy kills
- building node groupings for:
  - entrances
  - windows
  - interior positions
  - building search points
- building-facing commands:
  - `EnterBuildingCommand`
  - `DefendBuildingCommand`
  - `SearchBuildingCommand`
- first-order building visibility / fire gating:
  - exterior units cannot freely observe or fire at interior non-window occupants
  - window / doorway / entrance nodes act as tactical firing and observation contexts

This is still not a full interior combat system. It does not include room-level pathfinding,
stacking logic, breaching, or structural destruction.

## 19. M9 Campaign Mission Loop

M9 extends the code-only loop from mission setup into settlement back to Campaign state.

Current intended flow:

1. `CampaignState`
2. `MissionLaunchPlanFactory`
3. `BattleStateFromMissionFactory`
4. `BattleService`
5. `BattleResult`
6. `MissionSettlementService`
7. `CampaignSettlementSystem`
8. `CampaignState`

M9 keeps the boundary strict:

- `Campaign` stores long-term state only
- `Application` bridges `Campaign` and `Combat`
- `Combat` still does not reference `Campaign`
- `MissionLaunchPlan` describes the mission loadout and site context without exposing Campaign internals
- `CampaignSettlement` records the result write-back without depending on Combat types

This is still a technical loop, not the final campaign economy or save/load architecture.

## 20. M10 Campaign Resource and Base Loop

M10 extends the settlement loop into long-term resources and base maintenance.

Current intended flow:

1. `CampaignState`
2. `MissionLaunchPlanFactory`
3. `BattleStateFromMissionFactory`
4. `BattleService`
5. `BattleResult`
6. `MissionSettlementService`
7. `CampaignSettlementSystem`
8. `CampaignResourceConsumptionSystem`
9. `CampaignState`

M10 keeps the boundary strict:

- `MissionLaunchPlanFactory` validates and builds plans, but does not mutate `CampaignState`
- `MissionPreparationService.StartMission` is the explicit point where `CurrentMission` is reserved
- `BattleResult` is translated into campaign-side settlement data before mutating long-term state
- loot becomes strategic resources, item stacks, weapon instances, and mission history entries
- base maintenance is a long-term resource drain, not a per-tick hard fail

This is still a technical loop. It is not full base construction, merchants, formal save/load, or a complete economy model.

## 21. M11 Campaign World / Outpost Loop

M11 extends the Campaign layer from settlement into repeatable world progression.

Current intended flow:

1. `CampaignState`
2. `CampaignTimeSystem`
3. `CampaignWorldTickSystem`
4. `CampaignSiteSystem`
5. `CampaignOutpostSystem`
6. `MissionLaunchPlanFactory`
7. `BattleStateFromMissionFactory`
8. `BattleService`
9. `BattleResult`
10. `MissionSettlementService`
11. `CampaignSettlementSystem`
12. `CampaignState`

M11 adds:

- repeatable site state with discovery, search, exhaustion, occupation, visit count, loot remaining, and threat drift
- outposts as lightweight local campaign nodes, not full bases
- site resource decay over time and on successful search
- launch-plan context that reads the current site state instead of only the static site definition

This remains a bounded campaign loop. It does not add merchants, formal save/load, or a full world-map simulation.

## 22. M12 Campaign Save / Load Loop

M12 adds an Application-side save layer that captures long-term Campaign state and restores it without involving `Combat.BattleState`.

Current intended flow:

1. `StartingCampaignFactory`
2. `CampaignLifecycleService.NewGame`
3. `CampaignState`
4. `MissionPreparationService`
5. `MissionSettlementService`
6. `WorldProgressionService`
7. `CampaignSaveMapper`
8. `SaveGameSerializer`
9. `SaveGameRepository`
10. `LoadGame`
11. `CampaignState`

M12 keeps the boundary strict:

- save DTOs live in `Application`
- `CampaignState` remains the source of truth for long-term data
- `Combat.BattleState` is not saved
- load restores Campaign state by reconstruction, not by mutating Combat data

This is a code-only persistence loop. It is not a Unity file IO feature, not cloud save, and not a version migration framework.
