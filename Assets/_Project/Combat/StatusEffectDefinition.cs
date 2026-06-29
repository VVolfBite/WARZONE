namespace Warzone.Combat
{
    public sealed class StatusEffectDefinition
    {
        public StatusEffectDefinition(string id, string displayName, float durationSeconds, float tickIntervalSeconds, int flatDamagePerTick, float moveSpeedMultiplier, float rangeMultiplier)
        {
            Id = id;
            DisplayName = displayName;
            DurationSeconds = durationSeconds;
            TickIntervalSeconds = tickIntervalSeconds;
            FlatDamagePerTick = flatDamagePerTick;
            MoveSpeedMultiplier = moveSpeedMultiplier;
            RangeMultiplier = rangeMultiplier;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public float DurationSeconds { get; }
        public float TickIntervalSeconds { get; }
        public int FlatDamagePerTick { get; }
        public float MoveSpeedMultiplier { get; }
        public float RangeMultiplier { get; }
    }
}
