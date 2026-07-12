# 当前 Warzone 代码框架审阅与改造方向

## 1. 总体判断

当前 Warzone-code 不是废弃物，也不适合完全推倒。它已经有一个可运行原型壳：Unity 工程存在，asmdef 已经初步分层，Combat、Content、Meta、Application、Adapters、Controls、Presentation、Tests 等目录已经出现，Sandbox 场景相关代码也较完整。它适合作为迁移材料和最小验证环境。

但当前代码的核心战斗模型与现在确定的设计方向不匹配。当前模型基本是“一个小队一个位置，一个小队视图，一个小队内有若干血量单位”。而我们现在确定的是“玩家指挥小队，成员是真实战斗载体”。这意味着当前战斗核心需要重建，而不是继续在现有 CombatResolver 上叠功能。

因此建议：保留 Unity 工程和可用外壳，重建核心架构。

## 2. 可保留部分

可以保留或迁移的内容包括：

- Unity 工程本身、ProjectSettings、Packages、场景基础。
- 当前 asmdef 分层的基本意识。
- Content 中已有的 UnitDefinition、WeaponDefinition、MissionDefinition、ContentCatalog、Authoring Asset 雏形。
- 当前 Sandbox 中的相机、输入、选择框、HUD、波次刷怪、View 绑定等原型能力。
- JsonSaveService、SettingsService、MetaStateRepository 的部分思路。
- Tests/Architecture 和 Tests/Combat 的测试目录与基础测试意识。
- System.Numerics 用于脱离 Unity 的纯逻辑层，这点方向正确。

这些内容不一定原样保留，但可以作为迁移对象。

## 3. 主要问题

### 3.1 Combat 过于单体

BattleSession 同时负责 Tick 顺序、命令、AI、状态效果、移动、攻击、伤害事件、结果生成和默认效果库。这会逐渐变成 God Object。后续应拆成 BattleState + BattleSimulation + 多个 Systems。

### 3.2 小队模型不符合目标设计

BattleSquadState 当前有单一 Position、MoveDestination、AttackTargetSquadId、CommandState。Presentation 也按 Squad 生成一个 GameObject。成员 BattleUnitState 主要是小队内部血量对象，没有独立位置、武器挂载、压力、占位节点、局部意图。

这与“玩家不操作个人，但个人是真实载体”的核心设计不一致。必须改为成员有独立战斗状态，小队保存指挥意图和成员列表。

### 3.3 CombatResolver 不可继续扩展

CombatResolver 当前让攻击方每个活单位攻击防守方第一个存活单位，伤害表极简，目标选择和伤害计算绑在一起。它只能支撑最早的波次原型，不能支撑视野、烟雾、掩体、窗口、火力段、压力、撤离和建筑战斗。

后续应拆为 FireSystem、DamageSystem、TargetSelectionSystem、Rules 和 Queries。

### 3.4 Meta 依赖 Combat

Warzone.Meta.asmdef 当前引用 Warzone.Combat，ProgressionService 直接接收 BattleResult 并使用 MissionOutcome。这违反了我们刚确定的规则。长期应改为 Campaign 不依赖 Combat，由 Application 接收 BattleResult 并转成 Campaign Settlement。

### 3.5 Application 依赖 Unity

SceneFlow 直接使用 UnityEngine.SceneManagement。Application.asmdef 当前 noEngineReferences 为 false。这违反了“Application 只负责流程，不依赖 Unity”的目标。场景加载应移动到 Runtime/Scene，Application 只定义 ISceneLoader 或流程接口。

### 3.6 Foundation/Framework 边界模糊

Foundation 当前只有 SimpleObjectPool，但 asmdef 允许 UnityEngine。Framework 目前几乎空，但已被多个模块引用。这两个目录容易变成杂项垃圾桶。建议 Foundation 改为 Core，Framework 暂时移除或禁止新增内容。

### 3.7 Adapters 承担过多职责

Adapters 中混合了 Runtime Host、输入、HUD、Sandbox、Presentation Sync、相机、Audio、NavMesh、View Registry、Spawn Factory 等。它实际是 Runtime + Sandbox + Presentation 的混合层。建议拆成 Runtime 与 Sandbox。

### 3.8 Controls 与 Presentation 分裂

Controls 中有 MainMenu、Debrief 等 UI；Presentation 只有 UnitView。后续应统一进入 Runtime/UI、Runtime/Views 和 Runtime/Presenters，不再保留独立 Controls 层。

## 4. 推荐改造方向

第一阶段不做复杂玩法。重点是结构重建。

目标：

- 新建 Core、Campaign、Runtime、Sandbox、Editor、Tests 分层。
- 将 Foundation 迁移为 Core。
- 将 Meta 迁移为 Campaign，但切断 Combat 依赖。
- 将 Adapters 拆为 Runtime 与 Sandbox。
- 将 Controls 并入 Runtime/UI。
- 将 Presentation 并入 Runtime/Views。
- 建立 Content/Data、Definitions、Catalog、Queries、Validation、Authoring 的基本结构。
- 建立 Combat/State、Commands、Systems、Rules、Queries、Events、Results、Factories、Snapshots 的结构。
- 建立 asmdef 依赖规则和 Architecture.Tests。

第一阶段之后，项目应能编译，Sandbox 入口仍能存在，原有简单战斗可以作为 Legacy 或临时 Demo 保留，但不能作为未来 Combat 核心继续扩展。

## 5. 对旧代码的处理原则

不要删除所有旧代码。应将其分为三类：

第一类，可直接迁移：输入、相机、简单 HUD、Sandbox 视图、Content Authoring、Save/Settings 的部分代码。

第二类，可暂时保留但标记 Legacy：当前 BattleSession、CombatResolver、BattleSquadState、BattleUnitState、SandboxBattleFactory。它们可用于保持 Demo 暂时可运行，但不作为新设计核心。

第三类，应拆解重写：CombatResolver、MissionFlow 与 Meta 的直接耦合、SceneFlow 的 Unity 依赖、Adapters 的混合职责。

## 6. 第一轮改造验收标准

- Unity 能编译。
- 新 asmdef 依赖方向符合规则。
- Architecture.Tests 能检测关键反向依赖。
- Application 不再直接引用 UnityEngine。
- Campaign 不再直接引用 Combat。
- Runtime/Sandbox 承接旧 Adapters 的 Unity 运行职责。
- Combat 新目录结构建立，但复杂玩法不需要实现。
- 至少保留一个能启动的 Sandbox 或 Boot 路径。
