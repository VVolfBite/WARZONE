namespace Warzone.Content.Definitions
{
    public sealed class ResourcePackageDefinition
    {
        public ResourcePackageDefinition(
            string id,
            string displayName,
            string category,
            bool isStrategicResource = true,
            int defaultStackLimit = 999)
        {
            Id = id;
            DisplayName = displayName;
            Category = category;
            IsStrategicResource = isStrategicResource;
            DefaultStackLimit = defaultStackLimit > 0 ? defaultStackLimit : 1;
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string Category { get; private set; }
        public bool IsStrategicResource { get; private set; }
        public int DefaultStackLimit { get; private set; }
    }
}
