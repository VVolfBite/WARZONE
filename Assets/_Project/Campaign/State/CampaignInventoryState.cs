using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignInventoryState
    {
        private readonly Dictionary<string, CampaignWeaponInstanceState> _weaponInstances = new Dictionary<string, CampaignWeaponInstanceState>();
        private readonly Dictionary<string, CampaignItemStackState> _itemStacks = new Dictionary<string, CampaignItemStackState>();
        private readonly Dictionary<string, int> _resourcePackages = new Dictionary<string, int>();

        public IReadOnlyDictionary<string, CampaignWeaponInstanceState> WeaponInstances
        {
            get { return _weaponInstances; }
        }

        public IReadOnlyDictionary<string, CampaignItemStackState> ItemStacks
        {
            get { return _itemStacks; }
        }

        public IReadOnlyDictionary<string, int> ResourcePackages
        {
            get { return _resourcePackages; }
        }

        public void AddWeaponInstance(CampaignWeaponInstanceState instance)
        {
            if (instance == null || string.IsNullOrEmpty(instance.InstanceId))
            {
                return;
            }

            _weaponInstances[instance.InstanceId] = instance;
        }

        public void AddItemStack(CampaignItemStackState stack)
        {
            if (stack == null || string.IsNullOrEmpty(stack.ItemId))
            {
                return;
            }

            CampaignItemStackState existingStack;
            if (_itemStacks.TryGetValue(stack.ItemId, out existingStack))
            {
                existingStack.Add(stack.Count);
                return;
            }

            _itemStacks[stack.ItemId] = stack;
        }

        public void AddResourcePackage(string packageId, int amount)
        {
            if (string.IsNullOrEmpty(packageId) || amount <= 0)
            {
                return;
            }

            int currentAmount;
            _resourcePackages.TryGetValue(packageId, out currentAmount);
            _resourcePackages[packageId] = currentAmount + amount;
        }
    }
}
