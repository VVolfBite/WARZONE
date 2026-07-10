namespace Warzone.Content.Definitions
{
    public sealed class ResourceRewardDefinition
    {
        public ResourceRewardDefinition(string resourceId, int amount)
        {
            ResourceId = resourceId;
            Amount = amount;
        }

        public string ResourceId { get; private set; }
        public int Amount { get; private set; }
    }
}
