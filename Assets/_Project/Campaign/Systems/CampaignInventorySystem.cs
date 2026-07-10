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

            campaignState.Inventory.AddResourcePackage("generic.loot", amount);
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
    }
}
