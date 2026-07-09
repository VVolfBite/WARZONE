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
                if (!memberState.CanFight)
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

                        float effectiveDetectionRange = EnvironmentalVisibilityRule.GetEffectiveDetectionRange(
                            battleState.EnvironmentState,
                            memberState.Position,
                            memberState.DetectionRange,
                            memberState.NightVisionLevel,
                            enemyState.Position,
                            enemyState.HasLightSource);
                        float distance = Vec2.Distance(memberState.Position, enemyState.Position);
                        if (distance > effectiveDetectionRange)
                        {
                            if (distance <= memberState.DetectionRange)
                            {
                                battleState.AddEvent(new BattleEventRecord(
                                    BattleEventTypes.TargetLostToDarkness,
                                    memberState.SquadId,
                                    memberState.MemberId,
                                    "night",
                                    enemyState.EnemyId));
                                break;
                            }

                            continue;
                        }

                        EnvironmentalZoneState blockingSmokeZone;
                        if (EnvironmentalVisibilityRule.TryGetBlockingSmokeZone(battleState.EnvironmentState, memberState.Position, enemyState.Position, memberState.SmokeVisionLevel, out blockingSmokeZone))
                        {
                            battleState.AddEvent(new BattleEventRecord(
                                BattleEventTypes.SmokeLineOfSightBlocked,
                                memberState.SquadId,
                                memberState.MemberId,
                                blockingSmokeZone.ZoneId.ToString(),
                                enemyState.EnemyId));
                            break;
                        }

                        LineOfSightResult blockedLine = LineOfSightRule.Evaluate(battleState, memberState.Position, enemyState.Position);
                        if (!blockedLine.HasLineOfSight)
                        {
                            battleState.AddEvent(new BattleEventRecord(
                                BattleEventTypes.LineOfSightBlocked,
                                memberState.SquadId,
                                memberState.MemberId,
                                blockedLine.BlockingObstacleType.HasValue ? blockedLine.BlockingObstacleType.Value.ToString() :
                                blockedLine.BlockingZoneType.HasValue ? blockedLine.BlockingZoneType.Value.ToString() : "Obstacle",
                                enemyState.EnemyId));
                            break;
                        }

                    }
                }
            }
        }
    }
}
