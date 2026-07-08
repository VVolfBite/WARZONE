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
