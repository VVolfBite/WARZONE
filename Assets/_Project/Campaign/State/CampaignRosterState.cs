using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignRosterState
    {
        private readonly Dictionary<string, CampaignMemberState> _membersById = new Dictionary<string, CampaignMemberState>();
        private readonly Dictionary<int, CampaignSquadState> _squadsById = new Dictionary<int, CampaignSquadState>();

        public IReadOnlyDictionary<string, CampaignMemberState> MembersById
        {
            get { return _membersById; }
        }

        public IReadOnlyDictionary<int, CampaignSquadState> SquadsById
        {
            get { return _squadsById; }
        }

        public void AddMember(CampaignMemberState member)
        {
            if (member == null || string.IsNullOrEmpty(member.MemberId))
            {
                return;
            }

            _membersById[member.MemberId] = member;
        }

        public void AddSquad(CampaignSquadState squad)
        {
            if (squad == null)
            {
                return;
            }

            _squadsById[squad.SquadId] = squad;
        }

        public bool TryGetMember(string memberId, out CampaignMemberState member)
        {
            return _membersById.TryGetValue(memberId, out member);
        }

        public bool TryGetSquad(int squadId, out CampaignSquadState squad)
        {
            return _squadsById.TryGetValue(squadId, out squad);
        }
    }
}
