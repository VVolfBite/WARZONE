using System.Collections.Generic;
using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class TerrainMap
    {
        private readonly List<TerrainSample> _samples;

        public TerrainMap(IReadOnlyList<TerrainSample> samples)
        {
            _samples = new List<TerrainSample>(samples);
        }

        public static TerrainMap CreateDefault()
        {
            return new TerrainMap(new[]
            {
                new TerrainSample(new Vec2(-6f, 7f), 3.2f, TerrainType.Forest, 0.82f, 0.9f, true),
                new TerrainSample(new Vec2(7f, -8f), 2.8f, TerrainType.Rough, 0.9f, 0.95f, false),
                new TerrainSample(new Vec2(3f, 9f), 2.5f, TerrainType.Road, 1.08f, 1.05f, false),
                new TerrainSample(new Vec2(-2f, -6f), 2.2f, TerrainType.Water, 0.45f, 0.75f, true)
            });
        }

        public TerrainType GetTerrainAt(Vec2 position)
        {
            TerrainSample sample = FindSample(position);
            return sample != null ? sample.TerrainType : TerrainType.Plain;
        }

        public float GetMoveSpeedMultiplier(Vec2 position)
        {
            TerrainSample sample = FindSample(position);
            return sample != null ? sample.SpeedMultiplier : 1f;
        }

        public float GetRangeMultiplier(Vec2 position)
        {
            TerrainSample sample = FindSample(position);
            return sample != null ? sample.RangeMultiplier : 1f;
        }

        public bool BlocksLineOfSight(Vec2 from, Vec2 to)
        {
            for (int i = 0; i < _samples.Count; i++)
            {
                TerrainSample sample = _samples[i];
                if (!sample.BlocksLineOfSight)
                {
                    continue;
                }

                if (IntersectsCircle(from, to, sample.Center, sample.Radius))
                {
                    return true;
                }
            }

            return false;
        }

        private TerrainSample FindSample(Vec2 position)
        {
            for (int i = 0; i < _samples.Count; i++)
            {
                TerrainSample sample = _samples[i];
                if (sample.Contains(position))
                {
                    return sample;
                }
            }

            return null;
        }

        private static bool IntersectsCircle(Vec2 a, Vec2 b, Vec2 center, float radius)
        {
            Vec2 ab = b - a;
            float lengthSquared = ab.LengthSquared;
            if (lengthSquared <= 0.0001f)
            {
                return Vec2.DistanceSquared(a, center) <= radius * radius;
            }

            float t = Vec2.Dot(center - a, ab) / lengthSquared;
            t = t < 0f ? 0f : t > 1f ? 1f : t;
            Vec2 closest = a + (ab * t);
            return Vec2.DistanceSquared(closest, center) <= radius * radius;
        }
    }
}




