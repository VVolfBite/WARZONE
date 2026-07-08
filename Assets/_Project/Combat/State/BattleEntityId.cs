namespace Warzone.Combat
{
    public struct BattleEntityId
    {
        private readonly int _value;

        public BattleEntityId(int value)
        {
            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }
    }
}



