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
        {
            SiteId = siteId;
            SearchCompleted = searchCompleted;
            IsCleared = isCleared;
            ThreatLevel = threatLevel;
            LastVisitedTime = lastVisitedTime;
        }

        public string SiteId { get; private set; }
        public bool SearchCompleted { get; private set; }
        public bool IsCleared { get; private set; }
        public int ThreatLevel { get; private set; }
        public float LastVisitedTime { get; private set; }
    }
}
