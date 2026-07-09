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
            float attackRange = 0f,
            int nightVisionLevel = 0,
            bool hasLightSource = false)
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
            NightVisionLevel = nightVisionLevel;
            HasLightSource = hasLightSource;
            EffectiveDetectionRange = detectionRange;
        }

        public BattleEntityId EnemyId { get; private set; }
        public string DefinitionId { get; private set; }
        public FactionId FactionId { get; private set; }
        public Vec2 Position { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public float MovementSpeed { get; private set; }
        public float DetectionRange { get; private set; }
        public float EffectiveDetectionRange { get; private set; }
        public float AttackRange { get; private set; }
        public int NightVisionLevel { get; private set; }
        public bool HasLightSource { get; private set; }
        public BattleEntityId? CurrentTargetMemberId { get; private set; }
        public float AttackCooldownRemaining { get; private set; }
        public int? OccupiedTacticalNodeId { get; private set; }

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

        public void TickAttackCooldown(float deltaTimeSeconds)
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

        public void SetNightVisionLevel(int nightVisionLevel)
        {
            NightVisionLevel = nightVisionLevel < 0 ? 0 : nightVisionLevel;
        }

        public void SetHasLightSource(bool hasLightSource)
        {
            HasLightSource = hasLightSource;
        }

        public void SetEffectiveDetectionRange(float effectiveDetectionRange)
        {
            EffectiveDetectionRange = effectiveDetectionRange < 0f ? 0f : effectiveDetectionRange;
        }

        public void SetOccupiedTacticalNode(int? nodeId)
        {
            OccupiedTacticalNodeId = nodeId;
        }
    }
}
