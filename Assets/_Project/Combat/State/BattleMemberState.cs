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
            float accuracyModifier = 1f,
            float maxPressure = 100f,
            int nightVisionLevel = 0,
            int smokeVisionLevel = 0,
            bool hasLightSource = false)
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
            MaxPressure = maxPressure > 0f ? maxPressure : 100f;
            NightVisionLevel = nightVisionLevel;
            SmokeVisionLevel = smokeVisionLevel;
            HasLightSource = hasLightSource;
            EffectiveDetectionRange = detectionRange;
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
        public float MaxPressure { get; private set; }
        public float Suppression { get; private set; }
        public bool IsSuppressed { get; private set; }
        public bool IsBroken { get; private set; }
        public bool IsRetreating { get; private set; }
        public Vec2? RetreatTargetPosition { get; private set; }
        public BattleEntityId? LastDamageSourceEnemyId { get; private set; }
        public float RecentIncomingFireSeconds { get; private set; }
        public string WeaponId { get; private set; }
        public string DefinitionId { get; private set; }
        public MemberIntent CurrentIntent { get; private set; }
        public float DetectionRange { get; private set; }
        public float EffectiveDetectionRange { get; private set; }
        public float AttackRange { get; private set; }
        public float AccuracyModifier { get; private set; }
        public int NightVisionLevel { get; private set; }
        public int SmokeVisionLevel { get; private set; }
        public bool HasLightSource { get; private set; }
        public float AttackCooldownRemaining { get; private set; }
        public BattleEntityId? CurrentTargetEnemyId { get; private set; }
        public bool IsExtracted { get; private set; }
        public int? OccupiedTacticalNodeId { get; private set; }

        public bool IsAlive
        {
            get { return Health > 0; }
        }

        public bool CanAct
        {
            get { return IsAlive && !IsExtracted; }
        }

        public bool CanReceiveOrders
        {
            get { return CanAct && !IsRetreating; }
        }

        public bool CanFight
        {
            get { return CanAct && !IsRetreating; }
        }

        public bool HasAssignedCover
        {
            get { return OccupiedTacticalNodeId.HasValue; }
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
            if (damage <= 0 || !IsAlive || IsExtracted)
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
            if (pressure < 0f)
            {
                Pressure = 0f;
                return;
            }

            Pressure = pressure > MaxPressure ? MaxPressure : pressure;
        }

        public void AddPressure(float pressure)
        {
            if (pressure <= 0f || !IsAlive || IsExtracted)
            {
                return;
            }

            SetPressure(Pressure + pressure);
        }

        public void ReducePressure(float pressure)
        {
            if (pressure <= 0f)
            {
                return;
            }

            SetPressure(Pressure - pressure);
        }

        public void SetSuppression(float suppression)
        {
            if (suppression < 0f)
            {
                Suppression = 0f;
                return;
            }

            Suppression = suppression > 1f ? 1f : suppression;
        }

        public void SetSuppressed(bool suppressed)
        {
            IsSuppressed = suppressed;
        }

        public void SetBroken(bool broken)
        {
            IsBroken = broken;
        }

        public void BeginRetreat(Vec2 retreatTargetPosition)
        {
            IsRetreating = true;
            RetreatTargetPosition = retreatTargetPosition;
        }

        public void ClearRetreat()
        {
            IsRetreating = false;
            IsBroken = false;
            RetreatTargetPosition = null;
        }

        public void SetRetreatTargetPosition(Vec2? retreatTargetPosition)
        {
            RetreatTargetPosition = retreatTargetPosition;
        }

        public void SetLastDamageSourceEnemy(BattleEntityId? enemyId)
        {
            LastDamageSourceEnemyId = enemyId;
        }

        public void SetRecentIncomingFire(float seconds)
        {
            RecentIncomingFireSeconds = seconds < 0f ? 0f : seconds;
        }

        public void TickIncomingFire(float deltaTimeSeconds)
        {
            if (RecentIncomingFireSeconds <= 0f)
            {
                return;
            }

            RecentIncomingFireSeconds -= deltaTimeSeconds;
            if (RecentIncomingFireSeconds < 0f)
            {
                RecentIncomingFireSeconds = 0f;
            }
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

        public void SetNightVisionLevel(int nightVisionLevel)
        {
            NightVisionLevel = nightVisionLevel < 0 ? 0 : nightVisionLevel;
        }

        public void SetSmokeVisionLevel(int smokeVisionLevel)
        {
            SmokeVisionLevel = smokeVisionLevel < 0 ? 0 : smokeVisionLevel;
        }

        public void SetHasLightSource(bool hasLightSource)
        {
            HasLightSource = hasLightSource;
        }

        public void SetEffectiveDetectionRange(float effectiveDetectionRange)
        {
            EffectiveDetectionRange = effectiveDetectionRange < 0f ? 0f : effectiveDetectionRange;
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

        public void MarkExtracted()
        {
            if (IsExtracted)
            {
                return;
            }

            IsExtracted = true;
            ClearIntent();
            SetCurrentTargetEnemy(null);
            SetVisibleEnemies(null);
            ClearOccupiedTacticalNode();
            ClearRetreat();
        }

        public void SetOccupiedTacticalNode(int? nodeId)
        {
            OccupiedTacticalNodeId = nodeId;
        }

        public void ClearOccupiedTacticalNode()
        {
            OccupiedTacticalNodeId = null;
        }
    }
}
