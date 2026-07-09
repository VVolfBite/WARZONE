namespace Warzone.Combat
{
    public sealed class FireLineResult
    {
        public FireLineResult(
            bool canFire,
            int? blockingObstacleId = null,
            TacticalObstacleType? blockingObstacleType = null,
            int? coverObstacleId = null,
            TacticalObstacleType? coverObstacleType = null,
            float damageMultiplier = 1f)
        {
            CanFire = canFire;
            BlockingObstacleId = blockingObstacleId;
            BlockingObstacleType = blockingObstacleType;
            CoverObstacleId = coverObstacleId;
            CoverObstacleType = coverObstacleType;
            DamageMultiplier = damageMultiplier;
        }

        public bool CanFire { get; private set; }
        public int? BlockingObstacleId { get; private set; }
        public TacticalObstacleType? BlockingObstacleType { get; private set; }
        public int? CoverObstacleId { get; private set; }
        public TacticalObstacleType? CoverObstacleType { get; private set; }
        public float DamageMultiplier { get; private set; }
    }
}
