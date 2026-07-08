using Warzone.Core.Math;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleEnemyState
    {
        public BattleEnemyState(
            BattleEntityId enemyId,
            string definitionId,
            FactionId factionId,
            Vec2 position,
            int health,
            int maxHealth,
            float movementSpeed,
            float detectionRange,
            float attackRange = 0f)
        {
            EnemyId = enemyId;
            DefinitionId = definitionId;
            FactionId = factionId;
            Position = position;
            Health = health;
            MaxHealth = maxHealth;
            MovementSpeed = movementSpeed;
            DetectionRange = detectionRange;
            AttackRange = attackRange;
        }

        public BattleEntityId EnemyId { get; private set; }
        public string DefinitionId { get; private set; }
        public FactionId FactionId { get; private set; }
        public Vec2 Position { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public float MovementSpeed { get; private set; }
        public float DetectionRange { get; private set; }
        public float AttackRange { get; private set; }
        public BattleEntityId? CurrentTargetMemberId { get; private set; }

        public bool IsAlive
        {
            get { return Health > 0; }
        }

        public void ApplyDamage(int damage)
        {
            if (damage <= 0 || !IsAlive)
            {
                return;
            }

            Health -= damage;
            if (Health < 0)
            {
                Health = 0;
            }
        }

        public void UpdatePosition(Vec2 position)
        {
            Position = position;
        }

        public void SetCurrentTargetMember(BattleEntityId? memberId)
        {
            CurrentTargetMemberId = memberId;
        }
    }
}
