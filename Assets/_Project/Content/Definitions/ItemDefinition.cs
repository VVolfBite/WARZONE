namespace Warzone.Content.Definitions
{
    public sealed class ItemDefinition
    {
        public ItemDefinition(
            string id,
            string displayName,
            ItemType itemType,
            int stackLimit = 1,
            string description = null)
        {
            Id = id;
            DisplayName = displayName;
            ItemType = itemType;
            StackLimit = stackLimit > 0 ? stackLimit : 1;
            Description = description;
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public ItemType ItemType { get; private set; }
        public int StackLimit { get; private set; }
        public string Description { get; private set; }
    }
}
