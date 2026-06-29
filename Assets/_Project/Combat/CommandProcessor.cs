using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class CommandProcessor
    {
        public IReadOnlyList<DamageEvent> Execute(
            Command command,
            IReadOnlyList<BattleSquadState> squads,
            CombatResolver combatResolver)
        {
            BattleSquadState sourceSquad = FindSquad(command.SourceSquadId, squads);
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
                case CommandType.Patrol when command.Destination.HasValue && command.SecondaryDestination.HasValue:
                    ApplyOrQueueCommand(sourceSquad, command);
                    return new List<DamageEvent>();
                case CommandType.HoldPosition:
                    ApplyOrQueueCommand(sourceSquad, command);
                    return new List<DamageEvent>();
                case CommandType.Retreat when command.Destination.HasValue:
                    ApplyOrQueueCommand(sourceSquad, command);
                    return new List<DamageEvent>();
                case CommandType.UseAbility:
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
                return;
            }

            if (command.CommandType == CommandType.Patrol && command.Destination.HasValue)
            {
                sourceSquad.SetMoveDestination(command.Destination.Value);
                return;
            }

            if (command.CommandType == CommandType.HoldPosition)
            {
                sourceSquad.Stop();
                return;
            }

            if (command.CommandType == CommandType.Retreat && command.Destination.HasValue)
            {
                sourceSquad.SetMoveDestination(command.Destination.Value);
                return;
            }

            if (command.CommandType == CommandType.UseAbility)
            {
                sourceSquad.Stop();
            }
        }

        private static BattleSquadState FindSquad(int squadId, IReadOnlyList<BattleSquadState> squads)
        {
            for (int i = 0; i < squads.Count; i++)
            {
                if (squads[i].SquadId == squadId)
                {
                    return squads[i];
                }
            }

            return null;
        }
    }
}
