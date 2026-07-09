namespace Warzone.Combat
{
    public static class CoverModifierRule
    {
        public const float LowCoverDamageMultiplier = 0.7f;
        public const float HighCoverDamageMultiplier = 0.5f;

        public static bool HasCoverNode(BattleState battleState, BattleMemberState memberState)
        {
            return FindCoverForMemberQuery.Execute(battleState, memberState) != null;
        }

        public static float GetDamageMultiplier(TacticalObstacleState obstacleState)
        {
            if (obstacleState == null || !obstacleState.ProvidesCover)
            {
                return 1f;
            }

            if (obstacleState.ObstacleType == TacticalObstacleType.HighCover || obstacleState.ObstacleType == TacticalObstacleType.Wall)
            {
                return HighCoverDamageMultiplier;
            }

            return LowCoverDamageMultiplier;
        }
    }
}
