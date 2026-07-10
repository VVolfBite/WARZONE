using System.Collections.Generic;
using Warzone.Campaign;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Application
{
    public sealed class MissionLaunchPlanFactory
    {
        private readonly ContentCatalog _contentCatalog;
        private readonly CampaignMissionSystem _campaignMissionSystem = new CampaignMissionSystem();

        public MissionLaunchPlanFactory(ContentCatalog contentCatalog)
        {
            _contentCatalog = contentCatalog;
        }

        public bool TryCreateLaunchPlan(
            CampaignState campaignState,
            string missionId,
            string siteId,
            IReadOnlyList<int> selectedSquadIds,
            int randomSeed,
            out MissionLaunchPlan launchPlan,
            out string reason)
        {
            launchPlan = null;
            reason = null;

            if (_contentCatalog == null)
            {
                reason = "Missing content catalog.";
                return false;
            }

            MissionDefinition missionDefinition;
            if (!_contentCatalog.TryGetMission(missionId, out missionDefinition))
            {
                reason = "Mission not found.";
                return false;
            }

            SiteDefinition siteDefinition;
            if (!_contentCatalog.TryGetSite(siteId, out siteDefinition))
            {
                reason = "Site not found.";
                return false;
            }

            if (!string.IsNullOrEmpty(missionDefinition.RequiredSiteType) &&
                !string.Equals(missionDefinition.RequiredSiteType, siteDefinition.SiteType.ToString(), System.StringComparison.OrdinalIgnoreCase))
            {
                reason = "Mission requires a different site type.";
                return false;
            }

            string lootProfileId = ResolveLootProfileId(missionDefinition, siteDefinition);

            string validationReason;
            if (!_campaignMissionSystem.CanLaunchMission(campaignState, missionId, siteId, selectedSquadIds, out validationReason))
            {
                reason = validationReason;
                return false;
            }

            List<MissionMemberLoadout> memberLoadouts = new List<MissionMemberLoadout>();
            List<MissionSquadLoadout> squadLoadouts = new List<MissionSquadLoadout>();
            Dictionary<int, string> battleMemberIdToCampaignMemberId = new Dictionary<int, string>();

            int nextBattleMemberId = 1000;
            string defaultWeaponId = GetDefaultWeaponId();
            for (int i = 0; i < selectedSquadIds.Count; i++)
            {
                int squadId = selectedSquadIds[i];
                CampaignSquadState squadState;
                if (!campaignState.Roster.TryGetSquad(squadId, out squadState))
                {
                    reason = "Squad not found.";
                    return false;
                }

                List<MissionMemberLoadout> squadMembers = new List<MissionMemberLoadout>();
                for (int j = 0; j < squadState.MemberIds.Count; j++)
                {
                    CampaignMemberState memberState;
                    if (!campaignState.Roster.TryGetMember(squadState.MemberIds[j], out memberState))
                    {
                        reason = "Member not found.";
                        return false;
                    }

                    WeaponSelection weaponSelection;
                    if (!TryResolveWeaponSelection(campaignState, memberState, defaultWeaponId, out weaponSelection, out reason))
                    {
                        return false;
                    }

                    MissionMemberLoadout loadout = new MissionMemberLoadout(
                        memberState.MemberId,
                        nextBattleMemberId++,
                        memberState.DisplayName,
                        squadId,
                        weaponSelection.WeaponDefinitionId,
                        weaponSelection.WeaponInstanceId,
                        memberState.IsAlive ? 100 : 1,
                        4f,
                        12f,
                        1,
                        0,
                        1f,
                        false);
                    memberLoadouts.Add(loadout);
                    squadMembers.Add(loadout);
                    battleMemberIdToCampaignMemberId.Add(loadout.BattleMemberId, loadout.CampaignMemberId);
                }

                squadLoadouts.Add(new MissionSquadLoadout(squadId, squadState.DisplayName, squadMembers));
            }

            CampaignSiteState campaignSiteState;
            campaignState.TryGetSite(siteId, out campaignSiteState);

            MissionSiteContext siteContext = new MissionSiteContext(
                siteDefinition.Id,
                siteDefinition.DisplayName,
                siteDefinition.SiteType,
                siteDefinition.IsEnterable,
                campaignSiteState != null ? campaignSiteState.ThreatLevel : siteDefinition.BaseThreatLevel,
                campaignSiteState != null ? campaignSiteState.SearchCompleted : false,
                campaignSiteState != null ? campaignSiteState.LootRemainingHint : siteDefinition.LootRemainingHint,
                campaignSiteState != null ? campaignSiteState.IsCleared : false,
                campaignSiteState != null ? campaignSiteState.IsExhausted : false,
                campaignSiteState != null ? campaignSiteState.IsOccupied : false,
                campaignSiteState != null ? campaignSiteState.LootRemaining : siteDefinition.InitialLootRemaining,
                campaignSiteState != null ? campaignSiteState.ResourceRichness : siteDefinition.ResourceRichness,
                campaignSiteState != null ? campaignSiteState.VisitCount : 0,
                new Vec2(0f, 0f),
                new Vec2(18f, 0f),
                new Vec2(9f, 1.5f),
                campaignSiteState != null ? campaignSiteState.LastVisitedTime : 0f,
                campaignSiteState != null ? campaignSiteState.OutpostId : null,
                campaignSiteState != null ? campaignSiteState.Tags : siteDefinition.Tags);

            launchPlan = new MissionLaunchPlan(
                missionDefinition.Id,
                siteDefinition.Id,
                missionDefinition,
                siteContext,
                lootProfileId,
                selectedSquadIds,
                squadLoadouts,
                memberLoadouts,
                battleMemberIdToCampaignMemberId,
                missionDefinition.Objectives,
                randomSeed,
                entryPointId: "entry.main");

            return true;
        }

        private string GetDefaultWeaponId()
        {
            foreach (KeyValuePair<string, WeaponDefinition> weapon in _contentCatalog.Weapons)
            {
                return weapon.Key;
            }

            return "sandbox.rifle";
        }

        private static string ResolveLootProfileId(MissionDefinition missionDefinition, SiteDefinition siteDefinition)
        {
            if (missionDefinition != null && missionDefinition.Reward != null)
            {
                if (!string.IsNullOrEmpty(missionDefinition.Reward.LootProfileId))
                {
                    return missionDefinition.Reward.LootProfileId;
                }

                if (!string.IsNullOrEmpty(missionDefinition.Reward.RewardProfileId))
                {
                    return missionDefinition.Reward.RewardProfileId;
                }
            }

            if (siteDefinition != null && !string.IsNullOrEmpty(siteDefinition.DefaultOutpostId))
            {
                return siteDefinition.SiteType.ToString().ToLowerInvariant();
            }

            return siteDefinition != null ? siteDefinition.SiteType.ToString().ToLowerInvariant() : "generic_loot";
        }

        private bool TryResolveWeaponSelection(
            CampaignState campaignState,
            CampaignMemberState memberState,
            string defaultWeaponId,
            out WeaponSelection selection,
            out string reason)
        {
            selection = null;
            reason = null;

            if (campaignState == null || memberState == null)
            {
                reason = "Missing member state.";
                return false;
            }

            if (!string.IsNullOrEmpty(memberState.CarriedWeaponInstanceId))
            {
                CampaignWeaponInstanceState weaponInstance;
                if (!campaignState.Inventory.TryGetWeaponInstance(memberState.CarriedWeaponInstanceId, out weaponInstance))
                {
                    reason = "Assigned weapon instance not found.";
                    return false;
                }

                if (weaponInstance.IsLost || !weaponInstance.IsAvailable)
                {
                    reason = "Assigned weapon instance is not available.";
                    return false;
                }

                if (string.IsNullOrEmpty(weaponInstance.DefinitionId))
                {
                    reason = "Assigned weapon instance is missing definition.";
                    return false;
                }

                selection = new WeaponSelection(weaponInstance.DefinitionId, weaponInstance.InstanceId);
                return true;
            }

            if (!string.IsNullOrEmpty(memberState.CarriedWeaponId))
            {
                WeaponDefinition carriedWeaponDefinition;
                if (_contentCatalog.TryGetWeapon(memberState.CarriedWeaponId, out carriedWeaponDefinition))
                {
                    selection = new WeaponSelection(carriedWeaponDefinition.Id, null);
                    return true;
                }
            }

            CampaignWeaponInstanceState ownedWeaponInstance;
            if (campaignState.Inventory.TryGetWeaponInstanceForMember(memberState.MemberId, out ownedWeaponInstance))
            {
                selection = new WeaponSelection(ownedWeaponInstance.DefinitionId, ownedWeaponInstance.InstanceId);
                return true;
            }

            WeaponDefinition defaultWeaponDefinition;
            if (_contentCatalog.TryGetWeapon(defaultWeaponId, out defaultWeaponDefinition))
            {
                selection = new WeaponSelection(defaultWeaponDefinition.Id, null);
                return true;
            }

            reason = "No available weapon definition.";
            return false;
        }

        private sealed class WeaponSelection
        {
            public WeaponSelection(string weaponDefinitionId, string weaponInstanceId)
            {
                WeaponDefinitionId = weaponDefinitionId;
                WeaponInstanceId = weaponInstanceId;
            }

            public string WeaponDefinitionId { get; private set; }
            public string WeaponInstanceId { get; private set; }
        }
    }
}
