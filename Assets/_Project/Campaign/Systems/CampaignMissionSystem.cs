using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignMissionSystem
    {
        public bool CanLaunchMission(
            CampaignState campaignState,
            string missionId,
            string siteId,
            IReadOnlyList<int> squadIds,
            out string reason)
        {
            reason = null;
            if (campaignState == null)
            {
                reason = "Missing campaign state.";
                return false;
            }

            if (string.IsNullOrEmpty(missionId))
            {
                reason = "Missing mission id.";
                return false;
            }

            if (string.IsNullOrEmpty(siteId))
            {
                reason = "Missing site id.";
                return false;
            }

            CampaignSiteState site;
            if (!campaignState.TryGetSite(siteId, out site))
            {
                reason = "Site not found.";
                return false;
            }

            if (!site.IsDiscovered)
            {
                reason = "Site is not discovered.";
                return false;
            }

            if (squadIds == null || squadIds.Count == 0)
            {
                reason = "No squad selected.";
                return false;
            }

            for (int i = 0; i < squadIds.Count; i++)
            {
                CampaignSquadState squad;
                if (!campaignState.Roster.TryGetSquad(squadIds[i], out squad))
                {
                    reason = "Squad not found.";
                    return false;
                }

                if (!squad.IsAvailable)
                {
                    reason = "Squad is not available.";
                    return false;
                }

                for (int j = 0; j < squad.MemberIds.Count; j++)
                {
                    CampaignMemberState member;
                    if (!campaignState.Roster.TryGetMember(squad.MemberIds[j], out member) || !member.IsAlive || !member.IsAvailable)
                    {
                        reason = "Squad has unavailable members.";
                        return false;
                    }
                }
            }

            return true;
        }

        public void RegisterMission(CampaignState campaignState, CampaignMissionState missionState)
        {
            if (campaignState == null || missionState == null)
            {
                return;
            }

            campaignState.SetCurrentMission(missionState);
        }

        public void CompleteMission(CampaignState campaignState, string missionId)
        {
            if (campaignState == null || string.IsNullOrEmpty(missionId))
            {
                return;
            }

            if (campaignState.CurrentMission != null && campaignState.CurrentMission.MissionId == missionId)
            {
                campaignState.SetCurrentMission(null);
            }
        }
    }
}
