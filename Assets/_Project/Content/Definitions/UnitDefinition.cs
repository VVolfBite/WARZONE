namespace Warzone.Content.Definitions
{
    public sealed class UnitDefinition
    {
        public UnitDefinition(
            string id,
            string displayName,
            FactionId factionId,
            int maxHealth,
            float moveSpeed,
            WeaponDefinition weapon,
            float aggroRange,
            float collisionRadius,
            ArmorType armorType,
            string defaultStatusEffectId = null,
            string activeAbilityId = null)
        {
            Id = id;
            DisplayName = displayName;
            FactionId = factionId;
            MaxHealth = maxHealth;
            MoveSpeed = moveSpeed;
            Weapon = weapon;
            AggroRange = aggroRange;
            CollisionRadius = collisionRadius;
            ArmorType = armorType;
            DefaultStatusEffectId = defaultStatusEffectId;
            ActiveAbilityId = activeAbilityId;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public FactionId FactionId { get; }
        public int MaxHealth { get; }
        public float MoveSpeed { get; }
        public WeaponDefinition Weapon { get; }
        public float AggroRange { get; }
        public float CollisionRadius { get; }
        public ArmorType ArmorType { get; }
        public string DefaultStatusEffectId { get; }
        public string ActiveAbilityId { get; }
    }
}
