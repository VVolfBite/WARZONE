namespace Warzone.Combat
{
    public sealed class ActiveStatusEffect
    {
        public ActiveStatusEffect(StatusEffectDefinition definition)
        {
            Definition = definition;
            RemainingDuration = definition.DurationSeconds;
            TickTimer = definition.TickIntervalSeconds;
            StackCount = 1;
        }

        public StatusEffectDefinition Definition { get; private set; }
        public float RemainingDuration { get; set; }
        public float TickTimer { get; set; }
        public int StackCount { get; set; }
    }
}

