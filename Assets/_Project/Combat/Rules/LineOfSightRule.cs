using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class LineOfSightRule
    {
        public static LineOfSightResult Evaluate(BattleState battleState, Vec2 origin, Vec2 target)
        {
            List<TacticalObstacleState> obstacles = FindObstaclesBetweenQuery.Execute(battleState, origin, target);
            for (int i = 0; i < obstacles.Count; i++)
            {
                TacticalObstacleState obstacleState = obstacles[i];
                if (!obstacleState.BlocksLineOfSight)
                {
                    continue;
                }

                return new LineOfSightResult(false, obstacleState.ObstacleId, obstacleState.ObstacleType);
            }

            return new LineOfSightResult(true);
        }
    }
}
