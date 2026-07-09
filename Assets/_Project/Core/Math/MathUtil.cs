namespace Warzone.Core.Math
{
    public static class MathUtil
    {
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
        }

        public static float DistancePointToSegment(Vec2 point, Vec2 segmentStart, Vec2 segmentEnd)
        {
            Vec2 segment = segmentEnd - segmentStart;
            float lengthSquared = segment.LengthSquared;
            if (lengthSquared <= 0.0001f)
            {
                return Vec2.Distance(point, segmentStart);
            }

            float projection = Vec2.Dot(point - segmentStart, segment) / lengthSquared;
            projection = Clamp(projection, 0f, 1f);
            Vec2 nearestPoint = segmentStart + (segment * projection);
            return Vec2.Distance(point, nearestPoint);
        }
    }
}
