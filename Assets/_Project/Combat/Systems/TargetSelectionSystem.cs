using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class TargetSelectionSystem
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
                    memberState.SetCurrentTargetEnemy(null);
                    continue;
                }

                List<BattleEnemyState> visibleEnemies = FindVisibleEnemiesQuery.Execute(battleState, memberState);
                BattleEnemyState targetEnemy = FindNearestEnemyQuery.Execute(memberState, visibleEnemies);
                BattleEntityId? previousTargetId = memberState.CurrentTargetEnemyId;
                memberState.SetCurrentTargetEnemy(targetEnemy != null ? (BattleEntityId?)targetEnemy.EnemyId : null);

                if (targetEnemy != null && (!previousTargetId.HasValue || previousTargetId.Value != targetEnemy.EnemyId))
                {
                    battleState.AddEvent(new BattleEventRecord(
                        BattleEventTypes.TargetAcquired,
                        memberState.SquadId,
                        memberState.MemberId,
                        targetEnemy.DefinitionId,
                        targetEnemy.EnemyId));
                }
            }
        }
    }
}
