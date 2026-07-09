using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class BreakRetreatRule
    {
        public const float BrokenThresholdRatio = 0.8f;
        public const float ClearOverridePressureRatio = 0.35f;
        public const float FallbackRetreatDistance = 6f;

        public static bool IsBroken(BattleMemberState memberState)
        {
            return memberState != null &&
                   memberState.MaxPressure > 0f &&
                   memberState.Pressure >= memberState.MaxPressure * BrokenThresholdRatio;
        }

        public static float GetClearOverridePressure(float maxPressure)
        {
            return maxPressure * ClearOverridePressureRatio;
        }

        public static Vec2 BuildFallbackRetreatTarget(Vec2 memberPosition, Vec2 nearestEnemyPosition)
        {
            Vec2 away = memberPosition - nearestEnemyPosition;
            if (away.LengthSquared <= 0.0001f)
            {
                away = new Vec2(-1f, 0f);
            }

            away = Vec2.Normalize(away);
            return memberPosition + (away * FallbackRetreatDistance);
        }
    }
}
