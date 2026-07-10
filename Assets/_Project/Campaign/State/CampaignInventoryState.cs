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

        public bool SpendItemStack(string itemId, int amount)
        {
            if (string.IsNullOrEmpty(itemId) || amount <= 0)
            {
                return false;
            }

            CampaignItemStackState stack;
            if (!_itemStacks.TryGetValue(itemId, out stack))
            {
                return false;
            }

            if (stack.Count < amount)
            {
                return false;
            }

            stack.Spend(amount);
            if (stack.Count <= 0)
            {
                _itemStacks.Remove(itemId);
            }

            return true;
        }

        public bool RemoveItemStack(string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                return false;
            }

            return _itemStacks.Remove(itemId);
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

        public bool SpendResourcePackage(string packageId, int amount)
        {
            if (string.IsNullOrEmpty(packageId) || amount <= 0)
            {
                return false;
            }

            int currentAmount;
            if (!_resourcePackages.TryGetValue(packageId, out currentAmount))
            {
                return false;
            }

            if (currentAmount < amount)
            {
                return false;
            }

            int nextAmount = currentAmount - amount;
            if (nextAmount <= 0)
            {
                _resourcePackages.Remove(packageId);
            }
            else
            {
                _resourcePackages[packageId] = nextAmount;
            }

            return true;
        }

        public int GetResourcePackageAmount(string packageId)
        {
            if (string.IsNullOrEmpty(packageId))
            {
                return 0;
            }

            int currentAmount;
            if (_resourcePackages.TryGetValue(packageId, out currentAmount))
            {
                return currentAmount < 0 ? 0 : currentAmount;
            }

            return 0;
        }

        public bool RemoveWeaponInstance(string instanceId)
        {
            if (string.IsNullOrEmpty(instanceId))
            {
                return false;
            }

            return _weaponInstances.Remove(instanceId);
        }
    }
}
