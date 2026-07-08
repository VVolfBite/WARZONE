namespace Warzone.Combat
{
    public sealed class BattleEventRecord
    {
        public BattleEventRecord(string eventType, int? squadId = null, BattleEntityId? memberId = null, string message = null)
        {
            EventType = eventType;
            SquadId = squadId;
            MemberId = memberId;
            Message = message;
        }

        public string EventType { get; private set; }
        public int? SquadId { get; private set; }
        public BattleEntityId? MemberId { get; private set; }
        public string Message { get; private set; }
    }
}
