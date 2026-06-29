namespace Warzone.Content.Definitions
{
    public sealed class WeaponDefinition
    {
        public WeaponDefinition(
            string id,
            float range,
            float attackIntervalSeconds,
            int damagePerHit,
            float projectileSpeed,
            DamageType damageType)
        {
            Id = id;
            Range = range;
            AttackIntervalSeconds = attackIntervalSeconds;
            DamagePerHit = damagePerHit;
            ProjectileSpeed = projectileSpeed;
            DamageType = damageType;
        }

        public string Id { get; }
        public float Range { get; }
        public float AttackIntervalSeconds { get; }
        public int DamagePerHit { get; }
        public float ProjectileSpeed { get; }
        public DamageType DamageType { get; }
    }
}
