using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class FindExtractionPointQuery
    {
        public static TacticalNodeState Execute(BattleState battleState, Vec2 position, int? squadId = null)
        {
            TacticalNodeState nearestNode = null;
            float nearestDistance = float.MaxValue;

            if (battleState == null)
            {
                return null;
            }

            foreach (TacticalNodeState nodeState in battleState.TacticalNodesById.Values)
            {
                if (nodeState == null || !nodeState.IsEnabled || nodeState.NodeType != TacticalNodeType.ExtractionPoint)
                {
                    continue;
                }

                if (squadId.HasValue && nodeState.ExtractionOwnerSquadId.HasValue && nodeState.ExtractionOwnerSquadId.Value != squadId.Value)
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
