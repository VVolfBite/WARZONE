namespace Warzone.Content.Definitions
{
    public sealed class EnemyDefinition
    {
        public EnemyDefinition(
            string id,
            string displayName,
            int maxHealth,
            float movementSpeed,
            float detectionRange,
            float attackRange,
            FactionId factionId,
            int attackDamage = 10,
            float attackIntervalSeconds = 0.9f)
        {
            Id = id;
            DisplayName = displayName;
            MaxHealth = maxHealth;
            MovementSpeed = movementSpeed;
            DetectionRange = detectionRange;
            AttackRange = attackRange;
            FactionId = factionId;
            AttackDamage = attackDamage;
            AttackIntervalSeconds = attackIntervalSeconds;
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public int MaxHealth { get; private set; }
        public float MovementSpeed { get; private set; }
        public float DetectionRange { get; private set; }
        public float AttackRange { get; private set; }
        public FactionId FactionId { get; private set; }
        public int AttackDamage { get; private set; }
        public float AttackIntervalSeconds { get; private set; }
    }
}
