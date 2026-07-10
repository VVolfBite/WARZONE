using System.Collections.Generic;
using NUnit.Framework;
using Warzone.Campaign;

namespace Warzone.Tests.Campaign
{
    public sealed class CampaignResourceBaseTests
    {
        [Test]
        public void ResourceLedger_AddSpendAndHasAtLeastWorks()
        {
            CampaignResourceLedgerState ledger = new CampaignResourceLedgerState();

            ledger.Add("food", 10);
            ledger.Add("food", 2);

            Assert.That(ledger.GetAmount("food"), Is.EqualTo(12));
            Assert.That(ledger.HasAtLeast("food", 10), Is.True);
            Assert.That(ledger.Spend("food", 3), Is.True);
            Assert.That(ledger.GetAmount("food"), Is.EqualTo(9));
            Assert.That(ledger.Spend("food", 20), Is.False);
            Assert.That(ledger.GetAmount("food"), Is.EqualTo(9));
        }

        [Test]
        public void Inventory_CanManageStacksPackagesAndWeapons()
        {
            CampaignInventoryState inventory = new CampaignInventoryState();

            inventory.AddItemStack(new CampaignItemStackState("medkit.basic", "Basic Medkit", 2));
            inventory.AddItemStack(new CampaignItemStackState("medkit.basic", "Basic Medkit", 1));
            inventory.AddResourcePackage("ammo", 5);
            inventory.AddResourcePackage("ammo", 3);
            inventory.AddWeaponInstance(new CampaignWeaponInstanceState("weapon.alpha", "sandbox.rifle", "member.1", true));

            Assert.That(inventory.ItemStacks["medkit.basic"].Count, Is.EqualTo(3));
            Assert.That(inventory.GetResourcePackageAmount("ammo"), Is.EqualTo(8));
            Assert.That(inventory.WeaponInstances.ContainsKey("weapon.alpha"), Is.True);

            Assert.That(inventory.SpendItemStack("medkit.basic", 1), Is.True);
            Assert.That(inventory.SpendResourcePackage("ammo", 3), Is.True);
            Assert.That(inventory.RemoveWeaponInstance("weapon.alpha"), Is.True);

            Assert.That(inventory.ItemStacks["medkit.basic"].Count, Is.EqualTo(2));
            Assert.That(inventory.GetResourcePackageAmount("ammo"), Is.EqualTo(5));
            Assert.That(inventory.WeaponInstances.ContainsKey("weapon.alpha"), Is.False);
        }

        [Test]
        public void BaseSystem_CreateStartingBase_HasCapabilitiesAndMaintenanceConsumesResources()
        {
            CampaignBaseSystem baseSystem = new CampaignBaseSystem();
            CampaignState campaignState = CreateCampaignState();

            campaignState.SetMainBase(baseSystem.CreateStartingBase("base.main", "Main Base", "site.alpha"));
            campaignState.ResourceLedger.Add("building_material", 4);
            campaignState.ResourceLedger.Add("medicine", 2);
            campaignState.ResourceLedger.Add("ammo", 2);
            campaignState.ResourceLedger.Add("food", 1);

            Assert.That(baseSystem.HasCapability(campaignState, "storage"), Is.True);
            Assert.That(baseSystem.HasCapability(campaignState, "infirmary"), Is.True);

            CampaignResourceConsumptionSystem consumptionSystem = new CampaignResourceConsumptionSystem();
            Assert.That(consumptionSystem.ApplyDailyMaintenance(campaignState), Is.True);
            Assert.That(campaignState.MainBase.IsOperational, Is.True);
            Assert.That(campaignState.ResourceLedger.GetAmount("food"), Is.EqualTo(0));
            Assert.That(campaignState.ResourceLedger.GetAmount("medicine"), Is.EqualTo(1));
        }

        [Test]
        public void BaseMaintenance_FlagsBaseWhenResourcesAreMissing()
        {
            CampaignBaseSystem baseSystem = new CampaignBaseSystem();
            CampaignState campaignState = CreateCampaignState();

            campaignState.SetMainBase(baseSystem.CreateStartingBase("base.main", "Main Base", "site.alpha"));
            campaignState.ResourceLedger.Add("food", 1);

            CampaignResourceConsumptionSystem consumptionSystem = new CampaignResourceConsumptionSystem();
            Assert.That(consumptionSystem.ApplyDailyMaintenance(campaignState), Is.False);
            Assert.That(campaignState.MainBase.IsOperational, Is.False);
            Assert.That(campaignState.MainBase.OperationalWarning, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public void SettlementSystem_AppliesResourceRewardsToLedger()
        {
            CampaignState campaignState = CreateCampaignState();
            CampaignSettlementSystem settlementSystem = new CampaignSettlementSystem();
            CampaignSettlement settlement = new CampaignSettlement(
                "mission.test",
                "site.alpha",
                true,
                new List<CampaignCasualtySettlement>(),
                new List<CampaignLootSettlement>(),
                new List<CampaignSiteSettlement>(),
                new List<CampaignSquadSettlement>(),
                new CampaignMissionHistoryRecord("mission.test", "site.alpha", true, 0, 0, 0f),
                new List<CampaignResourceRewardSettlement>
                {
                    new CampaignResourceRewardSettlement("medicine", 3, "mission.test"),
                    new CampaignResourceRewardSettlement("ammo", 2, "mission.test")
                },
                new List<CampaignItemRewardSettlement>(),
                new List<CampaignWeaponRewardSettlement>(),
                new List<CampaignBaseEffectSettlement>(),
                0,
                0);

            settlementSystem.Apply(campaignState, settlement);

            Assert.That(campaignState.ResourceLedger.GetAmount("medicine"), Is.EqualTo(3));
            Assert.That(campaignState.ResourceLedger.GetAmount("ammo"), Is.EqualTo(2));
            Assert.That(campaignState.MissionHistory.Count, Is.EqualTo(1));
        }

        [Test]
        public void SettlementSystem_ZeroRewardsDoesNotChangeLedger()
        {
            CampaignState campaignState = CreateCampaignState();
            CampaignSettlementSystem settlementSystem = new CampaignSettlementSystem();
            CampaignSettlement settlement = new CampaignSettlement(
                "mission.empty",
                "site.alpha",
                false,
                new List<CampaignCasualtySettlement>(),
                new List<CampaignLootSettlement>(),
                new List<CampaignSiteSettlement>(),
                new List<CampaignSquadSettlement>(),
                null,
                new List<CampaignResourceRewardSettlement>(),
                new List<CampaignItemRewardSettlement>(),
                new List<CampaignWeaponRewardSettlement>(),
                new List<CampaignBaseEffectSettlement>(),
                0,
                0);

            settlementSystem.Apply(campaignState, settlement);

            Assert.That(campaignState.ResourceLedger.GetAmount("generic_loot"), Is.EqualTo(0));
        }

        private static CampaignState CreateCampaignState()
        {
            CampaignState campaignState = new CampaignState();
            campaignState.AddMember(new CampaignMemberState("campaign.member.1", "Alpha-1", true, false, true, 0, 1, "sandbox.rifle"));
            campaignState.AddMember(new CampaignMemberState("campaign.member.2", "Alpha-2", true, false, true, 0, 1, "sandbox.rifle"));
            campaignState.AddSquad(new CampaignSquadState(1, "Alpha Squad", new[] { "campaign.member.1", "campaign.member.2" }));
            campaignState.AddSite(new CampaignSiteState("site.alpha", "Alpha Site", true, false, 2, false, "generic loot", 0f));
            return campaignState;
        }
    }
}
