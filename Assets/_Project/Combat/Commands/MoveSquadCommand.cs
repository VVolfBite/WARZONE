using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class MoveSquadCommand : BattleCommand
    {
        public MoveSquadCommand(int squadId, Vec2 destination)
            : base(squadId)
        {
            Destination = destination;
        }

        public override string Name
        {
            get { return "MoveSquad"; }
        }

        public Vec2 Destination { get; private set; }
    }
}

