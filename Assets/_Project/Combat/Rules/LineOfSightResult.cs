namespace Warzone.Combat
{
    public sealed class LineOfSightResult
    {
        public LineOfSightResult(bool hasLineOfSight, int? blockingObstacleId = null, TacticalObstacleType? blockingObstacleType = null)
        {
            HasLineOfSight = hasLineOfSight;
            BlockingObstacleId = blockingObstacleId;
            BlockingObstacleType = blockingObstacleType;
        }

        public bool HasLineOfSight { get; private set; }
        public int? BlockingObstacleId { get; private set; }
        public TacticalObstacleType? BlockingObstacleType { get; private set; }
    }
}
