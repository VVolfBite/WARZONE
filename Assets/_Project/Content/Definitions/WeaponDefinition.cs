namespace Warzone.Content.Definitions
{
    public sealed class WeaponDefinition
    {
        public WeaponDefinition(
            string id,
            float range,
            float attackIntervalSeconds,
            int damagePerHit,
            float projectileSpeed)
        {
            Id = id;
            Range = range;
            AttackIntervalSeconds = attackIntervalSeconds;
            DamagePerHit = damagePerHit;
            ProjectileSpeed = projectileSpeed;
        }

        public string Id { get; }
        public float Range { get; }
        public float AttackIntervalSeconds { get; }
        public int DamagePerHit { get; }
        public float ProjectileSpeed { get; }
    }
}
