namespace Warzone.Combat
{
    public static class FindAvailableInteriorNodeQuery
    {
        public static TacticalNodeState Execute(BattleState battleState, int buildingId)
        {
            BuildingState buildingState = FindBuildingByIdQuery.Execute(battleState, buildingId);
            if (buildingState == null)
            {
                return null;
            }

            for (int i = 0; i < buildingState.InteriorNodeIds.Count; i++)
            {
                TacticalNodeState nodeState;
                if (battleState.TryGetTacticalNode(buildingState.InteriorNodeIds[i], out nodeState) && nodeState.IsAvailable)
                {
                    return nodeState;
                }
            }

            return null;
        }
    }
}
