using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignWorldTickSystem
    {
        private readonly CampaignSiteSystem _siteSystem = new CampaignSiteSystem();
        private readonly CampaignResourceConsumptionSystem _resourceConsumptionSystem = new CampaignResourceConsumptionSystem();
        private readonly CampaignOutpostSystem _outpostSystem = new CampaignOutpostSystem();

        public void ApplyHourlyTick(CampaignState campaignState)
        {
            if (campaignState == null)
            {
                return;
            }

            // Intentionally conservative: hourly progression is a future hook and
            // should not produce strong world-state drift.
            ApplySiteHourlyEffects(campaignState);
        }

        public void ApplyDailyTick(CampaignState campaignState)
        {
            if (campaignState == null)
            {
                return;
            }

            ApplySiteDailyEffects(campaignState);
            _outpostSystem.ApplyOutpostDailyEffects(campaignState);
            _resourceConsumptionSystem.ApplyDailyMaintenance(campaignState);
        }

        private void ApplySiteHourlyEffects(CampaignState campaignState)
        {
            return;
        }

        private void ApplySiteDailyEffects(CampaignState campaignState)
        {
            foreach (KeyValuePair<string, CampaignSiteState> pair in campaignState.SitesById)
            {
                CampaignSiteState site = pair.Value;
                if (site == null)
                {
                    continue;
                }

                if (site.IsSearched && !site.IsExhausted)
                {
                    _siteSystem.ReduceLoot(campaignState, site.SiteId, 1);
                }

                if (!site.IsCleared)
                {
                    if (!HasActiveOutpostAtSite(campaignState, site.SiteId))
                    {
                        _siteSystem.IncreaseThreat(campaignState, site.SiteId, 1);
                    }
                }
                else if (!HasActiveOutpostAtSite(campaignState, site.SiteId))
                {
                    float hoursSinceVisit = campaignState.CampaignTime - site.LastVisitedTime;
                    if (hoursSinceVisit >= 72f)
                    {
                        _siteSystem.IncreaseThreat(campaignState, site.SiteId, 1);
                        if (site.ThreatLevel >= 2)
                        {
                            site.SetOccupied(true);
                            site.SetThreatLevel(2);
                        }
                    }
                }
            }
        }

        private bool HasActiveOutpostAtSite(CampaignState campaignState, string siteId)
        {
            return _outpostSystem.HasOutpostAtSite(campaignState, siteId);
        }
    }
}
