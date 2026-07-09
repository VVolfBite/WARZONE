using Warzone.Core.Math;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleEnemySnapshot
    {
        public BattleEnemySnapshot(
            BattleEntityId enemyId,
            string definitionId,
            FactionId factionId,
            Vec2 position,
            int health,
            int maxHealth,
            bool isAlive,
            int? occupiedTacticalNodeId,
            BattleEntityId? currentTargetMemberId,
            float attackCooldownRemaining)
        {
            EnemyId = enemyId;
            DefinitionId = definitionId;
            FactionId = factionId;
            Position = position;
            Health = health;
            MaxHealth = maxHealth;
            IsAlive = isAlive;
            OccupiedTacticalNodeId = occupiedTacticalNodeId;
            CurrentTargetMemberId = currentTargetMemberId;
            AttackCooldownRemaining = attackCooldownRemaining;
        }

        public BattleEntityId EnemyId { get; private set; }
        public string DefinitionId { get; private set; }
        public FactionId FactionId { get; private set; }
        public Vec2 Position { get; private set; }
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public bool IsAlive { get; private set; }
        public int? OccupiedTacticalNodeId { get; private set; }
        public BattleEntityId? CurrentTargetMemberId { get; private set; }
        public float AttackCooldownRemaining { get; private set; }
    }
}
