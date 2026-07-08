using System.Collections.Generic;
using Warzone.Content;
using Warzone.Content.Definitions;

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

            EnemyDefinition raider = new EnemyDefinition("sandbox.raider", "Raider", 45, 0f, 10f, 0f, FactionId.Enemy);

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
    }
}
