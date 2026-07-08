using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class DefendAreaCommand : BattleCommand
    {
        public DefendAreaCommand(int squadId, Vec2 areaCenter, float radius)
            : base(squadId)
        {
            AreaCenter = areaCenter;
            Radius = radius;
        }

        public override string Name
        {
            get { return "DefendArea"; }
        }

        public Vec2 AreaCenter { get; private set; }
        public float Radius { get; private set; }
    }
}

