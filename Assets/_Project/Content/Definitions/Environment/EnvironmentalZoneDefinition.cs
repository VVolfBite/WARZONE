namespace Warzone.Content.Definitions
{
    public sealed class EnvironmentalZoneDefinition
    {
        public EnvironmentalZoneDefinition(
            string id,
            string displayName,
            Warzone.Combat.EnvironmentalZoneType zoneType,
            float defaultRadius,
            float defaultIntensity,
            float defaultDurationSeconds,
            float visionPenalty,
            float damagePerSecond,
            float pressurePerSecond)
        {
            Id = id;
            DisplayName = displayName;
            ZoneType = zoneType;
            DefaultRadius = defaultRadius;
            DefaultIntensity = defaultIntensity;
            DefaultDurationSeconds = defaultDurationSeconds;
            VisionPenalty = visionPenalty;
            DamagePerSecond = damagePerSecond;
            PressurePerSecond = pressurePerSecond;
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public Warzone.Combat.EnvironmentalZoneType ZoneType { get; private set; }
        public float DefaultRadius { get; private set; }
        public float DefaultIntensity { get; private set; }
        public float DefaultDurationSeconds { get; private set; }
        public float VisionPenalty { get; private set; }
        public float DamagePerSecond { get; private set; }
        public float PressurePerSecond { get; private set; }
    }
}

