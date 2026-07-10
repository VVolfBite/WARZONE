using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignBaseModuleState
    {
        private readonly List<string> _providedCapabilities = new List<string>();
        private readonly Dictionary<string, int> _dailyResourceCosts = new Dictionary<string, int>();

        public CampaignBaseModuleState(
            string moduleId,
            string displayName,
            bool isActive = true,
            IReadOnlyList<string> providedCapabilities = null,
            IReadOnlyDictionary<string, int> dailyResourceCosts = null)
        {
            ModuleId = moduleId;
            DisplayName = displayName;
            IsActive = isActive;
            SyncCapabilities(providedCapabilities);
            SyncCosts(dailyResourceCosts);
        }

        public string ModuleId { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsActive { get; private set; }

        public IReadOnlyList<string> ProvidedCapabilities
        {
            get { return _providedCapabilities; }
        }

        public IReadOnlyDictionary<string, int> DailyResourceCosts
        {
            get { return _dailyResourceCosts; }
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
        }

        public void AddCapability(string capabilityId)
        {
            if (string.IsNullOrEmpty(capabilityId) || _providedCapabilities.Contains(capabilityId))
            {
                return;
            }

            _providedCapabilities.Add(capabilityId);
        }

        public void SetDailyResourceCost(string resourceId, int amount)
        {
            if (string.IsNullOrEmpty(resourceId))
            {
                return;
            }

            _dailyResourceCosts[resourceId] = amount < 0 ? 0 : amount;
        }

        public bool HasCapability(string capabilityId)
        {
            return !string.IsNullOrEmpty(capabilityId) && _providedCapabilities.Contains(capabilityId);
        }

        private void SyncCapabilities(IReadOnlyList<string> capabilities)
        {
            _providedCapabilities.Clear();
            if (capabilities == null)
            {
                return;
            }

            for (int i = 0; i < capabilities.Count; i++)
            {
                AddCapability(capabilities[i]);
            }
        }

        private void SyncCosts(IReadOnlyDictionary<string, int> costs)
        {
            _dailyResourceCosts.Clear();
            if (costs == null)
            {
                return;
            }

            foreach (KeyValuePair<string, int> cost in costs)
            {
                SetDailyResourceCost(cost.Key, cost.Value);
            }
        }
    }
}
