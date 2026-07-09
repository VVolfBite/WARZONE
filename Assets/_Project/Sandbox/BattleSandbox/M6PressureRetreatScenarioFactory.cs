using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public static class M6PressureRetreatScenarioFactory
    {
        public static M6PressureRetreatScenario CreateScenario()
        {
            M4SpatialCombatScenario baseScenario = M4SpatialCombatScenarioFactory.CreateScenario();
            BattleState battleState = baseScenario.BattleState;
            BattleStateFactory battleStateFactory = new BattleStateFactory();

            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(30, TacticalNodeType.RallyPoint, new Vec2(-10.5f, -6.4f), 1.2f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(31, TacticalNodeType.RallyPoint, new Vec2(-6f, -5.8f), 1.2f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(32, TacticalNodeType.DefensivePosition, new Vec2(-1f, -3.6f), 0.9f));

            battleState.AddEnemy(battleStateFactory.CreateEnemy(9406, "sandbox.raider", Warzone.Content.Definitions.FactionId.Enemy, new Vec2(0.5f, -4f), 60, 2.75f, 14f, 8f));

            return new M6PressureRetreatScenario(baseScenario.ContentCatalog, battleState);
        }
    }
}
