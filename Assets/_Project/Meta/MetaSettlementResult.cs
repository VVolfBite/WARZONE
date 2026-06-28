namespace Warzone.Meta
{
    public sealed class MetaSettlementResult
    {
        public MetaSettlementResult(bool missionCompleted, int unitsLost)
        {
            MissionCompleted = missionCompleted;
            UnitsLost = unitsLost;
        }

        public bool MissionCompleted { get; }
        public int UnitsLost { get; }
    }
}
