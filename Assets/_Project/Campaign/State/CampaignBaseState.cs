using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignBaseState
    {
        private readonly Dictionary<string, CampaignBaseModuleState> _moduleStates = new Dictionary<string, CampaignBaseModuleState>();

        public CampaignBaseState(
            string baseId,
            string displayName,
            string siteId,
            int storageCapacity = 0,
            bool isOperational = true)
        {
            BaseId = baseId;
            DisplayName = displayName;
            SiteId = siteId;
            StorageCapacity = storageCapacity < 0 ? 0 : storageCapacity;
            IsOperational = isOperational;
        }

        public string BaseId { get; private set; }
        public string DisplayName { get; private set; }
        public string SiteId { get; private set; }
        public int StorageCapacity { get; private set; }
        public bool IsOperational { get; private set; }
        public string OperationalWarning { get; private set; }

        public IReadOnlyDictionary<string, CampaignBaseModuleState> ModuleStates
        {
            get { return _moduleStates; }
        }

        public void AddModule(CampaignBaseModuleState moduleState)
        {
            if (moduleState == null || string.IsNullOrEmpty(moduleState.ModuleId))
            {
                return;
            }

            _moduleStates[moduleState.ModuleId] = moduleState;
        }

        public bool RemoveModule(string moduleId)
        {
            if (string.IsNullOrEmpty(moduleId))
            {
                return false;
            }

            return _moduleStates.Remove(moduleId);
        }

        public bool TryGetModule(string moduleId, out CampaignBaseModuleState moduleState)
        {
            return _moduleStates.TryGetValue(moduleId, out moduleState);
        }

        public bool HasCapability(string capabilityId)
        {
            if (string.IsNullOrEmpty(capabilityId))
            {
                return false;
            }

            foreach (CampaignBaseModuleState module in _moduleStates.Values)
            {
                if (module != null && module.IsActive && module.HasCapability(capabilityId))
                {
                    return true;
                }
            }

            return false;
        }

        public Dictionary<string, int> GetTotalMaintenanceCosts()
        {
            Dictionary<string, int> totals = new Dictionary<string, int>();
            foreach (CampaignBaseModuleState module in _moduleStates.Values)
            {
                if (module == null || !module.IsActive)
                {
                    continue;
                }

                foreach (KeyValuePair<string, int> cost in module.DailyResourceCosts)
                {
                    int currentAmount;
                    totals.TryGetValue(cost.Key, out currentAmount);
                    totals[cost.Key] = currentAmount + (cost.Value < 0 ? 0 : cost.Value);
                }
            }

            return totals;
        }

        public void SetOperational(bool isOperational, string warning = null)
        {
            IsOperational = isOperational;
            OperationalWarning = warning;
        }
    }
}
