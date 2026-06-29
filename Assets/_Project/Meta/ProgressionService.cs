using Warzone.Combat;

namespace Warzone.Meta
{
    public sealed class ProgressionService
    {
        public MetaSettlementResult ApplyBattleResult(BattleResult battleResult)
        {
            return new MetaSettlementResult(
                missionCompleted: battleResult.MissionOutcome == MissionOutcome.Victory,
                unitsLost: battleResult.Casualties.Count);
        }
    }
}
