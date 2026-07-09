namespace Warzone.Combat
{
    public static class SuppressionRule
    {
        public const float SuppressionThresholdRatio = 0.4f;
        public const float MovementSpeedMultiplier = 0.8f;
        public const float AttackCooldownMultiplier = 1.5f;

        public static bool IsSuppressed(BattleMemberState memberState)
        {
            return memberState != null &&
                   memberState.MaxPressure > 0f &&
                   memberState.Pressure >= memberState.MaxPressure * SuppressionThresholdRatio;
        }

        public static float ApplyMovementPenalty(float baseSpeed, bool isSuppressed)
        {
            return isSuppressed ? baseSpeed * MovementSpeedMultiplier : baseSpeed;
        }

        public static float ApplyAttackCooldownPenalty(float baseCooldown, bool isSuppressed)
        {
            return isSuppressed ? baseCooldown * AttackCooldownMultiplier : baseCooldown;
        }
    }
}
