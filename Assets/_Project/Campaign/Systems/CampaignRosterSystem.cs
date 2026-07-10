namespace Warzone.Campaign
{
    public sealed class CampaignRosterSystem
    {
        public bool TryGetMember(CampaignState campaignState, string memberId, out CampaignMemberState member)
        {
            member = null;
            if (campaignState == null || campaignState.Roster == null)
            {
                return false;
            }

            return campaignState.Roster.TryGetMember(memberId, out member);
        }

        public bool TryGetSquad(CampaignState campaignState, int squadId, out CampaignSquadState squad)
        {
            squad = null;
            if (campaignState == null || campaignState.Roster == null)
            {
                return false;
            }

            return campaignState.Roster.TryGetSquad(squadId, out squad);
        }

        public void MarkMemberDead(CampaignState campaignState, string memberId)
        {
            CampaignMemberState member;
            if (TryGetMember(campaignState, memberId, out member))
            {
                member.MarkDead();
                member.SetAvailable(false);
            }
        }

        public void MarkMemberWounded(CampaignState campaignState, string memberId)
        {
            CampaignMemberState member;
            if (TryGetMember(campaignState, memberId, out member))
            {
                member.MarkWounded();
            }
        }

        public void MarkMemberAvailable(CampaignState campaignState, string memberId)
        {
            CampaignMemberState member;
            if (TryGetMember(campaignState, memberId, out member))
            {
                member.SetAvailable(true);
            }
        }
    }
}
