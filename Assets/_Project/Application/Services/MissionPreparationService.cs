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
        private readonly CampaignMissionSystem _campaignMissionSystem = new CampaignMissionSystem();

        public MissionPreparationService(ContentCatalog contentCatalog)
        {
            _launchPlanFactory = new MissionLaunchPlanFactory(contentCatalog);
            _battleStateFromMissionFactory = new BattleStateFromMissionFactory(contentCatalog);
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

            StartMission(campaignState, launchPlan);
            battleState = _battleStateFromMissionFactory.Create(launchPlan);
            return battleState != null;
        }

        public void StartMission(CampaignState campaignState, MissionLaunchPlan launchPlan)
        {
            if (campaignState == null || launchPlan == null)
            {
                return;
            }

            if (launchPlan.MemberLoadouts != null)
            {
                for (int i = 0; i < launchPlan.MemberLoadouts.Count; i++)
                {
                    MissionMemberLoadout loadout = launchPlan.MemberLoadouts[i];
                    if (loadout == null || string.IsNullOrEmpty(loadout.WeaponInstanceId))
                    {
                        continue;
                    }

                    CampaignWeaponInstanceState weaponInstance;
                    if (campaignState.Inventory.TryGetWeaponInstance(loadout.WeaponInstanceId, out weaponInstance))
                    {
                        weaponInstance.MarkDeployed(launchPlan.MissionId);
                    }
                }
            }

            _campaignMissionSystem.RegisterMission(
                campaignState,
                new CampaignMissionState(
                    launchPlan.MissionId,
                    launchPlan.SiteId,
                    launchPlan.MissionDefinition != null ? launchPlan.MissionDefinition.DisplayName : launchPlan.MissionId,
                    true,
                    launchPlan.SiteContext != null ? launchPlan.SiteContext.ThreatLevel : 0,
                    launchPlan.MissionDefinition != null ? launchPlan.MissionDefinition.MissionType.ToString() : null));
        }
    }
}
