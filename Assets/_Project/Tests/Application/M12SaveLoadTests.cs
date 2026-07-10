using System.Collections.Generic;
using NUnit.Framework;
using Warzone.Application;
using Warzone.Application.Save;
using Warzone.Application.Services;
using Warzone.Campaign;

namespace Warzone.Tests.Application
{
    public sealed class M12SaveLoadTests
    {
        [Test]
        public void SaveGameSnapshot_ContainsVersionAndCampaignData()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            CampaignSaveMapper mapper = new CampaignSaveMapper();

            SaveGameSnapshot snapshot = mapper.ToSnapshot(campaignState, "slot.alpha");

            Assert.That(snapshot, Is.Not.Null);
            Assert.That(snapshot.SaveVersion, Is.EqualTo(SaveGameVersion.Current));
            Assert.That(snapshot.Metadata.SaveVersion, Is.EqualTo(SaveGameVersion.Current));
            Assert.That(snapshot.Metadata.SlotId, Is.EqualTo("slot.alpha"));
            Assert.That(snapshot.Campaign, Is.Not.Null);
            Assert.That(snapshot.Campaign.CampaignTime, Is.EqualTo(campaignState.CampaignTime));
            Assert.That(snapshot.Campaign.Roster.Members, Has.Count.EqualTo(campaignState.Roster.MembersById.Count));
            Assert.That(snapshot.Campaign.Sites, Has.Count.GreaterThanOrEqualTo(3));
        }

        [Test]
        public void CampaignSaveMapper_RoundTripsModifiedCampaignState()
        {
            CampaignState campaignState = CreateModifiedCampaignState();
            CampaignSaveMapper mapper = new CampaignSaveMapper();

            SaveGameSnapshot snapshot = mapper.ToSnapshot(campaignState, "slot.beta");
            CampaignState loaded = mapper.FromSnapshot(snapshot);

            AssertCampaignEquivalent(campaignState, loaded);
        }

        [Test]
        public void CampaignLifecycleService_NewGameCreatesStartingCampaign()
        {
            CampaignLifecycleService lifecycleService = new CampaignLifecycleService();
            CampaignState campaignState = lifecycleService.NewGame();

            Assert.That(campaignState, Is.Not.Null);
            Assert.That(campaignState.CurrentMission, Is.Null);
            Assert.That(campaignState.SitesById.Count, Is.GreaterThanOrEqualTo(3));
            Assert.That(campaignState.MainBase, Is.Not.Null);
        }

        [Test]
        public void JsonSaveGameSerializer_RoundTripsSnapshot()
        {
            CampaignSaveMapper mapper = new CampaignSaveMapper();
            SaveGameSnapshot snapshot = mapper.ToSnapshot(CreateModifiedCampaignState(), "slot.gamma");
            JsonSaveGameSerializer serializer = new JsonSaveGameSerializer();

            string json = serializer.Serialize(snapshot);
            Assert.That(string.IsNullOrWhiteSpace(json), Is.False);

            SaveGameSnapshot loadedSnapshot;
            string reason;
            Assert.That(serializer.TryDeserialize(json, out loadedSnapshot, out reason), Is.True, reason);
            Assert.That(loadedSnapshot.Metadata.SaveVersion, Is.EqualTo(SaveGameVersion.Current));
            Assert.That(loadedSnapshot.Campaign.Sites, Has.Count.GreaterThanOrEqualTo(3));
        }

        [Test]
        public void JsonSaveGameSerializer_InvalidDataFailsCleanly()
        {
            JsonSaveGameSerializer serializer = new JsonSaveGameSerializer();

            SaveGameSnapshot snapshot;
            string reason;
            Assert.That(serializer.TryDeserialize("not json", out snapshot, out reason), Is.False);
            Assert.That(reason, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void SaveGameService_SavesAndLoadsEquivalentCampaign()
        {
            SaveGameService saveGameService = new SaveGameService();
            CampaignState original = CreateModifiedCampaignState();
            string reason;

            Assert.That(saveGameService.SaveCampaign("slot.delta", original, out reason), Is.True, reason);
            CampaignState loaded;
            Assert.That(saveGameService.TryLoadCampaign("slot.delta", out loaded, out reason), Is.True, reason);

            AssertCampaignEquivalent(original, loaded);
        }

        [Test]
        public void SaveGameService_SaveDoesNotMutateCampaign()
        {
            SaveGameService saveGameService = new SaveGameService();
            CampaignState campaignState = CreateModifiedCampaignState();
            float timeBefore = campaignState.CampaignTime;
            int ammoBefore = campaignState.ResourceLedger.GetAmount("ammo");
            string reason;

            Assert.That(saveGameService.SaveCampaign("slot.epsilon", campaignState, out reason), Is.True, reason);

            Assert.That(campaignState.CampaignTime, Is.EqualTo(timeBefore));
            Assert.That(campaignState.ResourceLedger.GetAmount("ammo"), Is.EqualTo(ammoBefore));
        }

        [Test]
        public void SaveGameService_LoadMissingSlotFailsCleanly()
        {
            SaveGameService saveGameService = new SaveGameService();
            CampaignState loaded;
            string reason;

            Assert.That(saveGameService.TryLoadCampaign("missing.slot", out loaded, out reason), Is.False);
            Assert.That(loaded, Is.Null);
            Assert.That(reason, Is.Not.Null.And.Not.Empty);
        }

        private static CampaignState CreateModifiedCampaignState()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            CampaignOutpostSystem outpostSystem = new CampaignOutpostSystem();

            campaignState.SetCampaignTime(72f);
            campaignState.SetCurrentMission(new CampaignMissionState(
                "mission.m12.save",
                "site.alpha",
                "M12 Save Mission",
                true,
                3,
                "tactical"));

            CampaignSiteState siteAlpha;
            Assert.That(campaignState.TryGetSite("site.alpha", out siteAlpha), Is.True);
            siteAlpha.MarkVisited(48f, 4);
            siteAlpha.MarkSearched();
            siteAlpha.SetLootRemaining(0);
            siteAlpha.MarkExhausted();

            campaignState.ResourceLedger.Add("ammo", 5);
            campaignState.Inventory.AddResourcePackage("ammo.cache", 3);
            campaignState.Inventory.AddItemStack(new CampaignItemStackState("field.rations", "Field Rations", 6));
            campaignState.AddMissionHistory(new CampaignMissionHistoryRecord(
                "mission.m12.save",
                "site.alpha",
                true,
                1,
                4,
                72f,
                "Success",
                new Dictionary<string, int> { { "ammo", 5 } }));

            Assert.That(outpostSystem.EstablishOutpost(campaignState, "site.alpha", "outpost.alpha"), Is.True);
            return campaignState;
        }

        private static void AssertCampaignEquivalent(CampaignState expected, CampaignState actual)
        {
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.CampaignTime, Is.EqualTo(expected.CampaignTime).Within(0.0001f));
            Assert.That(actual.CurrentMissionId, Is.EqualTo(expected.CurrentMissionId));
            Assert.That(actual.CurrentMission, Is.Not.Null);
            Assert.That(actual.Roster.MembersById.Count, Is.EqualTo(expected.Roster.MembersById.Count));
            Assert.That(actual.Roster.SquadsById.Count, Is.EqualTo(expected.Roster.SquadsById.Count));
            Assert.That(actual.Inventory.ItemStacks.Count, Is.EqualTo(expected.Inventory.ItemStacks.Count));
            Assert.That(actual.Inventory.WeaponInstances.Count, Is.EqualTo(expected.Inventory.WeaponInstances.Count));
            Assert.That(actual.Inventory.ResourcePackages.Count, Is.EqualTo(expected.Inventory.ResourcePackages.Count));
            Assert.That(actual.ResourceLedger.GetAmount("ammo"), Is.EqualTo(expected.ResourceLedger.GetAmount("ammo")));
            Assert.That(actual.ResourceLedger.GetAmount("building_material"), Is.EqualTo(expected.ResourceLedger.GetAmount("building_material")));
            Assert.That(actual.SitesById.Count, Is.EqualTo(expected.SitesById.Count));
            Assert.That(actual.OutpostsById.Count, Is.EqualTo(expected.OutpostsById.Count));
            Assert.That(actual.MissionHistory.Count, Is.EqualTo(expected.MissionHistory.Count));

            CampaignSiteState expectedSite;
            CampaignSiteState actualSite;
            Assert.That(expected.TryGetSite("site.alpha", out expectedSite), Is.True);
            Assert.That(actual.TryGetSite("site.alpha", out actualSite), Is.True);
            Assert.That(actualSite.SearchCompleted, Is.EqualTo(expectedSite.SearchCompleted));
            Assert.That(actualSite.IsExhausted, Is.EqualTo(expectedSite.IsExhausted));
            Assert.That(actualSite.VisitCount, Is.EqualTo(expectedSite.VisitCount));
            Assert.That(actualSite.OutpostId, Is.EqualTo(expectedSite.OutpostId));

            CampaignOutpostState expectedOutpost;
            CampaignOutpostState actualOutpost;
            Assert.That(expected.TryGetOutpost("outpost.alpha", out expectedOutpost), Is.True);
            Assert.That(actual.TryGetOutpost("outpost.alpha", out actualOutpost), Is.True);
            Assert.That(actualOutpost.SiteId, Is.EqualTo(expectedOutpost.SiteId));
            Assert.That(actualOutpost.IsActive, Is.EqualTo(expectedOutpost.IsActive));
            Assert.That(actualOutpost.Capabilities, Has.Count.EqualTo(expectedOutpost.Capabilities.Count));
        }
    }
}
