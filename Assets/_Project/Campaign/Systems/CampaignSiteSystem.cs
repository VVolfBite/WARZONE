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

        public void MarkSearched(CampaignState campaignState, string siteId)
        {
            MarkSearchCompleted(campaignState, siteId);
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

        public void IncreaseThreat(CampaignState campaignState, string siteId, int amount)
        {
            CampaignSiteState site;
            if (TryGetSite(campaignState, siteId, out site))
            {
                site.IncreaseThreat(amount);
            }
        }

        public void ReduceThreat(CampaignState campaignState, string siteId, int amount)
        {
            CampaignSiteState site;
            if (TryGetSite(campaignState, siteId, out site))
            {
                site.ReduceThreat(amount);
            }
        }

        public void ReduceLoot(CampaignState campaignState, string siteId, int amount)
        {
            CampaignSiteState site;
            if (TryGetSite(campaignState, siteId, out site))
            {
                site.ReduceLoot(amount);
            }
        }

        public void SetOccupied(CampaignState campaignState, string siteId, bool isOccupied)
        {
            CampaignSiteState site;
            if (TryGetSite(campaignState, siteId, out site))
            {
                site.SetOccupied(isOccupied);
            }
        }

        public void MarkExhausted(CampaignState campaignState, string siteId)
        {
            CampaignSiteState site;
            if (TryGetSite(campaignState, siteId, out site))
            {
                site.MarkExhausted();
            }
        }

        public void MarkVisited(CampaignState campaignState, string siteId, float time)
        {
            MarkVisited(campaignState, siteId, time, 1);
        }

        public void MarkVisited(CampaignState campaignState, string siteId, float time, int visitCountDelta)
        {
            CampaignSiteState site;
            if (TryGetSite(campaignState, siteId, out site))
            {
                site.MarkVisited(time, visitCountDelta);
            }
        }
    }
}
