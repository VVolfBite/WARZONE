# Warzone 战斗方案设计

## 1. 控制原则

战斗采用实时进行与战术暂停结合的方式。暂停不是辅助功能，而是正式核心机制。玩家可以在暂停中执行与实时状态完全一致的命令，包括移动、攻击、撤离、调整姿态、设置防御区、使用消耗品和查看战场信息。游戏不以操作速度作为核心挑战。

玩家控制小队、火力组、车组和战斗群。玩家不直接控制每一个成员，但成员不是装饰性数据。每个成员有生命、装备、武器挂载、压力、经验和任务结果。玩家下达的是小队级意图，系统负责把它分解为成员级执行。

## 2. 小队与成员

小队是指挥单位，成员是后果单位。

小队保存当前命令、姿态、目标区域、防线、成员列表、压力摘要、队形、可用战术和当前状态。成员保存位置、生命、武器、护甲、消耗品、压力、受伤、死亡、是否撤离、是否占用窗口/掩体/车辆座位。

小队不应再被实现成一个战场点和一根血条。成员应当在战场中有自己的位置，至少在逻辑上有成员目标点、阵型槽位和局部执行状态。玩家不操作这些成员，但战斗系统必须能让成员单独受伤、死亡、掉落武器或占据窗口节点。

## 3. 小队姿态

首版小队至少应支持以下姿态：

快速移动：优先抵达目标，警戒和掩体偏好较低。适合穿越安全区域或快速撤离。

战术推进：保持基本队形，主动利用掩体，适合未知区域和普通推进。

防御驻守：占据防御区、窗口、门口和掩体，优先守住区域和射界。

谨慎搜索：移动较慢，优先搜索容器、建筑和资源点，同时保留警戒。

强攻突入：适用于建筑入口、门窗、狭窄通道和短距离高威胁目标。

撤离优先：优先维持队伍完整、保护伤员和战利品，避免无价值追击。

## 4. 掩体、建筑与战术节点

战斗地图中应存在可被系统理解的战术节点。节点不是玩家逐个微操的点，而是系统用来把小队命令分配给成员的场景语义。

节点类型包括：掩体、窗口、门口、搜索点、撤离点、车辆停靠点、敌人入口点、后撤安全点、建筑区域、危险区。

普通建筑可以进入、驻守、搜索、受损和局部破坏，但不追求完整物理坍塌和逐人室内微操。玩家可以命令小队进入建筑、驻守建筑、清理建筑或搜索建筑。系统根据窗口、门口、室内掩体和成员武器自动分配站位。

玩家可以画防御区或防线，但不应指定“某人站左窗、某人站右窗”。系统根据成员武器、伤病、压力、节点容量和威胁方向自动部署。

## 5. 武器与伤害

普通自动武器采用火力段结算。音效和视觉上可以表现为多发射击，但底层不必逐颗普通子弹模拟。一次机枪扫射可按若干火力段结算伤害、压制、噪声和掩体影响。

高价值单发武器采用逐发结算，例如狙击枪、反器材武器、火箭筒、榴弹、坦克炮和火炮。它们的飞行、命中、爆炸、破甲和范围影响应更明确。

人类成员采用固定生命值、受伤、死亡的简化模型。受伤后统一降低战斗效率、移动、搜索、搬运或压力承受能力，不做眼睛损伤、断肢、永久残疾等细粒度模拟。护甲按武器类别修正伤害，而不是做复杂身体部位防护。载具以后可以使用模块化损伤，例如发动机、武器、轮胎/履带、车体、座位等。

## 6. 爆炸、穿透与掩体

掩体首版可按简单修正或可破坏中立实体处理。报废汽车、墙体、路障、柜台、混凝土块都可以有掩体等级、材质、耐久、遮挡视线和阻挡弹药能力。被破坏后不一定消失，可以变成更弱掩体、燃烧残骸或道路障碍。

穿透是长期目标，但首版可只覆盖最有辨识度的几类：步枪穿玻璃/木门，机枪穿轻掩体，狙击枪穿轻掩体，反器材武器穿车辆或轻墙。完整弹道和材质表后置。

爆炸建议分三层：冲击/爆压、破片、持续危险区。冲击可按半径分层结算；破片可用离散射线处理；火焰、毒雾、烟雾和酸液等作为持续区域状态存在。

## 7. 视野、声音与烟雾

战斗信息应有不完全性。敌人可通过视线、最后已知位置、枪声、脚步、尖叫、车辆声、火焰、爆炸等被感知。烟雾直接作用于视野和锁定，不额外提供魔法减伤。夜战是核心风险层，影响视野、敌人强度、收益和特殊材料获取。

敌人信息不应完全透明。小型敌人不需要显示精确血条；大型敌人、Boss 或高耐久目标可以通过五段血块、外观破损、发黑、动作迟缓等方式提示状态。档案信息可以逐步解锁弱点和行为。

## 8. 压力与溃退

压力主要影响玩家单位。压力不是随机惩罚，而是来自持续伤亡、压制、恐惧效果、夜战、高危敌人、弹药不足、撤离路线被切断等明确原因。普通压力影响资源利用率、行动效率和命令执行。

溃退是小队级状态，不会自动引发其他小队连锁崩溃。只有短时间伤亡惨重或被特殊恐惧效果影响时才触发。触发后不可取消，表现为小队暂时失去正常控制，沿撤离方向后撤。溃退不是丢盔弃甲系统，也不是免费安全撤离；它是战场上的逃兵/崩溃行为。敌人仍按正常规则追击和攻击。

## 9. M4 Tactical Addendum

M4 treats cover as a tactical rules abstraction, not a full physics simulation.
Low cover, high cover, walls, windows, and building blockers are represented as
combat-space obstacles that affect line of sight, fire lines, and incoming damage.

Windows and doorways are first-order tactical nodes, not full interior simulation.
They can anchor defend assignments and create firing positions around a building
shell, but they do not yet imply room ownership, detailed entry stacks, or indoor
pathfinding.

This slice intentionally stops before smoke, night fighting, structural destruction,
ballistic penetration, and full building interiors. Those remain later expansions on
top of the current obstacle, node, and mission-result skeleton.

## 10. M5 Engineering Sandbox Addendum

M5 is an integration and validation milestone. It does not add a new combat mechanic.
The current work is focused on launcher consolidation, view/input cleanup, reset flow,
and Unity-side sandbox reliability around the existing M1-M4 combat slice.

## 11. M6 Pressure / Retreat Addendum

M6 adds first-order battlefield stress feedback:

- `Pressure` rises from incoming fire and direct damage.
- `Suppression` is a threshold state that slows movement and stretches fire cadence.
- `Broken` members switch into retreat behavior and stop accepting normal squad orders.

This is not a full psychology system. It does not model panic contagion across the
entire force, complex morale recovery trees, or narrative trauma states.

Retreat is intended as a clear tactical risk signal. In the current slice it is hard
to override, and normal move / defend / search assignment should not immediately pull
a broken member back into formation.
