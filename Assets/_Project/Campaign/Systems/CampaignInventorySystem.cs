namespace Warzone.Campaign
{
    public sealed class CampaignInventorySystem
    {
        public void AddGenericLoot(CampaignState campaignState, int amount)
        {
            if (campaignState == null || campaignState.Inventory == null)
            {
                return;
            }

            AddResourcePackage(campaignState, "generic.loot", amount);
        }

        public void AddResourcePackage(CampaignState campaignState, string packageId, int amount)
        {
            if (campaignState == null || campaignState.Inventory == null)
            {
                return;
            }

            campaignState.Inventory.AddResourcePackage(packageId, amount);
        }

        public void AddItemStack(CampaignState campaignState, string itemId, string displayName, int count)
        {
            if (campaignState == null || campaignState.Inventory == null)
            {
                return;
            }

            campaignState.Inventory.AddItemStack(new CampaignItemStackState(itemId, displayName, count));
        }

        public void AddWeaponInstance(CampaignState campaignState, CampaignWeaponInstanceState instance)
        {
            if (campaignState == null || campaignState.Inventory == null)
            {
                return;
            }

            campaignState.Inventory.AddWeaponInstance(instance);
        }

        public bool SpendItemStack(CampaignState campaignState, string itemId, int amount)
        {
            if (campaignState == null || campaignState.Inventory == null)
            {
                return false;
            }

            return campaignState.Inventory.SpendItemStack(itemId, amount);
        }

        public bool SpendResourcePackage(CampaignState campaignState, string packageId, int amount)
        {
            if (campaignState == null || campaignState.Inventory == null)
            {
                return false;
            }

            return campaignState.Inventory.SpendResourcePackage(packageId, amount);
        }

        public int GetResourcePackageAmount(CampaignState campaignState, string packageId)
        {
            if (campaignState == null || campaignState.Inventory == null)
            {
                return 0;
            }

            return campaignState.Inventory.GetResourcePackageAmount(packageId);
        }

        public bool RemoveWeaponInstance(CampaignState campaignState, string instanceId)
        {
            if (campaignState == null || campaignState.Inventory == null)
            {
                return false;
            }

            return campaignState.Inventory.RemoveWeaponInstance(instanceId);
        }
    }
}
