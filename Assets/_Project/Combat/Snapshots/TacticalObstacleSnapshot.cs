using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class TacticalObstacleSnapshot
    {
        public TacticalObstacleSnapshot(
            int obstacleId,
            TacticalObstacleType obstacleType,
            Vec2 position,
            float radius,
            bool blocksMovement,
            bool blocksLineOfSight,
            bool blocksFire,
            bool providesCover,
            float damageReductionFactor,
            bool isDestroyed)
        {
            ObstacleId = obstacleId;
            ObstacleType = obstacleType;
            Position = position;
            Radius = radius;
            BlocksMovement = blocksMovement;
            BlocksLineOfSight = blocksLineOfSight;
            BlocksFire = blocksFire;
            ProvidesCover = providesCover;
            DamageReductionFactor = damageReductionFactor;
            IsDestroyed = isDestroyed;
        }

        public int ObstacleId { get; private set; }
        public TacticalObstacleType ObstacleType { get; private set; }
        public Vec2 Position { get; private set; }
        public float Radius { get; private set; }
        public bool BlocksMovement { get; private set; }
        public bool BlocksLineOfSight { get; private set; }
        public bool BlocksFire { get; private set; }
        public bool ProvidesCover { get; private set; }
        public float DamageReductionFactor { get; private set; }
        public bool IsDestroyed { get; private set; }
    }
}
