using System.Collections.Generic;
using NUnit.Framework;
using Warzone.Application;
using Warzone.Application.Services;
using Warzone.Campaign;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Tests.Application
{
    public sealed class M15DemoFreezeCleanupTests
    {
        [Test]
        public void BattleResult_DefaultWoundResultIsEmpty()
        {
            BattleResult battleResult = new BattleResult(
                MissionOutcome.Victory,
                new List<UnitOutcome>(),
                new List<BattleEntityId>(),
                new BattleStatistics(0, 0),
                0f,
                1);

            Assert.That(battleResult.WoundResult, Is.Not.Null);
            Assert.That(battleResult.WoundResult.WoundedMemberIds, Is.Empty);
        }

        [Test]
        public void BattleResultSystem_PopulatesWoundedMembersFromHealth()
        {
            BattleState battleState = new BattleState("battle.m15");
            battleState.AddMember(new BattleMemberState(new BattleEntityId(1000), 1, FactionId.Player, new Vec2(0f, 0f), 50, 100, 4f, "sandbox.rifle", "campaign.member.1"));
            battleState.AddMember(new BattleMemberState(new BattleEntityId(1001), 1, FactionId.Player, new Vec2(1f, 0f), 100, 100, 4f, "sandbox.rifle", "campaign.member.2"));
            battleState.SetMissionDefinitionId("sandbox.m5.integrated");
            battleState.SetMissionStatus(new BattleMissionStatusSnapshot(0, 0, 0, 0, 0, 0, 0, 2, true, false, false, false, false, true, BattleCompletionType.Success, 0));

            BattleResultSystem resultSystem = new BattleResultSystem(DemoContentFactory.CreateDemoCatalog());
            BattleResult battleResult = resultSystem.UpdateBattleResult(battleState);

            Assert.That(battleResult, Is.Not.Null);
            Assert.That(ContainsBattleId(battleResult.WoundResult.WoundedMemberIds, 1000), Is.True);
            Assert.That(ContainsBattleId(battleResult.WoundResult.WoundedMemberIds, 1001), Is.False);
        }

        [Test]
        public void MissionLaunchPlanFactory_RejectsDamagedOrUnavailableWeaponInstances()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            CampaignWeaponInstanceState weaponInstance;
            Assert.That(campaignState.Inventory.TryGetWeaponInstance("weapon.alpha.rifle.1", out weaponInstance), Is.True);
            weaponInstance.MarkDamaged(0.25f);

            MissionPreparationService preparationService = new MissionPreparationService(DemoContentFactory.CreateDemoCatalog());
            MissionLaunchPlan launchPlan;
            string reason;

            Assert.That(preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "medical_clinic", new[] { 1 }, 123, out launchPlan, out reason), Is.False);
            Assert.That(reason, Is.Not.Null.And.Not.Empty);

            weaponInstance = null;
            Assert.That(campaignState.Inventory.TryGetWeaponInstance("weapon.alpha.rifle.1", out weaponInstance), Is.True);
            weaponInstance = campaignState.Inventory.WeaponInstances["weapon.alpha.rifle.1"];
            weaponInstance.SetAvailable(false);

            Assert.That(preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "medical_clinic", new[] { 1 }, 123, out launchPlan, out reason), Is.False);
            Assert.That(reason, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void MissionLaunchPlanFactory_FallsBackToDefaultWeaponOnlyWhenNoWeaponAssigned()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            CampaignMemberState member = new CampaignMemberState("campaign.member.5", "Alpha-5", true, false, true, 0, 2, null, null, null);
            campaignState.AddMember(member);
            campaignState.AddSquad(new CampaignSquadState(2, "Reserve Squad", new[] { member.MemberId }));

            MissionPreparationService preparationService = new MissionPreparationService(DemoContentFactory.CreateDemoCatalog());
            MissionLaunchPlan launchPlan;
            string reason;

            Assert.That(preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "medical_clinic", new[] { 2 }, 123, out launchPlan, out reason), Is.True, reason);
            Assert.That(launchPlan.MemberLoadouts[0].WeaponInstanceId, Is.Null);
            Assert.That(launchPlan.MemberLoadouts[0].WeaponDefinitionId, Is.EqualTo("rifle"));
        }

        [Test]
        public void SettlementService_DoesNotWoundUninjuredExtractedMembers_AfterSaveLoad()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            MissionPreparationService preparationService = new MissionPreparationService(DemoContentFactory.CreateDemoCatalog());

            MissionLaunchPlan launchPlan;
            string reason;
            Assert.That(preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "medical_clinic", new[] { 1 }, 123, out launchPlan, out reason), Is.True, reason);

            BattleResult battleResult = CreateBattleResult(launchPlan, woundedIndices: null);
            MissionSettlementService settlementService = new MissionSettlementService(DemoContentFactory.CreateDemoCatalog());
            settlementService.ApplyBattleResult(campaignState, launchPlan, battleResult);

            CampaignMemberState member;
            Assert.That(campaignState.Roster.TryGetMember("campaign.member.1", out member), Is.True);
            Assert.That(member.IsWounded, Is.False);
            Assert.That(member.IsAvailable, Is.True);

            SaveGameService saveGameService = new SaveGameService();
            string saveReason;
            Assert.That(saveGameService.SaveCampaign("slot.m15", campaignState, out saveReason), Is.True, saveReason);

            CampaignState loaded;
            Assert.That(saveGameService.TryLoadCampaign("slot.m15", out loaded, out saveReason), Is.True, saveReason);

            CampaignMemberState loadedMember;
            Assert.That(loaded.Roster.TryGetMember("campaign.member.1", out loadedMember), Is.True);
            Assert.That(loadedMember.IsWounded, Is.False);
            Assert.That(loadedMember.IsAvailable, Is.True);
        }

        [Test]
        public void SettlementService_ReturnsUninjuredWeaponsUndamaged()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            campaignState.MainBase.RemoveModule("base.workshop");

            MissionPreparationService preparationService = new MissionPreparationService(DemoContentFactory.CreateDemoCatalog());
            MissionLaunchPlan launchPlan;
            string reason;
            Assert.That(preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "medical_clinic", new[] { 1 }, 123, out launchPlan, out reason), Is.True, reason);

            BattleResult battleResult = CreateBattleResult(launchPlan, woundedIndices: null);
            MissionSettlementService settlementService = new MissionSettlementService(DemoContentFactory.CreateDemoCatalog());
            settlementService.ApplyBattleResult(campaignState, launchPlan, battleResult);

            CampaignWeaponInstanceState weapon;
            Assert.That(campaignState.Inventory.TryGetWeaponInstance("weapon.alpha.rifle.1", out weapon), Is.True);
            Assert.That(weapon.IsAvailable, Is.True);
            Assert.That(weapon.IsDamaged, Is.False);
        }

        [Test]
        public void SettlementService_WorkshopPreventsDamageForWoundedWeapons()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();

            MissionPreparationService preparationService = new MissionPreparationService(DemoContentFactory.CreateDemoCatalog());
            MissionLaunchPlan launchPlan;
            string reason;
            Assert.That(preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "medical_clinic", new[] { 1 }, 123, out launchPlan, out reason), Is.True, reason);

            BattleResult battleResult = CreateBattleResult(launchPlan, new[] { 0 });
            MissionSettlementService settlementService = new MissionSettlementService(DemoContentFactory.CreateDemoCatalog());
            settlementService.ApplyBattleResult(campaignState, launchPlan, battleResult);

            CampaignWeaponInstanceState weapon;
            Assert.That(campaignState.Inventory.TryGetWeaponInstance("weapon.alpha.rifle.1", out weapon), Is.True);
            Assert.That(weapon.IsAvailable, Is.True);
            Assert.That(weapon.IsDamaged, Is.False);
        }

        [Test]
        public void SettlementService_WoundedWeaponsDamageWithoutWorkshop()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            campaignState.MainBase.RemoveModule("base.workshop");

            MissionPreparationService preparationService = new MissionPreparationService(DemoContentFactory.CreateDemoCatalog());
            MissionLaunchPlan launchPlan;
            string reason;
            Assert.That(preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "medical_clinic", new[] { 1 }, 123, out launchPlan, out reason), Is.True, reason);

            BattleResult battleResult = CreateBattleResult(launchPlan, new[] { 0 });
            MissionSettlementService settlementService = new MissionSettlementService(DemoContentFactory.CreateDemoCatalog());
            settlementService.ApplyBattleResult(campaignState, launchPlan, battleResult);

            CampaignWeaponInstanceState weapon;
            Assert.That(campaignState.Inventory.TryGetWeaponInstance("weapon.alpha.rifle.1", out weapon), Is.True);
            Assert.That(weapon.IsAvailable, Is.True);
            Assert.That(weapon.IsDamaged, Is.True);
        }

        [Test]
        public void SettlementService_WorkshopPreventsDamageForWoundedWeapons_WithWorkshop()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();

            MissionPreparationService preparationService = new MissionPreparationService(DemoContentFactory.CreateDemoCatalog());
            MissionLaunchPlan launchPlan;
            string reason;
            Assert.That(preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "medical_clinic", new[] { 1 }, 123, out launchPlan, out reason), Is.True, reason);

            BattleResult battleResult = CreateBattleResult(launchPlan, new[] { 0 });
            MissionSettlementService settlementService = new MissionSettlementService(DemoContentFactory.CreateDemoCatalog());
            settlementService.ApplyBattleResult(campaignState, launchPlan, battleResult);

            CampaignWeaponInstanceState weapon;
            Assert.That(campaignState.Inventory.TryGetWeaponInstance("weapon.alpha.rifle.1", out weapon), Is.True);
            Assert.That(weapon.IsAvailable, Is.True);
            Assert.That(weapon.IsDamaged, Is.False);
        }

        private static BattleResult CreateBattleResult(MissionLaunchPlan launchPlan, IEnumerable<int> woundedIndices)
        {
            List<int> woundedIndexList = new List<int>(woundedIndices ?? new int[0]);
            List<UnitOutcome> unitOutcomes = new List<UnitOutcome>();
            List<BattleEntityId> extractedMemberIds = new List<BattleEntityId>();
            List<BattleEntityId> woundedMemberIds = new List<BattleEntityId>();

            for (int i = 0; i < launchPlan.MemberLoadouts.Count; i++)
            {
                MissionMemberLoadout loadout = launchPlan.MemberLoadouts[i];
                unitOutcomes.Add(new UnitOutcome(new BattleEntityId(loadout.BattleMemberId), loadout.CampaignMemberId, true));
                extractedMemberIds.Add(new BattleEntityId(loadout.BattleMemberId));

                if (woundedIndexList.Contains(i))
                {
                    woundedMemberIds.Add(new BattleEntityId(loadout.BattleMemberId));
                }
            }

            return new BattleResult(
                MissionOutcome.Victory,
                unitOutcomes,
                new List<BattleEntityId>(),
                new BattleStatistics(launchPlan.MemberLoadouts.Count, 0),
                24f,
                15,
                BattleCompletionType.Success,
                new[]
                {
                    new BattleObjectiveResult(MissionObjectiveType.SearchPoint, "search.main", 1, 1, true),
                    new BattleObjectiveResult(MissionObjectiveType.ExtractSquad, "extract.alpha", extractedMemberIds.Count, 1, true)
                },
                new BattleCasualtyResult(new List<BattleEntityId>(), new List<BattleEntityId>()),
                new BattleLootResult(0),
                new BattleExtractionResult(extractedMemberIds, launchPlan.MemberLoadouts.Count),
                new BattleWoundResult(woundedMemberIds));
        }

        private static bool ContainsBattleId(IReadOnlyList<BattleEntityId> ids, int battleMemberId)
        {
            if (ids == null)
            {
                return false;
            }

            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i].Value == battleMemberId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
