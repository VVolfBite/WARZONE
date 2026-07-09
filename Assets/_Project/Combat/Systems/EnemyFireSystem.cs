using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class EnemyFireSystem
    {
        private readonly ContentCatalog _contentCatalog;

        public EnemyFireSystem(ContentCatalog contentCatalog)
        {
            _contentCatalog = contentCatalog;
        }

        public void Execute(BattleState battleState, float deltaTimeSeconds)
        {
            if (battleState == null)
            {
                return;
            }

            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                ExecuteForEnemy(battleState, enemyState, deltaTimeSeconds);
            }
        }

        private void ExecuteForEnemy(BattleState battleState, BattleEnemyState enemyState, float deltaTimeSeconds)
        {
            if (enemyState == null || !enemyState.IsAlive)
            {
                return;
            }

            enemyState.TickAttackCooldown(deltaTimeSeconds);
            if (!enemyState.CurrentTargetMemberId.HasValue)
            {
                return;
            }

            BattleMemberState memberState;
            if (!battleState.TryGetMember(enemyState.CurrentTargetMemberId.Value, out memberState) || !memberState.CanAct)
            {
                enemyState.SetCurrentTargetMember(null);
                return;
            }

            float distance = Vec2.Distance(enemyState.Position, memberState.Position);
            if (distance > enemyState.AttackRange || enemyState.AttackCooldownRemaining > 0f)
            {
                return;
            }

            float effectiveDetectionRange = EnvironmentalVisibilityRule.GetEffectiveDetectionRange(
                battleState.EnvironmentState,
                enemyState.Position,
                enemyState.DetectionRange,
                enemyState.NightVisionLevel,
                memberState.Position,
                memberState.HasLightSource);
            if (distance > effectiveDetectionRange)
            {
                battleState.AddEvent(new BattleEventRecord(
                    BattleEventTypes.TargetLostToDarkness,
                    null,
                    enemyState.EnemyId,
                    "night",
                    memberState.MemberId));
                enemyState.SetCurrentTargetMember(null);
                return;
            }

            EnvironmentalZoneState blockingSmokeZone;
            if (EnvironmentalVisibilityRule.TryGetBlockingSmokeZone(battleState.EnvironmentState, enemyState.Position, memberState.Position, 0, out blockingSmokeZone))
            {
                battleState.AddEvent(new BattleEventRecord(
                    BattleEventTypes.SmokeLineOfSightBlocked,
                    null,
                    enemyState.EnemyId,
                    blockingSmokeZone.ZoneId.ToString(),
                    memberState.MemberId));
                enemyState.SetCurrentTargetMember(null);
                return;
            }

            EnemyDefinition enemyDefinition;
            if (!_contentCatalog.TryGetEnemy(enemyState.DefinitionId, out enemyDefinition))
            {
                return;
            }

            FireLineResult fireLine = FireLineRule.Evaluate(battleState, enemyState.Position, memberState.Position);
            if (!fireLine.CanFire)
            {
                battleState.AddEvent(new BattleEventRecord(
                    BattleEventTypes.ShotBlocked,
                    null,
                    enemyState.EnemyId,
                    fireLine.BlockingObstacleType.HasValue ? fireLine.BlockingObstacleType.Value.ToString() : "Obstacle",
                    memberState.MemberId));
                return;
            }

            battleState.EnqueueDamage(new PendingDamageRequest(
                enemyState.EnemyId,
                memberState.MemberId,
                enemyDefinition.AttackDamage,
                enemyState.DefinitionId,
                true,
                fireLine.DamageMultiplier,
                fireLine.CoverObstacleId));

            enemyState.ResetAttackCooldown(enemyDefinition.AttackIntervalSeconds);
            battleState.AddEvent(new BattleEventRecord(
                BattleEventTypes.WeaponFired,
                null,
                enemyState.EnemyId,
                enemyState.DefinitionId,
                memberState.MemberId,
                enemyDefinition.AttackDamage));
        }
    }
}
