using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class FindBuildingEntranceQuery
    {
        public static TacticalNodeState Execute(BattleState battleState, int buildingId, Vec2 fromPosition)
        {
            BuildingState buildingState = FindBuildingByIdQuery.Execute(battleState, buildingId);
            if (buildingState == null)
            {
                return null;
            }

            TacticalNodeState nearestNode = null;
            float nearestDistance = float.MaxValue;
            for (int i = 0; i < buildingState.EntranceNodeIds.Count; i++)
            {
                TacticalNodeState nodeState;
                if (!battleState.TryGetTacticalNode(buildingState.EntranceNodeIds[i], out nodeState) || !nodeState.IsAvailable)
                {
                    continue;
                }

                float distance = Vec2.Distance(fromPosition, nodeState.Position);
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
