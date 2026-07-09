namespace Warzone.Combat
{
    public static class AccuracyRule
    {
        public static float ApplyCoverPenalty(float baseAccuracy, TacticalObstacleState obstacleState)
        {
            if (obstacleState == null)
            {
                return baseAccuracy;
            }

            float accuracy = baseAccuracy - obstacleState.AccuracyPenaltyAgainstOccupant;
            return accuracy < 0f ? 0f : accuracy;
        }
    }
}
