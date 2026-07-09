using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class EnvironmentalZoneState
    {
        public EnvironmentalZoneState(
            int zoneId,
            EnvironmentalZoneType zoneType,
            Vec2 position,
            float radius,
            float intensity,
            float durationRemaining,
            bool isActive,
            float visionPenalty,
            float damagePerSecond,
            float pressurePerSecond)
        {
            ZoneId = zoneId;
            ZoneType = zoneType;
            Position = position;
            Radius = radius;
            Intensity = intensity;
            DurationRemaining = durationRemaining;
            IsActive = isActive;
            VisionPenalty = visionPenalty;
            DamagePerSecond = damagePerSecond;
            PressurePerSecond = pressurePerSecond;
        }

        public int ZoneId { get; private set; }
        public EnvironmentalZoneType ZoneType { get; private set; }
        public Vec2 Position { get; private set; }
        public float Radius { get; private set; }
        public float Intensity { get; private set; }
        public float DurationRemaining { get; private set; }
        public bool IsActive { get; private set; }
        public float VisionPenalty { get; private set; }
        public float DamagePerSecond { get; private set; }
        public float PressurePerSecond { get; private set; }

        public bool HasDuration
        {
            get { return DurationRemaining > 0f; }
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
        }

        public void TickDuration(float deltaTimeSeconds)
        {
            if (!IsActive || DurationRemaining <= 0f || deltaTimeSeconds <= 0f)
            {
                return;
            }

            DurationRemaining -= deltaTimeSeconds;
            if (DurationRemaining <= 0f)
            {
                DurationRemaining = 0f;
                IsActive = false;
            }
        }

        public bool Contains(Vec2 position)
        {
            return Vec2.DistanceSquared(Position, position) <= Radius * Radius;
        }
    }
}

