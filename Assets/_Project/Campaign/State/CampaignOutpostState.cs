using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignOutpostState
    {
        private readonly List<string> _capabilities = new List<string>();
        private readonly Dictionary<string, int> _dailyResourceCosts = new Dictionary<string, int>();

        public CampaignOutpostState(
            string outpostId,
            string siteId,
            string displayName,
            bool isActive = true,
            bool providesSafeExtraction = true,
            bool reducesLocalThreat = true,
            int storageBonus = 0,
            IReadOnlyList<string> capabilities = null,
            IReadOnlyDictionary<string, int> dailyResourceCosts = null)
        {
            OutpostId = outpostId;
            SiteId = siteId;
            DisplayName = displayName;
            IsActive = isActive;
            ProvidesSafeExtraction = providesSafeExtraction;
            ReducesLocalThreat = reducesLocalThreat;
            StorageBonus = storageBonus < 0 ? 0 : storageBonus;
            SyncCapabilities(capabilities);
            SyncCosts(dailyResourceCosts);
        }

        public string OutpostId { get; private set; }
        public string SiteId { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsActive { get; private set; }
        public bool ProvidesSafeExtraction { get; private set; }
        public bool ReducesLocalThreat { get; private set; }
        public int StorageBonus { get; private set; }

        public IReadOnlyList<string> Capabilities
        {
            get { return _capabilities; }
        }

        public IReadOnlyDictionary<string, int> DailyResourceCosts
        {
            get { return _dailyResourceCosts; }
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
        }

        public bool HasCapability(string capabilityId)
        {
            return !string.IsNullOrEmpty(capabilityId) && _capabilities.Contains(capabilityId);
        }

        public void AddCapability(string capabilityId)
        {
            if (string.IsNullOrEmpty(capabilityId) || _capabilities.Contains(capabilityId))
            {
                return;
            }

            _capabilities.Add(capabilityId);
        }

        public void SetDailyResourceCost(string resourceId, int amount)
        {
            if (string.IsNullOrEmpty(resourceId))
            {
                return;
            }

            _dailyResourceCosts[resourceId] = amount < 0 ? 0 : amount;
        }

        private void SyncCapabilities(IReadOnlyList<string> capabilities)
        {
            _capabilities.Clear();
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
