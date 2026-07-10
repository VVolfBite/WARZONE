# Warzone 核心循环与系统总览

## 1. 三层结构

《Warzone》的游戏结构分为三层：战略地图、任务战斗、基地与整备。

战略地图负责展示世界区域、道路、兴趣点、基地位置、前哨、商人路线、任务机会、区域威胁和资源趋势。它给玩家提供选择：去哪里、做什么、带谁、带多少装备、是否迁移基地、是否建立前哨、是否进行夜间行动。

任务战斗负责一次具体行动。玩家带入小队、装备、车辆和补给，在战斗地图中完成主目标，搜刮资源，清理威胁，救援幸存者，回收装备，最后从撤离方向离开。任务没有硬倒计时，但地图资源有限，敌人会从外部威胁方向持续补充，玩家携带的弹药、医疗、燃料和人员承受能力也有限，因此继续停留的风险会逐渐上升。

基地与整备负责长期成长。基地不是单纯菜单，而是一个可进入的安全场景。玩家可以查看人员、车辆、仓储、伤员和设施模块。基地通过栏位和模块提供医疗、训练、维修、仓储、情报、贸易、防御、生活保障和改造处理等能力。

## 2. 一次行动的标准流程

一次行动通常包括以下阶段：

第一，战略层选择目标。玩家在地图上查看地点、任务、资源和威胁。地点可能是医院、仓库、农场、加油站、街区、巢穴、道路关口、废弃工厂、商场或故事关键地点。

第二，准备战斗群。玩家选择小队、火力组、车辆、装备、消耗品和补给。兵力越多，战斗越强，但消耗更高、噪声更大、撤离更复杂。载具是稀缺高价值资产，可以提供运输、火力、撤离和回收能力，但损坏或遗弃会带来长期损失。

第三，选择进入点。进入点不仅决定出生位置，还影响路线、敌人警觉、载具是否可达、撤离方向和初期风险。普通任务以回到进入方向撤离为主，特殊任务可以有临时撤离点、车辆撤离点或剧情撤离点。

第四，执行任务。玩家通过实时战斗和战术暂停控制小队行动。战斗中可进入建筑、占据窗口、使用掩体、部署烟雾、搜索资源、处理伤员、压制敌人、清理巢穴、完成主目标。

第五，决定撤离。完成主目标后不会自动结束。主目标完成只是给玩家提供撤离资格。玩家可以立刻撤离，也可以继续搜索高价值资源、寻找隐藏目标、回收车辆或清理额外威胁。

第六，结算。任务结束后，存活人员、受伤人员、死亡人员、遗失装备、带回资源、车辆状态、地图状态变化和任务结果写回长期状态。

## 3. 资源驱动而非时间驱动

游戏不依赖硬倒计时逼迫玩家推进。战略层时间按行为推进，而不是玩家站在地图界面不动时自然流逝。旅行、训练、修复、建设、任务、休息、交易等行动会推进时间。

世界推进的核心是资源。区域资源类似补给速率会衰减的令牌桶：初期一个区域较容易提供资源，随着反复搜刮和长期驻留，补给速度逐渐下降，最后仍可能有少量基础产出，但不足以支撑大型战斗群长期高效运转。玩家可以一直留在低风险区域，但收益会变差，发展会变慢。游戏不强制玩家推进，但更危险的新区域天然提供更高价值的资源、装备、基地位置和剧情线索。

## 4. 任务结构

常规任务长度目标为二十到四十五分钟，但实际长度由玩家决定。普通任务可以较短，高价值地点、夜战、主线地点、大型建筑和回收任务可能更久。

任务采用“明确主目标 + 多个可选收益点”的结构。主目标保证任务身份和方向；可选目标则让玩家根据战场实际情况决定是否贪取更多。多任务叠加是核心收益来源：一次搜刮任务中可能临时发现幸存者、巢穴、遗失车辆、特殊材料、支线线索或高价值敌人。

首批动态任务优先包含：指定资源搜刮、清理巢穴或威胁源、搜救幸存者。后续再扩展回收遗失装备/车辆、夺回道路、护送运输、侦察标记、防守据点和寻找特殊物件。

## 5. 地图结构

常规战斗地图以小型连续区域为主。它不是单个房间，也不是完整开放世界，而是一块由道路、外围区域、普通建筑、核心建筑、资源点、敌人入口和撤离方向组成的局部地图。

固定内容包括道路骨架、核心建筑、撤离方向和主要地标。随机内容包括普通建筑组合、敌人位置、资源容器、巢穴方向、局部障碍和事件叠加。普通地图以模块化/程序化为主，主线和大型特殊地点以手工地图为主。

地图允许重复进入。地图结构由随机种子保持稳定，玩家上次行动获得的信息具有价值。已清理建筑、已搜索容器、已破坏墙体和关键地图变化大多保留。敌人状态不完整永久保存，而是保留部分态势、巢穴、警戒和威胁等级，其余按规则重组。

## 6. 成长主轴

玩家长期成长不是单独的人物等级、装备等级或基地等级，而是战斗群整体能力提升。它包括人员规模、编制类型、武器护甲、载具、补给储备、基地模块、前哨网络、区域情报、商人关系、改造能力和可进入区域。

人员可替换，但长期幸存者应留下履历和记忆点。高级武器和载具重要，但需要人员训练、维修、弹药、燃料和基地支持。基地设施不是独立经营游戏，而是战斗群能力的具象化表现。

## 9. M9 局外到局内再回局外

M9 只补最小闭环，不做完整战役系统：

1. 选择可用小队和任务地点
2. 生成 `MissionLaunchPlan`
3. 由 `MissionLaunchPlan` 创建 `BattleState`
4. 任务结束后生成 `BattleResult`
5. `MissionSettlementService` 将结果写回 `CampaignState`

这一轮的重点是边界清楚，而不是玩法完整。Campaign 负责长期状态，Combat 负责单次战斗，Application 负责把两者接起来。
## 10. M10 局外到局内再回局外

M10 把战斗结果真正写回长期状态，但仍然不做完整基地经济。

1. `CampaignState` 保存长期事实。
2. `MissionLaunchPlanFactory` 只负责验证与生成计划，不直接改写 Campaign。
3. `MissionPreparationService.StartMission` 是显式任务开始点。
4. `MissionSettlementService` 把 `BattleResult` 转成 `CampaignSettlement`。
5. `CampaignSettlementSystem` 只应用结算，不自己猜任务历史。
6. `CampaignResourceConsumptionSystem` 负责基地维护和资源消耗。

这一轮的重点是闭环清楚，不是系统做满。

## 11. M11 世界循环与前哨

M11 继续把 Campaign 扩展成可重复进入的世界循环，而不是一次性任务列表。

1. `CampaignSiteState` 记录发现、搜索、清理、占领、耗尽、威胁、访问次数和剩余资源。
2. `CampaignTimeSystem` 与 `CampaignWorldTickSystem` 推进时间并驱动地点变化。
3. 未清理地点会随时间缓慢升高威胁。
4. 搜索会消耗地点资源，重复进入时收益会下降。
5. 已清理地点如果没有前哨，会随着时间回到再占领风险。
6. `CampaignOutpostSystem` 提供最小前哨能力，用来缓解局部威胁和支持安全撤离。
7. `MissionLaunchPlanFactory` 读取当前地点状态，而不是只看静态定义。

这一轮的重点是让世界状态持续演化，而不是每个地点都是一次性的关卡。

## 12. M12 Save / Load in the Core Loop

M12 adds a code-only persistence step to the long-term loop.

1. `StartingCampaignFactory` creates the initial campaign.
2. `SaveGameSnapshot` captures the current Campaign state.
3. `SaveGameSerializer` turns the snapshot into JSON text.
4. `SaveGameRepository` stores the serialized save.
5. `LoadGame` restores `CampaignState` from the snapshot.
6. The campaign can continue through mission preparation, settlement, world ticks, and outpost updates.

This is not combat save/load. It is long-term state persistence only.
## M14 post-battle progression

After a mission, the long-term loop also updates:

- member experience and level
- wounds and recovery time
- weapon loss, return, and damage
- base capability modifiers such as infirmary and workshop

These are Campaign-state changes, not Combat-state changes.
