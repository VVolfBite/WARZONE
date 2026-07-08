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

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public float DurationSeconds { get; private set; }
        public float TickIntervalSeconds { get; private set; }
        public int FlatDamagePerTick { get; private set; }
        public int FlatHealingPerTick { get; private set; }
        public float MoveSpeedMultiplier { get; private set; }
        public float RangeMultiplier { get; private set; }
    }
}




