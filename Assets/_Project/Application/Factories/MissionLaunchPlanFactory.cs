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

                    string weaponId = string.IsNullOrEmpty(memberState.CarriedWeaponId) ? defaultWeaponId : memberState.CarriedWeaponId;
                    MissionMemberLoadout loadout = new MissionMemberLoadout(
                        memberState.MemberId,
                        nextBattleMemberId++,
                        memberState.DisplayName,
                        squadId,
                        weaponId,
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

            MissionSiteContext siteContext = new MissionSiteContext(
                siteDefinition.Id,
                siteDefinition.DisplayName,
                siteDefinition.SiteType,
                siteDefinition.IsEnterable,
                siteDefinition.BaseThreatLevel,
                false,
                siteDefinition.LootRemainingHint,
                new Vec2(0f, 0f),
                new Vec2(18f, 0f),
                new Vec2(9f, 1.5f),
                0f);

            launchPlan = new MissionLaunchPlan(
                missionDefinition.Id,
                siteDefinition.Id,
                missionDefinition,
                siteContext,
                selectedSquadIds,
                squadLoadouts,
                memberLoadouts,
                battleMemberIdToCampaignMemberId,
                missionDefinition.Objectives,
                randomSeed,
                entryPointId: "entry.main");

            campaignState.SetCurrentMission(new CampaignMissionState(
                missionDefinition.Id,
                siteDefinition.Id,
                missionDefinition.DisplayName,
                true,
                siteDefinition.BaseThreatLevel,
                missionDefinition.MissionType.ToString()));
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
    }
}
