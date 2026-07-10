using Warzone.Campaign;
using Warzone.Combat;
using Warzone.Application.Services;

namespace Warzone.Application.Flows
{
    public sealed class MissionSettlementFlow
    {
        private readonly MissionSettlementService _service;

        public MissionSettlementFlow(MissionSettlementService service)
        {
            _service = service;
        }

        public CampaignSettlement ApplyBattleResult(
            CampaignState campaignState,
            MissionLaunchPlan launchPlan,
            BattleResult battleResult)
        {
            return _service.ApplyBattleResult(campaignState, launchPlan, battleResult);
        }
    }
}
