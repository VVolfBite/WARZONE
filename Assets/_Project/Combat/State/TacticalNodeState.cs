using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class TacticalNodeState
    {
        public TacticalNodeState(
            int nodeId,
            TacticalNodeType nodeType,
            Vec2 position,
            float radius,
            bool isEnabled = true,
            float requiredSearchSeconds = 3f,
            int? extractionOwnerSquadId = null,
            int? buildingId = null,
            bool isInsideBuilding = false,
            bool allowsFireThrough = false,
            bool allowsVisionThrough = false,
            bool isEntryPoint = false,
            bool isSearchPoint = false)
        {
            NodeId = nodeId;
            NodeType = nodeType;
            Position = position;
            Radius = radius;
            IsEnabled = isEnabled;
            RequiredSearchSeconds = requiredSearchSeconds < 0f ? 0f : requiredSearchSeconds;
            ExtractionOwnerSquadId = extractionOwnerSquadId;
            BuildingId = buildingId;
            IsInsideBuilding = isInsideBuilding;
            AllowsFireThrough = allowsFireThrough;
            AllowsVisionThrough = allowsVisionThrough;
            IsEntryPoint = isEntryPoint;
            IsSearchPoint = isSearchPoint || nodeType == TacticalNodeType.SearchPoint;
        }

        public int NodeId { get; private set; }
        public TacticalNodeType NodeType { get; private set; }
        public Vec2 Position { get; private set; }
        public float Radius { get; private set; }
        public bool IsEnabled { get; private set; }
        public BattleEntityId? OccupyingMemberId { get; private set; }
        public BattleEntityId? ReservedByMemberId { get; private set; }
        public bool IsReserved { get; private set; }
        public float SearchProgress { get; private set; }
        public bool SearchStarted { get; private set; }
        public bool IsSearched { get; private set; }
        public bool LootDiscovered { get; private set; }
        public float RequiredSearchSeconds { get; private set; }
        public int? ExtractionOwnerSquadId { get; private set; }
        public int? BuildingId { get; private set; }
        public bool IsInsideBuilding { get; private set; }
        public bool AllowsFireThrough { get; private set; }
        public bool AllowsVisionThrough { get; private set; }
        public bool IsEntryPoint { get; private set; }
        public bool IsSearchPoint { get; private set; }

        public bool IsAvailable
        {
            get { return IsEnabled && !IsReserved && !OccupyingMemberId.HasValue; }
        }

        public void SetEnabled(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        public void ReserveFor(BattleEntityId memberId)
        {
            IsReserved = true;
            ReservedByMemberId = memberId;
        }

        public void ClearReservation()
        {
            IsReserved = false;
            ReservedByMemberId = null;
        }

        public void SetOccupyingMember(BattleEntityId? memberId)
        {
            OccupyingMemberId = memberId;
        }

        public void MarkSearchStarted()
        {
            SearchStarted = true;
        }

        public void AdvanceSearch(float deltaTimeSeconds)
        {
            if (IsSearched || deltaTimeSeconds <= 0f)
            {
                return;
            }

            SearchProgress += deltaTimeSeconds;
            if (SearchProgress > RequiredSearchSeconds)
            {
                SearchProgress = RequiredSearchSeconds;
            }
        }

        public void MarkSearched()
        {
            IsSearched = true;
            SearchStarted = true;
            SearchProgress = RequiredSearchSeconds;
        }

        public void MarkLootDiscovered()
        {
            LootDiscovered = true;
        }
    }
}
