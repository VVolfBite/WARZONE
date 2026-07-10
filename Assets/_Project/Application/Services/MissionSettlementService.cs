using System.Collections.Generic;
using Warzone.Campaign;
using Warzone.Combat;

namespace Warzone.Application.Services
{
    public sealed class MissionSettlementService
    {
        private readonly CampaignSettlementSystem _settlementSystem = new CampaignSettlementSystem();

        public CampaignSettlement ApplyBattleResult(
            CampaignState campaignState,
            MissionLaunchPlan launchPlan,
            BattleResult battleResult)
        {
            if (campaignState == null || launchPlan == null || battleResult == null)
            {
                return null;
            }

            List<CampaignCasualtySettlement> casualties = new List<CampaignCasualtySettlement>();
            for (int i = 0; i < battleResult.CasualtyResult.DeadMemberIds.Count; i++)
            {
                BattleEntityId deadMemberId = battleResult.CasualtyResult.DeadMemberIds[i];
                string campaignMemberId = ResolveCampaignMemberId(launchPlan, deadMemberId);
                casualties.Add(new CampaignCasualtySettlement(campaignMemberId, true, true, false));
            }

            for (int i = 0; i < launchPlan.MemberLoadouts.Count; i++)
            {
                MissionMemberLoadout loadout = launchPlan.MemberLoadouts[i];
                bool isDead = ContainsBattleId(battleResult.CasualtyResult.DeadMemberIds, loadout.BattleMemberId);
                bool isExtracted = ContainsBattleId(battleResult.ExtractionResult.ExtractedMemberIds, loadout.BattleMemberId);
                if (isDead)
                {
                    continue;
                }

                if (isExtracted)
                {
                    casualties.Add(new CampaignCasualtySettlement(loadout.CampaignMemberId, false, false, true));
                    continue;
                }

                casualties.Add(new CampaignCasualtySettlement(loadout.CampaignMemberId, false, true, false));
            }

            List<CampaignLootSettlement> loot = new List<CampaignLootSettlement>
            {
                new CampaignLootSettlement("generic.loot", battleResult.LootResult.LootCount)
            };

            CampaignSiteState siteState;
            campaignState.TryGetSite(launchPlan.SiteId, out siteState);
            List<CampaignSiteSettlement> siteSettlements = new List<CampaignSiteSettlement>
            {
                new CampaignSiteSettlement(
                    launchPlan.SiteId,
                    true,
                    battleResult.MissionOutcome == MissionOutcome.Victory,
                    siteState != null ? (battleResult.MissionOutcome == MissionOutcome.Victory ? 0 : siteState.ThreatLevel) : 0,
                    battleResult.ElapsedTimeSeconds)
            };

            List<CampaignSquadSettlement> squadSettlements = new List<CampaignSquadSettlement>();
            for (int i = 0; i < launchPlan.SelectedSquadIds.Count; i++)
            {
                int squadId = launchPlan.SelectedSquadIds[i];
                bool squadAvailable = battleResult.ExtractionResult.SurvivingMemberCount > 0;
                squadSettlements.Add(new CampaignSquadSettlement(squadId, squadAvailable, battleResult.ObjectiveResults.Count));
            }

            CampaignMissionHistoryRecord history = new CampaignMissionHistoryRecord(
                launchPlan.MissionId,
                launchPlan.SiteId,
                battleResult.MissionOutcome == MissionOutcome.Victory,
                battleResult.CasualtyResult.DeadMemberIds.Count,
                battleResult.LootResult.LootCount,
                battleResult.ElapsedTimeSeconds,
                battleResult.CompletionType.ToString());

            CampaignSettlement settlement = new CampaignSettlement(
                launchPlan.MissionId,
                launchPlan.SiteId,
                battleResult.MissionOutcome == MissionOutcome.Victory,
                casualties,
                loot,
                siteSettlements,
                squadSettlements,
                history,
                battleResult.CasualtyResult.DeadMemberIds.Count,
                battleResult.ExtractionResult.SurvivingMemberCount);

            _settlementSystem.Apply(campaignState, settlement);
            return settlement;
        }

        private static bool ContainsBattleId(IReadOnlyList<BattleEntityId> ids, int battleMemberId)
        {
            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i].Value == battleMemberId)
                {
                    return true;
                }
            }

            return false;
        }

        private static string ResolveCampaignMemberId(MissionLaunchPlan launchPlan, BattleEntityId battleMemberId)
        {
            string campaignMemberId;
            if (launchPlan.BattleMemberIdToCampaignMemberId.TryGetValue(battleMemberId.Value, out campaignMemberId))
            {
                return campaignMemberId;
            }

            return battleMemberId.Value.ToString();
        }
    }
}
