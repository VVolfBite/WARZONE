using System.Collections.Generic;
using Warzone.Campaign;
using Warzone.Combat;
using Warzone.Content;

namespace Warzone.Application.Services
{
    public sealed class MissionPreparationService
    {
        private readonly MissionLaunchPlanFactory _launchPlanFactory;
        private readonly BattleStateFromMissionFactory _battleStateFromMissionFactory;

        public MissionPreparationService(ContentCatalog contentCatalog)
        {
            _launchPlanFactory = new MissionLaunchPlanFactory(contentCatalog);
            _battleStateFromMissionFactory = new BattleStateFromMissionFactory();
        }

        public bool TryCreateLaunchPlan(
            CampaignState campaignState,
            string missionId,
            string siteId,
            IReadOnlyList<int> selectedSquadIds,
            int randomSeed,
            out MissionLaunchPlan launchPlan,
            out string reason)
        {
            return _launchPlanFactory.TryCreateLaunchPlan(campaignState, missionId, siteId, selectedSquadIds, randomSeed, out launchPlan, out reason);
        }

        public bool TryPrepareBattle(
            CampaignState campaignState,
            string missionId,
            string siteId,
            IReadOnlyList<int> selectedSquadIds,
            int randomSeed,
            out MissionLaunchPlan launchPlan,
            out BattleState battleState,
            out string reason)
        {
            battleState = null;
            if (!_launchPlanFactory.TryCreateLaunchPlan(campaignState, missionId, siteId, selectedSquadIds, randomSeed, out launchPlan, out reason))
            {
                return false;
            }

            battleState = _battleStateFromMissionFactory.Create(launchPlan);
            return battleState != null;
        }
    }
}
