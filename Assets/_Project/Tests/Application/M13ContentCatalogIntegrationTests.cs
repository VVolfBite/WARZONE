using NUnit.Framework;
using Warzone.Application;
using Warzone.Application.Services;
using Warzone.Campaign;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Tests.Application
{
    public sealed class M13ContentCatalogIntegrationTests
    {
        [Test]
        public void StartingCampaignFactory_UsesDemoCatalogIds()
        {
            StartingCampaignFactory factory = new StartingCampaignFactory(DemoContentFactory.CreateDemoCatalog());
            CampaignState campaignState = factory.CreateStartingCampaign();

            CampaignMemberState memberState;
            Assert.That(campaignState.Roster.TryGetMember("campaign.member.1", out memberState), Is.True);
            Assert.That(memberState.CarriedWeaponId, Is.EqualTo("rifle"));

            CampaignSiteState siteState;
            Assert.That(campaignState.TryGetSite("medical_clinic", out siteState), Is.True);
            Assert.That(siteState.OutpostId, Is.EqualTo("basic_outpost"));
        }

        [Test]
        public void MissionLaunchPlanFactory_CarriesLootProfileIdFromCatalog()
        {
            ContentCatalog catalog = DemoContentFactory.CreateDemoCatalog();
            MissionPreparationService preparationService = new MissionPreparationService(catalog);
            CampaignState campaignState = new StartingCampaignFactory(catalog).CreateStartingCampaign();

            MissionLaunchPlan launchPlan;
            string reason;
            bool prepared = preparationService.TryCreateLaunchPlan(campaignState, "mission.search_medical_supplies", "medical_clinic", new[] { 1 }, 42, out launchPlan, out reason);

            Assert.That(prepared, Is.True, reason);
            Assert.That(launchPlan.LootProfileId, Is.EqualTo("medical_loot"));
        }

        [Test]
        public void MissionSettlementService_UsesDemoCatalogLootProfiles()
        {
            ContentCatalog catalog = DemoContentFactory.CreateDemoCatalog();
            MissionPreparationService preparationService = new MissionPreparationService(catalog);
            CampaignState campaignState = new StartingCampaignFactory(catalog).CreateStartingCampaign();

            MissionLaunchPlan launchPlan;
            string reason;
            bool prepared = preparationService.TryCreateLaunchPlan(campaignState, "mission.search_medical_supplies", "medical_clinic", new[] { 1 }, 42, out launchPlan, out reason);
            Assert.That(prepared, Is.True, reason);

            BattleResult battleResult = new BattleResult(
                MissionOutcome.Victory,
                new[] { new UnitOutcome(new BattleEntityId(1000), "campaign.member.1", true) },
                new BattleEntityId[0],
                new BattleStatistics(1, 0),
                12f,
                1,
                BattleCompletionType.Success,
                new[]
                {
                    new BattleObjectiveResult(MissionObjectiveType.SearchPoint, "search.medical", 1, 1, true),
                    new BattleObjectiveResult(MissionObjectiveType.ExtractSquad, "extract.alpha", 1, 1, true)
                },
                new BattleCasualtyResult(new BattleEntityId[0], new BattleEntityId[0]),
                new BattleLootResult(4),
                new BattleExtractionResult(new[] { new BattleEntityId(1000) }, 1));

            MissionSettlementService settlementService = new MissionSettlementService(catalog);
            settlementService.ApplyBattleResult(campaignState, launchPlan, battleResult);

            Assert.That(campaignState.ResourceLedger.GetAmount("medicine"), Is.GreaterThanOrEqualTo(12));
            Assert.That(campaignState.ResourceLedger.GetAmount("generic_loot"), Is.EqualTo(0));
        }
    }
}
