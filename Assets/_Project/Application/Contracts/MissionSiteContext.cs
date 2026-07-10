using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Application
{
    public sealed class MissionSiteContext
    {
        public MissionSiteContext(
            string siteId,
            string displayName,
            SiteType siteType,
            bool isEnterable,
            int threatLevel,
            bool searchCompleted,
            string lootRemainingHint,
            Vec2 entryPosition,
            Vec2 extractionPosition,
            Vec2 searchPosition,
            float lastVisitedTime = 0f)
        {
            SiteId = siteId;
            DisplayName = displayName;
            SiteType = siteType;
            IsEnterable = isEnterable;
            ThreatLevel = threatLevel;
            SearchCompleted = searchCompleted;
            LootRemainingHint = lootRemainingHint;
            EntryPosition = entryPosition;
            ExtractionPosition = extractionPosition;
            SearchPosition = searchPosition;
            LastVisitedTime = lastVisitedTime;
        }

        public string SiteId { get; private set; }
        public string DisplayName { get; private set; }
        public SiteType SiteType { get; private set; }
        public bool IsEnterable { get; private set; }
        public int ThreatLevel { get; private set; }
        public bool SearchCompleted { get; private set; }
        public string LootRemainingHint { get; private set; }
        public Vec2 EntryPosition { get; private set; }
        public Vec2 ExtractionPosition { get; private set; }
        public Vec2 SearchPosition { get; private set; }
        public float LastVisitedTime { get; private set; }
    }
}
