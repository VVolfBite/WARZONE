namespace Warzone.Combat
{
    public sealed class BattleEventRecord
    {
        public BattleEventRecord(
            string eventType,
            int? squadId = null,
            BattleEntityId? memberId = null,
            string message = null,
            BattleEntityId? targetId = null,
            int amount = 0)
        {
            EventType = eventType;
            SquadId = squadId;
            MemberId = memberId;
            Message = message;
            TargetId = targetId;
            Amount = amount;
        }

        public string EventType { get; private set; }
        public int? SquadId { get; private set; }
        public BattleEntityId? MemberId { get; private set; }
        public string Message { get; private set; }
        public BattleEntityId? TargetId { get; private set; }
        public int Amount { get; private set; }
    }
}

