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

        public string Id { get; private set; }
        public float Range { get; private set; }
        public float AttackIntervalSeconds { get; private set; }
        public int DamagePerHit { get; private set; }
        public float ProjectileSpeed { get; private set; }
        public DamageType DamageType { get; private set; }
    }
}



