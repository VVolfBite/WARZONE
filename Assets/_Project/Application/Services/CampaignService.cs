using Warzone.Campaign;

namespace Warzone.Application.Services
{
    public sealed class CampaignService
    {
        private readonly CampaignSettlementSystem _settlementSystem = new CampaignSettlementSystem();
        private readonly StartingCampaignFactory _startingCampaignFactory = new StartingCampaignFactory();

        public CampaignService()
        {
            CampaignState = CreateNewCampaign();
        }

        public CampaignState CampaignState { get; private set; }

        public CampaignState CreateNewCampaign()
        {
            CampaignState = _startingCampaignFactory.CreateStartingCampaign();
            return CampaignState;
        }

        public void ApplySettlement(CampaignSettlement settlement)
        {
            _settlementSystem.Apply(CampaignState, settlement);
        }
    }
}
