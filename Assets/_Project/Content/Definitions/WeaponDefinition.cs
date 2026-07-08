namespace Warzone.Content.Definitions
{
    public sealed class WeaponDefinition
    {
        public WeaponDefinition(
            string id,
            string displayName,
            WeaponCategory category,
            AmmoCategory ammoCategory,
            FireMode fireMode,
            float range,
            float fireIntervalSeconds,
            int damage,
            float accuracy,
            int burstSize,
            float suppression,
            float projectileSpeed,
            DamageType damageType)
        {
            Id = id;
            DisplayName = displayName;
            Category = category;
            AmmoCategory = ammoCategory;
            FireMode = fireMode;
            Range = range;
            FireIntervalSeconds = fireIntervalSeconds;
            Damage = damage;
            Accuracy = accuracy;
            BurstSize = burstSize;
            Suppression = suppression;
            ProjectileSpeed = projectileSpeed;
            DamageType = damageType;
        }

        public WeaponDefinition(
            string id,
            float range,
            float attackIntervalSeconds,
            int damagePerHit,
            float projectileSpeed,
            DamageType damageType)
            : this(
                id,
                id,
                WeaponCategory.Rifle,
                AmmoCategory.Rifle,
                FireMode.Automatic,
                range,
                attackIntervalSeconds,
                damagePerHit,
                1f,
                1,
                0f,
                projectileSpeed,
                damageType)
        {
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public WeaponCategory Category { get; private set; }
        public AmmoCategory AmmoCategory { get; private set; }
        public FireMode FireMode { get; private set; }
        public float Range { get; private set; }
        public float FireIntervalSeconds { get; private set; }
        public int Damage { get; private set; }
        public float Accuracy { get; private set; }
        public int BurstSize { get; private set; }
        public float Suppression { get; private set; }
        public float ProjectileSpeed { get; private set; }
        public DamageType DamageType { get; private set; }

        public float AttackIntervalSeconds
        {
            get { return FireIntervalSeconds; }
        }

        public int DamagePerHit
        {
            get { return Damage; }
        }
    }
}
