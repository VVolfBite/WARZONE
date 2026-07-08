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

        public MissionOutcome MissionOutcome { get; private set; }
        public IReadOnlyList<UnitOutcome> UnitOutcomes { get; private set; }
        public IReadOnlyList<BattleEntityId> Casualties { get; private set; }
        public BattleStatistics BattleStatistics { get; private set; }
        public float ElapsedTimeSeconds { get; private set; }
        public int Seed { get; private set; }
    }
}



