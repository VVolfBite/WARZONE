using System.Collections.Generic;
using NUnit.Framework;
using Warzone.Application;
using Warzone.Application.Save;
using Warzone.Application.Services;
using Warzone.Campaign;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Tests.Application
{
    public sealed class M14ProgressionRecoveryEquipmentTests
    {
        [Test]
        public void MissionLaunchPlanFactory_RejectsLostWeaponInstance()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            CampaignWeaponInstanceState weaponInstance;
            Assert.That(campaignState.Inventory.TryGetWeaponInstance("weapon.alpha.rifle.1", out weaponInstance), Is.True);
            weaponInstance.MarkLost("mission.lost");
            weaponInstance.SetAvailable(false);

            MissionPreparationService preparationService = new MissionPreparationService(DemoContentFactory.CreateDemoCatalog());
            MissionLaunchPlan launchPlan;
            string reason;

            bool prepared = preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "medical_clinic", new[] { 1 }, 123, out launchPlan, out reason);

            Assert.That(prepared, Is.False);
            Assert.That(launchPlan, Is.Null);
            Assert.That(reason, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void MissionLaunchPlanFactory_IncludesWeaponInstanceIds()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            MissionPreparationService preparationService = new MissionPreparationService(DemoContentFactory.CreateDemoCatalog());
            MissionLaunchPlan launchPlan;
            string reason;

            bool prepared = preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "medical_clinic", new[] { 1 }, 123, out launchPlan, out reason);

            Assert.That(prepared, Is.True, reason);
            Assert.That(launchPlan.MemberLoadouts, Is.Not.Null);
            Assert.That(launchPlan.MemberLoadouts[0].WeaponInstanceId, Is.EqualTo("weapon.alpha.rifle.1"));
        }

        [Test]
        public void SettlementService_AppliesProgressionAndRecoveryState()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            campaignState.MainBase.RemoveModule("base.infirmary");

            MissionLaunchPlan launchPlan = CreateLaunchPlan(campaignState);
            BattleResult battleResult = CreateBattleResult(launchPlan, new[] { 1 }, new[] { 0, 2, 3 }, new[] { 0 }, 4, true);

            MissionSettlementService settlementService = new MissionSettlementService(DemoContentFactory.CreateDemoCatalog());
            settlementService.ApplyBattleResult(campaignState, launchPlan, battleResult);

            CampaignMemberState survivor;
            Assert.That(campaignState.Roster.TryGetMember("campaign.member.1", out survivor), Is.True);
            Assert.That(survivor.Experience, Is.GreaterThanOrEqualTo(25));
            Assert.That(survivor.MissionsCompleted, Is.EqualTo(1));
            Assert.That(survivor.IsWounded, Is.True);
            Assert.That(survivor.IsRecovering, Is.True);
            Assert.That(survivor.RecoveryDaysRemaining, Is.EqualTo(1));
        }

        [Test]
        public void SettlementService_DoesNotWoundUninjuredExtractedMembers()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            MissionLaunchPlan launchPlan = CreateLaunchPlan(campaignState);
            BattleResult battleResult = CreateBattleResult(launchPlan, new[] { 1 }, new[] { 0, 2, 3 }, null, 4, true);

            MissionSettlementService settlementService = new MissionSettlementService(DemoContentFactory.CreateDemoCatalog());
            settlementService.ApplyBattleResult(campaignState, launchPlan, battleResult);

            CampaignMemberState survivor;
            Assert.That(campaignState.Roster.TryGetMember("campaign.member.1", out survivor), Is.True);
            Assert.That(survivor.IsWounded, Is.False);
            Assert.That(survivor.IsRecovering, Is.False);
            Assert.That(survivor.IsAvailable, Is.True);
        }

        [Test]
        public void InfirmaryReducesRecoveryDays()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            CampaignRecoverySystem recoverySystem = new CampaignRecoverySystem();

            CampaignMemberState member;
            Assert.That(campaignState.Roster.TryGetMember("campaign.member.1", out member), Is.True);

            campaignState.MainBase.RemoveModule("base.infirmary");
            recoverySystem.ApplyWound(campaignState, member, WoundSeverity.Moderate, 3, "mission.test");
            Assert.That(member.RecoveryDaysRemaining, Is.EqualTo(3));

            CampaignState campaignWithInfirmary = new StartingCampaignFactory().CreateStartingCampaign();
            CampaignMemberState treatedMember;
            Assert.That(campaignWithInfirmary.Roster.TryGetMember("campaign.member.1", out treatedMember), Is.True);
            recoverySystem.ApplyWound(campaignWithInfirmary, treatedMember, WoundSeverity.Moderate, 3, "mission.test");
            Assert.That(treatedMember.RecoveryDaysRemaining, Is.EqualTo(2));
        }

        [Test]
        public void SettlementService_ReturnsAndLosesWeapons()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            campaignState.MainBase.RemoveModule("base.workshop");

            MissionLaunchPlan launchPlan = CreateLaunchPlan(campaignState);
            BattleResult battleResult = CreateBattleResult(launchPlan, new[] { 1 }, new[] { 0, 2, 3 }, new[] { 0 }, 3, true);

            MissionSettlementService settlementService = new MissionSettlementService(DemoContentFactory.CreateDemoCatalog());
            settlementService.ApplyBattleResult(campaignState, launchPlan, battleResult);

            CampaignWeaponInstanceState returnedWeapon;
            Assert.That(campaignState.Inventory.TryGetWeaponInstance("weapon.alpha.rifle.1", out returnedWeapon), Is.True);
            Assert.That(returnedWeapon.IsAvailable, Is.True);
            Assert.That(returnedWeapon.IsDamaged, Is.True);

            CampaignWeaponInstanceState lostWeapon;
            Assert.That(campaignState.Inventory.TryGetWeaponInstance("weapon.alpha.rifle.2", out lostWeapon), Is.True);
            Assert.That(lostWeapon.IsLost, Is.True);
            Assert.That(lostWeapon.IsAvailable, Is.False);
        }

        [Test]
        public void SaveLoad_RoundTripsProgressionRecoveryAndEquipmentFields()
        {
            CampaignState campaignState = new StartingCampaignFactory().CreateStartingCampaign();
            CampaignMemberState member;
            Assert.That(campaignState.Roster.TryGetMember("campaign.member.1", out member), Is.True);
            member.AddExperience(135);
            member.AddMissionCompleted(3);
            member.AddKill(7);
            member.ApplyWound(WoundSeverity.Severe, 4, "mission.m14");
            member.SetCarriedWeaponInstanceId("weapon.alpha.rifle.1");

            CampaignWeaponInstanceState weaponInstance;
            Assert.That(campaignState.Inventory.TryGetWeaponInstance("weapon.alpha.rifle.1", out weaponInstance), Is.True);
            weaponInstance.MarkDamaged(0.5f);
            weaponInstance.MarkLost("mission.m14");

            SaveGameService saveGameService = new SaveGameService();
            string reason;
            Assert.That(saveGameService.SaveCampaign("slot.m14", campaignState, out reason), Is.True, reason);

            CampaignState loaded;
            Assert.That(saveGameService.TryLoadCampaign("slot.m14", out loaded, out reason), Is.True, reason);

            CampaignMemberState loadedMember;
            Assert.That(loaded.Roster.TryGetMember("campaign.member.1", out loadedMember), Is.True);
            Assert.That(loadedMember.Level, Is.GreaterThanOrEqualTo(2));
            Assert.That(loadedMember.MissionsCompleted, Is.EqualTo(3));
            Assert.That(loadedMember.Kills, Is.EqualTo(7));
            Assert.That(loadedMember.WoundSeverity, Is.EqualTo(WoundSeverity.Severe));
            Assert.That(loadedMember.RecoveryDaysRemaining, Is.EqualTo(4));
            Assert.That(loadedMember.IsRecovering, Is.True);

            CampaignWeaponInstanceState loadedWeapon;
            Assert.That(loaded.Inventory.TryGetWeaponInstance("weapon.alpha.rifle.1", out loadedWeapon), Is.True);
            Assert.That(loadedWeapon.IsLost, Is.True);
            Assert.That(loadedWeapon.IsDamaged, Is.True);
            Assert.That(loadedWeapon.IsAvailable, Is.False);
        }

        [Test]
        public void EnvironmentalZoneRuntimeFactory_MapsContentZoneTypes()
        {
            EnvironmentalZoneRuntimeFactory factory = new EnvironmentalZoneRuntimeFactory();

            Assert.That(factory.Map(EnvironmentalZoneDefinitionType.Smoke), Is.EqualTo(EnvironmentalZoneType.Smoke));
            Assert.That(factory.Map(EnvironmentalZoneDefinitionType.Fire), Is.EqualTo(EnvironmentalZoneType.Fire));
            Assert.That(factory.Map(EnvironmentalZoneDefinitionType.Toxic), Is.EqualTo(EnvironmentalZoneType.Toxic));
            Assert.That(factory.Map(EnvironmentalZoneDefinitionType.Light), Is.EqualTo(EnvironmentalZoneType.Light));
            Assert.That(factory.Map(EnvironmentalZoneDefinitionType.Darkness), Is.EqualTo(EnvironmentalZoneType.Darkness));
        }

        private static MissionLaunchPlan CreateLaunchPlan(CampaignState campaignState)
        {
            MissionPreparationService preparationService = new MissionPreparationService(DemoContentFactory.CreateDemoCatalog());
            MissionLaunchPlan launchPlan;
            string reason;
            bool result = preparationService.TryCreateLaunchPlan(campaignState, "mission.m9.loop", "medical_clinic", new[] { 1 }, 123, out launchPlan, out reason);
            Assert.That(result, Is.True, reason);
            return launchPlan;
        }

        private static BattleResult CreateBattleResult(
            MissionLaunchPlan launchPlan,
            IEnumerable<int> deadIndices,
            IEnumerable<int> extractedIndices,
            IEnumerable<int> woundedIndices,
            int lootCount,
            bool victory)
        {
            List<int> deadIndexList = new List<int>(deadIndices ?? new int[0]);
            List<int> extractedIndexList = new List<int>(extractedIndices ?? new int[0]);
            List<int> woundedIndexList = new List<int>(woundedIndices ?? new int[0]);
            List<UnitOutcome> unitOutcomes = new List<UnitOutcome>();
            List<BattleEntityId> deadMemberIds = new List<BattleEntityId>();
            List<BattleEntityId> extractedMemberIds = new List<BattleEntityId>();
            List<BattleEntityId> woundedMemberIds = new List<BattleEntityId>();

            for (int i = 0; i < launchPlan.MemberLoadouts.Count; i++)
            {
                MissionMemberLoadout loadout = launchPlan.MemberLoadouts[i];
                bool isDead = deadIndexList.Contains(i);
                bool isExtracted = extractedIndexList.Contains(i);
                bool isWounded = woundedIndexList.Contains(i);
                unitOutcomes.Add(new UnitOutcome(new BattleEntityId(loadout.BattleMemberId), loadout.CampaignMemberId, !isDead));
                if (isDead)
                {
                    deadMemberIds.Add(new BattleEntityId(loadout.BattleMemberId));
                }

                if (isExtracted)
                {
                    extractedMemberIds.Add(new BattleEntityId(loadout.BattleMemberId));
                }

                if (isWounded && !isDead)
                {
                    woundedMemberIds.Add(new BattleEntityId(loadout.BattleMemberId));
                }
            }

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
                new[]
                {
                    new BattleObjectiveResult(MissionObjectiveType.SearchPoint, "search.main", 1, 1, true),
                    new BattleObjectiveResult(MissionObjectiveType.ExtractSquad, "extract.alpha", extractedMemberIds.Count, 1, extractedMemberIds.Count > 0)
                },
                new BattleCasualtyResult(deadMemberIds, new BattleEntityId[0]),
                new BattleLootResult(lootCount),
                new BattleExtractionResult(extractedMemberIds, launchPlan.MemberLoadouts.Count - deadMemberIds.Count),
                new BattleWoundResult(woundedMemberIds));
        }
    }
}
