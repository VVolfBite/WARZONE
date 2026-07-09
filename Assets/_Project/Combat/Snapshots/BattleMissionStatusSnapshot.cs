namespace Warzone.Combat
{
    public sealed class BattleMissionStatusSnapshot
    {
        public BattleMissionStatusSnapshot(
            int aliveEnemyCount,
            int totalEnemyCount,
            int searchedPointCount,
            int totalSearchPointCount,
            int extractedMemberCount,
            int totalAliveMemberCount,
            bool isObjectiveComplete)
        {
            AliveEnemyCount = aliveEnemyCount;
            TotalEnemyCount = totalEnemyCount;
            SearchedPointCount = searchedPointCount;
            TotalSearchPointCount = totalSearchPointCount;
            ExtractedMemberCount = extractedMemberCount;
            TotalAliveMemberCount = totalAliveMemberCount;
            IsObjectiveComplete = isObjectiveComplete;
        }

        public int AliveEnemyCount { get; private set; }
        public int TotalEnemyCount { get; private set; }
        public int SearchedPointCount { get; private set; }
        public int TotalSearchPointCount { get; private set; }
        public int ExtractedMemberCount { get; private set; }
        public int TotalAliveMemberCount { get; private set; }
        public bool IsObjectiveComplete { get; private set; }
    }
}
