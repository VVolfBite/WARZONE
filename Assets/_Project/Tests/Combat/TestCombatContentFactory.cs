using System.Collections.Generic;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Tests.Combat
{
    internal static class TestCombatContentFactory
    {
        public static ContentCatalog CreateCatalog()
        {
            WeaponDefinition rifle = new WeaponDefinition(
                "sandbox.rifle",
                "Sandbox Rifle",
                WeaponCategory.Rifle,
                AmmoCategory.Rifle,
                FireMode.Automatic,
                10f,
                0.45f,
                18,
                1f,
                1,
                0f,
                16f,
                DamageType.Kinetic);

            EnemyDefinition raider = new EnemyDefinition("sandbox.raider", "Raider", 45, 0f, 10f, 0f, FactionId.Enemy, 10, 0.9f);

            Dictionary<string, WeaponDefinition> weapons = new Dictionary<string, WeaponDefinition>
            {
                { rifle.Id, rifle }
            };

            Dictionary<string, EnemyDefinition> enemies = new Dictionary<string, EnemyDefinition>
            {
                { raider.Id, raider }
            };

            Dictionary<string, UnitDefinition> units = new Dictionary<string, UnitDefinition>
            {
                { "sandbox.rifleman", new UnitDefinition("sandbox.rifleman", "Rifleman", FactionId.Player, 100, 4f, rifle, 12f, 0.5f, ArmorType.Light) }
            };

            return new ContentCatalog(units, new Dictionary<string, MissionDefinition>(), null, weapons, enemies);
        }

        public static ContentCatalog CreateTacticalCatalog()
        {
            WeaponDefinition rifle = new WeaponDefinition(
                "sandbox.rifle",
                "Sandbox Rifle",
                WeaponCategory.Rifle,
                AmmoCategory.Rifle,
                FireMode.Automatic,
                11f,
                0.45f,
                18,
                1f,
                1,
                0f,
                16f,
                DamageType.Kinetic);

            EnemyDefinition raider = new EnemyDefinition("sandbox.raider", "Raider", 55, 2.5f, 14f, 7f, FactionId.Enemy, 11, 0.95f);
            MissionDefinition mission = new MissionDefinition(
                "sandbox.m3.tactical-mission",
                "Tactical Mission Slice",
                1,
                1,
                new[]
                {
                    new MissionObjectiveDefinition(MissionObjectiveType.SearchPoint, "search.main"),
                    new MissionObjectiveDefinition(MissionObjectiveType.EliminateEnemies, "enemy.all"),
                    new MissionObjectiveDefinition(MissionObjectiveType.ExtractSquad, "extract.alpha")
                });

            return new ContentCatalog(
                new Dictionary<string, UnitDefinition>
                {
                    { "sandbox.rifleman", new UnitDefinition("sandbox.rifleman", "Rifleman", FactionId.Player, 100, 4f, rifle, 12f, 0.5f, ArmorType.Light) }
                },
                new Dictionary<string, MissionDefinition>
                {
                    { mission.Id, mission }
                },
                null,
                new Dictionary<string, WeaponDefinition>
                {
                    { rifle.Id, rifle }
                },
                new Dictionary<string, EnemyDefinition>
                {
                    { raider.Id, raider }
                });
        }

        public static ContentCatalog CreateSpatialCombatCatalog()
        {
            WeaponDefinition rifle = new WeaponDefinition(
                "sandbox.rifle",
                "Sandbox Rifle",
                WeaponCategory.Rifle,
                AmmoCategory.Rifle,
                FireMode.Automatic,
                12f,
                0.45f,
                18,
                1f,
                1,
                0f,
                16f,
                DamageType.Kinetic);

            EnemyDefinition raider = new EnemyDefinition("sandbox.raider", "Raider", 60, 2.75f, 14f, 8f, FactionId.Enemy, 12, 0.95f);
            UnitDefinition rifleman = new UnitDefinition("sandbox.rifleman", "Rifleman", FactionId.Player, 100, 4f, rifle, 12f, 0.5f, ArmorType.Light);

            MissionDefinition mission = new MissionDefinition(
                "sandbox.m4.spatial-combat",
                "Spatial Combat Slice",
                1,
                1,
                new[]
                {
                    new MissionObjectiveDefinition(MissionObjectiveType.SearchPoint, "search.main", 1),
                    new MissionObjectiveDefinition(MissionObjectiveType.EliminateEnemies, "enemy.all", 5),
                    new MissionObjectiveDefinition(MissionObjectiveType.ExtractSquad, "extract.alpha", 1)
                });

            return new ContentCatalog(
                new Dictionary<string, UnitDefinition> { { rifleman.Id, rifleman } },
                new Dictionary<string, MissionDefinition> { { mission.Id, mission } },
                null,
                new Dictionary<string, WeaponDefinition> { { rifle.Id, rifle } },
                new Dictionary<string, EnemyDefinition> { { raider.Id, raider } });
        }

        public static BattleState CreateSpatialBattleState()
        {
            BattleStateFactory battleStateFactory = new BattleStateFactory();
            BattleState battleState = battleStateFactory.CreateMemberSquadBattle(
                "sandbox.m4.spatial-combat",
                squadId: 1,
                factionId: FactionId.Player,
                rallyPosition: new Vec2(-14f, -5f),
                memberCount: 4,
                formationSpacing: 1.5f,
                startingMemberId: 500,
                memberDefinitionId: "sandbox.rifleman",
                weaponId: "sandbox.rifle",
                memberHealth: 100,
                movementSpeed: 4f,
                detectionRange: 12f,
                attackRange: 12f,
                missionDefinitionId: "sandbox.m4.spatial-combat");

            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(10, TacticalNodeType.Cover, new Vec2(-4f, -2.5f), 0.8f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(11, TacticalNodeType.Cover, new Vec2(-2.5f, -2f), 0.8f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(12, TacticalNodeType.Cover, new Vec2(-0.5f, -1.2f), 0.8f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(13, TacticalNodeType.DefensivePosition, new Vec2(1.5f, 2.2f), 0.9f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(14, TacticalNodeType.DefensivePosition, new Vec2(3f, 2f), 0.9f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(15, TacticalNodeType.Cover, new Vec2(4.5f, -1.8f), 0.8f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(16, TacticalNodeType.Window, new Vec2(6.5f, 2.4f), 0.75f, true, 0f, null, 100));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(17, TacticalNodeType.Window, new Vec2(8.5f, 2.4f), 0.75f, true, 0f, null, 100));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(18, TacticalNodeType.Doorway, new Vec2(7.5f, 0.8f), 0.8f, true, 0f, null, 100));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(20, TacticalNodeType.SearchPoint, new Vec2(11f, 1.2f), 1.3f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(21, TacticalNodeType.ExtractionPoint, new Vec2(15f, -4.5f), 1.8f, true, 0f, 1));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(22, TacticalNodeType.EnemyIngress, new Vec2(13.5f, 4.8f), 1.2f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(23, TacticalNodeType.BuildingEntrance, new Vec2(7.5f, -0.4f), 0.8f, true, 0f, null, 100));

            battleState.AddBuilding(battleStateFactory.CreateBuilding(100, new Vec2(7.5f, 1.5f), 2.8f, true, new List<int> { 16, 17, 18, 23 }));

            battleState.AddObstacle(battleStateFactory.CreateObstacle(200, TacticalObstacleType.LowCover, new Vec2(-4.2f, -2.4f), 0.75f, false, false, false, true, 0.3f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(201, TacticalObstacleType.LowCover, new Vec2(-2.7f, -2f), 0.75f, false, false, false, true, 0.3f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(202, TacticalObstacleType.LowCover, new Vec2(-0.8f, -1.1f), 0.75f, false, false, false, true, 0.3f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(203, TacticalObstacleType.LowCover, new Vec2(4.6f, -1.8f), 0.75f, false, false, false, true, 0.3f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(204, TacticalObstacleType.HighCover, new Vec2(1.7f, 2.2f), 0.95f, true, false, false, true, 0.5f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(205, TacticalObstacleType.HighCover, new Vec2(3.1f, 2f), 0.95f, true, false, false, true, 0.5f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(206, TacticalObstacleType.BuildingBlocker, new Vec2(7.5f, 1.5f), 1.55f, true, true, true, false, 0f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(207, TacticalObstacleType.Wall, new Vec2(9.5f, 1.5f), 0.9f, true, true, true, false, 0f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(208, TacticalObstacleType.Window, new Vec2(6.5f, 2.4f), 0.35f, false, false, false, true, 0.3f, 0f));
            battleState.AddObstacle(battleStateFactory.CreateObstacle(209, TacticalObstacleType.Window, new Vec2(8.5f, 2.4f), 0.35f, false, false, false, true, 0.3f, 0f));

            battleState.AddEnemy(battleStateFactory.CreateEnemy(9401, "sandbox.raider", FactionId.Enemy, new Vec2(10.4f, 1.6f), 60, 2.75f, 14f, 8f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9402, "sandbox.raider", FactionId.Enemy, new Vec2(11.8f, 0.6f), 60, 2.75f, 14f, 8f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9403, "sandbox.raider", FactionId.Enemy, new Vec2(6.5f, 2.4f), 60, 2.75f, 14f, 8f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9404, "sandbox.raider", FactionId.Enemy, new Vec2(8.5f, 2.4f), 60, 2.75f, 14f, 8f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9405, "sandbox.raider", FactionId.Enemy, new Vec2(13.2f, 4.6f), 60, 2.75f, 14f, 8f));
            return battleState;
        }
    }
}
