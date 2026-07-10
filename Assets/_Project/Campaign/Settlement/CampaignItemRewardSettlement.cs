namespace Warzone.Campaign
{
    public sealed class CampaignItemRewardSettlement
    {
        public CampaignItemRewardSettlement(string itemId, string displayName, int count)
        {
            ItemId = itemId;
            DisplayName = displayName;
            Count = count;
        }

        public string ItemId { get; private set; }
        public string DisplayName { get; private set; }
        public int Count { get; private set; }
    }
}
