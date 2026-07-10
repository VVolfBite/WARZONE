using NUnit.Framework;
using Warzone.Campaign;

namespace Warzone.Tests.Campaign
{
    public sealed class CampaignStateTests
    {
        [Test]
        public void NewCampaign_HasRosterSquadInventoryAndSite()
        {
            CampaignState campaignState = CreateCampaignState();

            Assert.That(campaignState.Roster, Is.Not.Null);
            Assert.That(campaignState.Roster.SquadsById.Count, Is.GreaterThan(0));
            Assert.That(campaignState.Roster.MembersById.Count, Is.GreaterThan(0));
            Assert.That(campaignState.Inventory, Is.Not.Null);
            Assert.That(campaignState.Inventory.WeaponInstances.Count, Is.GreaterThan(0));
            Assert.That(campaignState.SitesById.Count, Is.GreaterThan(0));
        }

        [Test]
        public void CampaignSquad_KeepsIdentityAfterMemberCasualty()
        {
            CampaignState campaignState = CreateCampaignState();
            CampaignRosterSystem rosterSystem = new CampaignRosterSystem();

            rosterSystem.MarkMemberDead(campaignState, "campaign.member.1");

            CampaignSquadState squadState;
            Assert.That(campaignState.Roster.TryGetSquad(1, out squadState), Is.True);
            Assert.That(squadState.MemberIds, Has.Member("campaign.member.1"));

            CampaignMemberState memberState;
            Assert.That(campaignState.Roster.TryGetMember("campaign.member.1", out memberState), Is.True);
            Assert.That(memberState.IsAlive, Is.False);
        }

        [Test]
        public void Inventory_CanAddGenericLoot()
        {
            CampaignState campaignState = CreateCampaignState();
            CampaignInventorySystem inventorySystem = new CampaignInventorySystem();

            inventorySystem.AddGenericLoot(campaignState, 5);

            int lootCount;
            Assert.That(campaignState.Inventory.ResourcePackages.TryGetValue("generic.loot", out lootCount), Is.True);
            Assert.That(lootCount, Is.EqualTo(5));
        }

        [Test]
        public void SiteState_CanMarkSearchedAndCleared()
        {
            CampaignSiteState siteState = new CampaignSiteState("site.alpha", "Alpha Site", true, false, 3, false, "generic loot", 12f);

            siteState.MarkSearchCompleted();
            siteState.MarkCleared();

            Assert.That(siteState.SearchCompleted, Is.True);
            Assert.That(siteState.IsCleared, Is.True);
            Assert.That(siteState.ThreatLevel, Is.EqualTo(0));
        }

        private static CampaignState CreateCampaignState()
        {
            CampaignState campaignState = new CampaignState();
            campaignState.AddMember(new CampaignMemberState("campaign.member.1", "Alpha-1", true, false, true, 0, 1, "sandbox.rifle"));
            campaignState.AddMember(new CampaignMemberState("campaign.member.2", "Alpha-2", true, false, true, 0, 1, "sandbox.rifle"));
            campaignState.AddSquad(new CampaignSquadState(1, "Alpha Squad", new[] { "campaign.member.1", "campaign.member.2" }));
            campaignState.Inventory.AddWeaponInstance(new CampaignWeaponInstanceState("weapon.alpha.1", "sandbox.rifle", "campaign.member.1", true));
            campaignState.AddSite(new CampaignSiteState("site.alpha", "Alpha Site", true, false, 2, false, "generic loot", 0f));
            return campaignState;
        }
    }
}
