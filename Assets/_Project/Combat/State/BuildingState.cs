using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class BuildingState
    {
        private readonly List<int> _tacticalNodeIds = new List<int>();
        private readonly List<int> _entranceNodeIds = new List<int>();
        private readonly List<int> _windowNodeIds = new List<int>();
        private readonly List<int> _interiorNodeIds = new List<int>();
        private readonly List<int> _searchNodeIds = new List<int>();

        public BuildingState(
            int buildingId,
            Vec2 position,
            float radius,
            bool isEnterable,
            IReadOnlyList<int> tacticalNodeIds,
            IReadOnlyList<int> entranceNodeIds = null,
            IReadOnlyList<int> windowNodeIds = null,
            IReadOnlyList<int> interiorNodeIds = null,
            IReadOnlyList<int> searchNodeIds = null)
        {
            BuildingId = buildingId;
            Position = position;
            Radius = radius;
            IsEnterable = isEnterable;
            SyncTacticalNodeIds(tacticalNodeIds);
            SyncNodeIds(_entranceNodeIds, entranceNodeIds);
            SyncNodeIds(_windowNodeIds, windowNodeIds);
            SyncNodeIds(_interiorNodeIds, interiorNodeIds);
            SyncNodeIds(_searchNodeIds, searchNodeIds);
        }

        public int BuildingId { get; private set; }
        public Vec2 Position { get; private set; }
        public float Radius { get; private set; }
        public bool IsEnterable { get; private set; }
        public bool IsDamaged { get; private set; }
        public bool IsOnFire { get; private set; }

        public IReadOnlyList<int> TacticalNodeIds
        {
            get { return _tacticalNodeIds; }
        }

        public IReadOnlyList<int> EntranceNodeIds
        {
            get { return _entranceNodeIds; }
        }

        public IReadOnlyList<int> WindowNodeIds
        {
            get { return _windowNodeIds; }
        }

        public IReadOnlyList<int> InteriorNodeIds
        {
            get { return _interiorNodeIds; }
        }

        public IReadOnlyList<int> SearchNodeIds
        {
            get { return _searchNodeIds; }
        }

        public void SyncTacticalNodeIds(IReadOnlyList<int> tacticalNodeIds)
        {
            _tacticalNodeIds.Clear();
            SyncNodeIds(_tacticalNodeIds, tacticalNodeIds);
        }

        private static void SyncNodeIds(List<int> target, IReadOnlyList<int> source)
        {
            target.Clear();
            if (source == null)
            {
                return;
            }

            for (int i = 0; i < source.Count; i++)
            {
                target.Add(source[i]);
            }
        }
    }
}
