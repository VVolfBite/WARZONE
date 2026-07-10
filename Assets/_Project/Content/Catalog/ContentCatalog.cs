using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Content
{
    public sealed class ContentCatalog
    {
        private readonly Dictionary<string, UnitDefinition> _units;
        private readonly Dictionary<string, MissionDefinition> _missions;
        private readonly Dictionary<string, AbilityDefinition> _abilities;
        private readonly Dictionary<string, WeaponDefinition> _weapons;
        private readonly Dictionary<string, EnemyDefinition> _enemies;
        private readonly Dictionary<string, EnvironmentalZoneDefinition> _environmentalZones;
        private readonly Dictionary<string, VisionEquipmentDefinition> _visionEquipment;
        private readonly Dictionary<string, SiteDefinition> _sites;
        private readonly Dictionary<string, ItemDefinition> _items;
        private readonly Dictionary<string, ResourcePackageDefinition> _resourcePackages;
        private readonly Dictionary<string, OutpostDefinition> _outposts;
        private readonly Dictionary<string, LootProfileDefinition> _lootProfiles;
        private readonly Dictionary<string, BaseModuleDefinition> _baseModules;
        private readonly HashSet<string> _duplicateRegistrationIds = new HashSet<string>();

        public ContentCatalog(
            IReadOnlyDictionary<string, UnitDefinition> units = null,
            IReadOnlyDictionary<string, MissionDefinition> missions = null,
            IReadOnlyDictionary<string, AbilityDefinition> abilities = null,
            IReadOnlyDictionary<string, WeaponDefinition> weapons = null,
            IReadOnlyDictionary<string, EnemyDefinition> enemies = null,
            IReadOnlyDictionary<string, EnvironmentalZoneDefinition> environmentalZones = null,
            IReadOnlyDictionary<string, VisionEquipmentDefinition> visionEquipment = null,
            IReadOnlyDictionary<string, SiteDefinition> sites = null,
            IReadOnlyDictionary<string, ItemDefinition> items = null,
            IReadOnlyDictionary<string, ResourcePackageDefinition> resourcePackages = null,
            IReadOnlyDictionary<string, OutpostDefinition> outposts = null,
            IReadOnlyDictionary<string, LootProfileDefinition> lootProfiles = null,
            IReadOnlyDictionary<string, BaseModuleDefinition> baseModules = null)
        {
            _units = CopyDictionary(units);
            _missions = CopyDictionary(missions);
            _abilities = CopyDictionary(abilities);
            _weapons = CopyDictionary(weapons);
            _enemies = CopyDictionary(enemies);
            _environmentalZones = CopyDictionary(environmentalZones);
            _visionEquipment = CopyDictionary(visionEquipment);
            _sites = CopyDictionary(sites);
            _items = CopyDictionary(items);
            _resourcePackages = CopyDictionary(resourcePackages);
            _outposts = CopyDictionary(outposts);
            _lootProfiles = CopyDictionary(lootProfiles);
            _baseModules = CopyDictionary(baseModules);
        }

        public IReadOnlyDictionary<string, UnitDefinition> Units
        {
            get { return _units; }
        }

        public IReadOnlyDictionary<string, MissionDefinition> Missions
        {
            get { return _missions; }
        }

        public IReadOnlyDictionary<string, AbilityDefinition> Abilities
        {
            get { return _abilities; }
        }

        public IReadOnlyDictionary<string, WeaponDefinition> Weapons
        {
            get { return _weapons; }
        }

        public IReadOnlyDictionary<string, EnemyDefinition> Enemies
        {
            get { return _enemies; }
        }

        public IReadOnlyDictionary<string, EnvironmentalZoneDefinition> EnvironmentalZones
        {
            get { return _environmentalZones; }
        }

        public IReadOnlyDictionary<string, VisionEquipmentDefinition> VisionEquipment
        {
            get { return _visionEquipment; }
        }

        public IReadOnlyDictionary<string, SiteDefinition> Sites
        {
            get { return _sites; }
        }

        public IReadOnlyDictionary<string, ItemDefinition> Items
        {
            get { return _items; }
        }

        public IReadOnlyDictionary<string, ResourcePackageDefinition> ResourcePackages
        {
            get { return _resourcePackages; }
        }

        public IReadOnlyDictionary<string, OutpostDefinition> Outposts
        {
            get { return _outposts; }
        }

        public IReadOnlyDictionary<string, LootProfileDefinition> LootProfiles
        {
            get { return _lootProfiles; }
        }

        public IReadOnlyDictionary<string, BaseModuleDefinition> BaseModules
        {
            get { return _baseModules; }
        }

        public IReadOnlyCollection<string> DuplicateRegistrationIds
        {
            get { return _duplicateRegistrationIds; }
        }

        public bool TryRegisterUnit(UnitDefinition definition)
        {
            return TryRegister(_units, definition, "unit", false);
        }

        public void RegisterUnit(UnitDefinition definition)
        {
            TryRegister(_units, definition, "unit", true);
        }

        public bool TryRegisterMission(MissionDefinition definition)
        {
            return TryRegister(_missions, definition, "mission", false);
        }

        public void RegisterMission(MissionDefinition definition)
        {
            TryRegister(_missions, definition, "mission", true);
        }

        public bool TryRegisterAbility(AbilityDefinition definition)
        {
            return TryRegister(_abilities, definition, "ability", false);
        }

        public void RegisterAbility(AbilityDefinition definition)
        {
            TryRegister(_abilities, definition, "ability", true);
        }

        public bool TryRegisterWeapon(WeaponDefinition definition)
        {
            return TryRegister(_weapons, definition, "weapon", false);
        }

        public void RegisterWeapon(WeaponDefinition definition)
        {
            TryRegister(_weapons, definition, "weapon", true);
        }

        public bool TryRegisterEnemy(EnemyDefinition definition)
        {
            return TryRegister(_enemies, definition, "enemy", false);
        }

        public void RegisterEnemy(EnemyDefinition definition)
        {
            TryRegister(_enemies, definition, "enemy", true);
        }

        public bool TryRegisterEnvironmentalZone(EnvironmentalZoneDefinition definition)
        {
            return TryRegister(_environmentalZones, definition, "environment", false);
        }

        public void RegisterEnvironmentalZone(EnvironmentalZoneDefinition definition)
        {
            TryRegister(_environmentalZones, definition, "environment", true);
        }

        public bool TryRegisterVisionEquipment(VisionEquipmentDefinition definition)
        {
            return TryRegister(_visionEquipment, definition, "vision", false);
        }

        public void RegisterVisionEquipment(VisionEquipmentDefinition definition)
        {
            TryRegister(_visionEquipment, definition, "vision", true);
        }

        public bool TryRegisterSite(SiteDefinition definition)
        {
            return TryRegister(_sites, definition, "site", false);
        }

        public void RegisterSite(SiteDefinition definition)
        {
            TryRegister(_sites, definition, "site", true);
        }

        public bool TryRegisterItem(ItemDefinition definition)
        {
            return TryRegister(_items, definition, "item", false);
        }

        public void RegisterItem(ItemDefinition definition)
        {
            TryRegister(_items, definition, "item", true);
        }

        public bool TryRegisterResourcePackage(ResourcePackageDefinition definition)
        {
            return TryRegister(_resourcePackages, definition, "resource", false);
        }

        public void RegisterResourcePackage(ResourcePackageDefinition definition)
        {
            TryRegister(_resourcePackages, definition, "resource", true);
        }

        public bool TryRegisterOutpost(OutpostDefinition definition)
        {
            return TryRegister(_outposts, definition, "outpost", false);
        }

        public void RegisterOutpost(OutpostDefinition definition)
        {
            TryRegister(_outposts, definition, "outpost", true);
        }

        public bool TryRegisterLootProfile(LootProfileDefinition definition)
        {
            return TryRegister(_lootProfiles, definition, "loot_profile", false);
        }

        public void RegisterLootProfile(LootProfileDefinition definition)
        {
            TryRegister(_lootProfiles, definition, "loot_profile", true);
        }

        public bool TryRegisterBaseModule(BaseModuleDefinition definition)
        {
            return TryRegister(_baseModules, definition, "base_module", false);
        }

        public void RegisterBaseModule(BaseModuleDefinition definition)
        {
            TryRegister(_baseModules, definition, "base_module", true);
        }

        public bool TryGetWeapon(string weaponId, out WeaponDefinition definition)
        {
            return _weapons.TryGetValue(weaponId, out definition);
        }

        public bool TryGetEnemy(string enemyId, out EnemyDefinition definition)
        {
            return _enemies.TryGetValue(enemyId, out definition);
        }

        public bool TryGetMission(string missionId, out MissionDefinition definition)
        {
            return _missions.TryGetValue(missionId, out definition);
        }

        public bool TryGetMissionReward(string missionId, out MissionRewardDefinition reward)
        {
            reward = null;
            MissionDefinition mission;
            if (!_missions.TryGetValue(missionId, out mission))
            {
                return false;
            }

            reward = mission.Reward;
            return reward != null;
        }

        public bool TryGetLootProfile(string lootProfileId, out LootProfileDefinition definition)
        {
            return _lootProfiles.TryGetValue(lootProfileId, out definition);
        }

        public bool TryGetEnvironmentalZoneDefinition(string zoneId, out EnvironmentalZoneDefinition definition)
        {
            return _environmentalZones.TryGetValue(zoneId, out definition);
        }

        public bool TryGetVisionEquipmentDefinition(string equipmentId, out VisionEquipmentDefinition definition)
        {
            return _visionEquipment.TryGetValue(equipmentId, out definition);
        }

        public bool TryGetSite(string siteId, out SiteDefinition definition)
        {
            return _sites.TryGetValue(siteId, out definition);
        }

        public bool TryGetOutpostDefinition(string outpostId, out OutpostDefinition definition)
        {
            return _outposts.TryGetValue(outpostId, out definition);
        }

        public bool TryGetItem(string itemId, out ItemDefinition definition)
        {
            return _items.TryGetValue(itemId, out definition);
        }

        public bool TryGetResourcePackage(string packageId, out ResourcePackageDefinition definition)
        {
            return _resourcePackages.TryGetValue(packageId, out definition);
        }

        public bool TryGetBaseModule(string moduleId, out BaseModuleDefinition definition)
        {
            return _baseModules.TryGetValue(moduleId, out definition);
        }

        private static Dictionary<string, T> CopyDictionary<T>(IReadOnlyDictionary<string, T> source)
            where T : class
        {
            Dictionary<string, T> copy = new Dictionary<string, T>();
            if (source == null)
            {
                return copy;
            }

            foreach (KeyValuePair<string, T> entry in source)
            {
                if (string.IsNullOrEmpty(entry.Key) || entry.Value == null)
                {
                    continue;
                }

                copy[entry.Key] = entry.Value;
            }

            return copy;
        }

        private bool TryRegister<T>(Dictionary<string, T> target, T definition, string prefix, bool overwrite)
            where T : class
        {
            if (definition == null)
            {
                return false;
            }

            string id = GetDefinitionId(definition);
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            if (target.ContainsKey(id))
            {
                _duplicateRegistrationIds.Add(prefix + ":" + id);
                if (!overwrite)
                {
                    return false;
                }
            }

            target[id] = definition;
            return true;
        }

        private static string GetDefinitionId(object definition)
        {
            if (definition is UnitDefinition)
            {
                return ((UnitDefinition)definition).Id;
            }

            if (definition is MissionDefinition)
            {
                return ((MissionDefinition)definition).Id;
            }

            if (definition is AbilityDefinition)
            {
                return ((AbilityDefinition)definition).Id;
            }

            if (definition is WeaponDefinition)
            {
                return ((WeaponDefinition)definition).Id;
            }

            if (definition is EnemyDefinition)
            {
                return ((EnemyDefinition)definition).Id;
            }

            if (definition is EnvironmentalZoneDefinition)
            {
                return ((EnvironmentalZoneDefinition)definition).Id;
            }

            if (definition is VisionEquipmentDefinition)
            {
                return ((VisionEquipmentDefinition)definition).Id;
            }

            if (definition is SiteDefinition)
            {
                return ((SiteDefinition)definition).Id;
            }

            if (definition is ItemDefinition)
            {
                return ((ItemDefinition)definition).Id;
            }

            if (definition is ResourcePackageDefinition)
            {
                return ((ResourcePackageDefinition)definition).Id;
            }

            if (definition is OutpostDefinition)
            {
                return ((OutpostDefinition)definition).Id;
            }

            if (definition is LootProfileDefinition)
            {
                return ((LootProfileDefinition)definition).Id;
            }

            if (definition is BaseModuleDefinition)
            {
                return ((BaseModuleDefinition)definition).Id;
            }

            return null;
        }
    }
}
