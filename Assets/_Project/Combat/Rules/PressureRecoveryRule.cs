namespace Warzone.Combat
{
    public static class PressureRecoveryRule
    {
        public const float RecoveryPerSecond = 6f;
        public const float SuppressionRecoveryPerSecond = 0.35f;

        public static float GetRecoveryAmount(float deltaTimeSeconds)
        {
            return deltaTimeSeconds <= 0f ? 0f : RecoveryPerSecond * deltaTimeSeconds;
        }

        public static float GetSuppressionRecoveryAmount(float deltaTimeSeconds)
        {
            return deltaTimeSeconds <= 0f ? 0f : SuppressionRecoveryPerSecond * deltaTimeSeconds;
        }
    }
}
