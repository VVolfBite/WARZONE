namespace Warzone.Combat
{
    public sealed class BattleMissionStatusSnapshot
    {
        public BattleMissionStatusSnapshot(
            int enteredBuildingCount,
            int totalBuildingObjectiveCount,
            int aliveEnemyCount,
            int totalEnemyCount,
            int searchedPointCount,
            int totalSearchPointCount,
            int extractedMemberCount,
            int totalAliveMemberCount,
            bool isObjectiveComplete,
            bool isEnterBuildingObjectiveComplete,
            bool isSearchObjectiveComplete,
            bool isEliminateObjectiveComplete,
            bool isExtractObjectiveComplete,
            bool isBattleComplete,
            BattleCompletionType resultType,
            int lootCount)
        {
            EnteredBuildingCount = enteredBuildingCount;
            TotalBuildingObjectiveCount = totalBuildingObjectiveCount;
            AliveEnemyCount = aliveEnemyCount;
            TotalEnemyCount = totalEnemyCount;
            SearchedPointCount = searchedPointCount;
            TotalSearchPointCount = totalSearchPointCount;
            ExtractedMemberCount = extractedMemberCount;
            TotalAliveMemberCount = totalAliveMemberCount;
            IsObjectiveComplete = isObjectiveComplete;
            IsEnterBuildingObjectiveComplete = isEnterBuildingObjectiveComplete;
            IsSearchObjectiveComplete = isSearchObjectiveComplete;
            IsEliminateObjectiveComplete = isEliminateObjectiveComplete;
            IsExtractObjectiveComplete = isExtractObjectiveComplete;
            IsBattleComplete = isBattleComplete;
            ResultType = resultType;
            LootCount = lootCount;
        }

        public int EnteredBuildingCount { get; private set; }
        public int TotalBuildingObjectiveCount { get; private set; }
        public int AliveEnemyCount { get; private set; }
        public int TotalEnemyCount { get; private set; }
        public int SearchedPointCount { get; private set; }
        public int TotalSearchPointCount { get; private set; }
        public int ExtractedMemberCount { get; private set; }
        public int TotalAliveMemberCount { get; private set; }
        public bool IsObjectiveComplete { get; private set; }
        public bool IsEnterBuildingObjectiveComplete { get; private set; }
        public bool IsSearchObjectiveComplete { get; private set; }
        public bool IsEliminateObjectiveComplete { get; private set; }
        public bool IsExtractObjectiveComplete { get; private set; }
        public bool IsBattleComplete { get; private set; }
        public BattleCompletionType ResultType { get; private set; }
        public int LootCount { get; private set; }
    }
}
