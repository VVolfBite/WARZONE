using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class FireLineRule
    {
        public static FireLineResult Evaluate(BattleState battleState, Vec2 origin, Vec2 target, bool ignoreBuildingBlockers = false)
        {
            List<TacticalObstacleState> obstacles = FindObstaclesBetweenQuery.Execute(battleState, origin, target);
            TacticalObstacleState coverObstacle = null;
            float coverDistance = float.MaxValue;

            for (int i = 0; i < obstacles.Count; i++)
            {
                TacticalObstacleState obstacleState = obstacles[i];
                float distanceToTarget = Vec2.Distance(obstacleState.Position, target);

                if (obstacleState.BlocksFire)
                {
                    if (ignoreBuildingBlockers &&
                        (obstacleState.ObstacleType == TacticalObstacleType.BuildingBlocker || obstacleState.ObstacleType == TacticalObstacleType.Wall))
                    {
                        continue;
                    }

                    return new FireLineResult(false, obstacleState.ObstacleId, obstacleState.ObstacleType);
                }

                if (obstacleState.ProvidesCover &&
                    distanceToTarget <= obstacleState.Radius + 1.5f &&
                    distanceToTarget < coverDistance)
                {
                    coverDistance = distanceToTarget;
                    coverObstacle = obstacleState;
                }
            }

            if (coverObstacle != null)
            {
                return new FireLineResult(
                    true,
                    null,
                    null,
                    coverObstacle.ObstacleId,
                    coverObstacle.ObstacleType,
                    CoverModifierRule.GetDamageMultiplier(coverObstacle));
            }

            return new FireLineResult(true);
        }
    }
}
