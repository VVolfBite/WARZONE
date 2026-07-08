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

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public float CooldownSeconds { get; private set; }
        public float Range { get; private set; }
        public int HealAmount { get; private set; }
        public string AppliedStatusEffectId { get; private set; }
    }
}



