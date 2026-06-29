namespace Warzone.Combat
{
    public sealed class StatusEffectDefinition
    {
        public StatusEffectDefinition(string id, string displayName, float durationSeconds, float tickIntervalSeconds, int flatDamagePerTick, int flatHealingPerTick, float moveSpeedMultiplier, float rangeMultiplier)
        {
            Id = id;
            DisplayName = displayName;
            DurationSeconds = durationSeconds;
            TickIntervalSeconds = tickIntervalSeconds;
            FlatDamagePerTick = flatDamagePerTick;
            FlatHealingPerTick = flatHealingPerTick;
            MoveSpeedMultiplier = moveSpeedMultiplier;
            RangeMultiplier = rangeMultiplier;
        }

        public string Id { get; }
        public string DisplayName { get; }
        public float DurationSeconds { get; }
        public float TickIntervalSeconds { get; }
        public int FlatDamagePerTick { get; }
        public int FlatHealingPerTick { get; }
        public float MoveSpeedMultiplier { get; }
        public float RangeMultiplier { get; }
    }
}
