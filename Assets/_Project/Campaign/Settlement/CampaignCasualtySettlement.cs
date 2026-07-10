namespace Warzone.Campaign
{
    public sealed class CampaignCasualtySettlement
    {
        public CampaignCasualtySettlement(
            string campaignMemberId,
            bool isDead,
            bool isWounded,
            bool isAvailable)
        {
            CampaignMemberId = campaignMemberId;
            IsDead = isDead;
            IsWounded = isWounded;
            IsAvailable = isAvailable;
        }

        public string CampaignMemberId { get; private set; }
        public bool IsDead { get; private set; }
        public bool IsWounded { get; private set; }
        public bool IsAvailable { get; private set; }
    }
}
