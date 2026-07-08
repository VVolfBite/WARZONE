# Warzone 依赖方向与 asmdef 规则

## 1. 总体规则

除明确允许的依赖方向外，任何反向依赖、横向乱依赖、跨层偷调都不允许。依赖方向必须通过 asmdef、架构测试和代码审查共同约束。

允许方向：

```text
Core
  被所有模块依赖

Content
  依赖 Core
  被 Combat / Campaign / Application / Runtime / Sandbox 依赖

Combat
  依赖 Core + Content
  被 Application / Runtime / Sandbox 依赖

Campaign
  依赖 Core + Content
  被 Application / Runtime / Sandbox 依赖

Application
  依赖 Core + Content + Combat + Campaign
  被 Runtime / Sandbox 依赖

Runtime
  依赖 Core + Content + Combat + Campaign + Application
  被 Sandbox 依赖

Sandbox
  依赖 Runtime + Application + Combat + Campaign + Content + Core

Editor
  可依赖必要模块，但只存在于 Editor 编译域
```

## 2. 严格禁止

Core 不得依赖 Content、Combat、Campaign、Application、Runtime、Sandbox、UnityEngine。

Content 不得依赖 Combat、Campaign、Application、Runtime、Sandbox。Content.Definitions 不得依赖 UnityEngine。

Combat 不得依赖 Campaign、Application、Runtime、Sandbox、UnityEngine。

Campaign 不得依赖 Combat、Application、Runtime、Sandbox、UnityEngine。

Application 不得依赖 Runtime、Sandbox、UnityEngine。

Runtime 不得依赖 Sandbox。

正式系统不得依赖 Sandbox。

## 3. Combat 与 Campaign 的隔离

Combat 只处理任务内战斗，不知道长期战役。它只能产出 BattleEvent、BattleSnapshot、BattleResult。

Campaign 只处理长期状态，不知道战斗内部。它不能直接引用 BattleState、BattleMemberState、DamageSystem 等内部类型。

如果 Campaign 需要接收任务结果，应由 Application 读取 Combat 的 BattleResult，然后转成 Campaign 可理解的 Settlement Command 或 DTO。首版可以由 Application/MissionSettlementFlow 直接执行中转。

## 4. Runtime 的边界

Runtime 可以读 Snapshot、消费 Event、发送 Command。Runtime 不能直接改真实 State。

正确方式：

```text
Runtime UI → Command → Application/Combat/Campaign → State 修改 → Snapshot/Event → Runtime 展示
```

错误方式：

```text
Runtime UI → member.Health -= 10
Runtime View → campaign.Resources += 5
Runtime Scene Object → mission.Completed = true
```

## 5. Content.Authoring 特殊规则

Content 的运行时 Definitions 必须是纯数据，不依赖 Unity。ScriptableObject 放在 Content.Authoring 中。

Combat 和 Campaign 不能直接引用 ScriptableObject。正确流程是：

```text
ScriptableObject / JSON → Data → Bake/Validate → Definition → ContentCatalog → Combat/Campaign 读取
```

## 6. 推荐 asmdef

- Warzone.Core：无引用，无 UnityEngine。
- Warzone.Content：引用 Warzone.Core，无 UnityEngine。
- Warzone.Content.Authoring：引用 Core + Content，可使用 UnityEngine；Editor 工具另放 Editor asmdef。
- Warzone.Combat：引用 Core + Content，无 UnityEngine，无 Campaign。
- Warzone.Campaign：引用 Core + Content，无 UnityEngine，无 Combat。
- Warzone.Application：引用 Core + Content + Combat + Campaign，无 Runtime，无 UnityEngine。
- Warzone.Runtime：引用 Application + Combat + Campaign + Content + Core，可使用 UnityEngine。
- Warzone.Sandbox：引用 Runtime + Application + Combat + Campaign + Content + Core。
- Warzone.Editor：Editor only，可引用必要模块。

## 7. 架构测试要求

必须建立 Architecture.Tests，至少检查：

- Core 不引用 UnityEngine。
- Content.Definitions 不引用 UnityEngine。
- Combat 不引用 Campaign、Application、Runtime、Sandbox、UnityEngine。
- Campaign 不引用 Combat、Application、Runtime、Sandbox、UnityEngine。
- Application 不引用 Runtime、Sandbox、UnityEngine。
- Runtime 不引用 Sandbox。
- 非 Sandbox 模块不引用 Sandbox。
- Domain State 不继承 MonoBehaviour 或 ScriptableObject。
- ScriptableObject 不作为运行时可变状态使用。

架构测试是防止后期偷懒的保险。Codex 每次改结构后，都必须更新并运行相关测试。
