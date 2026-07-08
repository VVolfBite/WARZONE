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
            if (memberState == null || !memberState.IsAlive)
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

            WeaponDefinition weaponDefinition;
            if (!_contentCatalog.TryGetWeapon(memberState.WeaponId, out weaponDefinition))
            {
                return;
            }

            battleState.EnqueueDamage(new PendingDamageRequest(
                memberState.MemberId,
                enemyState.EnemyId,
                weaponDefinition.Damage,
                weaponDefinition.Id));

            memberState.ResetAttackCooldown(weaponDefinition.FireIntervalSeconds);
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
