using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class LineOfSightRule
    {
        public static LineOfSightResult Evaluate(BattleState battleState, Vec2 origin, Vec2 target, bool ignoreBuildingBlockers = false)
        {
            List<TacticalObstacleState> obstacles = FindObstaclesBetweenQuery.Execute(battleState, origin, target);
            for (int i = 0; i < obstacles.Count; i++)
            {
                TacticalObstacleState obstacleState = obstacles[i];
                if (!obstacleState.BlocksLineOfSight)
                {
                    continue;
                }

                if (ignoreBuildingBlockers &&
                    (obstacleState.ObstacleType == TacticalObstacleType.BuildingBlocker || obstacleState.ObstacleType == TacticalObstacleType.Wall))
                {
                    continue;
                }

                return new LineOfSightResult(false, obstacleState.ObstacleId, obstacleState.ObstacleType);
            }

            EnvironmentalZoneState blockingSmokeZone;
            if (battleState != null &&
                EnvironmentalVisibilityRule.TryGetBlockingSmokeZone(battleState.EnvironmentState, origin, target, 0, out blockingSmokeZone))
            {
                return new LineOfSightResult(false, null, null, blockingSmokeZone.ZoneId, blockingSmokeZone.ZoneType);
            }

            return new LineOfSightResult(true);
        }
    }
}
