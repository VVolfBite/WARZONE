using Warzone.Campaign;

namespace Warzone.Application.Services
{
    public sealed class OutpostManagementService
    {
        private readonly CampaignOutpostSystem _outpostSystem = new CampaignOutpostSystem();

        public bool CanEstablishOutpost(CampaignState campaignState, string siteId)
        {
            return _outpostSystem.CanEstablishOutpost(campaignState, siteId);
        }

        public bool EstablishOutpost(CampaignState campaignState, string siteId, string outpostId)
        {
            return _outpostSystem.EstablishOutpost(campaignState, siteId, outpostId);
        }

        public bool AbandonOutpost(CampaignState campaignState, string outpostId)
        {
            return _outpostSystem.AbandonOutpost(campaignState, outpostId);
        }
    }
}
