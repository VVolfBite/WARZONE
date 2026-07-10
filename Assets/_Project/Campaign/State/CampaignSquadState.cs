using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignSquadState
    {
        private readonly List<string> _memberIds = new List<string>();

        public CampaignSquadState(
            int squadId,
            string displayName,
            IReadOnlyList<string> memberIds = null,
            bool isAvailable = true,
            int experience = 0,
            float familiarity = 0f)
        {
            SquadId = squadId;
            DisplayName = displayName;
            IsAvailable = isAvailable;
            Experience = experience;
            Familiarity = familiarity;
            SyncMemberIds(memberIds);
        }

        public int SquadId { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsAvailable { get; private set; }
        public int Experience { get; private set; }
        public float Familiarity { get; private set; }

        public IReadOnlyList<string> MemberIds
        {
            get { return _memberIds; }
        }

        public void SyncMemberIds(IReadOnlyList<string> memberIds)
        {
            _memberIds.Clear();
            if (memberIds == null)
            {
                return;
            }

            for (int i = 0; i < memberIds.Count; i++)
            {
                _memberIds.Add(memberIds[i]);
            }
        }

        public void SetAvailable(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Experience += amount;
        }
    }
}
