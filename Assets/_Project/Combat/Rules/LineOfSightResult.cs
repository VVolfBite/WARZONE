namespace Warzone.Combat
{
    public sealed class LineOfSightResult
    {
        public LineOfSightResult(
            bool hasLineOfSight,
            int? blockingObstacleId = null,
            TacticalObstacleType? blockingObstacleType = null,
            int? blockingZoneId = null,
            EnvironmentalZoneType? blockingZoneType = null)
        {
            HasLineOfSight = hasLineOfSight;
            BlockingObstacleId = blockingObstacleId;
            BlockingObstacleType = blockingObstacleType;
            BlockingZoneId = blockingZoneId;
            BlockingZoneType = blockingZoneType;
        }

        public bool HasLineOfSight { get; private set; }
        public int? BlockingObstacleId { get; private set; }
        public TacticalObstacleType? BlockingObstacleType { get; private set; }
        public int? BlockingZoneId { get; private set; }
        public EnvironmentalZoneType? BlockingZoneType { get; private set; }
    }
}
