using System.Collections.Generic;
using NUnit.Framework;
using Warzone.Application.Services;
using Warzone.Campaign;
using Warzone.Content.Definitions;

namespace Warzone.Tests.Campaign
{
    public sealed class M12CampaignWorldProgressionTests
    {
        [Test]
        public void WorldProgressionService_AdvanceDaysAndApplyWorldTicksAppliesDailyTicks()
        {
            CampaignState campaignState = CreateWorldCampaignState();
            WorldProgressionService worldProgressionService = new WorldProgressionService();

            worldProgressionService.AdvanceDaysAndApplyWorldTicks(campaignState, 2);

            CampaignSiteState siteState;
            Assert.That(campaignState.TryGetSite("site.alpha", out siteState), Is.True);
            Assert.That(campaignState.CampaignTime, Is.EqualTo(48f));
            Assert.That(siteState.LootRemaining, Is.EqualTo(2));
        }

        [Test]
        public void WorldProgressionService_AdvanceDaysDoesNotMutateSitesExceptTime()
        {
            CampaignState campaignState = CreateWorldCampaignState();
            WorldProgressionService worldProgressionService = new WorldProgressionService();
            CampaignSiteState siteState;
            Assert.That(campaignState.TryGetSite("site.alpha", out siteState), Is.True);
            int threatBefore = siteState.ThreatLevel;
            int lootBefore = siteState.LootRemaining;

            worldProgressionService.AdvanceDays(campaignState, 2);

            Assert.That(campaignState.CampaignTime, Is.EqualTo(48f));
            Assert.That(siteState.ThreatLevel, Is.EqualTo(threatBefore));
            Assert.That(siteState.LootRemaining, Is.EqualTo(lootBefore));
        }

        [Test]
        public void CampaignWorldTickSystem_HourlyTickDoesNotIncreaseThreat()
        {
            CampaignState campaignState = CreateWorldCampaignState();
            CampaignWorldTickSystem worldTickSystem = new CampaignWorldTickSystem();
            CampaignSiteState siteState;
            Assert.That(campaignState.TryGetSite("site.alpha", out siteState), Is.True);
            siteState.SetThreatLevel(2);
            siteState.SetOccupied(true);

            worldTickSystem.ApplyHourlyTick(campaignState);

            Assert.That(siteState.ThreatLevel, Is.EqualTo(2));
        }

        [Test]
        public void CampaignResourceLedgerState_SpendAllIsAtomic()
        {
            CampaignResourceLedgerState ledger = new CampaignResourceLedgerState();
            ledger.Add("food", 3);
            ledger.Add("medicine", 1);

            Dictionary<string, int> costs = new Dictionary<string, int>
            {
                { "food", 2 },
                { "medicine", 2 },
                { "ammo", 0 }
            };

            Assert.That(ledger.CanSpendAll(costs), Is.False);
            Assert.That(ledger.SpendAll(costs), Is.False);
            Assert.That(ledger.GetAmount("food"), Is.EqualTo(3));
            Assert.That(ledger.GetAmount("medicine"), Is.EqualTo(1));
            Assert.That(ledger.CanSpendAll(new Dictionary<string, int>()), Is.True);
            Assert.That(ledger.SpendAll(null), Is.True);
        }

        [Test]
        public void CampaignOutpostSystem_EstablishOutpostDoesNotPartiallyConsumeResources()
        {
            CampaignState campaignState = CreateWorldCampaignState();
            CampaignOutpostSystem outpostSystem = new CampaignOutpostSystem();
            CampaignSiteState siteState;
            Assert.That(campaignState.TryGetSite("site.alpha", out siteState), Is.True);
            siteState.MarkCleared();
            siteState.MarkDiscovered();
            campaignState.ResourceLedger.Set("building_material", 1);
            campaignState.ResourceLedger.Set("fuel", 1);

            Assert.That(outpostSystem.EstablishOutpost(campaignState, "site.alpha", "outpost.alpha"), Is.False);
            Assert.That(campaignState.ResourceLedger.GetAmount("building_material"), Is.EqualTo(1));
            Assert.That(campaignState.ResourceLedger.GetAmount("fuel"), Is.EqualTo(1));
            Assert.That(outpostSystem.HasOutpostAtSite(campaignState, "site.alpha"), Is.False);
        }

        [Test]
        public void CampaignOutpostSystem_ApplyOutpostDailyEffectsDoesNotPartiallyConsumeResources()
        {
            CampaignState campaignState = CreateWorldCampaignState();
            CampaignOutpostSystem outpostSystem = new CampaignOutpostSystem();
            CampaignSiteState siteState;
            Assert.That(campaignState.TryGetSite("site.alpha", out siteState), Is.True);
            siteState.MarkCleared();
            campaignState.ResourceLedger.Set("building_material", 3);
            campaignState.ResourceLedger.Set("fuel", 2);

            Assert.That(outpostSystem.EstablishOutpost(campaignState, "site.alpha", "outpost.alpha"), Is.True);
            campaignState.ResourceLedger.Set("building_material", 1);
            campaignState.ResourceLedger.Set("fuel", 0);

            outpostSystem.ApplyOutpostDailyEffects(campaignState);

            Assert.That(campaignState.ResourceLedger.GetAmount("building_material"), Is.EqualTo(1));
            Assert.That(campaignState.ResourceLedger.GetAmount("fuel"), Is.EqualTo(0));
            Assert.That(outpostSystem.HasOutpostAtSite(campaignState, "site.alpha"), Is.False);
        }

        private static CampaignState CreateWorldCampaignState()
        {
            CampaignState campaignState = new CampaignState();
            campaignState.AddSite(new CampaignSiteState(
                "site.alpha",
                "Alpha Site",
                SiteType.Compound,
                true,
                true,
                1,
                true,
                "loot",
                0f,
                5,
                3,
                4,
                false,
                false,
                true,
                null,
                new[] { "supply" }));
            return campaignState;
        }
    }
}
