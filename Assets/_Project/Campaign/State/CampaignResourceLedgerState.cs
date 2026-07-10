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

        public void Set(string resourceId, int amount)
        {
            if (string.IsNullOrEmpty(resourceId))
            {
                return;
            }

            _resources[resourceId] = amount < 0 ? 0 : amount;
        }

        public int GetAmount(string resourceId)
        {
            if (string.IsNullOrEmpty(resourceId))
            {
                return 0;
            }

            int amount;
            if (_resources.TryGetValue(resourceId, out amount))
            {
                return amount < 0 ? 0 : amount;
            }

            return 0;
        }

        public bool HasAtLeast(string resourceId, int amount)
        {
            if (string.IsNullOrEmpty(resourceId) || amount <= 0)
            {
                return true;
            }

            return GetAmount(resourceId) >= amount;
        }

        public bool Spend(string resourceId, int amount)
        {
            if (string.IsNullOrEmpty(resourceId) || amount <= 0)
            {
                return false;
            }

            int currentAmount = GetAmount(resourceId);
            if (currentAmount < amount)
            {
                return false;
            }

            _resources[resourceId] = currentAmount - amount;
            return true;
        }

        public bool CanSpendAll(IReadOnlyDictionary<string, int> costs)
        {
            if (costs == null || costs.Count == 0)
            {
                return true;
            }

            foreach (KeyValuePair<string, int> cost in costs)
            {
                if (string.IsNullOrEmpty(cost.Key) || cost.Value <= 0)
                {
                    continue;
                }

                if (GetAmount(cost.Key) < cost.Value)
                {
                    return false;
                }
            }

            return true;
        }

        public bool SpendAll(IReadOnlyDictionary<string, int> costs)
        {
            if (!CanSpendAll(costs))
            {
                return false;
            }

            if (costs == null || costs.Count == 0)
            {
                return true;
            }

            foreach (KeyValuePair<string, int> cost in costs)
            {
                if (string.IsNullOrEmpty(cost.Key) || cost.Value <= 0)
                {
                    continue;
                }

                Spend(cost.Key, cost.Value);
            }

            return true;
        }
    }
}
