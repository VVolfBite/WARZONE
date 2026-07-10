namespace Warzone.Campaign
{
    public sealed class CampaignWoundSettlement
    {
        public CampaignWoundSettlement(string campaignMemberId, WoundSeverity woundSeverity, int recoveryDaysRemaining, string missionId = null)
        {
            CampaignMemberId = campaignMemberId;
            WoundSeverity = woundSeverity;
            RecoveryDaysRemaining = recoveryDaysRemaining;
            MissionId = missionId;
        }

        public string CampaignMemberId { get; private set; }
        public WoundSeverity WoundSeverity { get; private set; }
        public int RecoveryDaysRemaining { get; private set; }
        public string MissionId { get; private set; }
    }
}
