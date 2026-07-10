using NUnit.Framework;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Content.Validation;

namespace Warzone.Tests.Content
{
    public sealed class ContentCatalogValidationTests
    {
        [Test]
        public void DemoCatalog_ContainsRequiredContentIds()
        {
            ContentCatalog catalog = DemoContentFactory.CreateDemoCatalog();
            WeaponDefinition weaponDefinition;
            EnemyDefinition enemyDefinition;
            SiteDefinition siteDefinition;
            ResourcePackageDefinition resourcePackageDefinition;
            OutpostDefinition outpostDefinition;

            Assert.That(catalog.TryGetWeapon("rifle", out weaponDefinition), Is.True);
            Assert.That(weaponDefinition, Is.Not.Null);
            Assert.That(catalog.TryGetEnemy("infected_basic", out enemyDefinition), Is.True);
            Assert.That(enemyDefinition, Is.Not.Null);
            Assert.That(catalog.TryGetSite("medical_clinic", out siteDefinition), Is.True);
            Assert.That(siteDefinition, Is.Not.Null);
            Assert.That(catalog.TryGetSite("supply_depot", out siteDefinition), Is.True);
            Assert.That(catalog.TryGetResourcePackage("food", out resourcePackageDefinition), Is.True);
            Assert.That(resourcePackageDefinition, Is.Not.Null);
            Assert.That(catalog.TryGetResourcePackage("medicine", out resourcePackageDefinition), Is.True);
            Assert.That(catalog.TryGetResourcePackage("ammo", out resourcePackageDefinition), Is.True);
            Assert.That(catalog.TryGetResourcePackage("fuel", out resourcePackageDefinition), Is.True);
            Assert.That(catalog.TryGetResourcePackage("building_material", out resourcePackageDefinition), Is.True);
            Assert.That(catalog.TryGetOutpostDefinition("basic_outpost", out outpostDefinition), Is.True);
            Assert.That(outpostDefinition, Is.Not.Null);
        }

        [Test]
        public void DemoCatalog_ValidatesWithoutCriticalIssues()
        {
            ContentCatalog catalog = DemoContentFactory.CreateDemoCatalog();
            ContentCatalogValidator validator = new ContentCatalogValidator();

            ContentValidationResult result = validator.Validate(catalog);

            Assert.That(result.HasCriticalIssues, Is.False);
        }

        [Test]
        public void DuplicateWeaponRegistration_IsRejectedAndRecorded()
        {
            ContentCatalog catalog = new ContentCatalog();
            Assert.That(catalog.TryRegisterWeapon(new WeaponDefinition("rifle", "Rifle", WeaponCategory.Rifle, AmmoCategory.Rifle, FireMode.Automatic, 12f, 0.45f, 18, 0.95f, 1, 0f, 16f, DamageType.Kinetic)), Is.True);
            Assert.That(catalog.TryRegisterWeapon(new WeaponDefinition("rifle", "Rifle Mk2", WeaponCategory.Rifle, AmmoCategory.Rifle, FireMode.Automatic, 13f, 0.4f, 19, 0.95f, 1, 0f, 16f, DamageType.Kinetic)), Is.False);
            Assert.That(catalog.DuplicateRegistrationIds, Has.Member("weapon:rifle"));
        }

        [Test]
        public void MissionReferencingMissingLootProfile_ProducesValidationIssue()
        {
            ContentCatalog catalog = new ContentCatalog();
            catalog.RegisterMission(new MissionDefinition(
                "mission.invalid",
                "Invalid Mission",
                1,
                1,
                new[] { new MissionObjectiveDefinition(MissionObjectiveType.SearchPoint, "search.main", 1) },
                MissionType.Tactical,
                MissionDifficulty.Normal,
                "Facility",
                new MissionRewardDefinition(0, 0, "missing_profile")));

            ContentCatalogValidator validator = new ContentCatalogValidator();
            ContentValidationResult result = validator.Validate(catalog);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Issues, Has.Some.Matches<ContentValidationIssue>(issue => issue.Code == "mission.reward.profile.missing"));
        }
    }
}
