using System.Collections.Generic;

namespace Warzone.Content.Definitions
{
    public sealed class LootRewardDefinition
    {
        public LootRewardDefinition(
            IReadOnlyList<ResourceRewardDefinition> resourceRewards = null,
            IReadOnlyList<ItemRewardDefinition> itemRewards = null,
            IReadOnlyList<WeaponRewardDefinition> weaponRewards = null)
        {
            ResourceRewards = resourceRewards ?? new List<ResourceRewardDefinition>();
            ItemRewards = itemRewards ?? new List<ItemRewardDefinition>();
            WeaponRewards = weaponRewards ?? new List<WeaponRewardDefinition>();
        }

        public IReadOnlyList<ResourceRewardDefinition> ResourceRewards { get; private set; }
        public IReadOnlyList<ItemRewardDefinition> ItemRewards { get; private set; }
        public IReadOnlyList<WeaponRewardDefinition> WeaponRewards { get; private set; }
    }
}
