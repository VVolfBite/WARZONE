namespace Warzone.Campaign
{
    public sealed class CampaignSiteState
    {
        public CampaignSiteState(
            string siteId,
            string displayName,
            bool isDiscovered = false,
            bool isCleared = false,
            int threatLevel = 0,
            bool searchCompleted = false,
            string lootRemainingHint = null,
            float lastVisitedTime = 0f)
        {
            SiteId = siteId;
            DisplayName = displayName;
            IsDiscovered = isDiscovered;
            IsCleared = isCleared;
            ThreatLevel = threatLevel;
            SearchCompleted = searchCompleted;
            LootRemainingHint = lootRemainingHint;
            LastVisitedTime = lastVisitedTime;
        }

        public string SiteId { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsDiscovered { get; private set; }
        public bool IsCleared { get; private set; }
        public int ThreatLevel { get; private set; }
        public bool SearchCompleted { get; private set; }
        public string LootRemainingHint { get; private set; }
        public float LastVisitedTime { get; private set; }

        public void MarkDiscovered()
        {
            IsDiscovered = true;
        }

        public void MarkCleared()
        {
            IsCleared = true;
            ThreatLevel = 0;
        }

        public void MarkSearchCompleted()
        {
            SearchCompleted = true;
        }

        public void SetThreatLevel(int threatLevel)
        {
            ThreatLevel = threatLevel < 0 ? 0 : threatLevel;
        }

        public void UpdateLastVisitedTime(float lastVisitedTime)
        {
            LastVisitedTime = lastVisitedTime < 0f ? 0f : lastVisitedTime;
        }
    }
}
