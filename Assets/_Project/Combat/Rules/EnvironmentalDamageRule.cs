namespace Warzone.Combat
{
    public static class EnvironmentalDamageRule
    {
        public static int GetDamagePerTick(EnvironmentalZoneState zoneState, float deltaTimeSeconds)
        {
            if (zoneState == null || !zoneState.IsActive || zoneState.DamagePerSecond <= 0f || deltaTimeSeconds <= 0f)
            {
                return 0;
            }

            float rawDamage = zoneState.DamagePerSecond * zoneState.Intensity * deltaTimeSeconds;
            int damage = (int)System.Math.Ceiling(rawDamage);
            return damage > 0 ? damage : 0;
        }
    }
}

