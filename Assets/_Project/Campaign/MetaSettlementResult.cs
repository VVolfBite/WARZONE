namespace Warzone.Campaign
{
    public sealed class MetaSettlementResult
    {
        public MetaSettlementResult(bool missionCompleted, int unitsLost, int unitsKept)
        {
            MissionCompleted = missionCompleted;
            UnitsLost = unitsLost;
            UnitsKept = unitsKept;
        }

        public bool MissionCompleted { get; private set; }
        public int UnitsLost { get; private set; }
        public int UnitsKept { get; private set; }
    }
}




