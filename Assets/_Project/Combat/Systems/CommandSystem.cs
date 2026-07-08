namespace Warzone.Combat
{
    public sealed class CommandSystem
    {
        public void Enqueue(BattleState battleState, BattleCommand command)
        {
            if (battleState == null || command == null)
            {
                return;
            }

            battleState.CommandQueue.Enqueue(command);
        }

        public BattleCommandProcessResult Process(BattleState battleState)
        {
            if (battleState == null)
            {
                return new BattleCommandProcessResult(0, 0);
            }

            int acceptedCount = 0;
            int rejectedCount = 0;

            BattleCommand command;
            while (battleState.CommandQueue.TryDequeue(out command))
            {
                BattleSquadState squadState;
                if (!battleState.TryGetSquad(command.SquadId, out squadState))
                {
                    rejectedCount++;
                    battleState.AddEvent(new BattleEventRecord(BattleEventTypes.CommandRejected, command.SquadId, message: command.Name));
                    continue;
                }

                ApplyCommand(battleState, squadState, command);
                acceptedCount++;
                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.CommandAccepted, squadState.SquadId, message: command.Name));
            }

            return new BattleCommandProcessResult(acceptedCount, rejectedCount);
        }

        private static void ApplyCommand(BattleState battleState, BattleSquadState squadState, BattleCommand command)
        {
            squadState.SetCurrentOrder(command);

            MoveSquadCommand moveSquadCommand = command as MoveSquadCommand;
            if (moveSquadCommand != null)
            {
                squadState.SetDesiredPosition(moveSquadCommand.Destination);
                squadState.SetMoveDestination(moveSquadCommand.Destination);
                squadState.SetStance(SquadStance.Moving);
                return;
            }

            DefendAreaCommand defendAreaCommand = command as DefendAreaCommand;
            if (defendAreaCommand != null)
            {
                squadState.SetDesiredPosition(defendAreaCommand.AreaCenter);
                squadState.SetStance(SquadStance.Defending);
                squadState.Stop();
                squadState.SetCurrentOrder(command);
                return;
            }

            if (command is ClearSquadOrderCommand)
            {
                squadState.Stop();
                for (int i = 0; i < squadState.MemberIds.Count; i++)
                {
                    BattleMemberState memberState;
                    if (battleState.TryGetMember(squadState.MemberIds[i], out memberState))
                    {
                        memberState.ClearIntent();
                    }
                }
            }
        }
    }
}

