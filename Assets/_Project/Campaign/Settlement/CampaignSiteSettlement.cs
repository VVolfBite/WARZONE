namespace Warzone.Campaign
{
    public sealed class CampaignSiteSettlement
    {
        public CampaignSiteSettlement(
            string siteId,
            bool searchCompleted,
            bool isCleared,
            int threatLevel,
            float lastVisitedTime)
            : this(siteId, searchCompleted, isCleared, threatLevel, lastVisitedTime, 0, 0, 1, false, false, null)
        {
        }

        public CampaignSiteSettlement(
            string siteId,
            bool searchCompleted,
            bool isCleared,
            int threatLevel,
            float lastVisitedTime,
            int lootRemainingDelta,
            int resourceRichnessDelta,
            int visitCountDelta,
            bool isOccupied,
            bool isExhausted,
            string outpostId)
        {
            SiteId = siteId;
            SearchCompleted = searchCompleted;
            IsCleared = isCleared;
            ThreatLevel = threatLevel;
            LastVisitedTime = lastVisitedTime;
            LootRemainingDelta = lootRemainingDelta;
            ResourceRichnessDelta = resourceRichnessDelta;
            VisitCountDelta = visitCountDelta;
            IsOccupied = isOccupied;
            IsExhausted = isExhausted;
            OutpostId = outpostId;
        }

        public string SiteId { get; private set; }
        public bool SearchCompleted { get; private set; }
        public bool IsCleared { get; private set; }
        public int ThreatLevel { get; private set; }
        public float LastVisitedTime { get; private set; }
        public int LootRemainingDelta { get; private set; }
        public int ResourceRichnessDelta { get; private set; }
        public int VisitCountDelta { get; private set; }
        public bool IsOccupied { get; private set; }
        public bool IsExhausted { get; private set; }
        public string OutpostId { get; private set; }
    }
}
