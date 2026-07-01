using System.Numerics;

namespace Warzone.Combat
{
    public sealed class Command
    {
        public Command(
            CommandType commandType,
            int sourceSquadId,
            int? targetSquadId = null,
            Vector2? destination = null,
            Vector2? secondaryDestination = null,
            string abilityId = null,
            bool queue = false)
        {
            CommandType = commandType;
            SourceSquadId = sourceSquadId;
            TargetSquadId = targetSquadId;
            Destination = destination;
            SecondaryDestination = secondaryDestination;
            AbilityId = abilityId;
            Queue = queue;
        }

        public CommandType CommandType { get; }
        public int SourceSquadId { get; }
        public int? TargetSquadId { get; }
        public Vector2? Destination { get; }
        public Vector2? SecondaryDestination { get; }
        public string AbilityId { get; }
        public bool Queue { get; }
    }
}
