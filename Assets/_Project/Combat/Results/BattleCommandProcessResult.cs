namespace Warzone.Combat
{
    public sealed class BattleCommandProcessResult
    {
        public BattleCommandProcessResult(int acceptedCount, int rejectedCount)
        {
            AcceptedCount = acceptedCount;
            RejectedCount = rejectedCount;
        }

        public int AcceptedCount { get; private set; }
        public int RejectedCount { get; private set; }
    }
}

