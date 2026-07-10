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
    public sealed class MissionLoopTests
    {
        [Test]
        public void MissionLaunchPlanFactory_DoesNotMutateCurrentMission()
        {
            CampaignService campaignService = new CampaignService();
            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog());

            string currentMissionBefore = campaignService.CampaignState.CurrentMissionId;

            MissionLaunchPlan launchPlan;
            string reason;
            bool result = preparationService.TryCreateLaunchPlan(campaignService.CampaignState, "mission.m9.loop", "site.alpha", new[] { 1 }, 123, out launchPlan, out reason);

            Assert.That(result, Is.True, reason);
            Assert.That(launchPlan, Is.Not.Null);
            Assert.That(campaignService.CampaignState.CurrentMissionId, Is.EqualTo(currentMissionBefore));
        }

        [Test]
        public void MissionPreparationService_StartMissionSetsCurrentMission()
        {
            CampaignService campaignService = new CampaignService();
            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog());

            MissionLaunchPlan launchPlan;
            string reason;
            bool result = preparationService.TryCreateLaunchPlan(campaignService.CampaignState, "mission.m9.loop", "site.alpha", new[] { 1 }, 123, out launchPlan, out reason);

            Assert.That(result, Is.True, reason);

            preparationService.StartMission(campaignService.CampaignState, launchPlan);

            Assert.That(campaignService.CampaignState.CurrentMissionId, Is.EqualTo("mission.m9.loop"));
            Assert.That(campaignService.CampaignState.CurrentMission, Is.Not.Null);
        }

        [Test]
        public void MissionLaunchPlanFactory_RejectsMissingSquad()
        {
            CampaignState campaignState = new CampaignService().CampaignState;
            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog());

            MissionLaunchPlan launchPlan;
            string reason;
            bool result = preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "site.alpha", new[] { 99 }, 123, out launchPlan, out reason);

            Assert.That(result, Is.False);
            Assert.That(launchPlan, Is.Null);
            Assert.That(reason, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void MissionLaunchPlanFactory_RejectsUnavailableMembers()
        {
            CampaignService campaignService = new CampaignService();
            CampaignMemberState memberState;
            campaignService.CampaignState.Roster.TryGetMember("campaign.member.1", out memberState);
            memberState.MarkDead();

            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog());

            MissionLaunchPlan launchPlan;
            string reason;
            bool result = preparationService.TryCreateLaunchPlan(campaignService.CampaignState, "mission.m9.loop", "site.alpha", new[] { 1 }, 123, out launchPlan, out reason);

            Assert.That(result, Is.False);
            Assert.That(launchPlan, Is.Null);
            Assert.That(reason, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void MissionLaunchPlanFactory_CreatesLaunchPlanForAvailableSquad()
        {
            CampaignService campaignService = new CampaignService();
            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog());

            MissionLaunchPlan launchPlan;
            string reason;
            bool result = preparationService.TryCreateLaunchPlan(campaignService.CampaignState, "mission.m9.loop", "site.alpha", new[] { 1 }, 123, out launchPlan, out reason);

            Assert.That(result, Is.True, reason);
            Assert.That(launchPlan, Is.Not.Null);
            Assert.That(launchPlan.MemberLoadouts.Count, Is.EqualTo(4));
            Assert.That(launchPlan.SelectedSquadIds, Has.Member(1));
            Assert.That(launchPlan.SiteContext.SiteId, Is.EqualTo("site.alpha"));
            Assert.That(launchPlan.ObjectiveDefinitions.Count, Is.GreaterThan(0));
        }

        [Test]
        public void BattleStateFromMissionFactory_CreatesBattleStateWithSquadsMembersAndObjectives()
        {
            MissionLaunchPlan launchPlan = CreateLaunchPlan();
            BattleStateFromMissionFactory factory = new BattleStateFromMissionFactory();

            BattleState battleState = factory.Create(launchPlan);

            Assert.That(battleState, Is.Not.Null);
            Assert.That(battleState.MembersById.Count, Is.EqualTo(launchPlan.MemberLoadouts.Count));
            Assert.That(battleState.SquadsById.Count, Is.EqualTo(launchPlan.SelectedSquadIds.Count));
            Assert.That(battleState.MissionRuntimeState.ObjectiveStates.Count, Is.GreaterThanOrEqualTo(launchPlan.ObjectiveDefinitions.Count));
        }

        [Test]
        public void SettlementService_WritesBattleResultBackToCampaignState()
        {
            CampaignService campaignService = new CampaignService();
            MissionLaunchPlan launchPlan = CreateLaunchPlan(campaignService.CampaignState);
            BattleResult battleResult = CreateBattleResult(launchPlan, new[] { 0 }, new[] { 1 }, 4, true);

            MissionSettlementService settlementService = new MissionSettlementService();
            CampaignSettlement settlement = settlementService.ApplyBattleResult(campaignService.CampaignState, launchPlan, battleResult);

            Assert.That(settlement, Is.Not.Null);

            CampaignMemberState deadMember;
            Assert.That(campaignService.CampaignState.Roster.TryGetMember("campaign.member.1", out deadMember), Is.True);
            Assert.That(deadMember.IsAlive, Is.False);

            CampaignSiteState siteState;
            Assert.That(campaignService.CampaignState.TryGetSite("site.alpha", out siteState), Is.True);
            Assert.That(siteState.SearchCompleted, Is.True);
            Assert.That(siteState.IsCleared, Is.True);

            Assert.That(campaignService.CampaignState.ResourceLedger.GetAmount("generic_loot"), Is.EqualTo(7));
            Assert.That(campaignService.CampaignState.Inventory.ResourcePackages.ContainsKey("generic.loot"), Is.False);

            Assert.That(campaignService.CampaignState.MissionHistory.Count, Is.GreaterThan(0));
        }

        [Test]
        public void SettlementService_MapsMedicalProfileLootToMedicine()
        {
            CampaignService campaignService = new CampaignService();
            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog(true, "medical", 0));

            MissionLaunchPlan launchPlan;
            string reason;
            bool prepared = preparationService.TryCreateLaunchPlan(campaignService.CampaignState, "mission.m9.loop", "site.alpha", new[] { 1 }, 777, out launchPlan, out reason);
            Assert.That(prepared, Is.True, reason);

            BattleResult battleResult = CreateBattleResult(launchPlan, new[] { 0 }, new[] { 1 }, 4, true, searchCompleted: true);
            MissionSettlementService settlementService = new MissionSettlementService();
            settlementService.ApplyBattleResult(campaignService.CampaignState, launchPlan, battleResult);

            Assert.That(campaignService.CampaignState.ResourceLedger.GetAmount("medicine"), Is.EqualTo(12));
        }

        [Test]
        public void SettlementService_ZeroLootDoesNotCreateGenericLootEntry()
        {
            CampaignService campaignService = new CampaignService();
            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog(false));

            MissionLaunchPlan launchPlan;
            string reason;
            bool prepared = preparationService.TryCreateLaunchPlan(campaignService.CampaignState, "mission.m9.loop", "site.alpha", new[] { 1 }, 777, out launchPlan, out reason);
            Assert.That(prepared, Is.True, reason);

            BattleResult battleResult = CreateBattleResult(launchPlan, new[] { 0 }, new[] { 1 }, 0, true, searchCompleted: false);
            MissionSettlementService settlementService = new MissionSettlementService();
            settlementService.ApplyBattleResult(campaignService.CampaignState, launchPlan, battleResult);

            Assert.That(campaignService.CampaignState.ResourceLedger.GetAmount("generic_loot"), Is.EqualTo(0));
            CampaignSiteState siteState;
            Assert.That(campaignService.CampaignState.TryGetSite("site.alpha", out siteState), Is.True);
            Assert.That(siteState.SearchCompleted, Is.False);
        }

        [Test]
        public void SettlementService_MultiSquadAvailabilityComputedPerSquad()
        {
            CampaignState campaignState = new CampaignService().CampaignState;
            campaignState.AddMember(new CampaignMemberState("campaign.member.5", "Bravo-1", true, false, true, 0, 1, "sandbox.rifle"));
            campaignState.AddMember(new CampaignMemberState("campaign.member.6", "Bravo-2", true, false, true, 0, 1, "sandbox.rifle"));
            campaignState.AddMember(new CampaignMemberState("campaign.member.7", "Bravo-3", true, false, true, 0, 1, "sandbox.rifle"));
            campaignState.AddMember(new CampaignMemberState("campaign.member.8", "Bravo-4", true, false, true, 0, 1, "sandbox.rifle"));
            campaignState.AddSquad(new CampaignSquadState(2, "Bravo Squad", new[] { "campaign.member.5", "campaign.member.6", "campaign.member.7", "campaign.member.8" }));

            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog(false));
            MissionLaunchPlan launchPlan;
            string reason;
            bool prepared = preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "site.alpha", new[] { 1, 2 }, 777, out launchPlan, out reason);
            Assert.That(prepared, Is.True, reason);

            BattleResult battleResult = CreateBattleResult(
                launchPlan,
                new[] { 0, 1, 2, 3 },
                new[] { 4, 5, 6, 7 },
                2,
                true);

            MissionSettlementService settlementService = new MissionSettlementService();
            CampaignSettlement settlement = settlementService.ApplyBattleResult(campaignState, launchPlan, battleResult);

            CampaignSquadState alphaSquad;
            CampaignSquadState bravoSquad;
            Assert.That(campaignState.Roster.TryGetSquad(1, out alphaSquad), Is.True);
            Assert.That(campaignState.Roster.TryGetSquad(2, out bravoSquad), Is.True);

            Assert.That(alphaSquad.IsAvailable, Is.False);
            Assert.That(bravoSquad.IsAvailable, Is.True);
            Assert.That(settlement.SquadSettlements.Count, Is.EqualTo(2));
        }

        [Test]
        public void EndToEndApplicationFlow_UpdatesCampaignState()
        {
            CampaignService campaignService = new CampaignService();
            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog());

            MissionLaunchPlan launchPlan;
            BattleState battleState;
            string reason;
            bool prepared = preparationService.TryPrepareBattle(campaignService.CampaignState, "mission.m9.loop", "site.alpha", new[] { 1 }, 777, out launchPlan, out battleState, out reason);

            Assert.That(prepared, Is.True, reason);
            Assert.That(battleState, Is.Not.Null);
            Assert.That(campaignService.CampaignState.CurrentMissionId, Is.EqualTo("mission.m9.loop"));

            BattleResult battleResult = CreateBattleResult(launchPlan, new[] { 0 }, new[] { 1 }, 3, true);
            MissionSettlementService settlementService = new MissionSettlementService();
            settlementService.ApplyBattleResult(campaignService.CampaignState, launchPlan, battleResult);

            Assert.That(campaignService.CampaignState.MissionHistory.Count, Is.EqualTo(1));
            Assert.That(campaignService.CampaignState.ResourceLedger.GetAmount("generic_loot"), Is.EqualTo(6));
        }

        private static MissionLaunchPlan CreateLaunchPlan()
        {
            return CreateLaunchPlan(new CampaignService().CampaignState);
        }

        private static MissionLaunchPlan CreateLaunchPlan(CampaignState campaignState)
        {
            MissionPreparationService preparationService = new MissionPreparationService(CreateCatalog());
            MissionLaunchPlan launchPlan;
            string reason;
            bool result = preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "site.alpha", new[] { 1 }, 123, out launchPlan, out reason);
            Assert.That(result, Is.True, reason);
            return launchPlan;
        }

        private static ContentCatalog CreateCatalog(bool includeReward = true, string rewardProfileId = "reward.loop", int genericLootCount = 3)
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

            MissionRewardDefinition reward = includeReward
                ? new MissionRewardDefinition(genericLootCount, 25, rewardProfileId)
                : new MissionRewardDefinition(0, 0, null);

            MissionDefinition mission = new MissionDefinition(
                "mission.m9.loop",
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

            SiteDefinition site = new SiteDefinition("site.alpha", "Alpha Site", SiteType.Compound, true, 3, "generic loot");

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
                });
        }

        private static BattleResult CreateBattleResult(
            MissionLaunchPlan launchPlan,
            IEnumerable<int> deadIndices,
            IEnumerable<int> extractedIndices,
            int lootCount,
            bool victory,
            bool searchCompleted = true)
        {
            List<int> deadIndexList = new List<int>(deadIndices ?? new int[0]);
            List<int> extractedIndexList = new List<int>(extractedIndices ?? new int[0]);
            List<UnitOutcome> unitOutcomes = new List<UnitOutcome>();
            List<BattleEntityId> deadMemberIds = new List<BattleEntityId>();
            List<BattleEntityId> extractedMemberIds = new List<BattleEntityId>();

            for (int i = 0; i < launchPlan.MemberLoadouts.Count; i++)
            {
                MissionMemberLoadout loadout = launchPlan.MemberLoadouts[i];
                bool isDead = deadIndexList.Contains(i);
                bool isExtracted = extractedIndexList.Contains(i);
                unitOutcomes.Add(new UnitOutcome(new BattleEntityId(loadout.BattleMemberId), loadout.CampaignMemberId, !isDead));
                if (isDead)
                {
                    deadMemberIds.Add(new BattleEntityId(loadout.BattleMemberId));
                }

                if (isExtracted)
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
                new BattleStatistics(
                    launchPlan.MemberLoadouts.Count - deadMemberIds.Count,
                    victory ? 0 : 1),
                42f,
                123,
                victory ? BattleCompletionType.Success : BattleCompletionType.Failure,
                objectiveResults,
                new BattleCasualtyResult(deadMemberIds, new List<BattleEntityId>()),
                new BattleLootResult(lootCount),
                new BattleExtractionResult(extractedMemberIds, launchPlan.MemberLoadouts.Count - deadMemberIds.Count));
        }
    }
}
