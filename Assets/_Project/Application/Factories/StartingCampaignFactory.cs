using System.Collections.Generic;
using Warzone.Campaign;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.Application
{
    public sealed class StartingCampaignFactory
    {
        private readonly ContentCatalog _contentCatalog;
        private readonly CampaignBaseSystem _baseSystem = new CampaignBaseSystem();

        public StartingCampaignFactory()
            : this(DemoContentFactory.CreateDemoCatalog())
        {
        }

        public StartingCampaignFactory(ContentCatalog contentCatalog)
        {
            _contentCatalog = contentCatalog ?? DemoContentFactory.CreateDemoCatalog();
        }

        public CampaignState CreateStartingCampaign()
        {
            CampaignState campaignState = new CampaignState();

            WeaponDefinition rifleDefinition = GetWeapon("rifle") ?? new WeaponDefinition(
                "rifle",
                "Rifle",
                WeaponCategory.Rifle,
                AmmoCategory.Rifle,
                FireMode.Automatic,
                12f,
                0.45f,
                18,
                0.95f,
                1,
                0.05f,
                16f,
                DamageType.Kinetic);

            CampaignMemberState member1 = new CampaignMemberState("campaign.member.1", "Alpha-1", true, false, true, 0, 1, rifleDefinition.Id, "loadout.alpha.1");
            CampaignMemberState member2 = new CampaignMemberState("campaign.member.2", "Alpha-2", true, false, true, 0, 1, rifleDefinition.Id, "loadout.alpha.2");
            CampaignMemberState member3 = new CampaignMemberState("campaign.member.3", "Alpha-3", true, false, true, 0, 1, rifleDefinition.Id, "loadout.alpha.3");
            CampaignMemberState member4 = new CampaignMemberState("campaign.member.4", "Alpha-4", true, false, true, 0, 1, rifleDefinition.Id, "loadout.alpha.4");

            campaignState.AddMember(member1);
            campaignState.AddMember(member2);
            campaignState.AddMember(member3);
            campaignState.AddMember(member4);

            CampaignSquadState squad = new CampaignSquadState(1, "Alpha Squad", new[] { member1.MemberId, member2.MemberId, member3.MemberId, member4.MemberId });
            campaignState.AddSquad(squad);

            campaignState.Inventory.AddWeaponInstance(new CampaignWeaponInstanceState("weapon.alpha.rifle", rifleDefinition.Id, member1.MemberId, true));
            campaignState.Inventory.AddItemStack(new CampaignItemStackState("medical_supplies", "Medical Supplies", 2));
            campaignState.Inventory.AddItemStack(new CampaignItemStackState("ammunition_box", "Ammunition Box", 4));

            campaignState.ResourceLedger.Add("food", 12);
            campaignState.ResourceLedger.Add("medicine", 8);
            campaignState.ResourceLedger.Add("ammo", 18);
            campaignState.ResourceLedger.Add("fuel", 6);
            campaignState.ResourceLedger.Add("building_material", 4);
            campaignState.ResourceLedger.Add("modification_material", 2);

            campaignState.AddSite(CreateSiteState("medical_clinic", "Alpha Medical Clinic", true, true, 1, true, 4, 6, 4, false, false, true, "basic_outpost", new[] { "medical", "clinic" }));
            campaignState.AddSite(CreateSiteState("supply_depot", "Beta Supply Depot", true, false, 3, false, 6, 4, 6, false, false, true, null, new[] { "supply", "depot" }));
            campaignState.AddSite(CreateSiteState("residential_block", "Gamma Residential Block", true, false, 2, false, 3, 3, 3, false, false, true, null, new[] { "residential" }));

            campaignState.AddOutpost(new CampaignOutpostState(
                "basic_outpost",
                "medical_clinic",
                "Alpha Medical Clinic Outpost",
                true,
                true,
                true,
                20,
                new[] { "safe_extraction", "storage", "watch" },
                new Dictionary<string, int> { { "building_material", 1 }, { "food", 1 } }));

            campaignState.SetMainBase(
                _baseSystem.CreateStartingBase(
                    "base.main",
                    "Main Base",
                    "medical_clinic",
                    ResolveStartingBaseModules()));

            return campaignState;
        }

        private List<BaseModuleDefinition> ResolveStartingBaseModules()
        {
            List<BaseModuleDefinition> modules = new List<BaseModuleDefinition>();
            AddModuleIfFound(modules, "storage");
            AddModuleIfFound(modules, "infirmary");
            AddModuleIfFound(modules, "workshop");
            AddModuleIfFound(modules, "training");
            AddModuleIfFound(modules, "watchtower");
            return modules;
        }

        private void AddModuleIfFound(List<BaseModuleDefinition> modules, string moduleId)
        {
            BaseModuleDefinition moduleDefinition;
            if (_contentCatalog.TryGetBaseModule(moduleId, out moduleDefinition))
            {
                modules.Add(moduleDefinition);
            }
        }

        private CampaignSiteState CreateSiteState(
            string siteId,
            string displayName,
            bool discovered,
            bool cleared,
            int threatLevel,
            bool searchCompleted,
            int lootRemaining,
            int resourceRichness,
            int maxThreatLevel,
            bool exhausted,
            bool occupied,
            bool canBecomeOutpost,
            string outpostId,
            IReadOnlyList<string> tags)
        {
            SiteDefinition siteDefinition;
            if (_contentCatalog.TryGetSite(siteId, out siteDefinition))
            {
                return new CampaignSiteState(
                    siteDefinition.Id,
                    siteDefinition.DisplayName,
                    siteDefinition.SiteType,
                    discovered,
                    cleared,
                    threatLevel,
                    searchCompleted,
                    siteDefinition.LootRemainingHint,
                    0f,
                    maxThreatLevel,
                    resourceRichness,
                    lootRemaining,
                    exhausted,
                    occupied,
                    canBecomeOutpost && siteDefinition.CanBecomeOutpost,
                    outpostId,
                    tags);
            }

            return new CampaignSiteState(
                siteId,
                displayName,
                SiteType.Outpost,
                discovered,
                cleared,
                threatLevel,
                searchCompleted,
                "generic_loot",
                0f,
                maxThreatLevel,
                resourceRichness,
                lootRemaining,
                exhausted,
                occupied,
                canBecomeOutpost,
                outpostId,
                tags);
        }

        private WeaponDefinition GetWeapon(string weaponId)
        {
            WeaponDefinition weaponDefinition;
            if (_contentCatalog.TryGetWeapon(weaponId, out weaponDefinition))
            {
                return weaponDefinition;
            }

            return null;
        }
    }
}
