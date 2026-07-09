using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class EnemyBehaviorSystem
    {
        public void Execute(BattleState battleState, float deltaTimeSeconds)
        {
            if (battleState == null)
            {
                return;
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
                    continue;
                }

                float distance = Vec2.Distance(enemyState.Position, memberState.Position);
                if (distance <= enemyState.AttackRange || enemyState.MovementSpeed <= 0f)
                {
                    continue;
                }

                float stepDistance = enemyState.MovementSpeed * deltaTimeSeconds;
                enemyState.UpdatePosition(Vec2.MoveTowards(enemyState.Position, memberState.Position, stepDistance));
            }
        }
    }
}
