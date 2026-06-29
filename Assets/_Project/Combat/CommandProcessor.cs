using System.Collections.Generic;
using System.Linq;

namespace Warzone.Combat
{
    public sealed class CommandProcessor
    {
        public IReadOnlyList<DamageEvent> Execute(
            Command command,
            IReadOnlyList<BattleSquadState> squads,
            CombatResolver combatResolver)
        {
            BattleSquadState sourceSquad = squads.FirstOrDefault(squad => squad.SquadId == command.SourceSquadId);
            if (sourceSquad == null)
            {
                return new List<DamageEvent>();
            }

            switch (command.CommandType)
            {
                case CommandType.Move when command.Destination.HasValue:
                    ApplyOrQueueCommand(sourceSquad, command);
                    return new List<DamageEvent>();
                case CommandType.Attack when command.TargetSquadId.HasValue:
                    ApplyOrQueueCommand(sourceSquad, command);
                    return new List<DamageEvent>();
                case CommandType.Stop:
                    sourceSquad.ClearQueuedCommands();
                    sourceSquad.Stop();
                    return new List<DamageEvent>();
                default:
                    return new List<DamageEvent>();
            }
        }

        private static void ApplyOrQueueCommand(BattleSquadState sourceSquad, Command command)
        {
            if (command.Queue && sourceSquad.HasActiveCommand())
            {
                sourceSquad.EnqueueCommand(command);
                return;
            }

            if (!command.Queue)
            {
                sourceSquad.ClearQueuedCommands();
            }

            ApplyImmediate(sourceSquad, command);
        }

        public static void ApplyImmediate(BattleSquadState sourceSquad, Command command)
        {
            if (command.CommandType == CommandType.Move && command.Destination.HasValue)
            {
                sourceSquad.SetMoveDestination(command.Destination.Value);
                return;
            }

            if (command.CommandType == CommandType.Attack && command.TargetSquadId.HasValue)
            {
                sourceSquad.SetAttackTarget(command.TargetSquadId.Value);
            }
        }
    }
}
