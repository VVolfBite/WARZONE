namespace Warzone.Combat
{
    public static class FindBuildingSearchPointQuery
    {
        public static TacticalNodeState Execute(BattleState battleState, int buildingId, bool onlyIncomplete = true)
        {
            BuildingState buildingState = FindBuildingByIdQuery.Execute(battleState, buildingId);
            if (buildingState == null)
            {
                return null;
            }

            for (int i = 0; i < buildingState.SearchNodeIds.Count; i++)
            {
                TacticalNodeState nodeState;
                if (!battleState.TryGetTacticalNode(buildingState.SearchNodeIds[i], out nodeState) || !nodeState.IsEnabled)
                {
                    continue;
                }

                if (onlyIncomplete && nodeState.IsSearched)
                {
                    continue;
                }

                return nodeState;
            }

            return null;
        }
    }
}
