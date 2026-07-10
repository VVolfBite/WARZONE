using System.Collections.Generic;
using Warzone.Campaign;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Application.Services
{
    public sealed class MissionSettlementService
    {
        private readonly CampaignSettlementSystem _settlementSystem = new CampaignSettlementSystem();
        private readonly MissionRewardResolver _rewardResolver;

        public MissionSettlementService()
            : this(null)
        {
        }

        public MissionSettlementService(ContentCatalog contentCatalog)
        {
            _rewardResolver = new MissionRewardResolver(contentCatalog);
        }

        public CampaignSettlement ApplyBattleResult(
            CampaignState campaignState,
            MissionLaunchPlan launchPlan,
            BattleResult battleResult)
        {
            if (campaignState == null || launchPlan == null || battleResult == null)
            {
                return null;
            }

            List<CampaignResourceRewardSettlement> resourceRewards = _rewardResolver.ResolveResourceRewards(launchPlan, battleResult);
            List<CampaignItemRewardSettlement> itemRewards = _rewardResolver.ResolveItemRewards(launchPlan, battleResult);
            List<CampaignWeaponRewardSettlement> weaponRewards = _rewardResolver.ResolveWeaponRewards(launchPlan, battleResult);
            List<CampaignCasualtySettlement> casualties = BuildCasualties(launchPlan, battleResult);
            List<CampaignWoundSettlement> woundSettlements = BuildWoundSettlements(campaignState, launchPlan, battleResult);
            List<CampaignEquipmentSettlement> equipmentSettlements = BuildEquipmentSettlements(campaignState, launchPlan, battleResult);
            List<CampaignExperienceSettlement> experienceSettlements = BuildExperienceSettlements(campaignState, launchPlan, battleResult);
            List<CampaignBaseEffectSettlement> baseEffects = BuildBaseEffects();

            CampaignSiteState siteState;
            campaignState.TryGetSite(launchPlan.SiteId, out siteState);

            bool searchCompleted = HasCompletedObjective(battleResult.ObjectiveResults, MissionObjectiveType.SearchPoint);
            bool missionSucceeded = battleResult.MissionOutcome == MissionOutcome.Victory;
            float visitedTime = campaignState.CampaignTime + battleResult.ElapsedTimeSeconds / 3600f;
            int lootCount = battleResult.LootResult != null ? battleResult.LootResult.LootCount : 0;
            int visitCountDelta = 1;
            int threatLevel = siteState != null ? siteState.ThreatLevel : 0;
            int resourceRichnessDelta = searchCompleted ? lootCount : 0;
            int lootRemainingDelta = searchCompleted ? lootCount : 0;
            bool isOccupied = siteState != null ? siteState.IsOccupied : false;
            bool isExhausted = siteState != null ? siteState.IsExhausted : false;
            string outpostId = siteState != null ? siteState.OutpostId : null;

            if (missionSucceeded)
            {
                threatLevel = 0;
                isOccupied = false;
            }

            List<CampaignSiteSettlement> siteSettlements = new List<CampaignSiteSettlement>
            {
                new CampaignSiteSettlement(
                    launchPlan.SiteId,
                    searchCompleted,
                    missionSucceeded,
                    threatLevel,
                    visitedTime,
                    lootRemainingDelta,
                    resourceRichnessDelta,
                    visitCountDelta,
                    isOccupied,
                    isExhausted,
                    outpostId)
            };

            List<CampaignSquadSettlement> squadSettlements = new List<CampaignSquadSettlement>();
            int objectiveCount = battleResult.ObjectiveResults != null ? battleResult.ObjectiveResults.Count : 0;
            for (int i = 0; i < launchPlan.SelectedSquadIds.Count; i++)
            {
                int squadId = launchPlan.SelectedSquadIds[i];
                bool squadAvailable = IsSquadAvailable(launchPlan, casualties, squadId);
                squadSettlements.Add(new CampaignSquadSettlement(squadId, squadAvailable, objectiveCount));
            }

            int deadCount = battleResult.CasualtyResult != null && battleResult.CasualtyResult.DeadMemberIds != null
                ? battleResult.CasualtyResult.DeadMemberIds.Count
                : 0;
            int extractedCount = battleResult.ExtractionResult != null ? battleResult.ExtractionResult.SurvivingMemberCount : 0;

            CampaignMissionHistoryRecord history = new CampaignMissionHistoryRecord(
                launchPlan.MissionId,
                launchPlan.SiteId,
                missionSucceeded,
                deadCount,
                lootCount,
                battleResult.ElapsedTimeSeconds,
                battleResult.CompletionType.ToString(),
                BuildResourceSummary(resourceRewards));

            CampaignSettlement settlement = new CampaignSettlement(
                launchPlan.MissionId,
                launchPlan.SiteId,
                missionSucceeded,
                casualties,
                new List<CampaignLootSettlement>(),
                siteSettlements,
                squadSettlements,
                history,
                resourceRewards,
                itemRewards,
                weaponRewards,
                baseEffects,
                experienceSettlements,
                woundSettlements,
                equipmentSettlements,
                deadCount,
                extractedCount);

            _settlementSystem.Apply(campaignState, settlement);
            campaignState.AdvanceCampaignTime(battleResult.ElapsedTimeSeconds / 3600f);
            return settlement;
        }

        private static List<CampaignCasualtySettlement> BuildCasualties(MissionLaunchPlan launchPlan, BattleResult battleResult)
        {
            List<CampaignCasualtySettlement> casualties = new List<CampaignCasualtySettlement>();
            IReadOnlyList<BattleEntityId> deadMemberIds = battleResult.CasualtyResult != null ? battleResult.CasualtyResult.DeadMemberIds : null;
            IReadOnlyList<BattleEntityId> extractedMemberIds = battleResult.ExtractionResult != null ? battleResult.ExtractionResult.ExtractedMemberIds : null;

            if (deadMemberIds != null)
            {
                for (int i = 0; i < deadMemberIds.Count; i++)
                {
                    BattleEntityId deadMemberId = deadMemberIds[i];
                    string campaignMemberId = ResolveCampaignMemberId(launchPlan, deadMemberId);
                    casualties.Add(new CampaignCasualtySettlement(campaignMemberId, true, true, false));
                }
            }

            for (int i = 0; i < launchPlan.MemberLoadouts.Count; i++)
            {
                MissionMemberLoadout loadout = launchPlan.MemberLoadouts[i];
                bool isDead = ContainsBattleId(deadMemberIds, loadout.BattleMemberId);
                bool isExtracted = ContainsBattleId(extractedMemberIds, loadout.BattleMemberId);

                if (isDead)
                {
                    continue;
                }

                if (isExtracted)
                {
                    casualties.Add(new CampaignCasualtySettlement(loadout.CampaignMemberId, false, false, true));
                    continue;
                }

                casualties.Add(new CampaignCasualtySettlement(loadout.CampaignMemberId, false, true, false));
            }

            return casualties;
        }

        private static List<CampaignBaseEffectSettlement> BuildBaseEffects()
        {
            return new List<CampaignBaseEffectSettlement>();
        }

        private static List<CampaignExperienceSettlement> BuildExperienceSettlements(CampaignState campaignState, MissionLaunchPlan launchPlan, BattleResult battleResult)
        {
            List<CampaignExperienceSettlement> settlements = new List<CampaignExperienceSettlement>();
            if (launchPlan == null || launchPlan.MemberLoadouts == null)
            {
                return settlements;
            }

            int experiencePerSurvivor = GetExperienceReward(battleResult);
            int trainingBonus = HasCapability(campaignState, "training") ? 5 : 0;
            IReadOnlyList<BattleEntityId> extractedMemberIds = battleResult.ExtractionResult != null ? battleResult.ExtractionResult.ExtractedMemberIds : null;
            IReadOnlyList<BattleEntityId> deadMemberIds = battleResult.CasualtyResult != null ? battleResult.CasualtyResult.DeadMemberIds : null;

            for (int i = 0; i < launchPlan.MemberLoadouts.Count; i++)
            {
                MissionMemberLoadout loadout = launchPlan.MemberLoadouts[i];
                if (loadout == null || string.IsNullOrEmpty(loadout.CampaignMemberId))
                {
                    continue;
                }

                if (ContainsBattleId(deadMemberIds, loadout.BattleMemberId))
                {
                    continue;
                }

                if (!ContainsBattleId(extractedMemberIds, loadout.BattleMemberId))
                {
                    continue;
                }

                int reward = experiencePerSurvivor + trainingBonus;
                if (reward <= 0)
                {
                    continue;
                }

                settlements.Add(new CampaignExperienceSettlement(loadout.CampaignMemberId, reward, 1, 0));
            }

            return settlements;
        }

        private static List<CampaignWoundSettlement> BuildWoundSettlements(CampaignState campaignState, MissionLaunchPlan launchPlan, BattleResult battleResult)
        {
            List<CampaignWoundSettlement> settlements = new List<CampaignWoundSettlement>();
            if (launchPlan == null || launchPlan.MemberLoadouts == null)
            {
                return settlements;
            }

            bool missionSucceeded = battleResult != null && battleResult.MissionOutcome == MissionOutcome.Victory;
            IReadOnlyList<BattleEntityId> deadMemberIds = battleResult != null && battleResult.CasualtyResult != null ? battleResult.CasualtyResult.DeadMemberIds : null;
            IReadOnlyList<BattleEntityId> extractedMemberIds = battleResult != null && battleResult.ExtractionResult != null ? battleResult.ExtractionResult.ExtractedMemberIds : null;
            IReadOnlyList<BattleEntityId> woundedMemberIds = battleResult != null && battleResult.WoundResult != null ? battleResult.WoundResult.WoundedMemberIds : null;

            for (int i = 0; i < launchPlan.MemberLoadouts.Count; i++)
            {
                MissionMemberLoadout loadout = launchPlan.MemberLoadouts[i];
                if (loadout == null || string.IsNullOrEmpty(loadout.CampaignMemberId) || ContainsBattleId(deadMemberIds, loadout.BattleMemberId))
                {
                    continue;
                }

                bool isExtracted = ContainsBattleId(extractedMemberIds, loadout.BattleMemberId);
                bool isWounded = ContainsBattleId(woundedMemberIds, loadout.BattleMemberId);

                if (isExtracted)
                {
                    if (isWounded)
                    {
                        settlements.Add(new CampaignWoundSettlement(
                            loadout.CampaignMemberId,
                            missionSucceeded ? WoundSeverity.Light : WoundSeverity.Moderate,
                            missionSucceeded ? 1 : 3,
                            launchPlan.MissionId));
                    }

                    continue;
                }

                settlements.Add(new CampaignWoundSettlement(loadout.CampaignMemberId, WoundSeverity.Severe, 5, launchPlan.MissionId));
            }

            return settlements;
        }

        private static List<CampaignEquipmentSettlement> BuildEquipmentSettlements(CampaignState campaignState, MissionLaunchPlan launchPlan, BattleResult battleResult)
        {
            List<CampaignEquipmentSettlement> settlements = new List<CampaignEquipmentSettlement>();
            if (launchPlan == null || launchPlan.MemberLoadouts == null)
            {
                return settlements;
            }

            bool hasWorkshop = campaignState != null && campaignState.MainBase != null && campaignState.MainBase.HasCapability("workshop");
            IReadOnlyList<BattleEntityId> deadMemberIds = battleResult != null && battleResult.CasualtyResult != null ? battleResult.CasualtyResult.DeadMemberIds : null;
            IReadOnlyList<BattleEntityId> extractedMemberIds = battleResult != null && battleResult.ExtractionResult != null ? battleResult.ExtractionResult.ExtractedMemberIds : null;
            IReadOnlyList<BattleEntityId> woundedMemberIds = battleResult != null && battleResult.WoundResult != null ? battleResult.WoundResult.WoundedMemberIds : null;

            for (int i = 0; i < launchPlan.MemberLoadouts.Count; i++)
            {
                MissionMemberLoadout loadout = launchPlan.MemberLoadouts[i];
                if (loadout == null || string.IsNullOrEmpty(loadout.WeaponInstanceId))
                {
                    continue;
                }

                bool isDead = ContainsBattleId(deadMemberIds, loadout.BattleMemberId);
                bool isExtracted = ContainsBattleId(extractedMemberIds, loadout.BattleMemberId);
                bool isWounded = ContainsBattleId(woundedMemberIds, loadout.BattleMemberId);
                bool shouldReturn = !isDead && isExtracted;
                bool shouldLose = isDead || !isExtracted;
                bool shouldDamage = shouldReturn && isWounded && !hasWorkshop;
                float durability = shouldDamage ? 0.75f : 1f;

                settlements.Add(new CampaignEquipmentSettlement(
                    loadout.CampaignMemberId,
                    loadout.WeaponInstanceId,
                    loadout.WeaponDefinitionId,
                    shouldReturn,
                    shouldLose,
                    shouldDamage,
                    durability,
                    launchPlan.MissionId));
            }

            return settlements;
        }

        private static bool HasCompletedObjective(IReadOnlyList<BattleObjectiveResult> objectiveResults, MissionObjectiveType objectiveType)
        {
            if (objectiveResults == null)
            {
                return false;
            }

            for (int i = 0; i < objectiveResults.Count; i++)
            {
                BattleObjectiveResult objective = objectiveResults[i];
                if (objective != null && objective.ObjectiveType == objectiveType && objective.IsCompleted)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsSquadAvailable(MissionLaunchPlan launchPlan, IReadOnlyList<CampaignCasualtySettlement> casualties, int squadId)
        {
            if (launchPlan == null || casualties == null || launchPlan.MemberLoadouts == null)
            {
                return false;
            }

            for (int i = 0; i < launchPlan.MemberLoadouts.Count; i++)
            {
                MissionMemberLoadout memberLoadout = launchPlan.MemberLoadouts[i];
                if (memberLoadout == null || memberLoadout.SquadId != squadId)
                {
                    continue;
                }

                CampaignCasualtySettlement casualty = FindCasualty(casualties, memberLoadout.CampaignMemberId);
                if (casualty != null && casualty.IsAvailable)
                {
                    return true;
                }
            }

            return false;
        }

        private static Dictionary<string, int> BuildResourceSummary(IReadOnlyList<CampaignResourceRewardSettlement> resourceRewards)
        {
            Dictionary<string, int> summary = new Dictionary<string, int>();
            if (resourceRewards == null)
            {
                return summary;
            }

            for (int i = 0; i < resourceRewards.Count; i++)
            {
                CampaignResourceRewardSettlement reward = resourceRewards[i];
                if (reward == null || string.IsNullOrEmpty(reward.ResourceId) || reward.Count <= 0)
                {
                    continue;
                }

                int current;
                summary.TryGetValue(reward.ResourceId, out current);
                summary[reward.ResourceId] = current + reward.Count;
            }

            return summary;
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

        private static int GetExperienceReward(BattleResult battleResult)
        {
            if (battleResult == null)
            {
                return 0;
            }

            switch (battleResult.CompletionType)
            {
                case BattleCompletionType.Success:
                    return 20;
                case BattleCompletionType.Partial:
                    return 10;
                case BattleCompletionType.Failure:
                    return 5;
                default:
                    return 10;
            }
        }

        private static bool HasCapability(CampaignState campaignState, string capabilityId)
        {
            return campaignState != null && campaignState.MainBase != null && campaignState.MainBase.HasCapability(capabilityId);
        }

        private static CampaignCasualtySettlement FindCasualty(IReadOnlyList<CampaignCasualtySettlement> casualties, string campaignMemberId)
        {
            if (casualties == null || string.IsNullOrEmpty(campaignMemberId))
            {
                return null;
            }

            for (int i = 0; i < casualties.Count; i++)
            {
                CampaignCasualtySettlement casualty = casualties[i];
                if (casualty != null && casualty.CampaignMemberId == campaignMemberId)
                {
                    return casualty;
                }
            }

            return null;
        }

        private static string ResolveCampaignMemberId(MissionLaunchPlan launchPlan, BattleEntityId battleMemberId)
        {
            string campaignMemberId;
            if (launchPlan.BattleMemberIdToCampaignMemberId.TryGetValue(battleMemberId.Value, out campaignMemberId))
            {
                return campaignMemberId;
            }

            return battleMemberId.Value.ToString();
        }
    }
}
