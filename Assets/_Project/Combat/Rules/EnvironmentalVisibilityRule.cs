using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class EnvironmentalVisibilityRule
    {
        public const float NightVisionLevelMitigation = 0.25f;
        public const float LightZoneMitigation = 0.2f;
        public const float DarknessPenaltyMultiplier = 0.6f;

        public static float GetEffectiveDetectionRange(
            BattleEnvironmentState environmentState,
            Vec2 observerPosition,
            float baseDetectionRange,
            int nightVisionLevel,
            Vec2 targetPosition,
            bool targetHasLightSource)
        {
            float effectiveRange = baseDetectionRange;
            if (environmentState == null)
            {
                return effectiveRange;
            }

            if (environmentState.IsNight)
            {
                float multiplier = environmentState.GlobalVisibilityMultiplier;
                multiplier += nightVisionLevel * NightVisionLevelMitigation;
                if (targetHasLightSource || IsInsideActiveZone(environmentState, EnvironmentalZoneType.Light, targetPosition))
                {
                    multiplier += LightZoneMitigation;
                }

                if (IsInsideActiveZone(environmentState, EnvironmentalZoneType.Darkness, targetPosition))
                {
                    multiplier *= DarknessPenaltyMultiplier;
                }

                if (multiplier > 1f)
                {
                    multiplier = 1f;
                }
                else if (multiplier < 0f)
                {
                    multiplier = 0f;
                }

                effectiveRange *= multiplier;
            }

            effectiveRange *= GetSmokeRangeMultiplier(environmentState, observerPosition, targetPosition, 0);
            return effectiveRange < 0f ? 0f : effectiveRange;
        }

        public static bool TryGetBlockingSmokeZone(BattleEnvironmentState environmentState, Vec2 origin, Vec2 target, int smokeVisionLevel, out EnvironmentalZoneState zoneState)
        {
            zoneState = null;
            if (environmentState == null)
            {
                return false;
            }

            foreach (EnvironmentalZoneState currentZoneState in environmentState.ZonesById.Values)
            {
                if (currentZoneState == null || !currentZoneState.IsActive || currentZoneState.ZoneType != EnvironmentalZoneType.Smoke)
                {
                    continue;
                }

                if (!DoesSegmentIntersectZone(origin, target, currentZoneState))
                {
                    continue;
                }

                float effectiveIntensity = currentZoneState.Intensity - (smokeVisionLevel * 0.2f);
                if (effectiveIntensity >= 0.5f)
                {
                    zoneState = currentZoneState;
                    return true;
                }
            }

            return false;
        }

        public static float GetSmokeRangeMultiplier(BattleEnvironmentState environmentState, Vec2 origin, Vec2 target, int smokeVisionLevel)
        {
            if (environmentState == null)
            {
                return 1f;
            }

            float multiplier = 1f;
            foreach (EnvironmentalZoneState zoneState in environmentState.ZonesById.Values)
            {
                if (zoneState == null || !zoneState.IsActive || zoneState.ZoneType != EnvironmentalZoneType.Smoke)
                {
                    continue;
                }

                if (!DoesSegmentIntersectZone(origin, target, zoneState))
                {
                    continue;
                }

                float effectivePenalty = zoneState.VisionPenalty - (smokeVisionLevel * 0.1f);
                if (effectivePenalty < 0f)
                {
                    effectivePenalty = 0f;
                }

                multiplier *= 1f - effectivePenalty;
            }

            return multiplier < 0.1f ? 0.1f : multiplier;
        }

        public static bool IsInsideActiveZone(BattleEnvironmentState environmentState, EnvironmentalZoneType zoneType, Vec2 position)
        {
            if (environmentState == null)
            {
                return false;
            }

            foreach (EnvironmentalZoneState zoneState in environmentState.ZonesById.Values)
            {
                if (zoneState != null && zoneState.IsActive && zoneState.ZoneType == zoneType && zoneState.Contains(position))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<EnvironmentalZoneState> FindZonesContaining(BattleEnvironmentState environmentState, Vec2 position)
        {
            List<EnvironmentalZoneState> zones = new List<EnvironmentalZoneState>();
            if (environmentState == null)
            {
                return zones;
            }

            foreach (EnvironmentalZoneState zoneState in environmentState.ZonesById.Values)
            {
                if (zoneState != null && zoneState.IsActive && zoneState.Contains(position))
                {
                    zones.Add(zoneState);
                }
            }

            return zones;
        }

        private static bool DoesSegmentIntersectZone(Vec2 origin, Vec2 target, EnvironmentalZoneState zoneState)
        {
            Vec2 segment = target - origin;
            float lengthSquared = segment.LengthSquared;
            if (lengthSquared <= 0.0001f)
            {
                return zoneState.Contains(origin);
            }

            float t = Vec2.Dot(zoneState.Position - origin, segment) / lengthSquared;
            if (t < 0f)
            {
                t = 0f;
            }
            else if (t > 1f)
            {
                t = 1f;
            }

            Vec2 closestPoint = origin + (segment * t);
            return Vec2.DistanceSquared(closestPoint, zoneState.Position) <= zoneState.Radius * zoneState.Radius;
        }
    }
}

