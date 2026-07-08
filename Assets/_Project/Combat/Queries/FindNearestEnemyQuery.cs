using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class FindNearestEnemyQuery
    {
        public static BattleEnemyState Execute(BattleMemberState memberState, IReadOnlyList<BattleEnemyState> enemies)
        {
            if (memberState == null || enemies == null)
            {
                return null;
            }

            BattleEnemyState nearestEnemy = null;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < enemies.Count; i++)
            {
                BattleEnemyState enemyState = enemies[i];
                if (enemyState == null || !enemyState.IsAlive)
                {
                    continue;
                }

                float distance = Vec2.Distance(memberState.Position, enemyState.Position);
                if (distance < nearestDistance)
                {
                    nearestEnemy = enemyState;
                    nearestDistance = distance;
                }
            }

            return nearestEnemy;
        }
    }
}
