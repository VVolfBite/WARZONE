using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class FindSearchPointQuery
    {
        public static TacticalNodeState Execute(BattleState battleState, Vec2 position, bool onlyIncomplete = true)
        {
            TacticalNodeState nodeState = FindNearestTacticalNodeQuery.Execute(battleState, position, TacticalNodeType.SearchPoint);
            if (nodeState == null)
            {
                return null;
            }

            if (onlyIncomplete && nodeState.IsSearched)
            {
                TacticalNodeState nearestIncomplete = null;
                float nearestDistance = float.MaxValue;
                foreach (TacticalNodeState candidate in battleState.TacticalNodesById.Values)
                {
                    if (candidate == null || !candidate.IsEnabled || candidate.NodeType != TacticalNodeType.SearchPoint || candidate.IsSearched)
                    {
                        continue;
                    }

                    float distance = Vec2.Distance(position, candidate.Position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestIncomplete = candidate;
                    }
                }

                return nearestIncomplete;
            }

            return nodeState;
        }
    }
}
