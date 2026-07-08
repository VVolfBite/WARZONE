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

        public float UpdateIntervalSeconds { get; private set; }
        public float AggroRange { get; private set; }
        public float LeashRadius { get; private set; }
    }
}



