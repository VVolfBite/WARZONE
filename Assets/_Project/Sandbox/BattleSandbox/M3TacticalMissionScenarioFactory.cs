using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public static class M3TacticalMissionScenarioFactory
    {
        public static M3TacticalMissionScenario CreateScenario()
        {
            ContentCatalog contentCatalog = SandboxCombatContentCatalogFactory.CreateTacticalMissionCatalog();
            BattleStateFactory battleStateFactory = new BattleStateFactory();
            BattleState battleState = battleStateFactory.CreateMemberSquadBattle(
                "sandbox.m3.tactical-mission",
                squadId: 1,
                factionId: FactionId.Player,
                rallyPosition: new Vec2(-12f, -5f),
                memberCount: 4,
                formationSpacing: 1.5f,
                startingMemberId: 300,
                memberDefinitionId: "sandbox.rifleman",
                weaponId: "sandbox.rifle",
                memberHealth: 100,
                movementSpeed: 4f,
                detectionRange: 12f,
                attackRange: 11f);

            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(10, TacticalNodeType.Cover, new Vec2(-1f, -2f), 0.8f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(11, TacticalNodeType.Cover, new Vec2(0.5f, -1.5f), 0.8f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(12, TacticalNodeType.Cover, new Vec2(2f, -2f), 0.8f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(13, TacticalNodeType.DefensivePosition, new Vec2(3f, 2f), 0.9f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(14, TacticalNodeType.DefensivePosition, new Vec2(4.5f, 1.8f), 0.9f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(15, TacticalNodeType.Cover, new Vec2(6f, 2.4f), 0.8f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(20, TacticalNodeType.SearchPoint, new Vec2(7.5f, 0.5f), 1.2f, true, 3f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(21, TacticalNodeType.ExtractionPoint, new Vec2(13f, -4f), 1.7f, true, 0f, 1));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(22, TacticalNodeType.EnemyIngress, new Vec2(11.5f, 4.5f), 1.2f));

            battleState.AddEnemy(battleStateFactory.CreateEnemy(9301, "sandbox.raider", FactionId.Enemy, new Vec2(6.5f, 1.5f), 55, 2.5f, 14f, 7f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9302, "sandbox.raider", FactionId.Enemy, new Vec2(8.5f, 1.8f), 55, 2.5f, 14f, 7f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9303, "sandbox.raider", FactionId.Enemy, new Vec2(10.5f, 4.5f), 55, 2.5f, 14f, 7f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9304, "sandbox.raider", FactionId.Enemy, new Vec2(12.2f, 3.8f), 55, 2.5f, 14f, 7f));

            return new M3TacticalMissionScenario(contentCatalog, battleState);
        }
    }
}
