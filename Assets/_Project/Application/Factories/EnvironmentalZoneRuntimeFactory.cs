using Warzone.Combat;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Application
{
    public sealed class EnvironmentalZoneRuntimeFactory
    {
        public EnvironmentalZoneType Map(EnvironmentalZoneDefinitionType zoneType)
        {
            switch (zoneType)
            {
                case EnvironmentalZoneDefinitionType.Smoke:
                    return EnvironmentalZoneType.Smoke;
                case EnvironmentalZoneDefinitionType.Fire:
                    return EnvironmentalZoneType.Fire;
                case EnvironmentalZoneDefinitionType.Toxic:
                    return EnvironmentalZoneType.Toxic;
                case EnvironmentalZoneDefinitionType.Light:
                    return EnvironmentalZoneType.Light;
                case EnvironmentalZoneDefinitionType.Darkness:
                    return EnvironmentalZoneType.Darkness;
                default:
                    return EnvironmentalZoneType.Smoke;
            }
        }

        public EnvironmentalZoneState Create(
            int zoneId,
            EnvironmentalZoneDefinition definition,
            Vec2 position,
            float radius,
            float intensity,
            float durationRemaining,
            bool isActive = true)
        {
            if (definition == null)
            {
                return null;
            }

            return new EnvironmentalZoneState(
                zoneId,
                Map(definition.ZoneType),
                position,
                radius > 0f ? radius : definition.DefaultRadius,
                intensity > 0f ? intensity : definition.DefaultIntensity,
                durationRemaining > 0f ? durationRemaining : definition.DefaultDurationSeconds,
                isActive,
                definition.VisionPenalty,
                definition.DamagePerSecond,
                definition.PressurePerSecond);
        }
    }
}
