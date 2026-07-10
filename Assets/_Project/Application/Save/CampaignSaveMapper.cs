using System.Collections.Generic;
using Warzone.Campaign;

namespace Warzone.Application.Save
{
    public sealed class CampaignSaveMapper
    {
        public SaveGameSnapshot ToSnapshot(CampaignState campaignState, string slotId = null)
        {
            if (campaignState == null)
            {
                return null;
            }

            SaveGameSnapshot snapshot = new SaveGameSnapshot();
            long createdAtUtcTicks = System.DateTime.UtcNow.Ticks;
            snapshot.SaveVersion = SaveGameVersion.Current;
            snapshot.CreatedAtUtcTicks = createdAtUtcTicks;
            snapshot.SlotId = slotId;
            snapshot.CampaignId = null;
            snapshot.Metadata.SaveVersion = SaveGameVersion.Current;
            snapshot.Metadata.CreatedAtUtcTicks = createdAtUtcTicks;
            snapshot.Metadata.SlotId = slotId;
            snapshot.Metadata.CampaignId = null;
            snapshot.Campaign = ToCampaignSaveData(campaignState);
            return snapshot;
        }

        public CampaignState FromSnapshot(SaveGameSnapshot snapshot)
        {
            if (snapshot == null || snapshot.Campaign == null)
            {
                return null;
            }

            return FromCampaignSaveData(snapshot.Campaign);
        }

        private static CampaignSaveData ToCampaignSaveData(CampaignState campaignState)
        {
            CampaignSaveData data = new CampaignSaveData();
            data.CampaignTime = campaignState.CampaignTime;
            data.CurrentMission = ToCurrentMissionSaveData(campaignState.CurrentMission);
            data.Roster = ToRosterSaveData(campaignState.Roster);
            data.Inventory = ToInventorySaveData(campaignState.Inventory);
            data.ResourceLedger = ToResourceLedgerSaveData(campaignState.ResourceLedger);
            data.Sites = ToSiteSaveDataList(campaignState.SitesById);
            data.Base = ToBaseSaveData(campaignState.MainBase);
            data.Outposts = ToOutpostSaveDataList(campaignState.OutpostsById);
            data.MissionHistory = ToMissionHistorySaveDataList(campaignState.MissionHistory);
            return data;
        }

        private static CampaignState FromCampaignSaveData(CampaignSaveData data)
        {
            CampaignState campaignState = new CampaignState(data.CurrentMission != null ? data.CurrentMission.MissionId : null);
            campaignState.SetCampaignTime(data.CampaignTime);

            if (data.ResourceLedger != null && data.ResourceLedger.Resources != null)
            {
                for (int i = 0; i < data.ResourceLedger.Resources.Count; i++)
                {
                    NamedAmountSaveData resource = data.ResourceLedger.Resources[i];
                    if (resource == null || string.IsNullOrEmpty(resource.Id))
                    {
                        continue;
                    }

                    campaignState.ResourceLedger.Set(resource.Id, resource.Amount);
                }
            }

            if (data.Roster != null)
            {
                RestoreMembers(campaignState, data.Roster.Members);
                RestoreSquads(campaignState, data.Roster.Squads);
            }

            if (data.Inventory != null)
            {
                RestoreInventory(campaignState, data.Inventory);
            }

            if (data.Sites != null)
            {
                RestoreSites(campaignState, data.Sites);
            }

            if (data.Outposts != null)
            {
                RestoreOutposts(campaignState, data.Outposts);
            }

            if (data.Base != null)
            {
                campaignState.SetMainBase(FromBaseSaveData(data.Base));
            }

            if (data.CurrentMission != null)
            {
                campaignState.SetCurrentMission(new CampaignMissionState(
                    data.CurrentMission.MissionId,
                    data.CurrentMission.SiteId,
                    data.CurrentMission.DisplayName,
                    data.CurrentMission.IsAvailable,
                    data.CurrentMission.ThreatLevel,
                    data.CurrentMission.MissionType));
            }

            if (data.MissionHistory != null)
            {
                for (int i = 0; i < data.MissionHistory.Count; i++)
                {
                    MissionHistorySaveData historyData = data.MissionHistory[i];
                    if (historyData == null)
                    {
                        continue;
                    }

                    campaignState.AddMissionHistory(FromMissionHistorySaveData(historyData));
                }
            }

            return campaignState;
        }

        private static CurrentMissionSaveData ToCurrentMissionSaveData(CampaignMissionState mission)
        {
            if (mission == null)
            {
                return null;
            }

            return new CurrentMissionSaveData
            {
                MissionId = mission.MissionId,
                SiteId = mission.SiteId,
                DisplayName = mission.DisplayName,
                IsAvailable = mission.IsAvailable,
                ThreatLevel = mission.ThreatLevel,
                MissionType = mission.MissionType
            };
        }

        private static RosterSaveData ToRosterSaveData(CampaignRosterState roster)
        {
            RosterSaveData data = new RosterSaveData();
            if (roster == null)
            {
                return data;
            }

            foreach (KeyValuePair<string, CampaignMemberState> pair in roster.MembersById)
            {
                data.Members.Add(ToMemberSaveData(pair.Value));
            }

            foreach (KeyValuePair<int, CampaignSquadState> pair in roster.SquadsById)
            {
                data.Squads.Add(ToSquadSaveData(pair.Value));
            }

            return data;
        }

        private static MemberSaveData ToMemberSaveData(CampaignMemberState member)
        {
            if (member == null)
            {
                return null;
            }

            return new MemberSaveData
            {
                MemberId = member.MemberId,
                DisplayName = member.DisplayName,
                IsAlive = member.IsAlive,
                IsWounded = member.IsWounded,
                IsAvailable = member.IsAvailable,
                Experience = member.Experience,
                AssignedSquadId = member.AssignedSquadId,
                CarriedWeaponId = member.CarriedWeaponId,
                LoadoutId = member.LoadoutId
            };
        }

        private static SquadSaveData ToSquadSaveData(CampaignSquadState squad)
        {
            if (squad == null)
            {
                return null;
            }

            return new SquadSaveData
            {
                SquadId = squad.SquadId,
                DisplayName = squad.DisplayName,
                MemberIds = new List<string>(squad.MemberIds),
                IsAvailable = squad.IsAvailable,
                Experience = squad.Experience,
                Familiarity = squad.Familiarity
            };
        }

        private static InventorySaveData ToInventorySaveData(CampaignInventoryState inventory)
        {
            InventorySaveData data = new InventorySaveData();
            if (inventory == null)
            {
                return data;
            }

            foreach (KeyValuePair<string, CampaignWeaponInstanceState> pair in inventory.WeaponInstances)
            {
                CampaignWeaponInstanceState instance = pair.Value;
                if (instance == null)
                {
                    continue;
                }

                data.WeaponInstances.Add(new WeaponInstanceSaveData
                {
                    InstanceId = instance.InstanceId,
                    DefinitionId = instance.DefinitionId,
                    OwnerMemberId = instance.OwnerMemberId,
                    IsEquipped = instance.IsEquipped
                });
            }

            foreach (KeyValuePair<string, CampaignItemStackState> pair in inventory.ItemStacks)
            {
                CampaignItemStackState stack = pair.Value;
                if (stack == null)
                {
                    continue;
                }

                data.ItemStacks.Add(new ItemStackSaveData
                {
                    ItemId = stack.ItemId,
                    DisplayName = stack.DisplayName,
                    Count = stack.Count
                });
            }

            foreach (KeyValuePair<string, int> pair in inventory.ResourcePackages)
            {
                data.ResourcePackages.Add(new NamedAmountSaveData(pair.Key, pair.Value));
            }

            return data;
        }

        private static ResourceLedgerSaveData ToResourceLedgerSaveData(CampaignResourceLedgerState resourceLedger)
        {
            ResourceLedgerSaveData data = new ResourceLedgerSaveData();
            if (resourceLedger == null)
            {
                return data;
            }

            foreach (KeyValuePair<string, int> pair in resourceLedger.Resources)
            {
                data.Resources.Add(new NamedAmountSaveData(pair.Key, pair.Value));
            }

            return data;
        }

        private static List<SiteSaveData> ToSiteSaveDataList(IReadOnlyDictionary<string, CampaignSiteState> sitesById)
        {
            List<SiteSaveData> data = new List<SiteSaveData>();
            if (sitesById == null)
            {
                return data;
            }

            foreach (KeyValuePair<string, CampaignSiteState> pair in sitesById)
            {
                data.Add(ToSiteSaveData(pair.Value));
            }

            return data;
        }

        private static SiteSaveData ToSiteSaveData(CampaignSiteState site)
        {
            if (site == null)
            {
                return null;
            }

            return new SiteSaveData
            {
                SiteId = site.SiteId,
                DisplayName = site.DisplayName,
                SiteType = site.SiteType.ToString(),
                IsDiscovered = site.IsDiscovered,
                IsCleared = site.IsCleared,
                ThreatLevel = site.ThreatLevel,
                SearchCompleted = site.SearchCompleted,
                LootRemainingHint = site.LootRemainingHint,
                LastVisitedTime = site.LastVisitedTime,
                MaxThreatLevel = site.MaxThreatLevel,
                ResourceRichness = site.ResourceRichness,
                LootRemaining = site.LootRemaining,
                IsExhausted = site.IsExhausted,
                IsOccupied = site.IsOccupied,
                CanBecomeOutpost = site.CanBecomeOutpost,
                OutpostId = site.OutpostId,
                VisitCount = site.VisitCount,
                Tags = new List<string>(site.Tags)
            };
        }

        private static BaseSaveData ToBaseSaveData(CampaignBaseState baseState)
        {
            if (baseState == null)
            {
                return null;
            }

            BaseSaveData data = new BaseSaveData
            {
                BaseId = baseState.BaseId,
                DisplayName = baseState.DisplayName,
                SiteId = baseState.SiteId,
                StorageCapacity = baseState.StorageCapacity,
                IsOperational = baseState.IsOperational,
                OperationalWarning = baseState.OperationalWarning
            };

            foreach (KeyValuePair<string, CampaignBaseModuleState> pair in baseState.ModuleStates)
            {
                data.Modules.Add(ToBaseModuleSaveData(pair.Value));
            }

            return data;
        }

        private static BaseModuleSaveData ToBaseModuleSaveData(CampaignBaseModuleState module)
        {
            if (module == null)
            {
                return null;
            }

            BaseModuleSaveData data = new BaseModuleSaveData
            {
                ModuleId = module.ModuleId,
                DisplayName = module.DisplayName,
                IsActive = module.IsActive
            };

            foreach (string capability in module.ProvidedCapabilities)
            {
                data.ProvidedCapabilities.Add(capability);
            }

            foreach (KeyValuePair<string, int> cost in module.DailyResourceCosts)
            {
                data.DailyResourceCosts.Add(new NamedAmountSaveData(cost.Key, cost.Value));
            }

            return data;
        }

        private static List<OutpostSaveData> ToOutpostSaveDataList(IReadOnlyDictionary<string, CampaignOutpostState> outpostsById)
        {
            List<OutpostSaveData> data = new List<OutpostSaveData>();
            if (outpostsById == null)
            {
                return data;
            }

            foreach (KeyValuePair<string, CampaignOutpostState> pair in outpostsById)
            {
                data.Add(ToOutpostSaveData(pair.Value));
            }

            return data;
        }

        private static OutpostSaveData ToOutpostSaveData(CampaignOutpostState outpost)
        {
            if (outpost == null)
            {
                return null;
            }

            OutpostSaveData data = new OutpostSaveData
            {
                OutpostId = outpost.OutpostId,
                SiteId = outpost.SiteId,
                DisplayName = outpost.DisplayName,
                IsActive = outpost.IsActive,
                ProvidesSafeExtraction = outpost.ProvidesSafeExtraction,
                ReducesLocalThreat = outpost.ReducesLocalThreat,
                StorageBonus = outpost.StorageBonus
            };

            foreach (string capability in outpost.Capabilities)
            {
                data.Capabilities.Add(capability);
            }

            foreach (KeyValuePair<string, int> cost in outpost.DailyResourceCosts)
            {
                data.DailyResourceCosts.Add(new NamedAmountSaveData(cost.Key, cost.Value));
            }

            return data;
        }

        private static List<MissionHistorySaveData> ToMissionHistorySaveDataList(IReadOnlyList<CampaignMissionHistoryRecord> records)
        {
            List<MissionHistorySaveData> data = new List<MissionHistorySaveData>();
            if (records == null)
            {
                return data;
            }

            for (int i = 0; i < records.Count; i++)
            {
                data.Add(ToMissionHistorySaveData(records[i]));
            }

            return data;
        }

        private static MissionHistorySaveData ToMissionHistorySaveData(CampaignMissionHistoryRecord record)
        {
            if (record == null)
            {
                return null;
            }

            MissionHistorySaveData data = new MissionHistorySaveData
            {
                MissionId = record.MissionId,
                SiteId = record.SiteId,
                Succeeded = record.Succeeded,
                Casualties = record.Casualties,
                Loot = record.Loot,
                Timestamp = record.Timestamp,
                CompletionType = record.CompletionType
            };

            foreach (KeyValuePair<string, int> reward in record.ResourceRewards)
            {
                data.ResourceRewards.Add(new NamedAmountSaveData(reward.Key, reward.Value));
            }

            return data;
        }

        private static void RestoreMembers(CampaignState campaignState, IReadOnlyList<MemberSaveData> members)
        {
            if (campaignState == null || members == null)
            {
                return;
            }

            for (int i = 0; i < members.Count; i++)
            {
                MemberSaveData memberData = members[i];
                if (memberData == null || string.IsNullOrEmpty(memberData.MemberId))
                {
                    continue;
                }

                CampaignMemberState member = new CampaignMemberState(
                    memberData.MemberId,
                    memberData.DisplayName,
                    memberData.IsAlive,
                    memberData.IsWounded,
                    memberData.IsAvailable,
                    memberData.Experience,
                    memberData.AssignedSquadId,
                    memberData.CarriedWeaponId,
                    memberData.LoadoutId);
                campaignState.AddMember(member);
            }
        }

        private static void RestoreSquads(CampaignState campaignState, IReadOnlyList<SquadSaveData> squads)
        {
            if (campaignState == null || squads == null)
            {
                return;
            }

            for (int i = 0; i < squads.Count; i++)
            {
                SquadSaveData squadData = squads[i];
                if (squadData == null)
                {
                    continue;
                }

                CampaignSquadState squad = new CampaignSquadState(
                    squadData.SquadId,
                    squadData.DisplayName,
                    squadData.MemberIds,
                    squadData.IsAvailable,
                    squadData.Experience,
                    squadData.Familiarity);
                campaignState.AddSquad(squad);
            }

            foreach (KeyValuePair<string, CampaignMemberState> pair in campaignState.Roster.MembersById)
            {
                CampaignMemberState member = pair.Value;
                if (member == null || member.AssignedSquadId == null)
                {
                    continue;
                }

                CampaignSquadState squad;
                if (campaignState.Roster.TryGetSquad(member.AssignedSquadId.Value, out squad) && squad != null && !ContainsMemberId(squad.MemberIds, member.MemberId))
                {
                    List<string> memberIds = new List<string>(squad.MemberIds);
                    memberIds.Add(member.MemberId);
                    squad.SyncMemberIds(memberIds);
                }
            }
        }

        private static void RestoreInventory(CampaignState campaignState, InventorySaveData inventory)
        {
            if (campaignState == null || inventory == null)
            {
                return;
            }

            if (inventory.WeaponInstances != null)
            {
                for (int i = 0; i < inventory.WeaponInstances.Count; i++)
                {
                    WeaponInstanceSaveData weaponData = inventory.WeaponInstances[i];
                    if (weaponData == null || string.IsNullOrEmpty(weaponData.InstanceId) || string.IsNullOrEmpty(weaponData.DefinitionId))
                    {
                        continue;
                    }

                    campaignState.Inventory.AddWeaponInstance(new CampaignWeaponInstanceState(
                        weaponData.InstanceId,
                        weaponData.DefinitionId,
                        weaponData.OwnerMemberId,
                        weaponData.IsEquipped));
                }
            }

            if (inventory.ItemStacks != null)
            {
                for (int i = 0; i < inventory.ItemStacks.Count; i++)
                {
                    ItemStackSaveData itemData = inventory.ItemStacks[i];
                    if (itemData == null || string.IsNullOrEmpty(itemData.ItemId))
                    {
                        continue;
                    }

                    campaignState.Inventory.AddItemStack(new CampaignItemStackState(itemData.ItemId, itemData.DisplayName, itemData.Count));
                }
            }

            if (inventory.ResourcePackages != null)
            {
                for (int i = 0; i < inventory.ResourcePackages.Count; i++)
                {
                    NamedAmountSaveData resourcePackage = inventory.ResourcePackages[i];
                    if (resourcePackage == null || string.IsNullOrEmpty(resourcePackage.Id))
                    {
                        continue;
                    }

                    campaignState.Inventory.AddResourcePackage(resourcePackage.Id, resourcePackage.Amount);
                }
            }
        }

        private static void RestoreSites(CampaignState campaignState, IReadOnlyList<SiteSaveData> sites)
        {
            if (campaignState == null || sites == null)
            {
                return;
            }

            for (int i = 0; i < sites.Count; i++)
            {
                SiteSaveData siteData = sites[i];
                if (siteData == null || string.IsNullOrEmpty(siteData.SiteId))
                {
                    continue;
                }

                Warzone.Content.Definitions.SiteType siteType;
                if (!System.Enum.TryParse(siteData.SiteType, out siteType))
                {
                    siteType = Warzone.Content.Definitions.SiteType.Outpost;
                }

                CampaignSiteState site = new CampaignSiteState(
                    siteData.SiteId,
                    siteData.DisplayName,
                    siteType,
                    siteData.IsDiscovered,
                    siteData.IsCleared,
                    siteData.ThreatLevel,
                    siteData.SearchCompleted,
                    siteData.LootRemainingHint,
                    siteData.LastVisitedTime,
                    siteData.MaxThreatLevel,
                    siteData.ResourceRichness,
                    siteData.LootRemaining,
                    siteData.IsExhausted,
                    siteData.IsOccupied,
                    siteData.CanBecomeOutpost,
                    siteData.OutpostId,
                    siteData.Tags);

                if (siteData.VisitCount > 0)
                {
                    site.MarkVisited(siteData.LastVisitedTime, siteData.VisitCount);
                }
                else if (siteData.IsDiscovered || siteData.IsCleared)
                {
                    site.MarkDiscovered();
                }

                if (siteData.SearchCompleted)
                {
                    site.MarkSearched();
                }

                if (siteData.IsCleared)
                {
                    site.MarkCleared();
                    site.SetThreatLevel(siteData.ThreatLevel);
                }
                else
                {
                    site.SetThreatLevel(siteData.ThreatLevel);
                }

                site.SetOccupied(siteData.IsOccupied);

                if (siteData.IsExhausted)
                {
                    site.MarkExhausted();
                }

                if (!string.IsNullOrEmpty(siteData.OutpostId))
                {
                    site.SetOutpost(siteData.OutpostId);
                }

                campaignState.AddSite(site);
            }
        }

        private static void RestoreOutposts(CampaignState campaignState, IReadOnlyList<OutpostSaveData> outposts)
        {
            if (campaignState == null || outposts == null)
            {
                return;
            }

            for (int i = 0; i < outposts.Count; i++)
            {
                OutpostSaveData outpostData = outposts[i];
                if (outpostData == null || string.IsNullOrEmpty(outpostData.OutpostId))
                {
                    continue;
                }

                CampaignOutpostState outpost = new CampaignOutpostState(
                    outpostData.OutpostId,
                    outpostData.SiteId,
                    outpostData.DisplayName,
                    outpostData.IsActive,
                    outpostData.ProvidesSafeExtraction,
                    outpostData.ReducesLocalThreat,
                    outpostData.StorageBonus,
                    outpostData.Capabilities,
                    ToCostDictionary(outpostData.DailyResourceCosts));
                campaignState.AddOutpost(outpost);

                CampaignSiteState site;
                if (campaignState.TryGetSite(outpostData.SiteId, out site))
                {
                    site.SetOutpost(outpostData.OutpostId);
                }
            }
        }

        private static CampaignBaseState FromBaseSaveData(BaseSaveData data)
        {
            if (data == null)
            {
                return null;
            }

            CampaignBaseState baseState = new CampaignBaseState(data.BaseId, data.DisplayName, data.SiteId, data.StorageCapacity, data.IsOperational);
            baseState.SetOperational(data.IsOperational, data.OperationalWarning);

            if (data.Modules != null)
            {
                for (int i = 0; i < data.Modules.Count; i++)
                {
                    BaseModuleSaveData moduleData = data.Modules[i];
                    if (moduleData == null || string.IsNullOrEmpty(moduleData.ModuleId))
                    {
                        continue;
                    }

                    CampaignBaseModuleState module = new CampaignBaseModuleState(
                        moduleData.ModuleId,
                        moduleData.DisplayName,
                        moduleData.IsActive,
                        moduleData.ProvidedCapabilities,
                        ToCostDictionary(moduleData.DailyResourceCosts));
                    baseState.AddModule(module);
                }
            }

            return baseState;
        }

        private static CampaignMissionHistoryRecord FromMissionHistorySaveData(MissionHistorySaveData data)
        {
            Dictionary<string, int> resourceRewards = ToCostDictionary(data.ResourceRewards);
            return new CampaignMissionHistoryRecord(
                data.MissionId,
                data.SiteId,
                data.Succeeded,
                data.Casualties,
                data.Loot,
                data.Timestamp,
                data.CompletionType,
                resourceRewards);
        }

        private static Dictionary<string, int> ToCostDictionary(IReadOnlyList<NamedAmountSaveData> amounts)
        {
            Dictionary<string, int> costs = new Dictionary<string, int>();
            if (amounts == null)
            {
                return costs;
            }

            for (int i = 0; i < amounts.Count; i++)
            {
                NamedAmountSaveData amount = amounts[i];
                if (amount == null || string.IsNullOrEmpty(amount.Id))
                {
                    continue;
                }

                costs[amount.Id] = amount.Amount;
            }

            return costs;
        }

        private static bool ContainsMemberId(IReadOnlyList<string> memberIds, string memberId)
        {
            if (memberIds == null || string.IsNullOrEmpty(memberId))
            {
                return false;
            }

            for (int i = 0; i < memberIds.Count; i++)
            {
                if (memberIds[i] == memberId)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
