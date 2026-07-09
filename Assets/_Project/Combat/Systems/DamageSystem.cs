namespace Warzone.Combat
{
    public sealed class DamageSystem
    {
        public void Execute(BattleState battleState)
        {
            if (battleState == null || battleState.PendingDamageRequests.Count == 0)
            {
                return;
            }

            for (int i = 0; i < battleState.PendingDamageRequests.Count; i++)
            {
                PendingDamageRequest damageRequest = battleState.PendingDamageRequests[i];

                if (damageRequest.TargetIsMember)
                {
                    BattleMemberState memberState;
                    if (!battleState.TryGetMember(damageRequest.TargetId, out memberState) || !memberState.CanAct)
                    {
                        continue;
                    }

                    int appliedDamage = DamageModifierRule.Apply(damageRequest.DamageAmount, damageRequest.DamageMultiplier);
                    memberState.ApplyDamage(appliedDamage);
                    battleState.AddEvent(new BattleEventRecord(
                        BattleEventTypes.DamageApplied,
                        memberState.SquadId,
                        damageRequest.SourceId,
                        damageRequest.WeaponId,
                        damageRequest.TargetId,
                        appliedDamage));

                    if (damageRequest.DamageMultiplier < 1f)
                    {
                        battleState.AddEvent(new BattleEventRecord(
                            BattleEventTypes.CoverReducedDamage,
                            memberState.SquadId,
                            damageRequest.TargetId,
                            damageRequest.CoverObstacleId.HasValue ? damageRequest.CoverObstacleId.Value.ToString() : "Cover",
                            null,
                            damageRequest.DamageAmount - appliedDamage));
                    }

                    if (!memberState.IsAlive)
                    {
                        battleState.AddEvent(new BattleEventRecord(
                            BattleEventTypes.MemberKilled,
                            memberState.SquadId,
                            damageRequest.SourceId,
                            memberState.MemberId.ToString(),
                            memberState.MemberId));
                    }

                    continue;
                }

                BattleEnemyState enemyState;
                if (!battleState.TryGetEnemy(damageRequest.TargetId, out enemyState) || !enemyState.IsAlive)
                {
                    continue;
                }

                int modifiedDamage = DamageModifierRule.Apply(damageRequest.DamageAmount, damageRequest.DamageMultiplier);
                enemyState.ApplyDamage(modifiedDamage);
                battleState.AddEvent(new BattleEventRecord(
                    BattleEventTypes.DamageApplied,
                    null,
                    damageRequest.SourceId,
                    damageRequest.WeaponId,
                    damageRequest.TargetId,
                    modifiedDamage));

                if (damageRequest.DamageMultiplier < 1f)
                {
                    battleState.AddEvent(new BattleEventRecord(
                        BattleEventTypes.CoverReducedDamage,
                        null,
                        damageRequest.TargetId,
                        damageRequest.CoverObstacleId.HasValue ? damageRequest.CoverObstacleId.Value.ToString() : "Cover",
                        null,
                        damageRequest.DamageAmount - modifiedDamage));
                }

                if (!enemyState.IsAlive)
                {
                    battleState.AddEvent(new BattleEventRecord(
                        BattleEventTypes.EnemyKilled,
                        null,
                        damageRequest.SourceId,
                        enemyState.DefinitionId,
                        enemyState.EnemyId));
                }
            }

            battleState.ClearPendingDamageRequests();
        }
    }
}
