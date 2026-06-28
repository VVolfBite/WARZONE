using System.Numerics;

namespace Warzone.BattleDomain
{
    public sealed class Command
    {
        public Command(
            CommandType commandType,
            int sourceSquadId,
            int? targetSquadId = null,
            Vector2? destination = null,
            bool queue = false)
        {
            CommandType = commandType;
            SourceSquadId = sourceSquadId;
            TargetSquadId = targetSquadId;
            Destination = destination;
            Queue = queue;
        }

        public CommandType CommandType { get; }
        public int SourceSquadId { get; }
        public int? TargetSquadId { get; }
        public Vector2? Destination { get; }
        public bool Queue { get; }
    }
}
