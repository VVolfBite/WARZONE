using System.Collections.Generic;
using Warzone.Campaign;
using Warzone.Combat;
using Warzone.Application.Services;

namespace Warzone.Application.Flows
{
    public sealed class MissionPreparationFlow
    {
        private readonly MissionPreparationService _service;

        public MissionPreparationFlow(MissionPreparationService service)
        {
            _service = service;
        }

        public bool TryPrepare(
            CampaignState campaignState,
            string missionId,
            string siteId,
            IReadOnlyList<int> selectedSquadIds,
            int randomSeed,
            out MissionLaunchPlan launchPlan,
            out BattleState battleState,
            out string reason)
        {
            return _service.TryPrepareBattle(campaignState, missionId, siteId, selectedSquadIds, randomSeed, out launchPlan, out battleState, out reason);
        }
    }
}
