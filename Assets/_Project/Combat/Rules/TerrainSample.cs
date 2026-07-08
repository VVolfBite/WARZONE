using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class TerrainSample
    {
        public TerrainSample(Vec2 center, float radius, TerrainType terrainType, float speedMultiplier, float rangeMultiplier, bool blocksLineOfSight)
        {
            Center = center;
            Radius = radius;
            TerrainType = terrainType;
            SpeedMultiplier = speedMultiplier;
            RangeMultiplier = rangeMultiplier;
            BlocksLineOfSight = blocksLineOfSight;
        }

        public Vec2 Center { get; private set; }
        public float Radius { get; private set; }
        public TerrainType TerrainType { get; private set; }
        public float SpeedMultiplier { get; private set; }
        public float RangeMultiplier { get; private set; }
        public bool BlocksLineOfSight { get; private set; }

        public bool Contains(Vec2 point)
        {
            return Vec2.DistanceSquared(Center, point) <= Radius * Radius;
        }
    }
}




