using System.Collections.Generic;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Sandbox.BattleSandbox
{
    public static class SandboxCombatContentCatalogFactory
    {
        public static ContentCatalog CreateMinimalCombatCatalog()
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

        public static ContentCatalog CreateTacticalMissionCatalog()
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
            UnitDefinition rifleman = new UnitDefinition("sandbox.rifleman", "Rifleman", FactionId.Player, 100, 4f, rifle, 12f, 0.5f, ArmorType.Light);

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
                new Dictionary<string, UnitDefinition> { { rifleman.Id, rifleman } },
                new Dictionary<string, MissionDefinition> { { mission.Id, mission } },
                null,
                new Dictionary<string, WeaponDefinition> { { rifle.Id, rifle } },
                new Dictionary<string, EnemyDefinition> { { raider.Id, raider } });
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

            Dictionary<string, EnvironmentalZoneDefinition> zones = new Dictionary<string, EnvironmentalZoneDefinition>
            {
                { "sandbox.smoke", new EnvironmentalZoneDefinition("sandbox.smoke", "Smoke Zone", Warzone.Combat.EnvironmentalZoneType.Smoke, 2.8f, 0.85f, 18f, 0.55f, 0f, 0f) },
                { "sandbox.fire", new EnvironmentalZoneDefinition("sandbox.fire", "Fire Zone", Warzone.Combat.EnvironmentalZoneType.Fire, 1.8f, 1f, 20f, 0f, 8f, 5f) },
                { "sandbox.toxic", new EnvironmentalZoneDefinition("sandbox.toxic", "Toxic Zone", Warzone.Combat.EnvironmentalZoneType.Toxic, 2.2f, 1f, 24f, 0.15f, 4f, 7f) },
                { "sandbox.light", new EnvironmentalZoneDefinition("sandbox.light", "Light Zone", Warzone.Combat.EnvironmentalZoneType.Light, 2.4f, 1f, 999f, 0f, 0f, 0f) },
                { "sandbox.darkness", new EnvironmentalZoneDefinition("sandbox.darkness", "Darkness Zone", Warzone.Combat.EnvironmentalZoneType.Darkness, 3f, 1f, 999f, 0.35f, 0f, 0f) }
            };

            Dictionary<string, VisionEquipmentDefinition> visionEquipment = new Dictionary<string, VisionEquipmentDefinition>
            {
                { "sandbox.nvg.basic", new VisionEquipmentDefinition("sandbox.nvg.basic", "Basic NVG", 1, 0) },
                { "sandbox.nvg.advanced", new VisionEquipmentDefinition("sandbox.nvg.advanced", "Advanced NVG", 2, 1) }
            };

            return new ContentCatalog(
                baseCatalog.Units,
                baseCatalog.Missions,
                baseCatalog.Abilities,
                baseCatalog.Weapons,
                baseCatalog.Enemies,
                zones,
                visionEquipment);
        }
    }
}
