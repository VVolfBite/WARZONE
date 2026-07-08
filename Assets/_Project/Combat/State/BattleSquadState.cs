using System.Collections.Generic;
using System.Numerics;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleSquadState
    {
        private readonly Queue<Command> _commandQueue = new Queue<Command>();
        private readonly List<BattleEntityId> _memberIds = new List<BattleEntityId>();
        private float _abilityCooldownRemaining;

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
            DesiredPosition = position;
            RallyPosition = position;
            Stance = SquadStance.Default;
            FormationSpacing = 1.5f;
            SyncLegacyMemberIds(units);
        }

        public BattleSquadState(
            int squadId,
            FactionId factionId,
            Vector2 position,
            IReadOnlyList<BattleEntityId> memberIds,
            float formationSpacing = 1.5f)
        {
            SquadId = squadId;
            FactionId = factionId;
            Position = position;
            Units = new List<BattleUnitState>();
            DesiredPosition = position;
            RallyPosition = position;
            Stance = SquadStance.Default;
            FormationSpacing = formationSpacing;
            SyncMemberIds(memberIds);
        }

        public int SquadId { get; private set; }
        public FactionId FactionId { get; private set; }
        public Vector2 Position { get; private set; }
        public IReadOnlyList<BattleUnitState> Units { get; private set; }
        public IReadOnlyList<BattleEntityId> MemberIds
        {
            get { return _memberIds; }
        }

        public Vector2? MoveDestination { get; private set; }
        public int? AttackTargetSquadId { get; private set; }
        public float AttackCooldownRemaining { get; private set; }
        public BattleCommand CurrentOrder { get; private set; }
        public SquadStance Stance { get; private set; }
        public float FormationSpacing { get; private set; }
        public Vector2 DesiredPosition { get; private set; }
        public Vector2 RallyPosition { get; private set; }

        public float AbilityCooldownRemaining
        {
            get { return _abilityCooldownRemaining; }
        }

        public SquadCommandState CommandState { get; private set; }

        public int QueuedCommandCount
        {
            get { return _commandQueue.Count; }
        }

        public bool HasLivingUnits
        {
            get
            {
                for (int i = 0; i < Units.Count; i++)
                {
                    if (Units[i].IsAlive)
                    {
                        return true;
                    }
                }

                return _memberIds.Count > 0;
            }
        }

        public int LivingUnitCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < Units.Count; i++)
                {
                    if (Units[i].IsAlive)
                    {
                        count++;
                    }
                }

                return count > 0 ? count : _memberIds.Count;
            }
        }

        public void SetMoveDestination(Vector2 destination)
        {
            MoveDestination = destination;
            AttackTargetSquadId = null;
            CommandState = SquadCommandState.Moving;
            DesiredPosition = destination;
            RallyPosition = destination;
            Stance = SquadStance.Moving;
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
            CurrentOrder = null;
            Stance = SquadStance.Default;
        }

        public void TickCooldown(float deltaTimeSeconds)
        {
            if (AttackCooldownRemaining > 0f)
            {
                AttackCooldownRemaining -= deltaTimeSeconds;
                if (AttackCooldownRemaining < 0f)
                {
                    AttackCooldownRemaining = 0f;
                }
            }

            if (_abilityCooldownRemaining > 0f)
            {
                _abilityCooldownRemaining -= deltaTimeSeconds;
                if (_abilityCooldownRemaining < 0f)
                {
                    _abilityCooldownRemaining = 0f;
                }
            }
        }

        public void ResetAttackCooldown(float cooldownSeconds)
        {
            AttackCooldownRemaining = cooldownSeconds;
        }

        public void ResetAbilityCooldown(float cooldownSeconds)
        {
            _abilityCooldownRemaining = cooldownSeconds;
        }

        public void UpdatePosition(Vector2 position)
        {
            Position = position;
        }

        public void SetCurrentOrder(BattleCommand order)
        {
            CurrentOrder = order;
        }

        public void SetDesiredPosition(Vector2 desiredPosition)
        {
            DesiredPosition = desiredPosition;
            RallyPosition = desiredPosition;
        }

        public void SetStance(SquadStance stance)
        {
            Stance = stance;
        }

        public void SetFormationSpacing(float formationSpacing)
        {
            FormationSpacing = formationSpacing > 0f ? formationSpacing : 1.5f;
        }

        public void SyncMemberIds(IReadOnlyList<BattleEntityId> memberIds)
        {
            _memberIds.Clear();
            if (memberIds == null)
            {
                return;
            }

            for (int i = 0; i < memberIds.Count; i++)
            {
                _memberIds.Add(memberIds[i]);
            }
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

        private void SyncLegacyMemberIds(IReadOnlyList<BattleUnitState> units)
        {
            _memberIds.Clear();
            if (units == null)
            {
                return;
            }

            for (int i = 0; i < units.Count; i++)
            {
                _memberIds.Add(units[i].EntityId);
            }
        }
    }
}
