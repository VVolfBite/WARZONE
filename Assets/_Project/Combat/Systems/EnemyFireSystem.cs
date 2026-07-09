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

            EnemyDefinition enemyDefinition;
            if (!_contentCatalog.TryGetEnemy(enemyState.DefinitionId, out enemyDefinition))
            {
                return;
            }

            battleState.EnqueueDamage(new PendingDamageRequest(
                enemyState.EnemyId,
                memberState.MemberId,
                enemyDefinition.AttackDamage,
                enemyState.DefinitionId,
                true));

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
