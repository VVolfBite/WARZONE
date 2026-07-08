using System;

namespace Warzone.Core.Math
{
    public struct Vec2 : IEquatable<Vec2>
    {
        private readonly float _x;
        private readonly float _y;

        public static readonly Vec2 Zero = new Vec2(0f, 0f);

        public Vec2(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public float X
        {
            get { return _x; }
        }

        public float Y
        {
            get { return _y; }
        }

        public float Magnitude
        {
            get { return (float)System.Math.Sqrt((_x * _x) + (_y * _y)); }
        }

        public float LengthSquared
        {
            get { return (_x * _x) + (_y * _y); }
        }

        public Vec2 NormalizeSafe()
        {
            float magnitude = Magnitude;
            if (magnitude <= 0.0001f)
            {
                return Zero;
            }

            return new Vec2(_x / magnitude, _y / magnitude);
        }

        public static Vec2 Add(Vec2 left, Vec2 right)
        {
            return new Vec2(left._x + right._x, left._y + right._y);
        }

        public static Vec2 Subtract(Vec2 left, Vec2 right)
        {
            return new Vec2(left._x - right._x, left._y - right._y);
        }

        public static Vec2 Multiply(Vec2 value, float scalar)
        {
            return new Vec2(value._x * scalar, value._y * scalar);
        }

        public static float Distance(Vec2 left, Vec2 right)
        {
            return Subtract(left, right).Magnitude;
        }

        public static float DistanceSquared(Vec2 left, Vec2 right)
        {
            return Subtract(left, right).LengthSquared;
        }

        public static float Dot(Vec2 left, Vec2 right)
        {
            return (left._x * right._x) + (left._y * right._y);
        }

        public static Vec2 Normalize(Vec2 value)
        {
            return value.NormalizeSafe();
        }

        public static Vec2 NormalizeSafe(Vec2 value)
        {
            return value.NormalizeSafe();
        }

        public static Vec2 MoveTowards(Vec2 current, Vec2 target, float maxDistanceDelta)
        {
            Vec2 delta = target - current;
            float distance = delta.Magnitude;
            if (distance <= maxDistanceDelta || distance <= 0.0001f)
            {
                return target;
            }

            return current + (delta.NormalizeSafe() * maxDistanceDelta);
        }

        public bool Equals(Vec2 other)
        {
            return System.Math.Abs(_x - other._x) <= 0.0001f &&
                   System.Math.Abs(_y - other._y) <= 0.0001f;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vec2))
            {
                return false;
            }

            return Equals((Vec2)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_x.GetHashCode() * 397) ^ _y.GetHashCode();
            }
        }

        public override string ToString()
        {
            return "(" + _x.ToString("F2") + ", " + _y.ToString("F2") + ")";
        }

        public static Vec2 operator +(Vec2 left, Vec2 right)
        {
            return Add(left, right);
        }

        public static Vec2 operator -(Vec2 left, Vec2 right)
        {
            return Subtract(left, right);
        }

        public static Vec2 operator *(Vec2 value, float scalar)
        {
            return Multiply(value, scalar);
        }

        public static Vec2 operator /(Vec2 value, float scalar)
        {
            return scalar == 0f ? Zero : new Vec2(value._x / scalar, value._y / scalar);
        }

        public static bool operator ==(Vec2 left, Vec2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vec2 left, Vec2 right)
        {
            return !left.Equals(right);
        }
    }
}
