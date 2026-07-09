using NUnit.Framework;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Tests.Combat
{
    public sealed class ContentCatalogTests
    {
        [Test]
        public void MinimalContentCatalog_CanReturnWeaponDefinition()
        {
            ContentCatalog catalog = TestCombatContentFactory.CreateCatalog();
            WeaponDefinition weaponDefinition;

            bool found = catalog.TryGetWeapon("sandbox.rifle", out weaponDefinition);

            Assert.That(found, Is.True);
            Assert.That(weaponDefinition.DisplayName, Is.EqualTo("Sandbox Rifle"));
        }

        [Test]
        public void MinimalContentCatalog_CanReturnEnemyDefinition()
        {
            ContentCatalog catalog = TestCombatContentFactory.CreateCatalog();
            EnemyDefinition enemyDefinition;

            bool found = catalog.TryGetEnemy("sandbox.raider", out enemyDefinition);

            Assert.That(found, Is.True);
            Assert.That(enemyDefinition.MaxHealth, Is.EqualTo(45));
        }

        [Test]
        public void MissingDefinition_ReturnsFalse()
        {
            ContentCatalog catalog = TestCombatContentFactory.CreateCatalog();
            WeaponDefinition weaponDefinition;

            bool found = catalog.TryGetWeapon("missing.weapon", out weaponDefinition);

            Assert.That(found, Is.False);
            Assert.That(weaponDefinition, Is.Null);
        }

        [Test]
        public void EnvironmentCatalog_CanReturnEnvironmentalZoneDefinition()
        {
            ContentCatalog catalog = TestCombatContentFactory.CreateEnvironmentCombatCatalog();
            EnvironmentalZoneDefinition zoneDefinition;

            bool found = catalog.TryGetEnvironmentalZoneDefinition("sandbox.smoke", out zoneDefinition);

            Assert.That(found, Is.True);
            Assert.That(zoneDefinition.ZoneType, Is.EqualTo(Warzone.Combat.EnvironmentalZoneType.Smoke));
        }

        [Test]
        public void EnvironmentCatalog_CanReturnVisionEquipmentDefinition()
        {
            ContentCatalog catalog = TestCombatContentFactory.CreateEnvironmentCombatCatalog();
            VisionEquipmentDefinition visionEquipmentDefinition;

            bool found = catalog.TryGetVisionEquipmentDefinition("sandbox.nvg.basic", out visionEquipmentDefinition);

            Assert.That(found, Is.True);
            Assert.That(visionEquipmentDefinition.NightVisionLevel, Is.EqualTo(1));
        }
    }
}
