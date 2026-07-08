namespace Warzone.Campaign
{
    public sealed class ProgressionService
    {
        public MetaSettlementResult ApplySettlement(CampaignSettlement settlement)
        {
            return new MetaSettlementResult(
                missionCompleted: settlement.MissionCompleted,
                unitsLost: settlement.UnitsLost,
                unitsKept: settlement.UnitsKept);
        }
    }
}



