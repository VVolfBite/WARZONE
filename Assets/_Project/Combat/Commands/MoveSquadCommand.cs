using System.Numerics;

namespace Warzone.Combat
{
    public sealed class MoveSquadCommand : BattleCommand
    {
        public MoveSquadCommand(int squadId, Vector2 destination)
            : base(squadId)
        {
            Destination = destination;
        }

        public override string Name
        {
            get { return "MoveSquad"; }
        }

        public Vector2 Destination { get; private set; }
    }
}
