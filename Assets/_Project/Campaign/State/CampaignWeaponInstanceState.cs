namespace Warzone.Campaign
{
    public sealed class CampaignWeaponInstanceState
    {
        public CampaignWeaponInstanceState(
            string instanceId,
            string definitionId,
            string ownerMemberId = null,
            bool isEquipped = true,
            bool isAvailable = true,
            bool isLost = false,
            bool isDamaged = false,
            float durability = 1f,
            string lastMissionId = null)
        {
            InstanceId = instanceId;
            DefinitionId = definitionId;
            OwnerMemberId = ownerMemberId;
            IsEquipped = isEquipped;
            IsAvailable = isAvailable;
            IsLost = isLost;
            IsDamaged = isDamaged;
            Durability = durability < 0f ? 0f : durability;
            LastMissionId = lastMissionId;
        }

        public string InstanceId { get; private set; }
        public string DefinitionId { get; private set; }
        public string OwnerMemberId { get; private set; }
        public bool IsEquipped { get; private set; }
        public bool IsAvailable { get; private set; }
        public bool IsLost { get; private set; }
        public bool IsDamaged { get; private set; }
        public float Durability { get; private set; }
        public string LastMissionId { get; private set; }

        public string AssignedMemberId
        {
            get { return OwnerMemberId; }
        }

        public void SetOwner(string ownerMemberId)
        {
            OwnerMemberId = ownerMemberId;
        }

        public void SetEquipped(bool isEquipped)
        {
            IsEquipped = isEquipped;
        }

        public void SetAvailable(bool isAvailable)
        {
            IsAvailable = isAvailable && !IsLost;
        }

        public void MarkDeployed(string missionId = null)
        {
            IsEquipped = true;
            IsAvailable = false;
            LastMissionId = missionId;
        }

        public void ReturnToInventory(string missionId = null)
        {
            if (IsLost)
            {
                return;
            }

            IsEquipped = false;
            IsAvailable = true;
            LastMissionId = missionId;
        }

        public void MarkDamaged(float durabilityLoss = 0.25f)
        {
            if (durabilityLoss < 0f)
            {
                durabilityLoss = 0f;
            }

            IsDamaged = true;
            Durability -= durabilityLoss;
            if (Durability < 0f)
            {
                Durability = 0f;
            }
        }

        public void MarkLost(string missionId = null)
        {
            IsLost = true;
            IsAvailable = false;
            IsEquipped = false;
            LastMissionId = missionId;
        }

        public bool IsUsableForLaunch()
        {
            return !IsLost && IsAvailable && !IsDamaged;
        }
    }
}
