using System.Collections.Generic;

namespace Warzone.Content.Definitions
{
    public sealed class OutpostDefinition
    {
        private readonly List<OutpostCapability> _capabilities = new List<OutpostCapability>();
        private readonly Dictionary<string, int> _establishCostResources = new Dictionary<string, int>();
        private readonly Dictionary<string, int> _dailyMaintenanceResources = new Dictionary<string, int>();

        public OutpostDefinition(
            string id,
            string displayName,
            IReadOnlyDictionary<string, int> establishCostResources = null,
            IReadOnlyDictionary<string, int> dailyMaintenanceResources = null,
            IReadOnlyList<OutpostCapability> capabilities = null)
        {
            Id = id;
            DisplayName = displayName;
            SyncCosts(_establishCostResources, establishCostResources);
            SyncCosts(_dailyMaintenanceResources, dailyMaintenanceResources);
            SyncCapabilities(capabilities);
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }

        public IReadOnlyDictionary<string, int> EstablishCostResources
        {
            get { return _establishCostResources; }
        }

        public IReadOnlyDictionary<string, int> DailyMaintenanceResources
        {
            get { return _dailyMaintenanceResources; }
        }

        public IReadOnlyList<OutpostCapability> Capabilities
        {
            get { return _capabilities; }
        }

        private void SyncCapabilities(IReadOnlyList<OutpostCapability> capabilities)
        {
            _capabilities.Clear();
            if (capabilities == null)
            {
                return;
            }

            for (int i = 0; i < capabilities.Count; i++)
            {
                OutpostCapability capability = capabilities[i];
                if (!_capabilities.Contains(capability))
                {
                    _capabilities.Add(capability);
                }
            }
        }

        private void SyncCosts(Dictionary<string, int> target, IReadOnlyDictionary<string, int> source)
        {
            target.Clear();
            if (source == null)
            {
                return;
            }

            foreach (KeyValuePair<string, int> entry in source)
            {
                if (string.IsNullOrEmpty(entry.Key))
                {
                    continue;
                }

                target[entry.Key] = entry.Value < 0 ? 0 : entry.Value;
            }
        }
    }
}
