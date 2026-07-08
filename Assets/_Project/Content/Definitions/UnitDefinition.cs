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

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public FactionId FactionId { get; private set; }
        public int MaxHealth { get; private set; }
        public float MoveSpeed { get; private set; }
        public WeaponDefinition Weapon { get; private set; }
        public float AggroRange { get; private set; }
        public float CollisionRadius { get; private set; }
        public ArmorType ArmorType { get; private set; }
        public string DefaultStatusEffectId { get; private set; }
        public string ActiveAbilityId { get; private set; }
    }
}



