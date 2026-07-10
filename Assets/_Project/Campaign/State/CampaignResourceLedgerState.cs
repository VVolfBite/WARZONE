using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignResourceLedgerState
    {
        private readonly Dictionary<string, int> _resources = new Dictionary<string, int>();

        public IReadOnlyDictionary<string, int> Resources
        {
            get { return _resources; }
        }

        public void Add(string resourceId, int amount)
        {
            if (string.IsNullOrEmpty(resourceId) || amount == 0)
            {
                return;
            }

            int currentAmount;
            _resources.TryGetValue(resourceId, out currentAmount);
            int nextAmount = currentAmount + amount;
            _resources[resourceId] = nextAmount < 0 ? 0 : nextAmount;
        }
    }
}
