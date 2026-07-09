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

        public static ContentCatalog CreateEnvironmentCombatCatalog()
        {
            ContentCatalog baseCatalog = CreateSpatialCombatCatalog();
            return new ContentCatalog(
                baseCatalog.Units,
                baseCatalog.Missions,
                baseCatalog.Abilities,
                baseCatalog.Weapons,
                baseCatalog.Enemies,
                new Dictionary<string, EnvironmentalZoneDefinition>
                {
                    { "sandbox.smoke", new EnvironmentalZoneDefinition("sandbox.smoke", "Smoke Zone", EnvironmentalZoneType.Smoke, 2.8f, 0.85f, 18f, 0.55f, 0f, 0f) },
                    { "sandbox.fire", new EnvironmentalZoneDefinition("sandbox.fire", "Fire Zone", EnvironmentalZoneType.Fire, 1.8f, 1f, 20f, 0f, 8f, 5f) },
                    { "sandbox.toxic", new EnvironmentalZoneDefinition("sandbox.toxic", "Toxic Zone", EnvironmentalZoneType.Toxic, 2.2f, 1f, 24f, 0.15f, 4f, 7f) },
                    { "sandbox.light", new EnvironmentalZoneDefinition("sandbox.light", "Light Zone", EnvironmentalZoneType.Light, 2.4f, 1f, 999f, 0f, 0f, 0f) },
                    { "sandbox.darkness", new EnvironmentalZoneDefinition("sandbox.darkness", "Darkness Zone", EnvironmentalZoneType.Darkness, 3f, 1f, 999f, 0.35f, 0f, 0f) }
                },
                new Dictionary<string, VisionEquipmentDefinition>
                {
                    { "sandbox.nvg.basic", new VisionEquipmentDefinition("sandbox.nvg.basic", "Basic NVG", 1, 0) }
                });
        }

        public static ContentCatalog CreateBuildingTacticsCatalog()
        {
            ContentCatalog baseCatalog = CreateEnvironmentCombatCatalog();
            MissionDefinition mission = new MissionDefinition(
                "sandbox.m8.building-tactics",
                "Building Tactics Slice",
                1,
                1,
                new[]
                {
                    new MissionObjectiveDefinition(MissionObjectiveType.EnterBuilding, "100", 1),
                    new MissionObjectiveDefinition(MissionObjectiveType.SearchPoint, "30", 1),
                    new MissionObjectiveDefinition(MissionObjectiveType.EliminateEnemies, "enemy.all", 6),
                    new MissionObjectiveDefinition(MissionObjectiveType.ExtractSquad, "extract.alpha", 1)
                });

            Dictionary<string, MissionDefinition> missions = new Dictionary<string, MissionDefinition>();
            foreach (KeyValuePair<string, MissionDefinition> pair in baseCatalog.Missions)
            {
                missions.Add(pair.Key, pair.Value);
            }

            missions[mission.Id] = mission;

            return new ContentCatalog(
                baseCatalog.Units,
                missions,
                baseCatalog.Abilities,
                baseCatalog.Weapons,
                baseCatalog.Enemies,
                baseCatalog.EnvironmentalZones,
                baseCatalog.VisionEquipment);
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

        public static BattleState CreateEnvironmentBattleState()
        {
            BattleState battleState = CreateSpatialBattleState();
            BattleStateFactory battleStateFactory = new BattleStateFactory();

            battleState.EnvironmentState.SetNight(true);
            battleState.EnvironmentState.SetGlobalVisibilityMultiplier(0.55f);
            battleState.EnvironmentState.SetAmbientLightLevel(0.35f);

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                memberState.SetNightVisionLevel(1);
                memberState.SetSmokeVisionLevel(0);
                memberState.SetHasLightSource(false);
            }

            bool assignedEnemyNightVision = false;
            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                enemyState.SetNightVisionLevel(assignedEnemyNightVision ? 0 : 1);
                enemyState.SetHasLightSource(false);
                assignedEnemyNightVision = true;
            }

            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1000, EnvironmentalZoneType.Smoke, new Vec2(-1.5f, -2f), 2.8f, 0.85f, 18f, true, 0.55f, 0f, 0f));
            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1001, EnvironmentalZoneType.Smoke, new Vec2(6.2f, 1.8f), 2.2f, 0.75f, 18f, true, 0.45f, 0f, 0f));
            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1002, EnvironmentalZoneType.Fire, new Vec2(3.4f, -0.8f), 1.5f, 1f, 24f, true, 0f, 8f, 5f));
            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1003, EnvironmentalZoneType.Toxic, new Vec2(10.2f, 1.2f), 2f, 1f, 30f, true, 0.15f, 4f, 7f));
            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1004, EnvironmentalZoneType.Light, new Vec2(11.1f, 1.2f), 2.4f, 1f, 999f, true, 0f, 0f, 0f));
            battleState.EnvironmentState.AddZone(battleStateFactory.CreateEnvironmentalZone(1005, EnvironmentalZoneType.Darkness, new Vec2(13.6f, 4.9f), 3f, 1f, 999f, true, 0.35f, 0f, 0f));

            return battleState;
        }

        public static BattleState CreateBuildingTacticsBattleState()
        {
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

            return battleState;
        }
    }
}
