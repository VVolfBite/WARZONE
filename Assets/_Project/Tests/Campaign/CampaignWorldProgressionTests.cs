using NUnit.Framework;
using Warzone.Campaign;
using Warzone.Content.Definitions;

namespace Warzone.Tests.Campaign
{
    public sealed class CampaignWorldProgressionTests
    {
        [Test]
        public void CampaignTimeSystem_AdvancesHoursAndDays()
        {
            CampaignState campaignState = new CampaignState();
            CampaignTimeSystem timeSystem = new CampaignTimeSystem();

            timeSystem.AdvanceHours(campaignState, 6f);
            timeSystem.AdvanceDays(campaignState, 2);

            Assert.That(campaignState.CampaignTime, Is.EqualTo(54f));
        }

        [Test]
        public void SettlementSystem_AppliesSearchVisitAndLootDecay()
        {
            CampaignState campaignState = CreateCampaignState();
            CampaignSettlementSystem settlementSystem = new CampaignSettlementSystem();

            CampaignSettlement settlement = new CampaignSettlement(
                "mission.world",
                "site.alpha",
                true,
                new[]
                {
                    new CampaignCasualtySettlement("member.1", false, false, true)
                },
                new CampaignLootSettlement[0],
                new[]
                {
                    new CampaignSiteSettlement("site.alpha", true, false, 2, 12f, 2, 1, 1, false, false, null)
                },
                new CampaignSquadSettlement[0],
                new CampaignMissionHistoryRecord("mission.world", "site.alpha", true, 1, 2, 12f),
                null,
                null,
                null,
                null,
                1,
                1);

            settlementSystem.Apply(campaignState, settlement);

            CampaignSiteState siteState;
            Assert.That(campaignState.TryGetSite("site.alpha", out siteState), Is.True);
            Assert.That(siteState.SearchCompleted, Is.True);
            Assert.That(siteState.VisitCount, Is.EqualTo(1));
            Assert.That(siteState.LootRemaining, Is.EqualTo(1));
            Assert.That(siteState.IsExhausted, Is.False);
        }

        [Test]
        public void SiteState_ThreatLevelClampsAndLootExhausts()
        {
            CampaignSiteState siteState = new CampaignSiteState(
                "site.beta",
                "Beta Site",
                SiteType.Compound,
                true,
                false,
                1,
                false,
                "ammo cache",
                0f,
                2,
                4,
                2,
                false,
                false,
                true,
                null,
                new[] { "supply" });

            siteState.IncreaseThreat(10);
            Assert.That(siteState.ThreatLevel, Is.EqualTo(2));

            siteState.ReduceThreat(10);
            Assert.That(siteState.ThreatLevel, Is.EqualTo(0));

            siteState.ReduceLoot(5);
            Assert.That(siteState.LootRemaining, Is.EqualTo(0));
            Assert.That(siteState.IsExhausted, Is.True);
        }

        [Test]
        public void WorldTick_IncreasesThreatForUnclearedSiteAfterTime()
        {
            CampaignState campaignState = CreateCampaignState();
            CampaignWorldTickSystem worldTickSystem = new CampaignWorldTickSystem();
            CampaignSiteState siteState;
            Assert.That(campaignState.TryGetSite("site.alpha", out siteState), Is.True);
            siteState.SetThreatLevel(1);
            siteState.UpdateLastVisitedTime(0f);
            campaignState.SetCampaignTime(96f);

            worldTickSystem.ApplyDailyTick(campaignState);

            Assert.That(siteState.ThreatLevel, Is.EqualTo(2));
            Assert.That(siteState.IsOccupied, Is.True);
        }

        [Test]
        public void OutpostEstablishmentConsumesResourcesAndPreventsReoccupation()
        {
            CampaignState campaignState = CreateCampaignState();
            CampaignOutpostSystem outpostSystem = new CampaignOutpostSystem();
            CampaignWorldTickSystem worldTickSystem = new CampaignWorldTickSystem();
            CampaignSiteState siteState;
            Assert.That(campaignState.TryGetSite("site.alpha", out siteState), Is.True);
            siteState.MarkCleared();
            siteState.MarkDiscovered();
            campaignState.ResourceLedger.Add("building_material", 3);
            campaignState.ResourceLedger.Add("fuel", 2);

            Assert.That(outpostSystem.EstablishOutpost(campaignState, "site.alpha", "outpost.alpha"), Is.True);
            Assert.That(campaignState.ResourceLedger.GetAmount("building_material"), Is.EqualTo(0));
            Assert.That(campaignState.ResourceLedger.GetAmount("fuel"), Is.EqualTo(0));
            Assert.That(outpostSystem.HasOutpostAtSite(campaignState, "site.alpha"), Is.True);

            campaignState.SetCampaignTime(96f);
            siteState.UpdateLastVisitedTime(0f);
            worldTickSystem.ApplyDailyTick(campaignState);

            Assert.That(outpostSystem.HasOutpostAtSite(campaignState, "site.alpha"), Is.True);
            Assert.That(siteState.ThreatLevel, Is.EqualTo(0));
            Assert.That(siteState.IsOccupied, Is.False);
        }

        [Test]
        public void AbandoningOutpostRemovesProtection()
        {
            CampaignState campaignState = CreateCampaignState();
            CampaignOutpostSystem outpostSystem = new CampaignOutpostSystem();
            CampaignWorldTickSystem worldTickSystem = new CampaignWorldTickSystem();
            CampaignSiteState siteState;
            Assert.That(campaignState.TryGetSite("site.alpha", out siteState), Is.True);
            siteState.MarkCleared();
            campaignState.ResourceLedger.Add("building_material", 3);
            campaignState.ResourceLedger.Add("fuel", 2);

            Assert.That(outpostSystem.EstablishOutpost(campaignState, "site.alpha", "outpost.alpha"), Is.True);
            Assert.That(outpostSystem.AbandonOutpost(campaignState, "outpost.alpha"), Is.True);
            Assert.That(outpostSystem.HasOutpostAtSite(campaignState, "site.alpha"), Is.False);

            campaignState.SetCampaignTime(96f);
            siteState.UpdateLastVisitedTime(0f);
            worldTickSystem.ApplyDailyTick(campaignState);

            Assert.That(siteState.ThreatLevel, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public void Outpost_DeactivatesWhenMaintenanceResourcesAreMissing()
        {
            CampaignState campaignState = CreateCampaignState();
            CampaignOutpostSystem outpostSystem = new CampaignOutpostSystem();
            CampaignWorldTickSystem worldTickSystem = new CampaignWorldTickSystem();
            CampaignSiteState siteState;
            Assert.That(campaignState.TryGetSite("site.alpha", out siteState), Is.True);
            siteState.MarkCleared();
            campaignState.ResourceLedger.Add("building_material", 2);
            campaignState.ResourceLedger.Add("fuel", 1);

            Assert.That(outpostSystem.EstablishOutpost(campaignState, "site.alpha", "outpost.alpha"), Is.True);
            Assert.That(outpostSystem.HasOutpostAtSite(campaignState, "site.alpha"), Is.True);

            campaignState.SetCampaignTime(96f);
            siteState.UpdateLastVisitedTime(0f);
            worldTickSystem.ApplyDailyTick(campaignState);

            Assert.That(outpostSystem.HasOutpostAtSite(campaignState, "site.alpha"), Is.False);
        }

        [Test]
        public void ResourceLedger_HasAtLeastAndSpendHandleZeroAmounts()
        {
            CampaignResourceLedgerState ledger = new CampaignResourceLedgerState();

            ledger.Add("food", 4);
            ledger.Add("food", -10);

            Assert.That(ledger.HasAtLeast("food", 0), Is.True);
            Assert.That(ledger.Spend("food", 0), Is.False);
            Assert.That(ledger.GetAmount("food"), Is.EqualTo(0));
        }

        private static CampaignState CreateCampaignState()
        {
            CampaignState campaignState = new CampaignState();
            campaignState.AddSite(new CampaignSiteState(
                "site.alpha",
                "Alpha Site",
                SiteType.Compound,
                true,
                false,
                1,
                false,
                "generic loot",
                0f,
                5,
                3,
                3,
                false,
                false,
                true,
                null,
                new[] { "supply" }));
            return campaignState;
        }
    }
}
