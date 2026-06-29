namespace Warzone.Meta
{
    public sealed class MetaSettlementResult
    {
        public MetaSettlementResult(bool missionCompleted, int unitsLost, int unitsKept)
        {
            MissionCompleted = missionCompleted;
            UnitsLost = unitsLost;
            UnitsKept = unitsKept;
        }

        public bool MissionCompleted { get; }
        public int UnitsLost { get; }
        public int UnitsKept { get; }
    }
}
