using System.Collections.Generic;
using Warzone.Core.Math;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleMemberState
    {
        private readonly List<BattleEntityId> _visibleEnemyIds = new List<BattleEntityId>();

        public BattleMemberState(
            BattleEntityId memberId,
            int squadId,
            FactionId factionId,
            Vec2 position,
            int health,
            int maxHealth,
            float movementSpeed,
            string weaponId = null,
            string definitionId = null,
            float detectionRange = 12f,
            float attackRange = 10f,
            float accuracyModifier = 1f)
        {
            MemberId = memberId;
            SquadId = squadId;
            FactionId = factionId;
            Position = position;
            Facing = new Vec2(0f, 1f);
            Health = health;
            MaxHealth = maxHealth;
            MovementSpeed = movementSpeed;
            WeaponId = weaponId;
            DefinitionId = definitionId;
            DetectionRange = detectionRange;
            AttackRange = attackRange;
            AccuracyModifier = accuracyModifier;
        }

        public BattleEntityId MemberId { get; private set; }
        public int SquadId { get; private set; }
        public FactionId FactionId { get; private set; }
        public Vec2 Position { get; private set; }
        public Vec2 Facing { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public float MovementSpeed { get; private set; }
        public float Pressure { get; private set; }
        public string WeaponId { get; private set; }
        public string DefinitionId { get; private set; }
        public MemberIntent CurrentIntent { get; private set; }
        public float DetectionRange { get; private set; }
        public float AttackRange { get; private set; }
        public float AccuracyModifier { get; private set; }
        public float AttackCooldownRemaining { get; private set; }
        public BattleEntityId? CurrentTargetEnemyId { get; private set; }

        public bool IsAlive
        {
            get { return Health > 0; }
        }

        public IReadOnlyList<BattleEntityId> VisibleEnemyIds
        {
            get { return _visibleEnemyIds; }
        }

        public void SetIntent(MemberIntent intent)
        {
            CurrentIntent = intent;
        }

        public void ClearIntent()
        {
            CurrentIntent = null;
        }

        public void UpdatePosition(Vec2 position)
        {
            Position = position;
        }

        public void UpdateFacing(Vec2 facing)
        {
            if (facing.LengthSquared <= 0.0001f)
            {
                return;
            }

            Facing = Vec2.Normalize(facing);
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

        public void SetPressure(float pressure)
        {
            Pressure = pressure < 0f ? 0f : pressure;
        }

        public void TickCooldown(float deltaTimeSeconds)
        {
            if (AttackCooldownRemaining <= 0f)
            {
                return;
            }

            AttackCooldownRemaining -= deltaTimeSeconds;
            if (AttackCooldownRemaining < 0f)
            {
                AttackCooldownRemaining = 0f;
            }
        }

        public void ResetAttackCooldown(float cooldownSeconds)
        {
            AttackCooldownRemaining = cooldownSeconds < 0f ? 0f : cooldownSeconds;
        }

        public void SetCurrentTargetEnemy(BattleEntityId? enemyId)
        {
            CurrentTargetEnemyId = enemyId;
        }

        public void SetVisibleEnemies(IReadOnlyList<BattleEntityId> enemyIds)
        {
            _visibleEnemyIds.Clear();
            if (enemyIds == null)
            {
                return;
            }

            for (int i = 0; i < enemyIds.Count; i++)
            {
                _visibleEnemyIds.Add(enemyIds[i]);
            }
        }
    }
}
