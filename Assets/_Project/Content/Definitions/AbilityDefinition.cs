namespace Warzone.Content.Definitions
{
    public sealed class AbilityDefinition
    {
        public AbilityDefinition(string id, string displayName, float cooldownSeconds, float range, int healAmount, string appliedStatusEffectId = null)
        {
            Id = id;
            DisplayName = displayName;
            CooldownSeconds = cooldownSeconds;
            Range = range;
            HealAmount = healAmount;
            AppliedStatusEffectId = appliedStatusEffectId;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public float CooldownSeconds { get; }
        public float Range { get; }
        public int HealAmount { get; }
        public string AppliedStatusEffectId { get; }
    }
}
