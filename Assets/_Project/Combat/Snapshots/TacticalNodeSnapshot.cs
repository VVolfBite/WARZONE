using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class TacticalNodeSnapshot
    {
        public TacticalNodeSnapshot(
            int nodeId,
            TacticalNodeType nodeType,
            Vec2 position,
            float radius,
            bool isEnabled,
            bool isSearched,
            float searchProgress,
            float requiredSearchSeconds,
            BattleEntityId? occupyingMemberId,
            BattleEntityId? reservedByMemberId,
            int? buildingId,
            bool isInsideBuilding,
            bool allowsFireThrough,
            bool allowsVisionThrough,
            bool isEntryPoint,
            bool isSearchPoint)
        {
            NodeId = nodeId;
            NodeType = nodeType;
            Position = position;
            Radius = radius;
            IsEnabled = isEnabled;
            IsSearched = isSearched;
            SearchProgress = searchProgress;
            RequiredSearchSeconds = requiredSearchSeconds;
            OccupyingMemberId = occupyingMemberId;
            ReservedByMemberId = reservedByMemberId;
            BuildingId = buildingId;
            IsInsideBuilding = isInsideBuilding;
            AllowsFireThrough = allowsFireThrough;
            AllowsVisionThrough = allowsVisionThrough;
            IsEntryPoint = isEntryPoint;
            IsSearchPoint = isSearchPoint;
        }

        public int NodeId { get; private set; }
        public TacticalNodeType NodeType { get; private set; }
        public Vec2 Position { get; private set; }
        public float Radius { get; private set; }
        public bool IsEnabled { get; private set; }
        public bool IsSearched { get; private set; }
        public float SearchProgress { get; private set; }
        public float RequiredSearchSeconds { get; private set; }
        public BattleEntityId? OccupyingMemberId { get; private set; }
        public BattleEntityId? ReservedByMemberId { get; private set; }
        public int? BuildingId { get; private set; }
        public bool IsInsideBuilding { get; private set; }
        public bool AllowsFireThrough { get; private set; }
        public bool AllowsVisionThrough { get; private set; }
        public bool IsEntryPoint { get; private set; }
        public bool IsSearchPoint { get; private set; }
    }
}
