using Warzone.Combat;

namespace Warzone.Meta
{
    public sealed class ProgressionService
    {
        public MetaSettlementResult ApplyBattleResult(BattleResult battleResult)
        {
            int keptUnits = 0;
            for (int i = 0; i < battleResult.UnitOutcomes.Count; i++)
            {
                if (battleResult.UnitOutcomes[i].IsSurvived)
                {
                    keptUnits++;
                }
            }

            return new MetaSettlementResult(
                missionCompleted: battleResult.MissionOutcome == MissionOutcome.Victory,
                unitsLost: battleResult.Casualties.Count,
                unitsKept: keptUnits);
        }
    }
}
