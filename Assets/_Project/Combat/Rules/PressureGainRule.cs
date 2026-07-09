namespace Warzone.Combat
{
    public static class PressureGainRule
    {
        public const float DamagePressureFactor = 1.5f;
        public const float IncomingFirePressure = 5f;
        public const float NearbyDeathPressure = 12f;
        public const float RecentIncomingFireDurationSeconds = 2f;

        public static float FromDamage(int damageAmount)
        {
            if (damageAmount <= 0)
            {
                return 0f;
            }

            return damageAmount * DamagePressureFactor;
        }
    }
}
