namespace Warzone.Core.Math
{
    public struct Vec3
    {
        private readonly float _x;
        private readonly float _y;
        private readonly float _z;

        public static readonly Vec3 Zero = new Vec3(0f, 0f, 0f);

        public Vec3(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public float X
        {
            get { return _x; }
        }

        public float Y
        {
            get { return _y; }
        }

        public float Z
        {
            get { return _z; }
        }
    }
}
