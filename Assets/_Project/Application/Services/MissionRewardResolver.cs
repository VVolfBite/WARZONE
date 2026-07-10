using System.Collections.Generic;
using Warzone.Campaign;
using Warzone.Combat;
using Warzone.Content.Definitions;

namespace Warzone.Application.Services
{
    public sealed class MissionRewardResolver
    {
        public List<CampaignResourceRewardSettlement> ResolveResourceRewards(MissionLaunchPlan launchPlan, BattleResult battleResult)
        {
            List<CampaignResourceRewardSettlement> rewards = new List<CampaignResourceRewardSettlement>();
            if (launchPlan == null || battleResult == null || launchPlan.MissionDefinition == null)
            {
                return rewards;
            }

            MissionRewardDefinition missionReward = launchPlan.MissionDefinition.Reward;
            if (missionReward == null)
            {
                return rewards;
            }

            bool hasExplicitResourceRewards = AddExplicitResourceRewards(rewards, missionReward.LootReward, launchPlan.MissionId);
            AddGenericLootReward(rewards, missionReward.GenericLootCount, launchPlan.MissionId);

            if (hasExplicitResourceRewards)
            {
                return rewards;
            }

            int lootCount = battleResult.LootResult != null ? battleResult.LootResult.LootCount : 0;
            if (lootCount <= 0)
            {
                return rewards;
            }

            string profile = missionReward.RewardProfileId;
            if (string.IsNullOrEmpty(profile))
            {
                profile = launchPlan.SiteContext != null ? launchPlan.SiteContext.SiteType.ToString() : "generic";
            }

            AddProfileRewards(rewards, profile, lootCount, launchPlan.MissionId);
            return rewards;
        }

        public List<CampaignItemRewardSettlement> ResolveItemRewards(MissionLaunchPlan launchPlan)
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

        public List<CampaignWeaponRewardSettlement> ResolveWeaponRewards(MissionLaunchPlan launchPlan)
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

        private static bool AddExplicitResourceRewards(List<CampaignResourceRewardSettlement> rewards, LootRewardDefinition lootReward, string sourceId)
        {
            if (lootReward == null || lootReward.ResourceRewards == null || lootReward.ResourceRewards.Count == 0)
            {
                return false;
            }

            bool hasExplicit = false;
            for (int i = 0; i < lootReward.ResourceRewards.Count; i++)
            {
                ResourceRewardDefinition resourceReward = lootReward.ResourceRewards[i];
                if (resourceReward == null || string.IsNullOrEmpty(resourceReward.ResourceId) || resourceReward.Amount <= 0)
                {
                    continue;
                }

                rewards.Add(new CampaignResourceRewardSettlement(resourceReward.ResourceId, resourceReward.Amount, sourceId));
                hasExplicit = true;
            }

            return hasExplicit;
        }

        private static void AddGenericLootReward(List<CampaignResourceRewardSettlement> rewards, int count, string sourceId)
        {
            if (count <= 0)
            {
                return;
            }

            rewards.Add(new CampaignResourceRewardSettlement("generic_loot", count, sourceId));
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
    }
}
