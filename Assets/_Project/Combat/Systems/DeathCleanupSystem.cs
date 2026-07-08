namespace Warzone.Combat
{
    public sealed class DeathCleanupSystem
    {
        public void Execute(BattleState battleState)
        {
            if (battleState == null)
            {
                return;
            }

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (memberState.CurrentTargetEnemyId.HasValue)
                {
                    BattleEnemyState enemyState;
                    if (!battleState.TryGetEnemy(memberState.CurrentTargetEnemyId.Value, out enemyState) || !enemyState.IsAlive)
                    {
                        memberState.SetCurrentTargetEnemy(null);
                    }
                }
            }
        }
    }
}
