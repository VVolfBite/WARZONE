namespace Warzone.Content.Definitions
{
    public sealed class ItemRewardDefinition
    {
        public ItemRewardDefinition(string itemId, int count)
        {
            ItemId = itemId;
            Count = count;
        }

        public string ItemId { get; private set; }
        public int Count { get; private set; }
    }
}
