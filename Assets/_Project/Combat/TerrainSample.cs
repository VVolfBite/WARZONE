using System.Numerics;

namespace Warzone.Combat
{
    public sealed class TerrainSample
    {
        public TerrainSample(Vector2 center, float radius, TerrainType terrainType, float speedMultiplier, float rangeMultiplier, bool blocksLineOfSight)
        {
            Center = center;
            Radius = radius;
            TerrainType = terrainType;
            SpeedMultiplier = speedMultiplier;
            RangeMultiplier = rangeMultiplier;
            BlocksLineOfSight = blocksLineOfSight;
        }

        public Vector2 Center { get; }
        public float Radius { get; }
        public TerrainType TerrainType { get; }
        public float SpeedMultiplier { get; }
        public float RangeMultiplier { get; }
        public bool BlocksLineOfSight { get; }

        public bool Contains(Vector2 point)
        {
            return Vector2.DistanceSquared(Center, point) <= Radius * Radius;
        }
    }
}
