using System.Collections.Generic;
using Warzone.Campaign;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Application.Services
{
    public sealed class MissionRewardResolver
    {
        private readonly ContentCatalog _contentCatalog;

        public MissionRewardResolver()
            : this(null)
        {
        }

        public MissionRewardResolver(ContentCatalog contentCatalog)
        {
            _contentCatalog = contentCatalog;
        }

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

            int lootCount = battleResult.LootResult != null ? battleResult.LootResult.LootCount : 0;
            if (lootCount <= 0)
            {
                return rewards;
            }

            if (AddExplicitResourceRewards(rewards, missionReward.LootReward, launchPlan.MissionId))
            {
                AddGenericLootReward(rewards, missionReward.GenericLootCount, launchPlan.MissionId);
                return rewards;
            }

            LootProfileDefinition lootProfile = ResolveLootProfile(launchPlan);
            if (lootProfile != null)
            {
                AddProfileRewards(rewards, lootProfile, lootCount, launchPlan.MissionId);
                AddGenericLootReward(rewards, missionReward.GenericLootCount, launchPlan.MissionId);
                return rewards;
            }

            AddFallbackProfileRewards(rewards, ResolveFallbackProfile(launchPlan, missionReward), lootCount, launchPlan.MissionId);
            AddGenericLootReward(rewards, missionReward.GenericLootCount, launchPlan.MissionId);
            return rewards;
        }

        public List<CampaignItemRewardSettlement> ResolveItemRewards(MissionLaunchPlan launchPlan, BattleResult battleResult)
        {
            List<CampaignItemRewardSettlement> rewards = new List<CampaignItemRewardSettlement>();
            if (launchPlan == null || battleResult == null || launchPlan.MissionDefinition == null || launchPlan.MissionDefinition.Reward == null)
            {
                return rewards;
            }

            int lootCount = battleResult.LootResult != null ? battleResult.LootResult.LootCount : 0;
            if (lootCount <= 0)
            {
                return rewards;
            }

            LootRewardDefinition lootReward = launchPlan.MissionDefinition.Reward.LootReward;
            if (lootReward != null && lootReward.ItemRewards != null && lootReward.ItemRewards.Count > 0)
            {
                AddItemRewards(rewards, lootReward.ItemRewards);
                return rewards;
            }

            LootProfileDefinition lootProfile = ResolveLootProfile(launchPlan);
            if (lootProfile != null && lootProfile.ItemRewards.Count > 0)
            {
                AddItemRewards(rewards, lootProfile.ItemRewards);
            }

            return rewards;
        }

        public List<CampaignWeaponRewardSettlement> ResolveWeaponRewards(MissionLaunchPlan launchPlan, BattleResult battleResult)
        {
            List<CampaignWeaponRewardSettlement> rewards = new List<CampaignWeaponRewardSettlement>();
            if (launchPlan == null || battleResult == null || launchPlan.MissionDefinition == null || launchPlan.MissionDefinition.Reward == null)
            {
                return rewards;
            }

            int lootCount = battleResult.LootResult != null ? battleResult.LootResult.LootCount : 0;
            if (lootCount <= 0)
            {
                return rewards;
            }

            LootRewardDefinition lootReward = launchPlan.MissionDefinition.Reward.LootReward;
            if (lootReward != null && lootReward.WeaponRewards != null && lootReward.WeaponRewards.Count > 0)
            {
                AddWeaponRewards(rewards, lootReward.WeaponRewards);
                return rewards;
            }

            LootProfileDefinition lootProfile = ResolveLootProfile(launchPlan);
            if (lootProfile != null && lootProfile.WeaponRewards.Count > 0)
            {
                AddWeaponRewards(rewards, lootProfile.WeaponRewards);
            }

            return rewards;
        }

        private LootProfileDefinition ResolveLootProfile(MissionLaunchPlan launchPlan)
        {
            if (_contentCatalog == null || launchPlan == null)
            {
                return null;
            }

            string lootProfileId = launchPlan.LootProfileId;
            if (string.IsNullOrEmpty(lootProfileId) && launchPlan.MissionDefinition != null && launchPlan.MissionDefinition.Reward != null)
            {
                lootProfileId = launchPlan.MissionDefinition.Reward.LootProfileId;
                if (string.IsNullOrEmpty(lootProfileId))
                {
                    lootProfileId = launchPlan.MissionDefinition.Reward.RewardProfileId;
                }
            }

            if (string.IsNullOrEmpty(lootProfileId))
            {
                return null;
            }

            LootProfileDefinition lootProfile;
            if (_contentCatalog.TryGetLootProfile(lootProfileId, out lootProfile))
            {
                return lootProfile;
            }

            return null;
        }

        private static string ResolveFallbackProfile(MissionLaunchPlan launchPlan, MissionRewardDefinition missionReward)
        {
            if (missionReward != null && !string.IsNullOrEmpty(missionReward.RewardProfileId))
            {
                return missionReward.RewardProfileId;
            }

            if (launchPlan != null && launchPlan.SiteContext != null)
            {
                return launchPlan.SiteContext.SiteType.ToString().ToLowerInvariant();
            }

            return "generic";
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

        private static void AddProfileRewards(List<CampaignResourceRewardSettlement> rewards, LootProfileDefinition profile, int totalLoot, string sourceId)
        {
            if (profile == null || totalLoot <= 0)
            {
                return;
            }

            if (profile.ResourceRewards.Count == 0)
            {
                if (!string.IsNullOrEmpty(profile.FallbackResourceId))
                {
                    rewards.Add(new CampaignResourceRewardSettlement(profile.FallbackResourceId, totalLoot, sourceId));
                }

                return;
            }

            int totalWeight = 0;
            for (int i = 0; i < profile.ResourceRewards.Count; i++)
            {
                ResourceRewardDefinition reward = profile.ResourceRewards[i];
                if (reward != null && !string.IsNullOrEmpty(reward.ResourceId) && reward.Amount > 0)
                {
                    totalWeight += reward.Amount;
                }
            }

            if (totalWeight <= 0)
            {
                string fallbackResourceId = !string.IsNullOrEmpty(profile.FallbackResourceId) ? profile.FallbackResourceId : "generic_loot";
                rewards.Add(new CampaignResourceRewardSettlement(fallbackResourceId, totalLoot, sourceId));
                return;
            }

            int remainingLoot = totalLoot;
            for (int i = 0; i < profile.ResourceRewards.Count; i++)
            {
                ResourceRewardDefinition reward = profile.ResourceRewards[i];
                if (reward == null || string.IsNullOrEmpty(reward.ResourceId) || reward.Amount <= 0)
                {
                    continue;
                }

                int rewardCount = i == profile.ResourceRewards.Count - 1
                    ? remainingLoot
                    : (totalLoot * reward.Amount) / totalWeight;

                if (rewardCount <= 0)
                {
                    continue;
                }

                rewards.Add(new CampaignResourceRewardSettlement(reward.ResourceId, rewardCount, sourceId));
                remainingLoot -= rewardCount;
            }

            if (remainingLoot > 0)
            {
                string fallbackResourceId = !string.IsNullOrEmpty(profile.FallbackResourceId) ? profile.FallbackResourceId : null;
                if (string.IsNullOrEmpty(fallbackResourceId))
                {
                    for (int i = 0; i < profile.ResourceRewards.Count; i++)
                    {
                        ResourceRewardDefinition reward = profile.ResourceRewards[i];
                        if (reward != null && !string.IsNullOrEmpty(reward.ResourceId))
                        {
                            fallbackResourceId = reward.ResourceId;
                            break;
                        }
                    }
                }

                if (string.IsNullOrEmpty(fallbackResourceId))
                {
                    fallbackResourceId = "generic_loot";
                }

                rewards.Add(new CampaignResourceRewardSettlement(fallbackResourceId, remainingLoot, sourceId));
            }
        }

        private static void AddFallbackProfileRewards(List<CampaignResourceRewardSettlement> rewards, string profileName, int totalLoot, string sourceId)
        {
            if (totalLoot <= 0)
            {
                return;
            }

            string normalizedProfile = profileName != null ? profileName.ToLowerInvariant() : string.Empty;
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

        private static void AddItemRewards(List<CampaignItemRewardSettlement> rewards, IReadOnlyList<ItemRewardDefinition> itemRewards)
        {
            for (int i = 0; i < itemRewards.Count; i++)
            {
                ItemRewardDefinition itemReward = itemRewards[i];
                if (itemReward == null || string.IsNullOrEmpty(itemReward.ItemId) || itemReward.Count <= 0)
                {
                    continue;
                }

                rewards.Add(new CampaignItemRewardSettlement(itemReward.ItemId, itemReward.ItemId, itemReward.Count));
            }
        }

        private static void AddWeaponRewards(List<CampaignWeaponRewardSettlement> rewards, IReadOnlyList<WeaponRewardDefinition> weaponRewards)
        {
            for (int i = 0; i < weaponRewards.Count; i++)
            {
                WeaponRewardDefinition weaponReward = weaponRewards[i];
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
        }
    }
}
