using System.Collections.Generic;
using Warzone.Campaign;
using Warzone.Combat;
using Warzone.Content.Definitions;

namespace Warzone.Application.Services
{
    public sealed class MissionSettlementService
    {
        private readonly CampaignSettlementSystem _settlementSystem = new CampaignSettlementSystem();

        public CampaignSettlement ApplyBattleResult(
            CampaignState campaignState,
            MissionLaunchPlan launchPlan,
            BattleResult battleResult)
        {
            if (campaignState == null || launchPlan == null || battleResult == null)
            {
                return null;
            }

            List<CampaignResourceRewardSettlement> resourceRewards = BuildResourceRewards(launchPlan, battleResult);
            List<CampaignItemRewardSettlement> itemRewards = BuildItemRewards(launchPlan);
            List<CampaignWeaponRewardSettlement> weaponRewards = BuildWeaponRewards(launchPlan);
            List<CampaignBaseEffectSettlement> baseEffects = BuildBaseEffects();
            List<CampaignCasualtySettlement> casualties = BuildCasualties(launchPlan, battleResult);

            CampaignSiteState siteState;
            campaignState.TryGetSite(launchPlan.SiteId, out siteState);

            bool searchCompleted = HasCompletedObjective(battleResult.ObjectiveResults, MissionObjectiveType.SearchPoint);
            bool missionSucceeded = battleResult.MissionOutcome == MissionOutcome.Victory;

            List<CampaignSiteSettlement> siteSettlements = new List<CampaignSiteSettlement>
            {
                new CampaignSiteSettlement(
                    launchPlan.SiteId,
                    searchCompleted,
                    missionSucceeded,
                    siteState != null ? (missionSucceeded ? 0 : siteState.ThreatLevel) : 0,
                    battleResult.ElapsedTimeSeconds)
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
            int lootCount = battleResult.LootResult != null ? battleResult.LootResult.LootCount : 0;

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

        private static List<CampaignResourceRewardSettlement> BuildResourceRewards(MissionLaunchPlan launchPlan, BattleResult battleResult)
        {
            List<CampaignResourceRewardSettlement> rewards = new List<CampaignResourceRewardSettlement>();
            if (launchPlan == null || battleResult == null)
            {
                return rewards;
            }

            int lootCount = battleResult.LootResult != null ? battleResult.LootResult.LootCount : 0;

            bool hasExplicitResourceRewards = false;
            if (launchPlan.MissionDefinition != null && launchPlan.MissionDefinition.Reward != null)
            {
                MissionRewardDefinition missionReward = launchPlan.MissionDefinition.Reward;
                if (missionReward.LootReward != null && missionReward.LootReward.ResourceRewards != null && missionReward.LootReward.ResourceRewards.Count > 0)
                {
                    for (int i = 0; i < missionReward.LootReward.ResourceRewards.Count; i++)
                    {
                        ResourceRewardDefinition resourceReward = missionReward.LootReward.ResourceRewards[i];
                        if (resourceReward == null || string.IsNullOrEmpty(resourceReward.ResourceId) || resourceReward.Amount <= 0)
                        {
                            continue;
                        }

                        rewards.Add(new CampaignResourceRewardSettlement(resourceReward.ResourceId, resourceReward.Amount, launchPlan.MissionId));
                    }

                    hasExplicitResourceRewards = true;
                }

                if (missionReward.GenericLootCount > 0)
                {
                    rewards.Add(new CampaignResourceRewardSettlement("generic_loot", missionReward.GenericLootCount, launchPlan.MissionId));
                }
            }

            if (!hasExplicitResourceRewards && lootCount > 0)
            {
                string profile = launchPlan.MissionDefinition != null && launchPlan.MissionDefinition.Reward != null && !string.IsNullOrEmpty(launchPlan.MissionDefinition.Reward.RewardProfileId)
                    ? launchPlan.MissionDefinition.Reward.RewardProfileId
                    : launchPlan.SiteContext != null ? launchPlan.SiteContext.SiteType.ToString() : "generic";
                AddProfileRewards(rewards, profile, lootCount, launchPlan.MissionId);
            }

            return rewards;
        }

        private static void AddProfileRewards(List<CampaignResourceRewardSettlement> rewards, string profile, int totalLoot, string sourceId)
        {
            if (totalLoot <= 0)
            {
                return;
            }

            string normalizedProfile = profile != null ? profile.ToLowerInvariant() : string.Empty;
            if (normalizedProfile.Contains("medical"))
            {
                rewards.Add(new CampaignResourceRewardSettlement("medicine", totalLoot, sourceId));
                return;
            }

            if (normalizedProfile.Contains("supply"))
            {
                int ammo = totalLoot / 2 + totalLoot % 2;
                int food = totalLoot / 2;
                if (ammo > 0)
                {
                    rewards.Add(new CampaignResourceRewardSettlement("ammo", ammo, sourceId));
                }

                if (food > 0)
                {
                    rewards.Add(new CampaignResourceRewardSettlement("food", food, sourceId));
                }

                return;
            }

            if (normalizedProfile.Contains("fuel"))
            {
                rewards.Add(new CampaignResourceRewardSettlement("fuel", totalLoot, sourceId));
                return;
            }

            if (normalizedProfile.Contains("construction") || normalizedProfile.Contains("compound"))
            {
                rewards.Add(new CampaignResourceRewardSettlement("building_material", totalLoot, sourceId));
                return;
            }

            rewards.Add(new CampaignResourceRewardSettlement("generic_loot", totalLoot, sourceId));
        }

        private static List<CampaignItemRewardSettlement> BuildItemRewards(MissionLaunchPlan launchPlan)
        {
            List<CampaignItemRewardSettlement> rewards = new List<CampaignItemRewardSettlement>();
            if (launchPlan == null || launchPlan.MissionDefinition == null || launchPlan.MissionDefinition.Reward == null)
            {
                return rewards;
            }

            LootRewardDefinition lootReward = launchPlan.MissionDefinition.Reward.LootReward;
            if (lootReward == null || lootReward.ItemRewards == null)
            {
                return rewards;
            }

            for (int i = 0; i < lootReward.ItemRewards.Count; i++)
            {
                ItemRewardDefinition itemReward = lootReward.ItemRewards[i];
                if (itemReward == null || string.IsNullOrEmpty(itemReward.ItemId) || itemReward.Count <= 0)
                {
                    continue;
                }

                rewards.Add(new CampaignItemRewardSettlement(itemReward.ItemId, itemReward.ItemId, itemReward.Count));
            }

            return rewards;
        }

        private static List<CampaignWeaponRewardSettlement> BuildWeaponRewards(MissionLaunchPlan launchPlan)
        {
            List<CampaignWeaponRewardSettlement> rewards = new List<CampaignWeaponRewardSettlement>();
            if (launchPlan == null || launchPlan.MissionDefinition == null || launchPlan.MissionDefinition.Reward == null)
            {
                return rewards;
            }

            LootRewardDefinition lootReward = launchPlan.MissionDefinition.Reward.LootReward;
            if (lootReward == null || lootReward.WeaponRewards == null)
            {
                return rewards;
            }

            for (int i = 0; i < lootReward.WeaponRewards.Count; i++)
            {
                WeaponRewardDefinition weaponReward = lootReward.WeaponRewards[i];
                if (weaponReward == null || string.IsNullOrEmpty(weaponReward.WeaponDefinitionId) || weaponReward.Count <= 0)
                {
                    continue;
                }

                for (int j = 0; j < weaponReward.Count; j++)
                {
                    rewards.Add(new CampaignWeaponRewardSettlement(
                        weaponReward.WeaponDefinitionId + ".reward." + j,
                        weaponReward.WeaponDefinitionId,
                        null,
                        false));
                }
            }

            return rewards;
        }

        private static List<CampaignBaseEffectSettlement> BuildBaseEffects()
        {
            return new List<CampaignBaseEffectSettlement>();
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
