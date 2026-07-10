namespace Warzone.Content.Definitions
{
    public sealed class MissionRewardDefinition
    {
        public MissionRewardDefinition(int genericLootCount = 0, int experienceReward = 0, string rewardProfileId = null)
        {
            GenericLootCount = genericLootCount;
            ExperienceReward = experienceReward;
            RewardProfileId = rewardProfileId;
        }

        public int GenericLootCount { get; private set; }
        public int ExperienceReward { get; private set; }
        public string RewardProfileId { get; private set; }
    }
}
