namespace Warzone.Combat
{
    public static class EnvironmentalPressureRule
    {
        public static float GetPressurePerTick(EnvironmentalZoneState zoneState, float deltaTimeSeconds)
        {
            if (zoneState == null || !zoneState.IsActive || zoneState.PressurePerSecond <= 0f || deltaTimeSeconds <= 0f)
            {
                return 0f;
            }

            return zoneState.PressurePerSecond * zoneState.Intensity * deltaTimeSeconds;
        }
    }
}
