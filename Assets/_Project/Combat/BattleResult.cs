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
        {
            MissionOutcome = missionOutcome;
            UnitOutcomes = unitOutcomes;
            Casualties = casualties;
            BattleStatistics = battleStatistics;
            ElapsedTimeSeconds = elapsedTimeSeconds;
            Seed = seed;
        }

        public MissionOutcome MissionOutcome { get; }
        public IReadOnlyList<UnitOutcome> UnitOutcomes { get; }
        public IReadOnlyList<BattleEntityId> Casualties { get; }
        public BattleStatistics BattleStatistics { get; }
        public float ElapsedTimeSeconds { get; }
        public int Seed { get; }
    }
}
