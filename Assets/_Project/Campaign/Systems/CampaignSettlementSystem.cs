using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignSettlementSystem
    {
        private readonly CampaignRosterSystem _rosterSystem = new CampaignRosterSystem();
        private readonly CampaignInventorySystem _inventorySystem = new CampaignInventorySystem();
        private readonly CampaignSiteSystem _siteSystem = new CampaignSiteSystem();

        public void Apply(CampaignState campaignState, CampaignSettlement settlement)
        {
            if (campaignState == null || settlement == null)
            {
                return;
            }

            ApplyCasualties(campaignState, settlement.Casualties);
            ApplyLoot(campaignState, settlement.Loot);
            ApplySites(campaignState, settlement.SiteSettlements);
            ApplySquads(campaignState, settlement.SquadSettlements);

            if (settlement.HistoryRecord != null)
            {
                campaignState.AddMissionHistory(settlement.HistoryRecord);
            }
        }

        private void ApplyCasualties(CampaignState campaignState, IReadOnlyList<CampaignCasualtySettlement> casualties)
        {
            if (casualties == null)
            {
                return;
            }

            for (int i = 0; i < casualties.Count; i++)
            {
                CampaignCasualtySettlement casualty = casualties[i];
                if (casualty == null)
                {
                    continue;
                }

                if (casualty.IsDead)
                {
                    _rosterSystem.MarkMemberDead(campaignState, casualty.CampaignMemberId);
                    continue;
                }

                if (casualty.IsWounded)
                {
                    _rosterSystem.MarkMemberWounded(campaignState, casualty.CampaignMemberId);
                }

                if (casualty.IsAvailable)
                {
                    _rosterSystem.MarkMemberAvailable(campaignState, casualty.CampaignMemberId);
                }
            }
        }

        private void ApplyLoot(CampaignState campaignState, IReadOnlyList<CampaignLootSettlement> loot)
        {
            if (loot == null)
            {
                return;
            }

            for (int i = 0; i < loot.Count; i++)
            {
                CampaignLootSettlement lootEntry = loot[i];
                if (lootEntry == null)
                {
                    continue;
                }

                _inventorySystem.AddItemStack(campaignState, lootEntry.LootId, lootEntry.LootId, lootEntry.Count);
                _inventorySystem.AddGenericLoot(campaignState, lootEntry.Count);
            }
        }

        private void ApplySites(CampaignState campaignState, IReadOnlyList<CampaignSiteSettlement> siteSettlements)
        {
            if (siteSettlements == null)
            {
                return;
            }

            for (int i = 0; i < siteSettlements.Count; i++)
            {
                CampaignSiteSettlement siteSettlement = siteSettlements[i];
                if (siteSettlement == null)
                {
                    continue;
                }

                if (siteSettlement.SearchCompleted)
                {
                    _siteSystem.MarkSearchCompleted(campaignState, siteSettlement.SiteId);
                }

                _siteSystem.MarkVisited(campaignState, siteSettlement.SiteId, siteSettlement.LastVisitedTime);

                if (siteSettlement.IsCleared)
                {
                    _siteSystem.MarkCleared(campaignState, siteSettlement.SiteId);
                    continue;
                }

                _siteSystem.UpdateThreat(campaignState, siteSettlement.SiteId, siteSettlement.ThreatLevel);
            }
        }

        private void ApplySquads(CampaignState campaignState, IReadOnlyList<CampaignSquadSettlement> squadSettlements)
        {
            if (squadSettlements == null)
            {
                return;
            }

            for (int i = 0; i < squadSettlements.Count; i++)
            {
                CampaignSquadSettlement squadSettlement = squadSettlements[i];
                if (squadSettlement == null)
                {
                    continue;
                }

                CampaignSquadState squad;
                if (campaignState.Roster.TryGetSquad(squadSettlement.SquadId, out squad))
                {
                    squad.SetAvailable(squadSettlement.IsAvailable);
                    squad.AddExperience(squadSettlement.ExperienceGained);
                }
            }
        }
    }
}
