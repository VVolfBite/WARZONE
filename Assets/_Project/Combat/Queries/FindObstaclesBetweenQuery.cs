using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class FindObstaclesBetweenQuery
    {
        public static List<TacticalObstacleState> Execute(BattleState battleState, Vec2 start, Vec2 end)
        {
            List<TacticalObstacleState> obstacles = new List<TacticalObstacleState>();
            if (battleState == null)
            {
                return obstacles;
            }

            foreach (TacticalObstacleState obstacleState in battleState.ObstaclesById.Values)
            {
                if (obstacleState == null || obstacleState.IsDestroyed)
                {
                    continue;
                }

                float distance = MathUtil.DistancePointToSegment(obstacleState.Position, start, end);
                if (distance <= obstacleState.Radius)
                {
                    obstacles.Add(obstacleState);
                }
            }

            return obstacles;
        }
    }
}
