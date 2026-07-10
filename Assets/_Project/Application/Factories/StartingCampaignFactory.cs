using Warzone.Campaign;
using Warzone.Content.Definitions;

namespace Warzone.Application
{
    public sealed class StartingCampaignFactory
    {
        private readonly CampaignBaseSystem _baseSystem = new CampaignBaseSystem();

        public CampaignState CreateStartingCampaign()
        {
            CampaignState campaignState = new CampaignState();

            CampaignMemberState member1 = new CampaignMemberState("campaign.member.1", "Alpha-1", true, false, true, 0, 1, "sandbox.rifle", "loadout.alpha.1");
            CampaignMemberState member2 = new CampaignMemberState("campaign.member.2", "Alpha-2", true, false, true, 0, 1, "sandbox.rifle", "loadout.alpha.2");
            CampaignMemberState member3 = new CampaignMemberState("campaign.member.3", "Alpha-3", true, false, true, 0, 1, "sandbox.rifle", "loadout.alpha.3");
            CampaignMemberState member4 = new CampaignMemberState("campaign.member.4", "Alpha-4", true, false, true, 0, 1, "sandbox.rifle", "loadout.alpha.4");

            campaignState.AddMember(member1);
            campaignState.AddMember(member2);
            campaignState.AddMember(member3);
            campaignState.AddMember(member4);

            CampaignSquadState squad = new CampaignSquadState(1, "Alpha Squad", new[] { member1.MemberId, member2.MemberId, member3.MemberId, member4.MemberId });
            campaignState.AddSquad(squad);

            campaignState.Inventory.AddWeaponInstance(new CampaignWeaponInstanceState("weapon.alpha.rifle", "sandbox.rifle", member1.MemberId, true));
            campaignState.Inventory.AddItemStack(new CampaignItemStackState("medkit.basic", "Basic Medkit", 2));
            campaignState.Inventory.AddItemStack(new CampaignItemStackState("ammo.pack", "Ammo Pack", 4));

            campaignState.ResourceLedger.Add("food", 12);
            campaignState.ResourceLedger.Add("medicine", 8);
            campaignState.ResourceLedger.Add("ammo", 18);
            campaignState.ResourceLedger.Add("fuel", 6);
            campaignState.ResourceLedger.Add("building_material", 4);

            campaignState.AddSite(new CampaignSiteState(
                "site.alpha",
                "Alpha Medical Clinic",
                SiteType.Facility,
                true,
                true,
                1,
                true,
                "medicine cache",
                0f,
                4,
                6,
                4,
                false,
                false,
                true,
                null,
                new[] { "medical", "clinic" }));

            campaignState.AddSite(new CampaignSiteState(
                "site.beta",
                "Beta Supply Depot",
                SiteType.Compound,
                true,
                false,
                3,
                false,
                "ammo cache",
                0f,
                5,
                4,
                6,
                false,
                true,
                false,
                null,
                new[] { "supply", "depot" }));

            campaignState.AddSite(new CampaignSiteState(
                "site.gamma",
                "Gamma Residence",
                SiteType.UrbanBlock,
                true,
                false,
                2,
                false,
                "generic loot",
                0f,
                3,
                3,
                3,
                false,
                false,
                true,
                null,
                new[] { "residential" }));

            campaignState.SetMainBase(_baseSystem.CreateStartingBase("base.main", "Main Base", "site.alpha"));

            return campaignState;
        }
    }
}
