using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignResourceConsumptionSystem
    {
        public bool ApplyDailyMaintenance(CampaignState campaignState)
        {
            if (campaignState == null || campaignState.MainBase == null || campaignState.ResourceLedger == null)
            {
                return false;
            }

            Dictionary<string, int> costs = campaignState.MainBase.GetTotalMaintenanceCosts();
            foreach (KeyValuePair<string, int> cost in costs)
            {
                if (!campaignState.ResourceLedger.HasAtLeast(cost.Key, cost.Value))
                {
                    campaignState.MainBase.SetOperational(false, "Missing maintenance resource: " + cost.Key);
                    return false;
                }
            }

            foreach (KeyValuePair<string, int> cost in costs)
            {
                campaignState.ResourceLedger.Spend(cost.Key, cost.Value);
            }

            campaignState.MainBase.SetOperational(true, null);
            return true;
        }
    }
}
