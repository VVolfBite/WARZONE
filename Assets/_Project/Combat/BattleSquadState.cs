using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleSquadState
    {
        private readonly Queue<Command> _commandQueue = new Queue<Command>();

        public BattleSquadState(
            int squadId,
            FactionId factionId,
            Vector2 position,
            IReadOnlyList<BattleUnitState> units)
        {
            SquadId = squadId;
            FactionId = factionId;
            Position = position;
            Units = units;
        }

        public int SquadId { get; }
        public FactionId FactionId { get; }
        public Vector2 Position { get; private set; }
        public IReadOnlyList<BattleUnitState> Units { get; }
        public Vector2? MoveDestination { get; private set; }
        public int? AttackTargetSquadId { get; private set; }
        public float AttackCooldownRemaining { get; private set; }
        public SquadCommandState CommandState { get; private set; }
        public int QueuedCommandCount => _commandQueue.Count;

        public bool HasLivingUnits => Units.Any(unit => unit.IsAlive);
        public int LivingUnitCount => Units.Count(unit => unit.IsAlive);

        public void SetMoveDestination(Vector2 destination)
        {
            MoveDestination = destination;
            AttackTargetSquadId = null;
            CommandState = SquadCommandState.Moving;
        }

        public void SetAttackTarget(int targetSquadId)
        {
            AttackTargetSquadId = targetSquadId;
            MoveDestination = null;
            CommandState = SquadCommandState.Attacking;
        }

        public void Stop()
        {
            MoveDestination = null;
            AttackTargetSquadId = null;
            CommandState = SquadCommandState.Idle;
        }

        public void TickCooldown(float deltaTimeSeconds)
        {
            if (AttackCooldownRemaining <= 0f)
            {
                return;
            }

            AttackCooldownRemaining -= deltaTimeSeconds;
            if (AttackCooldownRemaining < 0f)
            {
                AttackCooldownRemaining = 0f;
            }
        }

        public void ResetAttackCooldown(float cooldownSeconds)
        {
            AttackCooldownRemaining = cooldownSeconds;
        }

        public void UpdatePosition(Vector2 position)
        {
            Position = position;
        }

        public void ClearQueuedCommands()
        {
            _commandQueue.Clear();
        }

        public void EnqueueCommand(Command command)
        {
            _commandQueue.Enqueue(command);
        }

        public bool TryDequeueCommand(out Command command)
        {
            if (_commandQueue.Count > 0)
            {
                command = _commandQueue.Dequeue();
                return true;
            }

            command = null;
            return false;
        }

        public bool HasActiveCommand()
        {
            return MoveDestination.HasValue || AttackTargetSquadId.HasValue;
        }
    }
}
