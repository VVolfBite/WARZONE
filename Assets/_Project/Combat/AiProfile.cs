namespace Warzone.Combat
{
    public sealed class AiProfile
    {
        public AiProfile(float updateIntervalSeconds, float aggroRange, float leashRadius)
        {
            UpdateIntervalSeconds = updateIntervalSeconds;
            AggroRange = aggroRange;
            LeashRadius = leashRadius;
        }

        public float UpdateIntervalSeconds { get; }
        public float AggroRange { get; }
        public float LeashRadius { get; }
    }
}
