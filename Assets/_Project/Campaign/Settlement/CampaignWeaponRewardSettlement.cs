namespace Warzone.Campaign
{
    public sealed class CampaignWeaponRewardSettlement
    {
        public CampaignWeaponRewardSettlement(string instanceId, string definitionId, string ownerMemberId = null, bool isEquipped = false)
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
    }
}
