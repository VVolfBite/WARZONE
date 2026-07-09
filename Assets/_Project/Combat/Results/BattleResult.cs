using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class BattleResult
    {
        public BattleResult(
            MissionOutcome missionOutcome,
            IReadOnlyList<UnitOutcome> unitOutcomes,
            IReadOnlyList<BattleEntityId> casualties,
            BattleStatistics battleStatistics,
            float elapsedTimeSeconds,
            int seed)
            : this(
                missionOutcome,
                unitOutcomes,
                casualties,
                battleStatistics,
                elapsedTimeSeconds,
                seed,
                BattleCompletionType.Partial,
                new List<BattleObjectiveResult>(),
                new BattleCasualtyResult(casualties, new List<BattleEntityId>()),
                new BattleLootResult(0),
                new BattleExtractionResult(new List<BattleEntityId>(), 0))
        {
        }

        public BattleResult(
            MissionOutcome missionOutcome,
            IReadOnlyList<UnitOutcome> unitOutcomes,
            IReadOnlyList<BattleEntityId> casualties,
            BattleStatistics battleStatistics,
            float elapsedTimeSeconds,
            int seed,
            BattleCompletionType completionType,
            IReadOnlyList<BattleObjectiveResult> objectiveResults,
            BattleCasualtyResult casualtyResult,
            BattleLootResult lootResult,
            BattleExtractionResult extractionResult)
        {
            MissionOutcome = missionOutcome;
            UnitOutcomes = unitOutcomes;
            Casualties = casualties;
            BattleStatistics = battleStatistics;
            ElapsedTimeSeconds = elapsedTimeSeconds;
            Seed = seed;
            CompletionType = completionType;
            ObjectiveResults = objectiveResults;
            CasualtyResult = casualtyResult;
            LootResult = lootResult;
            ExtractionResult = extractionResult;
        }

        public MissionOutcome MissionOutcome { get; private set; }
        public IReadOnlyList<UnitOutcome> UnitOutcomes { get; private set; }
        public IReadOnlyList<BattleEntityId> Casualties { get; private set; }
        public BattleStatistics BattleStatistics { get; private set; }
        public float ElapsedTimeSeconds { get; private set; }
        public int Seed { get; private set; }
        public BattleCompletionType CompletionType { get; private set; }
        public IReadOnlyList<BattleObjectiveResult> ObjectiveResults { get; private set; }
        public BattleCasualtyResult CasualtyResult { get; private set; }
        public BattleLootResult LootResult { get; private set; }
        public BattleExtractionResult ExtractionResult { get; private set; }
    }
}
