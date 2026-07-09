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
                if (!memberState.CanAct)
                {
                    memberState.ClearIntent();
                    memberState.SetCurrentTargetEnemy(null);
                    memberState.ClearOccupiedTacticalNode();
                    continue;
                }

                if (memberState.CurrentTargetEnemyId.HasValue)
                {
                    BattleEnemyState enemyState;
                    if (!battleState.TryGetEnemy(memberState.CurrentTargetEnemyId.Value, out enemyState) || !enemyState.IsAlive)
                    {
                        memberState.SetCurrentTargetEnemy(null);
                    }
                }
            }

            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                if (enemyState == null || !enemyState.IsAlive || !enemyState.CurrentTargetMemberId.HasValue)
                {
                    continue;
                }

                BattleMemberState memberState;
                if (!battleState.TryGetMember(enemyState.CurrentTargetMemberId.Value, out memberState) || !memberState.CanAct)
                {
                    enemyState.SetCurrentTargetMember(null);
                }
            }
        }
    }
}
