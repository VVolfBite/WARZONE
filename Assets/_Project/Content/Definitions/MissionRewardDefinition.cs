using System.Collections.Generic;

namespace Warzone.Content.Definitions
{
    public sealed class MissionRewardDefinition
    {
        public MissionRewardDefinition(
            int genericLootCount = 0,
            int experienceReward = 0,
            string rewardProfileId = null,
            LootRewardDefinition lootReward = null,
            string lootProfileId = null)
        {
            GenericLootCount = genericLootCount;
            ExperienceReward = experienceReward;
            RewardProfileId = rewardProfileId;
            LootProfileId = string.IsNullOrEmpty(lootProfileId) ? rewardProfileId : lootProfileId;
            LootReward = lootReward ?? new LootRewardDefinition();
        }

        public int GenericLootCount { get; private set; }
        public int ExperienceReward { get; private set; }
        public string RewardProfileId { get; private set; }
        public string LootProfileId { get; private set; }
        public LootRewardDefinition LootReward { get; private set; }
    }
}
