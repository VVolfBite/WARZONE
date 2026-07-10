namespace Warzone.Campaign
{
    public sealed class CampaignItemStackState
    {
        public CampaignItemStackState(string itemId, string displayName, int count)
        {
            ItemId = itemId;
            DisplayName = displayName;
            Count = count;
        }

        public string ItemId { get; private set; }
        public string DisplayName { get; private set; }
        public int Count { get; private set; }

        public void Add(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Count += amount;
        }

        public bool Spend(int amount)
        {
            if (amount <= 0 || Count < amount)
            {
                return false;
            }

            Count -= amount;
            if (Count < 0)
            {
                Count = 0;
            }

            return true;
        }
    }
}
