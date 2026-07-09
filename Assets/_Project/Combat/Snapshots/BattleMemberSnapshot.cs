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
            int? occupiedTacticalNodeId,
            string weaponId,
            BattleEntityId? currentTargetEnemyId,
            float attackCooldownRemaining,
            string currentIntent,
            Vec2? moveTarget,
            bool hasReachedTarget,
            float pressure,
            float maxPressure,
            bool isSuppressed,
            bool isBroken,
            bool isRetreating,
            Vec2? retreatTargetPosition,
            int nightVisionLevel,
            int smokeVisionLevel,
            bool hasLightSource,
            float detectionRange,
            float effectiveDetectionRange)
        {
            MemberId = memberId;
            SquadId = squadId;
            Position = position;
            Facing = facing;
            Health = health;
            MaxHealth = maxHealth;
            IsAlive = isAlive;
            IsExtracted = isExtracted;
            OccupiedTacticalNodeId = occupiedTacticalNodeId;
            WeaponId = weaponId;
            CurrentTargetEnemyId = currentTargetEnemyId;
            AttackCooldownRemaining = attackCooldownRemaining;
            CurrentIntent = currentIntent;
            MoveTarget = moveTarget;
            HasReachedTarget = hasReachedTarget;
            Pressure = pressure;
            MaxPressure = maxPressure;
            IsSuppressed = isSuppressed;
            IsBroken = isBroken;
            IsRetreating = isRetreating;
            RetreatTargetPosition = retreatTargetPosition;
            NightVisionLevel = nightVisionLevel;
            SmokeVisionLevel = smokeVisionLevel;
            HasLightSource = hasLightSource;
            DetectionRange = detectionRange;
            EffectiveDetectionRange = effectiveDetectionRange;
        }

        public BattleEntityId MemberId { get; private set; }
        public int SquadId { get; private set; }
        public Vec2 Position { get; private set; }
        public Vec2 Facing { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public bool IsAlive { get; private set; }
        public bool IsExtracted { get; private set; }
        public int? OccupiedTacticalNodeId { get; private set; }
        public string WeaponId { get; private set; }
        public BattleEntityId? CurrentTargetEnemyId { get; private set; }
        public float AttackCooldownRemaining { get; private set; }
        public string CurrentIntent { get; private set; }
        public Vec2? MoveTarget { get; private set; }
        public bool HasReachedTarget { get; private set; }
        public float Pressure { get; private set; }
        public float MaxPressure { get; private set; }
        public bool IsSuppressed { get; private set; }
        public bool IsBroken { get; private set; }
        public bool IsRetreating { get; private set; }
        public Vec2? RetreatTargetPosition { get; private set; }
        public int NightVisionLevel { get; private set; }
        public int SmokeVisionLevel { get; private set; }
        public bool HasLightSource { get; private set; }
        public float DetectionRange { get; private set; }
        public float EffectiveDetectionRange { get; private set; }
    }
}
