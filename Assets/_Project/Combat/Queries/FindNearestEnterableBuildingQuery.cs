using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class FindNearestEnterableBuildingQuery
    {
        public static BuildingState Execute(BattleState battleState, Vec2 position)
        {
            if (battleState == null)
            {
                return null;
            }

            BuildingState nearestBuilding = null;
            float nearestDistance = float.MaxValue;
            foreach (BuildingState buildingState in battleState.BuildingsById.Values)
            {
                if (buildingState == null || !buildingState.IsEnterable)
                {
                    continue;
                }

                float distance = Vec2.Distance(position, buildingState.Position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestBuilding = buildingState;
                }
            }

            return nearestBuilding;
        }
    }
}
