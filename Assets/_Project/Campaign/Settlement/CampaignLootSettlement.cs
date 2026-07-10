namespace Warzone.Campaign
{
    public sealed class CampaignLootSettlement
    {
        public CampaignLootSettlement(string lootId, int count)
        {
            LootId = lootId;
            Count = count;
        }

        public string LootId { get; private set; }
        public int Count { get; private set; }
    }
}
