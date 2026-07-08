using System;

namespace Warzone.Combat
{
    public struct BattleEntityId : IEquatable<BattleEntityId>
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

        public bool Equals(BattleEntityId other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BattleEntityId))
            {
                return false;
            }

            return Equals((BattleEntityId)obj);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static bool operator ==(BattleEntityId left, BattleEntityId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BattleEntityId left, BattleEntityId right)
        {
            return !left.Equals(right);
        }
    }
}
