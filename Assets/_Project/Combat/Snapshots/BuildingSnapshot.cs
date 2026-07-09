using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class BuildingSnapshot
    {
        public BuildingSnapshot(int buildingId, Vec2 position, float radius, bool isEnterable, IReadOnlyList<int> tacticalNodeIds)
        {
            BuildingId = buildingId;
            Position = position;
            Radius = radius;
            IsEnterable = isEnterable;
            TacticalNodeIds = tacticalNodeIds;
        }

        public int BuildingId { get; private set; }
        public Vec2 Position { get; private set; }
        public float Radius { get; private set; }
        public bool IsEnterable { get; private set; }
        public IReadOnlyList<int> TacticalNodeIds { get; private set; }
    }
}
