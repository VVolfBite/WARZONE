namespace Warzone.Campaign
{
    public sealed class CampaignWeaponInstanceState
    {
        public CampaignWeaponInstanceState(
            string instanceId,
            string definitionId,
            string ownerMemberId = null,
            bool isEquipped = true)
        {
            InstanceId = instanceId;
            DefinitionId = definitionId;
            OwnerMemberId = ownerMemberId;
            IsEquipped = isEquipped;
        }

        public string InstanceId { get; private set; }
        public string DefinitionId { get; private set; }
        public string OwnerMemberId { get; private set; }
        public bool IsEquipped { get; private set; }

        public void SetOwner(string ownerMemberId)
        {
            OwnerMemberId = ownerMemberId;
        }

        public void SetEquipped(bool isEquipped)
        {
            IsEquipped = isEquipped;
        }
    }
}
