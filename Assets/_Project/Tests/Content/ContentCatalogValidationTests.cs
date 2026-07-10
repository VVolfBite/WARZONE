using NUnit.Framework;
using System.Linq;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Content.Validation;

namespace Warzone.Tests.Content
{
    public sealed class ContentCatalogValidationTests
    {
        [Test]
        public void DemoCatalog_ValidatesWithoutCriticalIssues()
        {
            ContentCatalog catalog = DemoContentFactory.CreateDemoCatalog();
            ContentValidationResult result = new ContentCatalogValidator().Validate(catalog);

            Assert.That(result.IsValid, Is.True);
            Assert.That(result.HasCriticalIssues, Is.False);
        }

        [Test]
        public void DemoCatalog_UsesContentEnvironmentalZoneTypes()
        {
            ContentCatalog catalog = DemoContentFactory.CreateDemoCatalog();
            EnvironmentalZoneDefinition zoneDefinition;

            Assert.That(catalog.TryGetEnvironmentalZoneDefinition("smoke_grenade_zone", out zoneDefinition), Is.True);
            Assert.That(zoneDefinition.ZoneType, Is.EqualTo(EnvironmentalZoneDefinitionType.Smoke));
        }

        [Test]
        public void MissingLootProfileProducesValidationIssue()
        {
            ContentCatalog catalog = DemoContentFactory.CreateDemoCatalog();
            catalog.RegisterMission(new MissionDefinition(
                "mission.invalid.profile",
                "Invalid Profile Mission",
                1,
                1,
                new[] { new MissionObjectiveDefinition(MissionObjectiveType.ExtractSquad, "extract.alpha", 1) },
                MissionType.Tactical,
                MissionDifficulty.Normal,
                "Compound",
                new MissionRewardDefinition(0, 0, "missing_profile")));

            ContentValidationResult result = new ContentCatalogValidator().Validate(catalog);

            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Issues.Any(issue => issue != null && issue.Code == "mission.reward.profile.missing"), Is.True);
        }
    }
}
