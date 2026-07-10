namespace Warzone.Campaign
{
    public sealed class CampaignSiteSystem
    {
        public bool TryGetSite(CampaignState campaignState, string siteId, out CampaignSiteState site)
        {
            site = null;
            if (campaignState == null)
            {
                return false;
            }

            return campaignState.TryGetSite(siteId, out site);
        }

        public void MarkSearchCompleted(CampaignState campaignState, string siteId)
        {
            CampaignSiteState site;
            if (TryGetSite(campaignState, siteId, out site))
            {
                site.MarkSearchCompleted();
            }
        }

        public void MarkCleared(CampaignState campaignState, string siteId)
        {
            CampaignSiteState site;
            if (TryGetSite(campaignState, siteId, out site))
            {
                site.MarkCleared();
            }
        }

        public void UpdateThreat(CampaignState campaignState, string siteId, int threatLevel)
        {
            CampaignSiteState site;
            if (TryGetSite(campaignState, siteId, out site))
            {
                site.SetThreatLevel(threatLevel);
            }
        }

        public void MarkVisited(CampaignState campaignState, string siteId, float time)
        {
            CampaignSiteState site;
            if (TryGetSite(campaignState, siteId, out site))
            {
                site.UpdateLastVisitedTime(time);
                site.MarkDiscovered();
            }
        }
    }
}
