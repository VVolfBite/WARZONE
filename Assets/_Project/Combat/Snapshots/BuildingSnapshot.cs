using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class BuildingSnapshot
    {
        public BuildingSnapshot(
            int buildingId,
            Vec2 position,
            float radius,
            bool isEnterable,
            bool isDamaged,
            IReadOnlyList<int> tacticalNodeIds,
            IReadOnlyList<int> entranceNodeIds,
            IReadOnlyList<int> windowNodeIds,
            IReadOnlyList<int> interiorNodeIds,
            IReadOnlyList<int> searchNodeIds)
        {
            BuildingId = buildingId;
            Position = position;
            Radius = radius;
            IsEnterable = isEnterable;
            IsDamaged = isDamaged;
            TacticalNodeIds = tacticalNodeIds;
            EntranceNodeIds = entranceNodeIds;
            WindowNodeIds = windowNodeIds;
            InteriorNodeIds = interiorNodeIds;
            SearchNodeIds = searchNodeIds;
        }

        public int BuildingId { get; private set; }
        public Vec2 Position { get; private set; }
        public float Radius { get; private set; }
        public bool IsEnterable { get; private set; }
        public bool IsDamaged { get; private set; }
        public IReadOnlyList<int> TacticalNodeIds { get; private set; }
        public IReadOnlyList<int> EntranceNodeIds { get; private set; }
        public IReadOnlyList<int> WindowNodeIds { get; private set; }
        public IReadOnlyList<int> InteriorNodeIds { get; private set; }
        public IReadOnlyList<int> SearchNodeIds { get; private set; }
    }
}
