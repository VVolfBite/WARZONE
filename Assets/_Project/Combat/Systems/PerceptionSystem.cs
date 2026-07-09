using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class PerceptionSystem
    {
        public void Execute(BattleState battleState)
        {
            if (battleState == null)
            {
                return;
            }

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (!memberState.CanAct)
                {
                    memberState.SetVisibleEnemies(null);
                    continue;
                }

                List<BattleEnemyState> visibleEnemies = FindVisibleEnemiesQuery.Execute(battleState, memberState);
                List<BattleEntityId> visibleEnemyIds = new List<BattleEntityId>(visibleEnemies.Count);
                for (int i = 0; i < visibleEnemies.Count; i++)
                {
                    visibleEnemyIds.Add(visibleEnemies[i].EnemyId);
                }

                memberState.SetVisibleEnemies(visibleEnemyIds);
                if (visibleEnemyIds.Count == 0)
                {
                    foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
                    {
                        if (enemyState == null || !enemyState.IsAlive)
                        {
                            continue;
                        }

                        if (Vec2.Distance(memberState.Position, enemyState.Position) > memberState.DetectionRange)
                        {
                            continue;
                        }

                        LineOfSightResult blockedLine = LineOfSightRule.Evaluate(battleState, memberState.Position, enemyState.Position);
                        if (!blockedLine.HasLineOfSight)
                        {
                            battleState.AddEvent(new BattleEventRecord(
                                BattleEventTypes.LineOfSightBlocked,
                                memberState.SquadId,
                                memberState.MemberId,
                                blockedLine.BlockingObstacleType.HasValue ? blockedLine.BlockingObstacleType.Value.ToString() : "Obstacle",
                                enemyState.EnemyId));
                            break;
                        }
                    }
                }
            }
        }
    }
}
