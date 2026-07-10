using Warzone.Campaign;

namespace Warzone.Application.Services
{
    public sealed class WorldProgressionService
    {
        private readonly CampaignTimeSystem _timeSystem = new CampaignTimeSystem();
        private readonly CampaignWorldTickSystem _worldTickSystem = new CampaignWorldTickSystem();

        public void AdvanceHours(CampaignState campaignState, float hours)
        {
            _timeSystem.AdvanceHours(campaignState, hours);
        }

        public void AdvanceDays(CampaignState campaignState, int days)
        {
            if (campaignState == null || days <= 0)
            {
                return;
            }

            for (int i = 0; i < days; i++)
            {
                _timeSystem.AdvanceDays(campaignState, 1);
                _worldTickSystem.ApplyDailyTick(campaignState);
            }
        }

        public void ApplyDailyTick(CampaignState campaignState)
        {
            _worldTickSystem.ApplyDailyTick(campaignState);
        }

        public void ApplyHourlyTick(CampaignState campaignState)
        {
            _worldTickSystem.ApplyHourlyTick(campaignState);
        }
    }
}
