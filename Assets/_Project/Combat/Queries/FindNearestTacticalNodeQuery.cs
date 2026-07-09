using System;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class FindNearestTacticalNodeQuery
    {
        public static TacticalNodeState Execute(BattleState battleState, Vec2 position, params TacticalNodeType[] nodeTypes)
        {
            if (battleState == null)
            {
                return null;
            }

            TacticalNodeState nearestNode = null;
            float nearestDistance = float.MaxValue;

            foreach (TacticalNodeState nodeState in battleState.TacticalNodesById.Values)
            {
                if (nodeState == null || !nodeState.IsEnabled)
                {
                    continue;
                }

                if (nodeTypes != null && nodeTypes.Length > 0 && Array.IndexOf(nodeTypes, nodeState.NodeType) < 0)
                {
                    continue;
                }

                float distance = Vec2.Distance(position, nodeState.Position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestNode = nodeState;
                }
            }

            return nearestNode;
        }
    }
}
