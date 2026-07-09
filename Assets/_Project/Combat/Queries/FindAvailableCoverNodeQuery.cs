using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class FindAvailableCoverNodeQuery
    {
        public static TacticalNodeState Execute(BattleState battleState, Vec2 center, float radius)
        {
            TacticalNodeState nearestNode = null;
            float nearestDistance = float.MaxValue;

            if (battleState == null)
            {
                return null;
            }

            foreach (TacticalNodeState nodeState in battleState.TacticalNodesById.Values)
            {
                if (nodeState == null || !nodeState.IsAvailable)
                {
                    continue;
                }

                if (nodeState.NodeType != TacticalNodeType.Cover &&
                    nodeState.NodeType != TacticalNodeType.DefensivePosition &&
                    nodeState.NodeType != TacticalNodeType.Window)
                {
                    continue;
                }

                float distance = Vec2.Distance(center, nodeState.Position);
                if (distance > radius || distance >= nearestDistance)
                {
                    continue;
                }

                nearestDistance = distance;
                nearestNode = nodeState;
            }

            return nearestNode;
        }
    }
}
