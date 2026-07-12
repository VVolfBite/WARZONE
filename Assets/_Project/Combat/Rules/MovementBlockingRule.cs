using Warzone.Core.Math;

namespace Warzone.Combat
{
    public static class MovementBlockingRule
    {
        private const float SafePadding = 0.08f;

        public static bool IsMovementBlocked(BattleState battleState, Vec2 from, Vec2 to, out TacticalObstacleState blocker)
        {
            Vec2 safePosition;
            return TryProjectMovement(battleState, from, to, out blocker, out safePosition);
        }

        public static bool TryProjectMovement(BattleState battleState, Vec2 from, Vec2 desiredTo, out TacticalObstacleState blocker, out Vec2 safePosition)
        {
            blocker = null;
            safePosition = desiredTo;
            if (battleState == null)
            {
                return false;
            }

            float nearestT = float.MaxValue;
            foreach (TacticalObstacleState obstacleState in battleState.ObstaclesById.Values)
            {
                if (obstacleState == null || !obstacleState.BlocksMovement || obstacleState.IsDestroyed)
                {
                    continue;
                }

                float t;
                if (!SegmentIntersectsCircle(from, desiredTo, obstacleState.Position, obstacleState.Radius, out t))
                {
                    continue;
                }

                if (t < nearestT)
                {
                    nearestT = t;
                    blocker = obstacleState;
                }
            }

            if (blocker == null)
            {
                return false;
            }

            Vec2 movement = desiredTo - from;
            float safeT = System.Math.Max(0f, nearestT - (SafePadding / System.Math.Max(movement.Magnitude, 0.0001f)));
            safePosition = from + (movement * safeT);
            return true;
        }

        private static bool SegmentIntersectsCircle(Vec2 from, Vec2 to, Vec2 center, float radius, out float firstT)
        {
            firstT = 0f;
            Vec2 d = to - from;
            Vec2 f = from - center;
            if (Vec2.DistanceSquared(from, center) <= radius * radius)
            {
                return false;
            }

            float a = Vec2.Dot(d, d);
            if (a <= 0.0001f)
            {
                return Vec2.DistanceSquared(from, center) <= radius * radius;
            }

            float b = 2f * Vec2.Dot(f, d);
            float c = Vec2.Dot(f, f) - (radius * radius);
            float discriminant = (b * b) - (4f * a * c);
            if (discriminant < 0f)
            {
                return false;
            }

            discriminant = (float)System.Math.Sqrt(discriminant);
            float t1 = (-b - discriminant) / (2f * a);
            float t2 = (-b + discriminant) / (2f * a);
            if (t1 >= 0f && t1 <= 1f)
            {
                firstT = t1;
                return true;
            }

            if (t2 >= 0f && t2 <= 1f)
            {
                firstT = t2;
                return true;
            }

            return false;
        }
    }
}
