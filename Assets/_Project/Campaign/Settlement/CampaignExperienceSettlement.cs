namespace Warzone.Campaign
{
    public sealed class CampaignExperienceSettlement
    {
        public CampaignExperienceSettlement(string campaignMemberId, int experienceGained, int missionsCompletedGained = 0, int killsGained = 0)
        {
            CampaignMemberId = campaignMemberId;
            ExperienceGained = experienceGained;
            MissionsCompletedGained = missionsCompletedGained;
            KillsGained = killsGained;
        }

        public string CampaignMemberId { get; private set; }
        public int ExperienceGained { get; private set; }
        public int MissionsCompletedGained { get; private set; }
        public int KillsGained { get; private set; }
    }
}
