using System.Collections.Generic;

namespace Warzone.Content.Definitions
{
    public sealed class LootProfileDefinition
    {
        private readonly List<ResourceRewardDefinition> _resourceRewards = new List<ResourceRewardDefinition>();
        private readonly List<ItemRewardDefinition> _itemRewards = new List<ItemRewardDefinition>();
        private readonly List<WeaponRewardDefinition> _weaponRewards = new List<WeaponRewardDefinition>();

        public LootProfileDefinition(
            string id,
            string displayName,
            IReadOnlyList<ResourceRewardDefinition> resourceRewards = null,
            IReadOnlyList<ItemRewardDefinition> itemRewards = null,
            IReadOnlyList<WeaponRewardDefinition> weaponRewards = null,
            string fallbackResourceId = null)
        {
            Id = id;
            DisplayName = displayName;
            FallbackResourceId = fallbackResourceId;
            SyncRewards(_resourceRewards, resourceRewards);
            SyncRewards(_itemRewards, itemRewards);
            SyncRewards(_weaponRewards, weaponRewards);
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public string FallbackResourceId { get; private set; }

        public IReadOnlyList<ResourceRewardDefinition> ResourceRewards
        {
            get { return _resourceRewards; }
        }

        public IReadOnlyList<ItemRewardDefinition> ItemRewards
        {
            get { return _itemRewards; }
        }

        public IReadOnlyList<WeaponRewardDefinition> WeaponRewards
        {
            get { return _weaponRewards; }
        }

        private static void SyncRewards<T>(List<T> target, IReadOnlyList<T> source)
        {
            target.Clear();
            if (source == null)
            {
                return;
            }

            for (int i = 0; i < source.Count; i++)
            {
                T entry = source[i];
                if (entry != null)
                {
                    target.Add(entry);
                }
            }
        }
    }
}
