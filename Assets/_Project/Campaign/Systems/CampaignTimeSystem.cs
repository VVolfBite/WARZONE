namespace Warzone.Campaign
{
    public sealed class CampaignTimeSystem
    {
        public void AdvanceHours(CampaignState campaignState, float hours)
        {
            if (campaignState == null || hours <= 0f)
            {
                return;
            }

            campaignState.AdvanceCampaignTime(hours);
        }

        public void AdvanceDays(CampaignState campaignState, int days)
        {
            if (campaignState == null || days <= 0)
            {
                return;
            }

            campaignState.AdvanceCampaignTime(days * 24f);
        }
    }
}
