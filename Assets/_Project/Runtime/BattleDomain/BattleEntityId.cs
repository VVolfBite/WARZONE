namespace Warzone.BattleDomain
{
    public struct BattleEntityId
    {
        public BattleEntityId(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
