using System.Collections.Generic;

namespace Warzone.Application.Save
{
    public sealed class NamedAmountSaveData
    {
        public NamedAmountSaveData()
        {
        }

        public NamedAmountSaveData(string id, int amount)
        {
            Id = id;
            Amount = amount;
        }

        public string Id { get; set; }
        public int Amount { get; set; }
    }

    public sealed class CurrentMissionSaveData
    {
        public string MissionId { get; set; }
        public string SiteId { get; set; }
        public string DisplayName { get; set; }
        public bool IsAvailable { get; set; }
        public int ThreatLevel { get; set; }
        public string MissionType { get; set; }
    }

    public sealed class MemberSaveData
    {
        public string MemberId { get; set; }
        public string DisplayName { get; set; }
        public bool IsAlive { get; set; }
        public bool IsWounded { get; set; }
        public bool IsAvailable { get; set; }
        public int Experience { get; set; }
        public int Level { get; set; }
        public int MissionsCompleted { get; set; }
        public int Kills { get; set; }
        public string WoundSeverity { get; set; }
        public int RecoveryDaysRemaining { get; set; }
        public bool IsRecovering { get; set; }
        public int SkillPoints { get; set; }
        public float Fatigue { get; set; }
        public string LastInjuryMissionId { get; set; }
        public int? AssignedSquadId { get; set; }
        public string CarriedWeaponId { get; set; }
        public string CarriedWeaponInstanceId { get; set; }
        public string LoadoutId { get; set; }
    }

    public sealed class SquadSaveData
    {
        public SquadSaveData()
        {
            MemberIds = new List<string>();
        }

        public int SquadId { get; set; }
        public string DisplayName { get; set; }
        public List<string> MemberIds { get; set; }
        public bool IsAvailable { get; set; }
        public int Experience { get; set; }
        public float Familiarity { get; set; }
    }

    public sealed class RosterSaveData
    {
        public RosterSaveData()
        {
            Members = new List<MemberSaveData>();
            Squads = new List<SquadSaveData>();
        }

        public List<MemberSaveData> Members { get; set; }
        public List<SquadSaveData> Squads { get; set; }
    }

    public sealed class WeaponInstanceSaveData
    {
        public string InstanceId { get; set; }
        public string DefinitionId { get; set; }
        public string OwnerMemberId { get; set; }
        public string AssignedMemberId { get; set; }
        public bool IsEquipped { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsLost { get; set; }
        public bool IsDamaged { get; set; }
        public float Durability { get; set; }
        public string LastMissionId { get; set; }
    }

    public sealed class ItemStackSaveData
    {
        public string ItemId { get; set; }
        public string DisplayName { get; set; }
        public int Count { get; set; }
    }

    public sealed class InventorySaveData
    {
        public InventorySaveData()
        {
            WeaponInstances = new List<WeaponInstanceSaveData>();
            ItemStacks = new List<ItemStackSaveData>();
            ResourcePackages = new List<NamedAmountSaveData>();
        }

        public List<WeaponInstanceSaveData> WeaponInstances { get; set; }
        public List<ItemStackSaveData> ItemStacks { get; set; }
        public List<NamedAmountSaveData> ResourcePackages { get; set; }
    }

    public sealed class ResourceLedgerSaveData
    {
        public ResourceLedgerSaveData()
        {
            Resources = new List<NamedAmountSaveData>();
        }

        public List<NamedAmountSaveData> Resources { get; set; }
    }

    public sealed class SiteSaveData
    {
        public SiteSaveData()
        {
            Tags = new List<string>();
        }

        public string SiteId { get; set; }
        public string DisplayName { get; set; }
        public string SiteType { get; set; }
        public bool IsDiscovered { get; set; }
        public bool IsCleared { get; set; }
        public int ThreatLevel { get; set; }
        public bool SearchCompleted { get; set; }
        public string LootRemainingHint { get; set; }
        public float LastVisitedTime { get; set; }
        public int MaxThreatLevel { get; set; }
        public int ResourceRichness { get; set; }
        public int LootRemaining { get; set; }
        public bool IsExhausted { get; set; }
        public bool IsOccupied { get; set; }
        public bool CanBecomeOutpost { get; set; }
        public string OutpostId { get; set; }
        public int VisitCount { get; set; }
        public List<string> Tags { get; set; }
    }

    public sealed class BaseModuleSaveData
    {
        public BaseModuleSaveData()
        {
            ProvidedCapabilities = new List<string>();
            DailyResourceCosts = new List<NamedAmountSaveData>();
        }

        public string ModuleId { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public List<string> ProvidedCapabilities { get; set; }
        public List<NamedAmountSaveData> DailyResourceCosts { get; set; }
    }

    public sealed class BaseSaveData
    {
        public BaseSaveData()
        {
            Modules = new List<BaseModuleSaveData>();
        }

        public string BaseId { get; set; }
        public string DisplayName { get; set; }
        public string SiteId { get; set; }
        public int StorageCapacity { get; set; }
        public bool IsOperational { get; set; }
        public string OperationalWarning { get; set; }
        public List<BaseModuleSaveData> Modules { get; set; }
    }

    public sealed class OutpostSaveData
    {
        public OutpostSaveData()
        {
            Capabilities = new List<string>();
            DailyResourceCosts = new List<NamedAmountSaveData>();
        }

        public string OutpostId { get; set; }
        public string SiteId { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public bool ProvidesSafeExtraction { get; set; }
        public bool ReducesLocalThreat { get; set; }
        public int StorageBonus { get; set; }
        public List<string> Capabilities { get; set; }
        public List<NamedAmountSaveData> DailyResourceCosts { get; set; }
    }

    public sealed class MissionHistorySaveData
    {
        public MissionHistorySaveData()
        {
            ResourceRewards = new List<NamedAmountSaveData>();
        }

        public string MissionId { get; set; }
        public string SiteId { get; set; }
        public bool Succeeded { get; set; }
        public int Casualties { get; set; }
        public int Loot { get; set; }
        public float Timestamp { get; set; }
        public string CompletionType { get; set; }
        public List<NamedAmountSaveData> ResourceRewards { get; set; }
    }
}
