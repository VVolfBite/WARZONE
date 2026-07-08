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

        public Vector2 Center { get; private set; }
        public float Radius { get; private set; }
        public TerrainType TerrainType { get; private set; }
        public float SpeedMultiplier { get; private set; }
        public float RangeMultiplier { get; private set; }
        public bool BlocksLineOfSight { get; private set; }

        public bool Contains(Vector2 point)
        {
            return Vector2.DistanceSquared(Center, point) <= Radius * Radius;
        }
    }
}



