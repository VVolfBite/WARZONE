using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class FireSystem
    {
        private readonly ContentCatalog _contentCatalog;

        public FireSystem(ContentCatalog contentCatalog)
        {
            _contentCatalog = contentCatalog;
        }

        public void Execute(BattleState battleState, float deltaTimeSeconds)
        {
            if (battleState == null)
            {
                return;
            }

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                ExecuteForMember(battleState, memberState, deltaTimeSeconds);
            }
        }

        private void ExecuteForMember(BattleState battleState, BattleMemberState memberState, float deltaTimeSeconds)
        {
            if (memberState == null || !memberState.CanFight)
            {
                return;
            }

            memberState.TickCooldown(deltaTimeSeconds);

            if (!memberState.CurrentTargetEnemyId.HasValue)
            {
                return;
            }

            BattleEnemyState enemyState;
            if (!battleState.TryGetEnemy(memberState.CurrentTargetEnemyId.Value, out enemyState) || !enemyState.IsAlive)
            {
                memberState.SetCurrentTargetEnemy(null);
                return;
            }

            float distance = Vec2.Distance(memberState.Position, enemyState.Position);
            if (distance > memberState.AttackRange || memberState.AttackCooldownRemaining > 0f)
            {
                return;
            }

            float effectiveDetectionRange = EnvironmentalVisibilityRule.GetEffectiveDetectionRange(
                battleState.EnvironmentState,
                memberState.Position,
                memberState.DetectionRange,
                memberState.NightVisionLevel,
                enemyState.Position,
                enemyState.HasLightSource);
            if (distance > effectiveDetectionRange)
            {
                battleState.AddEvent(new BattleEventRecord(
                    BattleEventTypes.TargetLostToDarkness,
                    memberState.SquadId,
                    memberState.MemberId,
                    "night",
                    enemyState.EnemyId));
                memberState.SetCurrentTargetEnemy(null);
                return;
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
                memberState.SetCurrentTargetEnemy(null);
                return;
            }

            WeaponDefinition weaponDefinition;
            if (!_contentCatalog.TryGetWeapon(memberState.WeaponId, out weaponDefinition))
            {
                return;
            }

            BuildingFireResult buildingFire = BuildingFireRule.Evaluate(battleState, memberState, enemyState);
            if (!buildingFire.CanFire)
            {
                battleState.AddEvent(new BattleEventRecord(
                    BattleEventTypes.BuildingFireBlocked,
                    memberState.SquadId,
                    memberState.MemberId,
                    buildingFire.BlockingBuildingId.HasValue ? buildingFire.BlockingBuildingId.Value.ToString() : "building",
                    enemyState.EnemyId));
                return;
            }

            FireLineResult fireLine = FireLineRule.Evaluate(battleState, memberState.Position, enemyState.Position, buildingFire.AllowThroughBuildingBlockers);
            if (!fireLine.CanFire)
            {
                battleState.AddEvent(new BattleEventRecord(
                    BattleEventTypes.ShotBlocked,
                    memberState.SquadId,
                    memberState.MemberId,
                    fireLine.BlockingObstacleType.HasValue ? fireLine.BlockingObstacleType.Value.ToString() : "Obstacle",
                    enemyState.EnemyId));
                return;
            }

            battleState.EnqueueDamage(new PendingDamageRequest(
                memberState.MemberId,
                enemyState.EnemyId,
                weaponDefinition.Damage,
                weaponDefinition.Id,
                false,
                fireLine.DamageMultiplier * buildingFire.TargetDamageMultiplier,
                fireLine.CoverObstacleId));

            memberState.ResetAttackCooldown(SuppressionRule.ApplyAttackCooldownPenalty(weaponDefinition.FireIntervalSeconds, memberState.IsSuppressed));
            battleState.AddEvent(new BattleEventRecord(
                BattleEventTypes.WeaponFired,
                memberState.SquadId,
                memberState.MemberId,
                weaponDefinition.Id,
                enemyState.EnemyId,
                weaponDefinition.Damage));
        }
    }
}
