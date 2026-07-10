using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Content
{
    public static class DemoContentFactory
    {
        public static ContentCatalog CreateDemoCatalog()
        {
            ContentCatalog catalog = new ContentCatalog();

            WeaponDefinition rifle = new WeaponDefinition(
                "rifle",
                "Rifle",
                WeaponCategory.Rifle,
                AmmoCategory.Rifle,
                FireMode.Automatic,
                12f,
                0.45f,
                18,
                0.95f,
                1,
                0.05f,
                16f,
                DamageType.Kinetic);

            WeaponDefinition pistol = new WeaponDefinition(
                "pistol",
                "Pistol",
                WeaponCategory.Sidearm,
                AmmoCategory.Rifle,
                FireMode.Single,
                8f,
                0.35f,
                10,
                0.9f,
                1,
                0.02f,
                14f,
                DamageType.Kinetic);

            WeaponDefinition claws = new WeaponDefinition(
                "enemy_claws",
                "Claws",
                WeaponCategory.Support,
                AmmoCategory.None,
                FireMode.Single,
                1.5f,
                1.1f,
                8,
                0.85f,
                1,
                0f,
                0f,
                DamageType.Kinetic);

            catalog.RegisterWeapon(rifle);
            catalog.RegisterWeapon(pistol);
            catalog.RegisterWeapon(claws);

            catalog.RegisterEnemy(new EnemyDefinition("infected_basic", "Infected Basic", 40, 2.2f, 10f, 1.5f, FactionId.Enemy, 8, 1f));
            catalog.RegisterEnemy(new EnemyDefinition("infected_runner", "Infected Runner", 28, 3.5f, 9f, 1.5f, FactionId.Enemy, 6, 0.85f));
            catalog.RegisterEnemy(new EnemyDefinition("hostile_scavenger", "Hostile Scavenger", 45, 2.7f, 12f, 8f, FactionId.Enemy, 11, 0.9f));

            catalog.RegisterResourcePackage(new ResourcePackageDefinition("food", "Food", "supply", true, 999));
            catalog.RegisterResourcePackage(new ResourcePackageDefinition("medicine", "Medicine", "medical", true, 999));
            catalog.RegisterResourcePackage(new ResourcePackageDefinition("ammo", "Ammo", "supply", true, 999));
            catalog.RegisterResourcePackage(new ResourcePackageDefinition("fuel", "Fuel", "strategic", true, 999));
            catalog.RegisterResourcePackage(new ResourcePackageDefinition("building_material", "Building Material", "construction", true, 999));
            catalog.RegisterResourcePackage(new ResourcePackageDefinition("modification_material", "Modification Material", "crafting", true, 999));
            catalog.RegisterResourcePackage(new ResourcePackageDefinition("generic_loot", "Generic Loot", "loot", false, 999));

            catalog.RegisterItem(new ItemDefinition("medical_supplies", "Medical Supplies", ItemType.Consumable, 10, "Basic field medical supplies."));
            catalog.RegisterItem(new ItemDefinition("ammunition_box", "Ammunition Box", ItemType.ResourcePackage, 10, "Ammo package."));
            catalog.RegisterItem(new ItemDefinition("scrap_material", "Scrap Material", ItemType.Material, 20, "General salvage."));

            catalog.RegisterSite(new SiteDefinition("medical_clinic", "Medical Clinic", SiteType.Facility, true, 1, "medical_loot", "tactical", "basic_outpost"));
            catalog.RegisterSite(new SiteDefinition("supply_depot", "Supply Depot", SiteType.Compound, true, 3, "supply_loot", "tactical", "basic_outpost"));
            catalog.RegisterSite(new SiteDefinition("residential_block", "Residential Block", SiteType.UrbanBlock, true, 2, "generic_loot", "tactical", "watch_post"));
            catalog.RegisterSite(new SiteDefinition("industrial_yard", "Industrial Yard", SiteType.Compound, true, 4, "generic_loot", "tactical", "watch_post"));

            LootProfileDefinition medicalLoot = new LootProfileDefinition(
                "medical_loot",
                "Medical Loot",
                new[] { new ResourceRewardDefinition("medicine", 1) },
                new[] { new ItemRewardDefinition("medical_supplies", 1) },
                null,
                "medicine");

            LootProfileDefinition supplyLoot = new LootProfileDefinition(
                "supply_loot",
                "Supply Loot",
                new[] { new ResourceRewardDefinition("ammo", 1), new ResourceRewardDefinition("food", 1) },
                new[] { new ItemRewardDefinition("ammunition_box", 1) },
                null,
                "ammo");

            LootProfileDefinition genericLoot = new LootProfileDefinition(
                "generic_loot",
                "Generic Loot",
                new[] { new ResourceRewardDefinition("generic_loot", 1) },
                new[] { new ItemRewardDefinition("scrap_material", 1) },
                null,
                "generic_loot");

            catalog.RegisterLootProfile(medicalLoot);
            catalog.RegisterLootProfile(supplyLoot);
            catalog.RegisterLootProfile(genericLoot);

            catalog.RegisterOutpost(new OutpostDefinition(
                "basic_outpost",
                "Basic Outpost",
                new Dictionary<string, int> { { "building_material", 2 }, { "fuel", 1 } },
                new Dictionary<string, int> { { "food", 1 } },
                new[] { OutpostCapability.SafeExtraction, OutpostCapability.Storage }));

            catalog.RegisterOutpost(new OutpostDefinition(
                "watch_post",
                "Watch Post",
                new Dictionary<string, int> { { "building_material", 3 }, { "fuel", 1 } },
                new Dictionary<string, int> { { "food", 1 } },
                new[] { OutpostCapability.SafeExtraction, OutpostCapability.Watch }));

            catalog.RegisterEnvironmentalZone(new EnvironmentalZoneDefinition("smoke_grenade_zone", "Smoke Grenade Zone", Warzone.Combat.EnvironmentalZoneType.Smoke, 2.8f, 0.8f, 18f, 0.55f, 0f, 0f));
            catalog.RegisterEnvironmentalZone(new EnvironmentalZoneDefinition("fire_patch", "Fire Patch", Warzone.Combat.EnvironmentalZoneType.Fire, 1.8f, 1f, 20f, 0f, 8f, 5f));
            catalog.RegisterEnvironmentalZone(new EnvironmentalZoneDefinition("toxic_spore_cloud", "Toxic Spore Cloud", Warzone.Combat.EnvironmentalZoneType.Toxic, 2.2f, 1f, 24f, 0.15f, 4f, 7f));
            catalog.RegisterEnvironmentalZone(new EnvironmentalZoneDefinition("light_flare", "Light Flare", Warzone.Combat.EnvironmentalZoneType.Light, 2.4f, 1f, 999f, 0f, 0f, 0f));
            catalog.RegisterEnvironmentalZone(new EnvironmentalZoneDefinition("darkness_pool", "Darkness Pool", Warzone.Combat.EnvironmentalZoneType.Darkness, 3f, 1f, 999f, 0.35f, 0f, 0f));

            catalog.RegisterVisionEquipment(new VisionEquipmentDefinition("basic_night_vision", "Basic Night Vision", 1, 0));
            catalog.RegisterVisionEquipment(new VisionEquipmentDefinition("flashlight", "Flashlight", 0, 1));

            catalog.RegisterBaseModule(new BaseModuleDefinition("storage", "Storage", new[] { "storage" }, new Dictionary<string, int> { { "building_material", 1 } }, 20));
            catalog.RegisterBaseModule(new BaseModuleDefinition("infirmary", "Infirmary", new[] { "infirmary" }, new Dictionary<string, int> { { "medicine", 1 } }, 0));
            catalog.RegisterBaseModule(new BaseModuleDefinition("workshop", "Workshop", new[] { "workshop" }, new Dictionary<string, int> { { "building_material", 1 }, { "ammo", 1 } }, 0));
            catalog.RegisterBaseModule(new BaseModuleDefinition("training", "Training Yard", new[] { "training" }, new Dictionary<string, int> { { "food", 1 } }, 0));
            catalog.RegisterBaseModule(new BaseModuleDefinition("watchtower", "Watchtower", new[] { "watchtower" }, new Dictionary<string, int> { { "ammo", 1 } }, 0));

            catalog.RegisterMission(new MissionDefinition(
                "mission.search_medical_supplies",
                "Search Medical Supplies",
                1,
                1,
                new[]
                {
                    new MissionObjectiveDefinition(MissionObjectiveType.SearchPoint, "search.medical", 1),
                    new MissionObjectiveDefinition(MissionObjectiveType.ExtractSquad, "extract.alpha", 1)
                },
                MissionType.Tactical,
                MissionDifficulty.Normal,
                "Facility",
                new MissionRewardDefinition(2, 25, "medical_loot")));

            catalog.RegisterMission(new MissionDefinition(
                "mission.raid_supply_depot",
                "Raid Supply Depot",
                1,
                1,
                new[]
                {
                    new MissionObjectiveDefinition(MissionObjectiveType.EliminateEnemies, "enemy.all", 3),
                    new MissionObjectiveDefinition(MissionObjectiveType.SearchPoint, "search.supply", 1),
                    new MissionObjectiveDefinition(MissionObjectiveType.ExtractSquad, "extract.alpha", 1)
                },
                MissionType.Tactical,
                MissionDifficulty.Normal,
                "Compound",
                new MissionRewardDefinition(3, 30, "supply_loot")));

            catalog.RegisterMission(new MissionDefinition(
                "mission.clear_residential_block",
                "Clear Residential Block",
                1,
                1,
                new[]
                {
                    new MissionObjectiveDefinition(MissionObjectiveType.EliminateEnemies, "enemy.all", 4),
                    new MissionObjectiveDefinition(MissionObjectiveType.SearchPoint, "search.residential", 1),
                    new MissionObjectiveDefinition(MissionObjectiveType.ExtractSquad, "extract.alpha", 1)
                },
                MissionType.Tactical,
                MissionDifficulty.Normal,
                "UrbanBlock",
                new MissionRewardDefinition(2, 20, "generic_loot")));

            catalog.RegisterMission(new MissionDefinition(
                "mission.m9.loop",
                "Campaign Loop Mission",
                1,
                1,
                new[]
                {
                    new MissionObjectiveDefinition(MissionObjectiveType.EnterBuilding, "building.100", 1),
                    new MissionObjectiveDefinition(MissionObjectiveType.SearchPoint, "search.main", 1),
                    new MissionObjectiveDefinition(MissionObjectiveType.EliminateEnemies, "enemy.all", 3),
                    new MissionObjectiveDefinition(MissionObjectiveType.ExtractSquad, "extract.alpha", 1)
                },
                MissionType.Tactical,
                MissionDifficulty.Normal,
                "Compound",
                new MissionRewardDefinition(3, 25, "generic_loot")));

            return catalog;
        }
    }
}
