using System.Collections.Generic;
using NUnit.Framework;
using Warzone.Application;
using Warzone.Application.Services;
using Warzone.Campaign;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Tests.Application
{
    public sealed class M11CampaignLoopTests
    {
        [Test]
        public void MissionRewardResolver_MapsProfilesAndExplicitResources()
        {
            ContentCatalog catalog = CreateCatalog("medical", 0, includeExplicitResources: false, includeOutpostDefinition: true);
            MissionPreparationService preparationService = new MissionPreparationService(catalog);
            CampaignService campaignService = new CampaignService();

            MissionLaunchPlan launchPlan;
            string reason;
            bool prepared = preparationService.TryCreateLaunchPlan(campaignService.CampaignState, "mission.m11.loop", "site.alpha", new[] { 1 }, 101, out launchPlan, out reason);
            Assert.That(prepared, Is.True, reason);

            BattleResult battleResult = CreateBattleResult(launchPlan, 4, true, true);
            MissionRewardResolver resolver = new MissionRewardResolver();
            List<CampaignResourceRewardSettlement> rewards = resolver.ResolveResourceRewards(launchPlan, battleResult);

            Assert.That(rewards, Has.Count.EqualTo(1));
            Assert.That(rewards[0].ResourceId, Is.EqualTo("medicine"));
            Assert.That(rewards[0].Count, Is.EqualTo(4));
        }

        [Test]
        public void MissionRewardResolver_ExplicitResourceRewardsOverrideProfileMapping()
        {
            ContentCatalog catalog = CreateCatalog("supply", 0, includeExplicitResources: true, includeOutpostDefinition: false);
            MissionPreparationService preparationService = new MissionPreparationService(catalog);
            CampaignService campaignService = new CampaignService();

            MissionLaunchPlan launchPlan;
            string reason;
            bool prepared = preparationService.TryCreateLaunchPlan(campaignService.CampaignState, "mission.m11.loop", "site.alpha", new[] { 1 }, 101, out launchPlan, out reason);
            Assert.That(prepared, Is.True, reason);

            BattleResult battleResult = CreateBattleResult(launchPlan, 5, true, true);
            MissionRewardResolver resolver = new MissionRewardResolver();
            List<CampaignResourceRewardSettlement> rewards = resolver.ResolveResourceRewards(launchPlan, battleResult);

            Assert.That(rewards, Has.Count.GreaterThanOrEqualTo(1));
            Assert.That(rewards[0].ResourceId, Is.EqualTo("medicine"));
            Assert.That(rewards[0].Count, Is.EqualTo(3));
            Assert.That(ContainsReward(rewards, "generic_loot"), Is.False);
        }

        [Test]
        public void MissionRewardResolver_ZeroLootProducesNoProfileReward()
        {
            ContentCatalog catalog = CreateCatalog("medical", 0, includeExplicitResources: false, includeOutpostDefinition: false);
            MissionPreparationService preparationService = new MissionPreparationService(catalog);
            CampaignService campaignService = new CampaignService();

            MissionLaunchPlan launchPlan;
            string reason;
            bool prepared = preparationService.TryCreateLaunchPlan(campaignService.CampaignState, "mission.m11.loop", "site.alpha", new[] { 1 }, 101, out launchPlan, out reason);
            Assert.That(prepared, Is.True, reason);

            BattleResult battleResult = CreateBattleResult(launchPlan, 0, true, true);
            MissionRewardResolver resolver = new MissionRewardResolver();
            List<CampaignResourceRewardSettlement> rewards = resolver.ResolveResourceRewards(launchPlan, battleResult);

            Assert.That(rewards, Is.Empty);
        }

        [Test]
        public void MissionLaunchPlanFactory_UsesCurrentSiteState()
        {
            CampaignService campaignService = new CampaignService();
            CampaignSiteState siteState;
            Assert.That(campaignService.CampaignState.TryGetSite("site.alpha", out siteState), Is.True);
            siteState.SetThreatLevel(4);
            siteState.SetLootRemaining(1);
            siteState.SetResourceRichness(1);
            siteState.MarkSearchCompleted();
            siteState.MarkExhausted();
            siteState.MarkVisited(96f, 3);

            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog("generic", 0, includeExplicitResources: false, includeOutpostDefinition: true));
            MissionLaunchPlan launchPlan;
            string reason;
            bool prepared = preparationService.TryCreateLaunchPlan(campaignService.CampaignState, "mission.m11.loop", "site.alpha", new[] { 1 }, 101, out launchPlan, out reason);

            Assert.That(prepared, Is.True, reason);
            Assert.That(launchPlan.SiteContext.ThreatLevel, Is.EqualTo(4));
            Assert.That(launchPlan.SiteContext.SearchCompleted, Is.True);
            Assert.That(launchPlan.SiteContext.IsExhausted, Is.True);
            Assert.That(launchPlan.SiteContext.LootRemaining, Is.EqualTo(0));
            Assert.That(launchPlan.SiteContext.VisitCount, Is.EqualTo(3));
            Assert.That(launchPlan.SiteContext.LastVisitedTime, Is.EqualTo(96f));
        }

        [Test]
        public void MissionSettlementService_SiteVisitCountAndSearchStateAreWrittenBack()
        {
            CampaignService campaignService = new CampaignService();
            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog("generic", 3, includeExplicitResources: false, includeOutpostDefinition: true));

            MissionLaunchPlan launchPlan;
            string reason;
            bool prepared = preparationService.TryCreateLaunchPlan(campaignService.CampaignState, "mission.m11.loop", "site.alpha", new[] { 1 }, 101, out launchPlan, out reason);
            Assert.That(prepared, Is.True, reason);

            BattleResult battleResult = CreateBattleResult(launchPlan, 1, true, true, searchCompleted: true);
            MissionSettlementService settlementService = new MissionSettlementService();
            settlementService.ApplyBattleResult(campaignService.CampaignState, launchPlan, battleResult);

            CampaignSiteState siteState;
            Assert.That(campaignService.CampaignState.TryGetSite("site.alpha", out siteState), Is.True);
            Assert.That(siteState.SearchCompleted, Is.True);
            Assert.That(siteState.VisitCount, Is.EqualTo(1));
            Assert.That(siteState.LastVisitedTime, Is.EqualTo(campaignService.CampaignState.CampaignTime).Within(0.0001f));
            Assert.That(siteState.LootRemaining, Is.EqualTo(2));
        }

        [Test]
        public void MissionSettlementService_DoesNotMarkSiteSearchedWithoutSearchObjective()
        {
            CampaignService campaignService = new CampaignService();
            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog("generic", 3, includeExplicitResources: false, includeOutpostDefinition: true));

            MissionLaunchPlan launchPlan;
            string reason;
            bool prepared = preparationService.TryCreateLaunchPlan(campaignService.CampaignState, "mission.m11.loop", "site.alpha", new[] { 1 }, 101, out launchPlan, out reason);
            Assert.That(prepared, Is.True, reason);

            BattleResult battleResult = CreateBattleResult(launchPlan, 0, true, true, searchCompleted: false);
            MissionSettlementService settlementService = new MissionSettlementService();
            settlementService.ApplyBattleResult(campaignService.CampaignState, launchPlan, battleResult);

            CampaignSiteState siteState;
            Assert.That(campaignService.CampaignState.TryGetSite("site.alpha", out siteState), Is.True);
            Assert.That(siteState.SearchCompleted, Is.False);
        }

        [Test]
        public void StartingCampaignFactory_CreatesMultipleSitesAndNoCurrentMission()
        {
            StartingCampaignFactory factory = new StartingCampaignFactory();
            CampaignState campaignState = factory.CreateStartingCampaign();

            Assert.That(campaignState.CurrentMission, Is.Null);
            Assert.That(campaignState.CurrentMissionId, Is.Null);
            Assert.That(campaignState.SitesById.Count, Is.GreaterThanOrEqualTo(3));
            Assert.That(campaignState.ResourceLedger.GetAmount("building_material"), Is.GreaterThanOrEqualTo(4));
        }

        [Test]
        public void ContentCatalog_CanRetrieveOutpostDefinition()
        {
            ContentCatalog catalog = CreateCatalog("generic", 0, includeExplicitResources: false, includeOutpostDefinition: true);
            OutpostDefinition outpostDefinition;

            Assert.That(catalog.TryGetOutpostDefinition("outpost.light", out outpostDefinition), Is.True);
            Assert.That(outpostDefinition, Is.Not.Null);
            Assert.That(outpostDefinition.Capabilities, Has.Member(OutpostCapability.SafeExtraction));
        }

        [Test]
        public void MissionLaunchPlanFactory_UsesExhaustedSiteContext()
        {
            CampaignService campaignService = new CampaignService();
            CampaignSiteState siteState;
            Assert.That(campaignService.CampaignState.TryGetSite("site.alpha", out siteState), Is.True);
            siteState.SetLootRemaining(0);
            siteState.MarkExhausted();

            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog("generic", 0, includeExplicitResources: false, includeOutpostDefinition: false));
            MissionLaunchPlan launchPlan;
            string reason;
            bool prepared = preparationService.TryCreateLaunchPlan(campaignService.CampaignState, "mission.m11.loop", "site.alpha", new[] { 1 }, 101, out launchPlan, out reason);

            Assert.That(prepared, Is.True, reason);
            Assert.That(launchPlan.SiteContext.IsExhausted, Is.True);
            Assert.That(launchPlan.SiteContext.LootRemaining, Is.EqualTo(0));
        }

        private static ContentCatalog CreateCatalog(string rewardProfileId, int genericLootCount, bool includeExplicitResources, bool includeOutpostDefinition)
        {
            WeaponDefinition rifle = new WeaponDefinition(
                "sandbox.rifle",
                "Sandbox Rifle",
                WeaponCategory.Rifle,
                AmmoCategory.Rifle,
                FireMode.Automatic,
                12f,
                0.45f,
                18,
                1f,
                1,
                0f,
                16f,
                DamageType.Kinetic);

            List<ResourceRewardDefinition> explicitRewards = new List<ResourceRewardDefinition>();
            if (includeExplicitResources)
            {
                explicitRewards.Add(new ResourceRewardDefinition("medicine", 3));
            }

            MissionRewardDefinition reward = new MissionRewardDefinition(
                genericLootCount,
                25,
                rewardProfileId,
                new LootRewardDefinition(explicitRewards, null, null));

            MissionDefinition mission = new MissionDefinition(
                "mission.m11.loop",
                "Campaign Loop Mission",
                1,
                1,
                new[]
                {
                    new MissionObjectiveDefinition(MissionObjectiveType.EnterBuilding, "building.100", 1),
                    new MissionObjectiveDefinition(MissionObjectiveType.SearchPoint, "search.main", 1),
                    new MissionObjectiveDefinition(MissionObjectiveType.EliminateEnemies, "enemy.all", 3),
                    new MissionObjectiveDefinition(MissionObjectiveType.ExtractSquad, "extract.alpha", 1)
                },
                MissionType.Tactical,
                MissionDifficulty.Normal,
                "Compound",
                reward);

            SiteDefinition site = new SiteDefinition(
                "site.alpha",
                "Alpha Site",
                SiteType.Compound,
                true,
                3,
                "generic loot");

            Dictionary<string, OutpostDefinition> outposts = new Dictionary<string, OutpostDefinition>();
            if (includeOutpostDefinition)
            {
                outposts["outpost.light"] = new OutpostDefinition(
                    "outpost.light",
                    "Light Outpost",
                    new Dictionary<string, int> { { "building_material", 2 }, { "fuel", 1 } },
                    new Dictionary<string, int> { { "food", 1 } },
                    new[] { OutpostCapability.SafeExtraction, OutpostCapability.Storage });
            }

            return new ContentCatalog(
                new Dictionary<string, UnitDefinition>
                {
                    { "sandbox.rifleman", new UnitDefinition("sandbox.rifleman", "Rifleman", FactionId.Player, 100, 4f, rifle, 12f, 0.5f, ArmorType.Light) }
                },
                new Dictionary<string, MissionDefinition>
                {
                    { mission.Id, mission }
                },
                null,
                new Dictionary<string, WeaponDefinition>
                {
                    { rifle.Id, rifle }
                },
                new Dictionary<string, EnemyDefinition>
                {
                    { "sandbox.raider", new EnemyDefinition("sandbox.raider", "Raider", 55, 2.75f, 14f, 8f, FactionId.Enemy, 12, 0.95f) }
                },
                null,
                null,
                new Dictionary<string, SiteDefinition>
                {
                    { site.Id, site }
                },
                null,
                null,
                outposts.Count > 0 ? outposts : null);
        }

        private static BattleResult CreateBattleResult(
            MissionLaunchPlan launchPlan,
            int lootCount,
            bool victory,
            bool extracted,
            bool searchCompleted = true)
        {
            List<UnitOutcome> unitOutcomes = new List<UnitOutcome>();
            List<BattleEntityId> deadMemberIds = new List<BattleEntityId>();
            List<BattleEntityId> extractedMemberIds = new List<BattleEntityId>();

            for (int i = 0; i < launchPlan.MemberLoadouts.Count; i++)
            {
                MissionMemberLoadout loadout = launchPlan.MemberLoadouts[i];
                unitOutcomes.Add(new UnitOutcome(new BattleEntityId(loadout.BattleMemberId), loadout.CampaignMemberId, true));
                if (extracted)
                {
                    extractedMemberIds.Add(new BattleEntityId(loadout.BattleMemberId));
                }
            }

            List<BattleObjectiveResult> objectiveResults = new List<BattleObjectiveResult>
            {
                new BattleObjectiveResult(MissionObjectiveType.EnterBuilding, "building.100", 1, 1, true),
                new BattleObjectiveResult(MissionObjectiveType.SearchPoint, "search.main", searchCompleted ? 1 : 0, 1, searchCompleted),
                new BattleObjectiveResult(MissionObjectiveType.EliminateEnemies, "enemy.all", 3, 3, victory),
                new BattleObjectiveResult(MissionObjectiveType.ExtractSquad, "extract.alpha", extractedMemberIds.Count, 1, extractedMemberIds.Count > 0)
            };

            return new BattleResult(
                victory ? MissionOutcome.Victory : MissionOutcome.Defeat,
                unitOutcomes,
                deadMemberIds,
                new BattleStatistics(launchPlan.MemberLoadouts.Count, 0),
                36f,
                100,
                victory ? BattleCompletionType.Success : BattleCompletionType.Failure,
                objectiveResults,
                new BattleCasualtyResult(deadMemberIds, new List<BattleEntityId>()),
                new BattleLootResult(lootCount),
                new BattleExtractionResult(extractedMemberIds, launchPlan.MemberLoadouts.Count));
        }

        private static bool ContainsReward(List<CampaignResourceRewardSettlement> rewards, string resourceId)
        {
            for (int i = 0; i < rewards.Count; i++)
            {
                CampaignResourceRewardSettlement reward = rewards[i];
                if (reward != null && reward.ResourceId == resourceId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
