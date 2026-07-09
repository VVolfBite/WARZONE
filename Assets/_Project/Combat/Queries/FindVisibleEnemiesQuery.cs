using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class FindVisibleEnemiesQuery
    {
        public static List<BattleEnemyState> Execute(BattleState battleState, BattleMemberState memberState)
        {
            List<BattleEnemyState> visibleEnemies = new List<BattleEnemyState>();
            if (battleState == null || memberState == null || !memberState.CanFight)
            {
                return visibleEnemies;
            }

            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                if (!enemyState.IsAlive)
                {
                    continue;
                }

                if (enemyState.FactionId == memberState.FactionId)
                {
                    continue;
                }

                float distance = Vec2.Distance(memberState.Position, enemyState.Position);
                if (distance <= memberState.DetectionRange)
                {
                    LineOfSightResult lineOfSight = LineOfSightRule.Evaluate(battleState, memberState.Position, enemyState.Position);
                    if (lineOfSight.HasLineOfSight)
                    {
                        visibleEnemies.Add(enemyState);
                    }
                }
            }

            return visibleEnemies;
        }
    }
}
