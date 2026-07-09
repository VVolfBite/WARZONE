using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class BattleLootRuntimeState
    {
        private readonly HashSet<int> _lootSourceNodeIds = new HashSet<int>();

        public int LootDiscoveredCount { get; private set; }

        public IReadOnlyCollection<int> LootSourceNodeIds
        {
            get { return _lootSourceNodeIds; }
        }

        public bool HasLootFromSearchPoint(int nodeId)
        {
            return _lootSourceNodeIds.Contains(nodeId);
        }

        public void AddLootFromSearchPoint(int nodeId, int amount)
        {
            if (!_lootSourceNodeIds.Add(nodeId))
            {
                return;
            }

            LootDiscoveredCount += amount > 0 ? amount : 1;
        }
    }
}
