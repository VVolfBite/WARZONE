namespace Warzone.Content.Definitions
{
    public sealed class SiteDefinition
    {
        public SiteDefinition(
            string id,
            string displayName,
            SiteType siteType,
            bool isEnterable = true,
            int baseThreatLevel = 0,
            string lootRemainingHint = null,
            string requiredMissionType = null)
        {
            Id = id;
            DisplayName = displayName;
            SiteType = siteType;
            IsEnterable = isEnterable;
            BaseThreatLevel = baseThreatLevel;
            LootRemainingHint = lootRemainingHint;
            RequiredMissionType = requiredMissionType;
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public SiteType SiteType { get; private set; }
        public bool IsEnterable { get; private set; }
        public int BaseThreatLevel { get; private set; }
        public string LootRemainingHint { get; private set; }
        public string RequiredMissionType { get; private set; }
    }
}
