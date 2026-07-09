using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class BuildingState
    {
        private readonly List<int> _tacticalNodeIds = new List<int>();

        public BuildingState(int buildingId, Vec2 position, float radius, bool isEnterable, IReadOnlyList<int> tacticalNodeIds)
        {
            BuildingId = buildingId;
            Position = position;
            Radius = radius;
            IsEnterable = isEnterable;
            SyncTacticalNodeIds(tacticalNodeIds);
        }

        public int BuildingId { get; private set; }
        public Vec2 Position { get; private set; }
        public float Radius { get; private set; }
        public bool IsEnterable { get; private set; }
        public bool IsDamaged { get; private set; }

        public IReadOnlyList<int> TacticalNodeIds
        {
            get { return _tacticalNodeIds; }
        }

        public void SyncTacticalNodeIds(IReadOnlyList<int> tacticalNodeIds)
        {
            _tacticalNodeIds.Clear();
            if (tacticalNodeIds == null)
            {
                return;
            }

            for (int i = 0; i < tacticalNodeIds.Count; i++)
            {
                _tacticalNodeIds.Add(tacticalNodeIds[i]);
            }
        }
    }
}
