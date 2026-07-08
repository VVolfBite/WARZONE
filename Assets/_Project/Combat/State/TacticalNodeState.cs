using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class TacticalNodeState
    {
        public TacticalNodeState(int nodeId, Vec2 position, float radius)
        {
            NodeId = nodeId;
            Position = position;
            Radius = radius;
        }

        public int NodeId { get; private set; }
        public Vec2 Position { get; private set; }
        public float Radius { get; private set; }
    }
}

