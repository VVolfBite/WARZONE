using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignOutpostSystem
    {
        private const string BuildingMaterialResourceId = "building_material";
        private const string FuelResourceId = "fuel";
        private const int EstablishBuildingMaterialCost = 2;
        private const int EstablishFuelCost = 1;

        public bool CanEstablishOutpost(CampaignState campaignState, string siteId)
        {
            CampaignSiteState site;
            if (!TryGetEligibleSite(campaignState, siteId, out site))
            {
                return false;
            }

            return site.CanBecomeOutpost;
        }

        public bool EstablishOutpost(CampaignState campaignState, string siteId, string outpostId)
        {
            CampaignSiteState site;
            if (!TryGetEligibleSite(campaignState, siteId, out site) || !site.CanBecomeOutpost)
            {
                return false;
            }

            if (campaignState == null || campaignState.ResourceLedger == null)
            {
                return false;
            }

            Dictionary<string, int> establishCosts = new Dictionary<string, int>
            {
                { BuildingMaterialResourceId, EstablishBuildingMaterialCost },
                { FuelResourceId, EstablishFuelCost }
            };

            if (!campaignState.ResourceLedger.SpendAll(establishCosts))
            {
                return false;
            }

            string resolvedOutpostId = string.IsNullOrEmpty(outpostId) ? siteId + ".outpost" : outpostId;
            CampaignOutpostState outpostState = new CampaignOutpostState(
                resolvedOutpostId,
                siteId,
                site.DisplayName + " Outpost",
                true,
                true,
                true,
                10,
                new List<string> { "safe_extraction", "storage", "watch" },
                new Dictionary<string, int> { { BuildingMaterialResourceId, 1 }, { FuelResourceId, 1 } });

            campaignState.AddOutpost(outpostState);
            site.SetOutpost(resolvedOutpostId);
            site.SetOccupied(false);
            site.ReduceThreat(1);
            site.MarkDiscovered();
            return true;
        }

        public bool AbandonOutpost(CampaignState campaignState, string outpostId)
        {
            if (campaignState == null || string.IsNullOrEmpty(outpostId))
            {
                return false;
            }

            CampaignOutpostState outpostState;
            if (!campaignState.TryGetOutpost(outpostId, out outpostState))
            {
                return false;
            }

            outpostState.SetActive(false);
            CampaignSiteState site;
            if (campaignState.TryGetSite(outpostState.SiteId, out site))
            {
                site.SetOutpost(null);
            }

            return true;
        }

        public bool HasOutpostAtSite(CampaignState campaignState, string siteId)
        {
            if (campaignState == null || string.IsNullOrEmpty(siteId))
            {
                return false;
            }

            foreach (KeyValuePair<string, CampaignOutpostState> pair in campaignState.OutpostsById)
            {
                CampaignOutpostState outpost = pair.Value;
                if (outpost != null && outpost.IsActive && outpost.SiteId == siteId)
                {
                    return true;
                }
            }

            return false;
        }

        public void ApplyOutpostDailyEffects(CampaignState campaignState)
        {
            if (campaignState == null)
            {
                return;
            }

            foreach (KeyValuePair<string, CampaignOutpostState> pair in campaignState.OutpostsById)
            {
                CampaignOutpostState outpost = pair.Value;
                if (outpost == null || !outpost.IsActive)
                {
                    continue;
                }

                CampaignSiteState site;
                if (!campaignState.TryGetSite(outpost.SiteId, out site))
                {
                    continue;
                }

                if (!campaignState.ResourceLedger.SpendAll(outpost.DailyResourceCosts))
                {
                    outpost.SetActive(false);
                    continue;
                }

                if (outpost.ReducesLocalThreat)
                {
                    site.ReduceThreat(1);
                }

                site.SetOccupied(false);
            }
        }

        private bool TryGetEligibleSite(CampaignState campaignState, string siteId, out CampaignSiteState site)
        {
            site = null;
            if (campaignState == null || string.IsNullOrEmpty(siteId))
            {
                return false;
            }

            if (!campaignState.TryGetSite(siteId, out site))
            {
                return false;
            }

            return site.IsDiscovered && site.IsCleared;
        }
    }
}
