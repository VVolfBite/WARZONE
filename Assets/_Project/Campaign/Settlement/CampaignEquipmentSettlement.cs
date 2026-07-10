namespace Warzone.Campaign
{
    public sealed class CampaignEquipmentSettlement
    {
        public CampaignEquipmentSettlement(
            string campaignMemberId,
            string weaponInstanceId,
            string definitionId,
            bool isReturned,
            bool isLost,
            bool isDamaged,
            float durability,
            string missionId = null)
        {
            CampaignMemberId = campaignMemberId;
            WeaponInstanceId = weaponInstanceId;
            DefinitionId = definitionId;
            IsReturned = isReturned;
            IsLost = isLost;
            IsDamaged = isDamaged;
            Durability = durability;
            MissionId = missionId;
        }

        public string CampaignMemberId { get; private set; }
        public string WeaponInstanceId { get; private set; }
        public string DefinitionId { get; private set; }
        public bool IsReturned { get; private set; }
        public bool IsLost { get; private set; }
        public bool IsDamaged { get; private set; }
        public float Durability { get; private set; }
        public string MissionId { get; private set; }
    }
}
