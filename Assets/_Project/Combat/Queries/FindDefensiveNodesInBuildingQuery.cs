using System.Collections.Generic;

namespace Warzone.Combat
{
    public static class FindDefensiveNodesInBuildingQuery
    {
        public static List<TacticalNodeState> Execute(BattleState battleState, int buildingId)
        {
            List<TacticalNodeState> nodes = new List<TacticalNodeState>();
            BuildingState buildingState = FindBuildingByIdQuery.Execute(battleState, buildingId);
            if (buildingState == null)
            {
                return nodes;
            }

            for (int i = 0; i < buildingState.WindowNodeIds.Count; i++)
            {
                TacticalNodeState nodeState;
                if (battleState.TryGetTacticalNode(buildingState.WindowNodeIds[i], out nodeState))
                {
                    nodes.Add(nodeState);
                }
            }

            for (int i = 0; i < buildingState.EntranceNodeIds.Count; i++)
            {
                TacticalNodeState nodeState;
                if (battleState.TryGetTacticalNode(buildingState.EntranceNodeIds[i], out nodeState) && !nodes.Contains(nodeState))
                {
                    nodes.Add(nodeState);
                }
            }

            for (int i = 0; i < buildingState.InteriorNodeIds.Count; i++)
            {
                TacticalNodeState nodeState;
                if (battleState.TryGetTacticalNode(buildingState.InteriorNodeIds[i], out nodeState) && !nodes.Contains(nodeState))
                {
                    nodes.Add(nodeState);
                }
            }

            return nodes;
        }
    }
}
