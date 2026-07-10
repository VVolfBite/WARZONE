using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignSettlementSystem
    {
        private readonly CampaignRosterSystem _rosterSystem = new CampaignRosterSystem();
        private readonly CampaignInventorySystem _inventorySystem = new CampaignInventorySystem();
        private readonly CampaignSiteSystem _siteSystem = new CampaignSiteSystem();
        private readonly CampaignBaseSystem _baseSystem = new CampaignBaseSystem();

        public void Apply(CampaignState campaignState, CampaignSettlement settlement)
        {
            if (campaignState == null || settlement == null)
            {
                return;
            }

            ApplyCasualties(campaignState, settlement.Casualties);
            ApplyRewards(campaignState, settlement);
            ApplySites(campaignState, settlement.SiteSettlements);
            ApplySquads(campaignState, settlement.SquadSettlements);
            ApplyBaseEffects(campaignState, settlement.BaseEffects);

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

        private void ApplyRewards(CampaignState campaignState, CampaignSettlement settlement)
        {
            if (settlement == null)
            {
                return;
            }

            bool appliedResourceRewards = false;
            if (settlement.ResourceRewards != null && settlement.ResourceRewards.Count > 0)
            {
                for (int i = 0; i < settlement.ResourceRewards.Count; i++)
                {
                    CampaignResourceRewardSettlement reward = settlement.ResourceRewards[i];
                    if (reward == null || string.IsNullOrEmpty(reward.ResourceId) || reward.Count <= 0)
                    {
                        continue;
                    }

                    campaignState.ResourceLedger.Add(reward.ResourceId, reward.Count);
                    appliedResourceRewards = true;
                }
            }

            if (!appliedResourceRewards && settlement.Loot != null && settlement.Loot.Count > 0)
            {
                for (int i = 0; i < settlement.Loot.Count; i++)
                {
                    CampaignLootSettlement lootEntry = settlement.Loot[i];
                    if (lootEntry == null || lootEntry.Count <= 0)
                    {
                        continue;
                    }

                    campaignState.ResourceLedger.Add("generic_loot", lootEntry.Count);
                }
            }

            if (settlement.ItemRewards != null)
            {
                for (int i = 0; i < settlement.ItemRewards.Count; i++)
                {
                    CampaignItemRewardSettlement itemReward = settlement.ItemRewards[i];
                    if (itemReward == null || string.IsNullOrEmpty(itemReward.ItemId) || itemReward.Count <= 0)
                    {
                        continue;
                    }

                    _inventorySystem.AddItemStack(campaignState, itemReward.ItemId, itemReward.DisplayName, itemReward.Count);
                }
            }

            if (settlement.WeaponRewards != null)
            {
                for (int i = 0; i < settlement.WeaponRewards.Count; i++)
                {
                    CampaignWeaponRewardSettlement weaponReward = settlement.WeaponRewards[i];
                    if (weaponReward == null || string.IsNullOrEmpty(weaponReward.InstanceId) || string.IsNullOrEmpty(weaponReward.DefinitionId))
                    {
                        continue;
                    }

                    _inventorySystem.AddWeaponInstance(
                        campaignState,
                        new CampaignWeaponInstanceState(weaponReward.InstanceId, weaponReward.DefinitionId, weaponReward.OwnerMemberId, weaponReward.IsEquipped));
                }
            }
        }

        private void ApplyBaseEffects(CampaignState campaignState, IReadOnlyList<CampaignBaseEffectSettlement> baseEffects)
        {
            if (campaignState == null || baseEffects == null)
            {
                return;
            }

            for (int i = 0; i < baseEffects.Count; i++)
            {
                CampaignBaseEffectSettlement effect = baseEffects[i];
                if (effect == null || campaignState.MainBase == null)
                {
                    continue;
                }

                if (campaignState.MainBase.BaseId == effect.BaseId)
                {
                    _baseSystem.SetOperational(campaignState, effect.IsOperational, effect.Warning);
                }
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

                _siteSystem.MarkVisited(campaignState, siteSettlement.SiteId, siteSettlement.LastVisitedTime, siteSettlement.VisitCountDelta);

                if (siteSettlement.SearchCompleted)
                {
                    _siteSystem.MarkSearched(campaignState, siteSettlement.SiteId);
                }

                if (siteSettlement.LootRemainingDelta > 0)
                {
                    _siteSystem.ReduceLoot(campaignState, siteSettlement.SiteId, siteSettlement.LootRemainingDelta);
                }
                else if (siteSettlement.ResourceRichnessDelta > 0)
                {
                    CampaignSiteState site;
                    if (_siteSystem.TryGetSite(campaignState, siteSettlement.SiteId, out site))
                    {
                        site.SetResourceRichness(site.ResourceRichness - siteSettlement.ResourceRichnessDelta);
                        if (site.ResourceRichness <= 0)
                        {
                            site.MarkExhausted();
                        }
                    }
                }

                if (siteSettlement.IsCleared)
                {
                    _siteSystem.MarkCleared(campaignState, siteSettlement.SiteId);
                }
                else
                {
                    _siteSystem.UpdateThreat(campaignState, siteSettlement.SiteId, siteSettlement.ThreatLevel);
                }

                _siteSystem.SetOccupied(campaignState, siteSettlement.SiteId, siteSettlement.IsOccupied);

                if (siteSettlement.IsExhausted)
                {
                    _siteSystem.MarkExhausted(campaignState, siteSettlement.SiteId);
                }

                if (!string.IsNullOrEmpty(siteSettlement.OutpostId))
                {
                    CampaignSiteState site;
                    if (_siteSystem.TryGetSite(campaignState, siteSettlement.SiteId, out site))
                    {
                        site.SetOutpost(siteSettlement.OutpostId);
                    }
                }
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
