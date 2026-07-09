using System.Collections.Generic;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public static class M8BuildingTacticsScenarioFactory
    {
        public static M8BuildingTacticsScenario CreateScenario()
        {
            ContentCatalog contentCatalog = SandboxCombatContentCatalogFactory.CreateBuildingTacticsCatalog();
            BattleStateFactory battleStateFactory = new BattleStateFactory();
            BattleState battleState = battleStateFactory.CreateMemberSquadBattle(
                "sandbox.m8.building-tactics",
                squadId: 1,
                factionId: FactionId.Player,
                rallyPosition: new Vec2(-14f, -4.5f),
                memberCount: 4,
                formationSpacing: 1.5f,
                startingMemberId: 500,
                memberDefinitionId: "sandbox.rifleman",
                weaponId: "sandbox.rifle",
                memberHealth: 100,
                movementSpeed: 4f,
                detectionRange: 12f,
                attackRange: 12f,
                missionDefinitionId: "sandbox.m8.building-tactics",
                nightVisionLevel: 1);

            battleState.EnvironmentState.SetNight(true);
            battleState.EnvironmentState.SetGlobalVisibilityMultiplier(0.55f);
            battleState.EnvironmentState.SetAmbientLightLevel(0.35f);

            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(16, TacticalNodeType.Window, new Vec2(6.4f, 2.7f), 0.75f, true, 0f, null, 100, false, true, true));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(17, TacticalNodeType.Window, new Vec2(8.6f, 2.7f), 0.75f, true, 0f, null, 100, false, true, true));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(18, TacticalNodeType.Doorway, new Vec2(7.5f, 0.8f), 0.8f, true, 0f, null, 100, false, true, true, true));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(23, TacticalNodeType.BuildingEntrance, new Vec2(7.5f, -0.5f), 0.8f, true, 0f, null, 100, false, true, true, true));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(24, TacticalNodeType.BuildingEntrance, new Vec2(9.2f, 0.1f), 0.8f, true, 0f, null, 100, false, true, true, true));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(26, TacticalNodeType.InteriorPosition, new Vec2(6.8f, 1.4f), 0.7f, true, 0f, null, 100, true));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(27, TacticalNodeType.InteriorPosition, new Vec2(8.2f, 1.4f), 0.7f, true, 0f, null, 100, true));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(28, TacticalNodeType.InteriorPosition, new Vec2(6.8f, 0.2f), 0.7f, true, 0f, null, 100, true));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(29, TacticalNodeType.InteriorPosition, new Vec2(8.2f, 0.2f), 0.7f, true, 0f, null, 100, true));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(30, TacticalNodeType.SearchPoint, new Vec2(7.5f, 1.4f), 0.8f, true, 3f, null, 100, true, false, false, false, true));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(21, TacticalNodeType.ExtractionPoint, new Vec2(15f, -4.5f), 1.8f, true, 0f, 1));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(22, TacticalNodeType.EnemyIngress, new Vec2(13.5f, 4.8f), 1.2f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(31, TacticalNodeType.Cover, new Vec2(4.4f, -1.8f), 0.8f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(32, TacticalNodeType.DefensivePosition, new Vec2(3.1f, 2.0f), 0.8f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(33, TacticalNodeType.RallyPoint, new Vec2(-10.5f, -3.5f), 1.2f));

            battleState.AddBuilding(battleStateFactory.CreateBuilding(
                100,
                new Vec2(7.5f, 1.2f),
                2.8f,
                true,
                new List<int> { 16, 17, 18, 23, 24, 26, 27, 28, 29, 30 },
                new List<int> { 18, 23, 24 },
                new List<int> { 16, 17 },
                new List<int> { 26, 27, 28, 29 },
                new List<int> { 30 }));

            battleState.AddObstacle(battleStateFactory.CreateObstacle(206, TacticalObstacleType.BuildingBlocker, new Vec2(7.5f, 1.2f), 1.7f, true, true, true, false, 0f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(207, TacticalObstacleType.Wall, new Vec2(9.5f, 1.2f), 0.9f, true, true, true, false, 0f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(208, TacticalObstacleType.Window, new Vec2(6.4f, 2.7f), 0.35f, false, false, false, true, 0.3f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(209, TacticalObstacleType.Window, new Vec2(8.6f, 2.7f), 0.35f, false, false, false, true, 0.3f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(210, TacticalObstacleType.LowCover, new Vec2(4.4f, -1.8f), 0.75f, false, false, false, true, 0.3f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(211, TacticalObstacleType.HighCover, new Vec2(3.1f, 2.0f), 0.95f, true, false, false, true, 0.5f, 0f));

            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1000, EnvironmentalZoneType.Smoke, new Vec2(-1.5f, -2f), 2.8f, 0.85f, 18f, true, 0.55f, 0f, 0f));
            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1002, EnvironmentalZoneType.Fire, new Vec2(3.4f, -0.8f), 1.5f, 1f, 24f, true, 0f, 8f, 5f));
            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1004, EnvironmentalZoneType.Light, new Vec2(7.5f, 1.2f), 2.6f, 1f, 999f, true, 0f, 0f, 0f));

            battleState.AddEnemy(battleStateFactory.CreateEnemy(9401, "sandbox.raider", FactionId.Enemy, new Vec2(6.8f, 1.4f), 60, 2.75f, 14f, 8f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9402, "sandbox.raider", FactionId.Enemy, new Vec2(8.2f, 1.4f), 60, 2.75f, 14f, 8f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9403, "sandbox.raider", FactionId.Enemy, new Vec2(10.8f, 0.9f), 60, 2.75f, 14f, 8f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9404, "sandbox.raider", FactionId.Enemy, new Vec2(11.6f, 2.4f), 60, 2.75f, 14f, 8f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9405, "sandbox.raider", FactionId.Enemy, new Vec2(13.2f, 4.6f), 60, 2.75f, 14f, 8f, 1));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9406, "sandbox.raider", FactionId.Enemy, new Vec2(9.2f, -0.4f), 60, 2.75f, 14f, 8f));

            BattleEnemyState enemyState;
            if (battleState.TryGetEnemy(new BattleEntityId(9401), out enemyState))
            {
                enemyState.SetOccupiedTacticalNode(26);
            }

            if (battleState.TryGetEnemy(new BattleEntityId(9402), out enemyState))
            {
                enemyState.SetOccupiedTacticalNode(27);
            }

            return new M8BuildingTacticsScenario(contentCatalog, battleState);
        }
    }
}
