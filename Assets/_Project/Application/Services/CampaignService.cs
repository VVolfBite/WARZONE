using Warzone.Campaign;

namespace Warzone.Application.Services
{
    public sealed class CampaignService
    {
        private readonly CampaignSettlementSystem _settlementSystem = new CampaignSettlementSystem();

        public CampaignService()
        {
            CampaignState = CreateNewCampaign();
        }

        public CampaignState CampaignState { get; private set; }

        public CampaignState CreateNewCampaign()
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
            campaignState.Inventory.AddItemStack(new CampaignItemStackState("generic.loot", "Generic Loot", 0));
            campaignState.AddSite(new CampaignSiteState("site.alpha", "Alpha Site", true, false, 2, false, "generic loot", 0f));
            campaignState.SetCurrentMission(new CampaignMissionState("mission.sandbox", "site.alpha", "Sandbox Mission", true, 2, "Tactical"));

            CampaignState = campaignState;
            return CampaignState;
        }

        public void ApplySettlement(CampaignSettlement settlement)
        {
            _settlementSystem.Apply(CampaignState, settlement);
        }
    }
}
