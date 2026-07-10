using System.Collections.Generic;

namespace Warzone.Content.Definitions
{
    public sealed class BaseModuleDefinition
    {
        private readonly List<string> _providedCapabilities = new List<string>();
        private readonly Dictionary<string, int> _dailyResourceCosts = new Dictionary<string, int>();

        public BaseModuleDefinition(
            string id,
            string displayName,
            IReadOnlyList<string> providedCapabilities = null,
            IReadOnlyDictionary<string, int> dailyResourceCosts = null,
            int storageBonus = 0)
        {
            Id = id;
            DisplayName = displayName;
            StorageBonus = storageBonus < 0 ? 0 : storageBonus;
            SyncCapabilities(providedCapabilities);
            SyncCosts(dailyResourceCosts);
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public int StorageBonus { get; private set; }

        public IReadOnlyList<string> ProvidedCapabilities
        {
            get { return _providedCapabilities; }
        }

        public IReadOnlyDictionary<string, int> DailyResourceCosts
        {
            get { return _dailyResourceCosts; }
        }

        private void SyncCapabilities(IReadOnlyList<string> providedCapabilities)
        {
            _providedCapabilities.Clear();
            if (providedCapabilities == null)
            {
                return;
            }

            for (int i = 0; i < providedCapabilities.Count; i++)
            {
                string capabilityId = providedCapabilities[i];
                if (!string.IsNullOrEmpty(capabilityId) && !_providedCapabilities.Contains(capabilityId))
                {
                    _providedCapabilities.Add(capabilityId);
                }
            }
        }

        private void SyncCosts(IReadOnlyDictionary<string, int> dailyResourceCosts)
        {
            _dailyResourceCosts.Clear();
            if (dailyResourceCosts == null)
            {
                return;
            }

            foreach (KeyValuePair<string, int> entry in dailyResourceCosts)
            {
                if (string.IsNullOrEmpty(entry.Key))
                {
                    continue;
                }

                _dailyResourceCosts[entry.Key] = entry.Value < 0 ? 0 : entry.Value;
            }
        }
    }
}
