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
            IReadOnlyDictionary<string, EnemyDefinition> enemies = null)
        {
            Units = units ?? new Dictionary<string, UnitDefinition>();
            Missions = missions ?? new Dictionary<string, MissionDefinition>();
            Abilities = abilities ?? new Dictionary<string, AbilityDefinition>();
            Weapons = weapons ?? new Dictionary<string, WeaponDefinition>();
            Enemies = enemies ?? new Dictionary<string, EnemyDefinition>();
        }

        public IReadOnlyDictionary<string, UnitDefinition> Units { get; private set; }
        public IReadOnlyDictionary<string, MissionDefinition> Missions { get; private set; }
        public IReadOnlyDictionary<string, AbilityDefinition> Abilities { get; private set; }
        public IReadOnlyDictionary<string, WeaponDefinition> Weapons { get; private set; }
        public IReadOnlyDictionary<string, EnemyDefinition> Enemies { get; private set; }

        public bool TryGetWeapon(string weaponId, out WeaponDefinition definition)
        {
            return Weapons.TryGetValue(weaponId, out definition);
        }

        public bool TryGetEnemy(string enemyId, out EnemyDefinition definition)
        {
            return Enemies.TryGetValue(enemyId, out definition);
        }
    }
}
