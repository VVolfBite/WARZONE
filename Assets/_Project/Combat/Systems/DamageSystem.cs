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

                    memberState.ApplyDamage(damageRequest.DamageAmount);
                    battleState.AddEvent(new BattleEventRecord(
                        BattleEventTypes.DamageApplied,
                        memberState.SquadId,
                        damageRequest.SourceId,
                        damageRequest.WeaponId,
                        damageRequest.TargetId,
                        damageRequest.DamageAmount));

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

                enemyState.ApplyDamage(damageRequest.DamageAmount);
                battleState.AddEvent(new BattleEventRecord(
                    BattleEventTypes.DamageApplied,
                    null,
                    damageRequest.SourceId,
                    damageRequest.WeaponId,
                    damageRequest.TargetId,
                    damageRequest.DamageAmount));

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
