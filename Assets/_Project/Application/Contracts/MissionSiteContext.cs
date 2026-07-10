using System.Collections.Generic;
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
            bool isCleared,
            bool isExhausted,
            bool isOccupied,
            int lootRemaining,
            int resourceRichness,
            int visitCount,
            Vec2 entryPosition,
            Vec2 extractionPosition,
            Vec2 searchPosition,
            float lastVisitedTime = 0f,
            string outpostId = null,
            IReadOnlyList<string> tags = null)
        {
            SiteId = siteId;
            DisplayName = displayName;
            SiteType = siteType;
            IsEnterable = isEnterable;
            ThreatLevel = threatLevel;
            SearchCompleted = searchCompleted;
            LootRemainingHint = lootRemainingHint;
            IsCleared = isCleared;
            IsExhausted = isExhausted;
            IsOccupied = isOccupied;
            LootRemaining = lootRemaining;
            ResourceRichness = resourceRichness;
            VisitCount = visitCount;
            EntryPosition = entryPosition;
            ExtractionPosition = extractionPosition;
            SearchPosition = searchPosition;
            LastVisitedTime = lastVisitedTime;
            OutpostId = outpostId;
            Tags = tags ?? new List<string>();
        }

        public string SiteId { get; private set; }
        public string DisplayName { get; private set; }
        public SiteType SiteType { get; private set; }
        public bool IsEnterable { get; private set; }
        public int ThreatLevel { get; private set; }
        public bool SearchCompleted { get; private set; }
        public string LootRemainingHint { get; private set; }
        public bool IsCleared { get; private set; }
        public bool IsExhausted { get; private set; }
        public bool IsOccupied { get; private set; }
        public int LootRemaining { get; private set; }
        public int ResourceRichness { get; private set; }
        public int VisitCount { get; private set; }
        public Vec2 EntryPosition { get; private set; }
        public Vec2 ExtractionPosition { get; private set; }
        public Vec2 SearchPosition { get; private set; }
        public float LastVisitedTime { get; private set; }
        public string OutpostId { get; private set; }
        public IReadOnlyList<string> Tags { get; private set; }
    }
}
