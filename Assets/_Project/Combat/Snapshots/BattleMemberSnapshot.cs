using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class BattleMemberSnapshot
    {
        public BattleMemberSnapshot(
            BattleEntityId memberId,
            int squadId,
            Vec2 position,
            Vec2 facing,
            int health,
            int maxHealth,
            bool isAlive,
            bool isExtracted,
            string weaponId,
            BattleEntityId? currentTargetEnemyId,
            float attackCooldownRemaining,
            string currentIntent,
            Vec2? moveTarget,
            bool hasReachedTarget)
        {
            MemberId = memberId;
            SquadId = squadId;
            Position = position;
            Facing = facing;
            Health = health;
            MaxHealth = maxHealth;
            IsAlive = isAlive;
            IsExtracted = isExtracted;
            WeaponId = weaponId;
            CurrentTargetEnemyId = currentTargetEnemyId;
            AttackCooldownRemaining = attackCooldownRemaining;
            CurrentIntent = currentIntent;
            MoveTarget = moveTarget;
            HasReachedTarget = hasReachedTarget;
        }

        public BattleEntityId MemberId { get; private set; }
        public int SquadId { get; private set; }
        public Vec2 Position { get; private set; }
        public Vec2 Facing { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public bool IsAlive { get; private set; }
        public bool IsExtracted { get; private set; }
        public string WeaponId { get; private set; }
        public BattleEntityId? CurrentTargetEnemyId { get; private set; }
        public float AttackCooldownRemaining { get; private set; }
        public string CurrentIntent { get; private set; }
        public Vec2? MoveTarget { get; private set; }
        public bool HasReachedTarget { get; private set; }
    }
}
