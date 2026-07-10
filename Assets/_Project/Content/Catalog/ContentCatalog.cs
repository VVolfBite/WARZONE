using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Content
{
    public sealed class ContentCatalog
    {
        public ContentCatalog(
            IReadOnlyDictionary<string, UnitDefinition> units,
            IReadOnlyDictionary<string, MissionDefinition> missions,
            IReadOnlyDictionary<string, AbilityDefinition> abilities = null,
            IReadOnlyDictionary<string, WeaponDefinition> weapons = null,
            IReadOnlyDictionary<string, EnemyDefinition> enemies = null,
            IReadOnlyDictionary<string, EnvironmentalZoneDefinition> environmentalZones = null,
            IReadOnlyDictionary<string, VisionEquipmentDefinition> visionEquipment = null,
            IReadOnlyDictionary<string, SiteDefinition> sites = null,
            IReadOnlyDictionary<string, ItemDefinition> items = null,
            IReadOnlyDictionary<string, ResourcePackageDefinition> resourcePackages = null,
            IReadOnlyDictionary<string, OutpostDefinition> outposts = null)
        {
            Units = units ?? new Dictionary<string, UnitDefinition>();
            Missions = missions ?? new Dictionary<string, MissionDefinition>();
            Abilities = abilities ?? new Dictionary<string, AbilityDefinition>();
            Weapons = weapons ?? new Dictionary<string, WeaponDefinition>();
            Enemies = enemies ?? new Dictionary<string, EnemyDefinition>();
            EnvironmentalZones = environmentalZones ?? new Dictionary<string, EnvironmentalZoneDefinition>();
            VisionEquipment = visionEquipment ?? new Dictionary<string, VisionEquipmentDefinition>();
            Sites = sites ?? new Dictionary<string, SiteDefinition>();
            Items = items ?? new Dictionary<string, ItemDefinition>();
            ResourcePackages = resourcePackages ?? new Dictionary<string, ResourcePackageDefinition>();
            Outposts = outposts ?? new Dictionary<string, OutpostDefinition>();
        }

        public IReadOnlyDictionary<string, UnitDefinition> Units { get; private set; }
        public IReadOnlyDictionary<string, MissionDefinition> Missions { get; private set; }
        public IReadOnlyDictionary<string, AbilityDefinition> Abilities { get; private set; }
        public IReadOnlyDictionary<string, WeaponDefinition> Weapons { get; private set; }
        public IReadOnlyDictionary<string, EnemyDefinition> Enemies { get; private set; }
        public IReadOnlyDictionary<string, EnvironmentalZoneDefinition> EnvironmentalZones { get; private set; }
        public IReadOnlyDictionary<string, VisionEquipmentDefinition> VisionEquipment { get; private set; }
        public IReadOnlyDictionary<string, SiteDefinition> Sites { get; private set; }
        public IReadOnlyDictionary<string, OutpostDefinition> Outposts { get; private set; }
        public IReadOnlyDictionary<string, ItemDefinition> Items { get; private set; }
        public IReadOnlyDictionary<string, ResourcePackageDefinition> ResourcePackages { get; private set; }

        public bool TryGetWeapon(string weaponId, out WeaponDefinition definition)
        {
            return Weapons.TryGetValue(weaponId, out definition);
        }

        public bool TryGetEnemy(string enemyId, out EnemyDefinition definition)
        {
            return Enemies.TryGetValue(enemyId, out definition);
        }

        public bool TryGetMission(string missionId, out MissionDefinition definition)
        {
            return Missions.TryGetValue(missionId, out definition);
        }

        public bool TryGetEnvironmentalZoneDefinition(string zoneId, out EnvironmentalZoneDefinition definition)
        {
            return EnvironmentalZones.TryGetValue(zoneId, out definition);
        }

        public bool TryGetVisionEquipmentDefinition(string equipmentId, out VisionEquipmentDefinition definition)
        {
            return VisionEquipment.TryGetValue(equipmentId, out definition);
        }

        public bool TryGetSite(string siteId, out SiteDefinition definition)
        {
            return Sites.TryGetValue(siteId, out definition);
        }

        public bool TryGetOutpostDefinition(string outpostId, out OutpostDefinition definition)
        {
            return Outposts.TryGetValue(outpostId, out definition);
        }

        public bool TryGetItem(string itemId, out ItemDefinition definition)
        {
            return Items.TryGetValue(itemId, out definition);
        }

        public bool TryGetResourcePackage(string packageId, out ResourcePackageDefinition definition)
        {
            return ResourcePackages.TryGetValue(packageId, out definition);
        }

        public bool TryGetMissionReward(string missionId, out MissionRewardDefinition reward)
        {
            MissionDefinition mission;
            reward = null;
            if (!Missions.TryGetValue(missionId, out mission))
            {
                return false;
            }

            reward = mission.Reward;
            return reward != null;
        }
    }
}
