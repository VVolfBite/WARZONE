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
                float effectiveDetectionRange = EnvironmentalVisibilityRule.GetEffectiveDetectionRange(
                    battleState.EnvironmentState,
                    memberState.Position,
                    memberState.DetectionRange,
                    memberState.NightVisionLevel,
                    enemyState.Position,
                    enemyState.HasLightSource);
                memberState.SetEffectiveDetectionRange(effectiveDetectionRange);

                if (distance <= effectiveDetectionRange)
                {
                    BuildingVisibilityResult buildingVisibility = BuildingVisibilityRule.Evaluate(battleState, memberState, enemyState);
                    if (!buildingVisibility.HasVisibility)
                    {
                        continue;
                    }

                    EnvironmentalZoneState blockingSmokeZone;
                    if (EnvironmentalVisibilityRule.TryGetBlockingSmokeZone(battleState.EnvironmentState, memberState.Position, enemyState.Position, memberState.SmokeVisionLevel, out blockingSmokeZone))
                    {
                        continue;
                    }

                    LineOfSightResult lineOfSight = LineOfSightRule.Evaluate(battleState, memberState.Position, enemyState.Position, buildingVisibility.AllowThroughBuildingBlockers);
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
