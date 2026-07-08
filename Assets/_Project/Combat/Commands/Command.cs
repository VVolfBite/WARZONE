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

        public CommandType CommandType { get; private set; }
        public int SourceSquadId { get; private set; }
        public int? TargetSquadId { get; private set; }
        public Vector2? Destination { get; private set; }
        public Vector2? SecondaryDestination { get; private set; }
        public string AbilityId { get; private set; }
        public bool Queue { get; private set; }
    }
}



