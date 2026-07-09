namespace Warzone.Combat
{
    public static class FindBuildingByIdQuery
    {
        public static BuildingState Execute(BattleState battleState, int buildingId)
        {
            BuildingState buildingState;
            return battleState != null && battleState.TryGetBuilding(buildingId, out buildingState) ? buildingState : null;
        }
    }
}
