using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public static class M2CombatSliceScenarioFactory
    {
        public static M2CombatScenario CreateScenario()
        {
            ContentCatalog contentCatalog = SandboxCombatContentCatalogFactory.CreateMinimalCombatCatalog();
            BattleStateFactory battleStateFactory = new BattleStateFactory();
            BattleState battleState = battleStateFactory.CreateMemberSquadBattle(
                "sandbox.m2.combat-slice",
                squadId: 1,
                factionId: FactionId.Player,
                rallyPosition: new Vec2(-6f, -4f),
                memberCount: 4,
                formationSpacing: 1.4f,
                startingMemberId: 200,
                memberDefinitionId: "sandbox.rifleman",
                weaponId: "sandbox.rifle",
                memberHealth: 100,
                movementSpeed: 4f,
                detectionRange: 12f,
                attackRange: 10f);

            battleState.AddEnemy(battleStateFactory.CreateEnemy(9101, "sandbox.raider", FactionId.Enemy, new Vec2(6f, 0f), 45, 0f, 10f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9102, "sandbox.raider", FactionId.Enemy, new Vec2(9f, 2f), 45, 0f, 10f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9103, "sandbox.raider", FactionId.Enemy, new Vec2(8f, -3f), 45, 0f, 10f));
            battleState.AddTacticalNode(new TacticalNodeState(2, new Vec2(2f, 1f), 2f));

            return new M2CombatScenario(contentCatalog, battleState);
        }
    }
}
