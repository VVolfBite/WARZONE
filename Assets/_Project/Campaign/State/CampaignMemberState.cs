namespace Warzone.Campaign
{
    public sealed class CampaignMemberState
    {
        public CampaignMemberState(
            string memberId,
            string displayName,
            bool isAlive = true,
            bool isWounded = false,
            bool isAvailable = true,
            int experience = 0,
            int? assignedSquadId = null,
            string carriedWeaponId = null,
            string loadoutId = null)
        {
            MemberId = memberId;
            DisplayName = displayName;
            IsAlive = isAlive;
            IsWounded = isWounded;
            IsAvailable = isAvailable;
            Experience = experience;
            AssignedSquadId = assignedSquadId;
            CarriedWeaponId = carriedWeaponId;
            LoadoutId = loadoutId;
        }

        public string MemberId { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsAlive { get; private set; }
        public bool IsWounded { get; private set; }
        public bool IsAvailable { get; private set; }
        public int Experience { get; private set; }
        public int? AssignedSquadId { get; private set; }
        public string CarriedWeaponId { get; private set; }
        public string LoadoutId { get; private set; }

        public void MarkDead()
        {
            IsAlive = false;
            IsAvailable = false;
            IsWounded = true;
        }

        public void MarkWounded()
        {
            if (!IsAlive)
            {
                return;
            }

            IsWounded = true;
        }

        public void SetAvailable(bool isAvailable)
        {
            IsAvailable = isAvailable && IsAlive;
        }

        public void SetAssignedSquadId(int? squadId)
        {
            AssignedSquadId = squadId;
        }

        public void SetCarriedWeaponId(string carriedWeaponId)
        {
            CarriedWeaponId = carriedWeaponId;
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
