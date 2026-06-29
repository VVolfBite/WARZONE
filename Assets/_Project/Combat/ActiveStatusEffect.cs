namespace Warzone.Combat
{
    public sealed class ActiveStatusEffect
    {
        public ActiveStatusEffect(StatusEffectDefinition definition)
        {
            Definition = definition;
            RemainingDuration = definition.DurationSeconds;
            TickTimer = definition.TickIntervalSeconds;
        }

        public StatusEffectDefinition Definition { get; }
        public float RemainingDuration { get; set; }
        public float TickTimer { get; set; }
    }
}
