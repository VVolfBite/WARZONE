using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Warzone.Content.Definitions;

namespace Warzone.BattleDomain
{
    public sealed class BattleSession
    {
        private readonly CommandProcessor _commandProcessor;
        private readonly CombatResolver _combatResolver;
        private readonly MissionRuntime _missionRuntime;
        private readonly int _seed;
        private float _elapsedTimeSeconds;
        private bool _resultFinalized;
        private readonly List<DamageEvent> _pendingDamageEvents = new List<DamageEvent>();

        public BattleSession(
            IReadOnlyList<BattleSquadState> squads,
            CommandProcessor commandProcessor,
            CombatResolver combatResolver,
            MissionRuntime missionRuntime,
            int seed)
        {
            Squads = squads;
            _commandProcessor = commandProcessor;
            _combatResolver = combatResolver;
            _missionRuntime = missionRuntime;
            _seed = seed;
        }

        public IReadOnlyList<BattleSquadState> Squads { get; }
        public MissionOutcome CurrentOutcome { get; private set; } = MissionOutcome.InProgress;
        public bool HasFinished => CurrentOutcome != MissionOutcome.InProgress;

        public void Tick(float deltaTimeSeconds)
        {
            if (CurrentOutcome != MissionOutcome.InProgress)
            {
                return;
            }

            _elapsedTimeSeconds += deltaTimeSeconds;
            foreach (BattleSquadState squad in Squads)
            {
                squad.TickCooldown(deltaTimeSeconds);
            }

            AcquireTargetsForFaction(FactionId.Player, FactionId.Enemy, autoAcquireForIdleSquadsOnly: true, requireAggroRange: true);
            AcquireTargetsForFaction(FactionId.Enemy, FactionId.Player, autoAcquireForIdleSquadsOnly: false, requireAggroRange: true);
            UpdateMovement(deltaTimeSeconds);
            ResolveUnitSeparation();
            ResolveAttacks();
            ProcessQueuedCommands();
            CurrentOutcome = _missionRuntime.Evaluate(Squads);
        }

        public IReadOnlyList<DamageEvent> ExecuteCommand(Command command)
        {
            if (CurrentOutcome != MissionOutcome.InProgress)
            {
                return new List<DamageEvent>();
            }

            IReadOnlyList<DamageEvent> events = _commandProcessor.Execute(command, Squads, _combatResolver);
            CurrentOutcome = _missionRuntime.Evaluate(Squads);
            return events;
        }

        public BattleResult BuildResult()
        {
            _resultFinalized = true;
            List<BattleEntityId> casualties = new List<BattleEntityId>();
            List<UnitOutcome> unitOutcomes = new List<UnitOutcome>();

            foreach (BattleUnitState unit in Squads.SelectMany(squad => squad.Units))
            {
                bool survived = unit.IsAlive;
                unitOutcomes.Add(new UnitOutcome(unit.EntityId, survived));
                if (!survived)
                {
                    casualties.Add(unit.EntityId);
                }
            }

            int playerUnitsRemaining = Squads
                .Where(squad => squad.FactionId == FactionId.Player)
                .Sum(squad => squad.LivingUnitCount);
            int enemyUnitsRemaining = Squads
                .Where(squad => squad.FactionId == FactionId.Enemy)
                .Sum(squad => squad.LivingUnitCount);

            return new BattleResult(
                CurrentOutcome,
                unitOutcomes,
                casualties,
                new BattleStatistics(playerUnitsRemaining, enemyUnitsRemaining),
                _elapsedTimeSeconds,
                _seed);
        }

        public bool TryBuildResultOnce(out BattleResult result)
        {
            if (!HasFinished || _resultFinalized)
            {
                result = null;
                return false;
            }

            result = BuildResult();
            return true;
        }

        public IReadOnlyList<DamageEvent> ConsumeDamageEvents()
        {
            if (_pendingDamageEvents.Count == 0)
            {
                return new List<DamageEvent>();
            }

            List<DamageEvent> events = new List<DamageEvent>(_pendingDamageEvents);
            _pendingDamageEvents.Clear();
            return events;
        }

        private void AcquireTargetsForFaction(FactionId attackerFaction, FactionId defenderFaction, bool autoAcquireForIdleSquadsOnly, bool requireAggroRange)
        {
            foreach (BattleSquadState squad in Squads.Where(squad => squad.FactionId == attackerFaction && squad.HasLivingUnits))
            {
                if (autoAcquireForIdleSquadsOnly && squad.CommandState != SquadCommandState.Idle)
                {
                    continue;
                }

                if (!autoAcquireForIdleSquadsOnly && squad.MoveDestination.HasValue)
                {
                    continue;
                }

                BattleSquadState target = FindNearestLivingEnemy(squad, defenderFaction);
                if (target == null)
                {
                    if (!squad.MoveDestination.HasValue)
                    {
                        squad.Stop();
                    }
                    continue;
                }

                if (requireAggroRange)
                {
                    float aggroRange = _combatResolver.GetAggroRange(squad);
                    if (CombatResolver.GetDistance(squad, target) > aggroRange)
                    {
                        if (!autoAcquireForIdleSquadsOnly)
                        {
                            squad.Stop();
                        }
                        continue;
                    }
                }

                if (!squad.AttackTargetSquadId.HasValue || squad.AttackTargetSquadId.Value != target.SquadId)
                {
                    squad.SetAttackTarget(target.SquadId);
                }
            }
        }

        private BattleSquadState FindNearestLivingEnemy(BattleSquadState sourceSquad, FactionId defenderFaction)
        {
            BattleSquadState nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (BattleSquadState squad in Squads)
            {
                if (squad.FactionId != defenderFaction || !squad.HasLivingUnits)
                {
                    continue;
                }

                float distance = CombatResolver.GetDistance(sourceSquad, squad);
                if (distance < nearestDistance)
                {
                    nearest = squad;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }

        private void UpdateMovement(float deltaTimeSeconds)
        {
            foreach (BattleSquadState squad in Squads.Where(squad => squad.HasLivingUnits))
            {
                if (squad.MoveDestination.HasValue)
                {
                    MoveTowardDestination(squad, squad.MoveDestination.Value, deltaTimeSeconds);
                    continue;
                }

                if (!squad.AttackTargetSquadId.HasValue)
                {
                    continue;
                }

                BattleSquadState target = Squads.FirstOrDefault(other => other.SquadId == squad.AttackTargetSquadId.Value && other.HasLivingUnits);
                if (target == null)
                {
                    squad.Stop();
                    continue;
                }

                float distance = CombatResolver.GetDistance(squad, target);
                float range = _combatResolver.GetAttackRange(squad);
                if (distance > range)
                {
                    MoveTowardDestination(squad, target.Position, deltaTimeSeconds);
                }
            }
        }

        private void ResolveAttacks()
        {
            foreach (BattleSquadState squad in Squads.Where(squad => squad.HasLivingUnits && squad.AttackTargetSquadId.HasValue))
            {
                BattleSquadState target = Squads.FirstOrDefault(other => other.SquadId == squad.AttackTargetSquadId.Value && other.HasLivingUnits);
                if (target == null)
                {
                    squad.Stop();
                    continue;
                }

                float distance = CombatResolver.GetDistance(squad, target);
                float range = _combatResolver.GetAttackRange(squad);
                if (distance > range || squad.AttackCooldownRemaining > 0f)
                {
                    continue;
                }

                IReadOnlyList<DamageEvent> damageEvents = _combatResolver.ResolveAttack(squad, target);
                for (int i = 0; i < damageEvents.Count; i++)
                {
                    _pendingDamageEvents.Add(damageEvents[i]);
                }
                squad.ResetAttackCooldown(_combatResolver.GetAttackInterval(squad));
            }
        }

        private void MoveTowardDestination(BattleSquadState squad, Vector2 destination, float deltaTimeSeconds)
        {
            Vector2 offset = destination - squad.Position;
            if (offset.LengthSquared() < 0.0001f)
            {
                squad.UpdatePosition(destination);
                if (squad.MoveDestination.HasValue)
                {
                    squad.Stop();
                }
                return;
            }

            float moveSpeed = _combatResolver.GetMoveSpeed(squad);
            Vector2 direction = Vector2.Normalize(offset);
            float step = moveSpeed * deltaTimeSeconds;
            if (step >= offset.Length())
            {
                squad.UpdatePosition(destination);
                if (squad.MoveDestination.HasValue)
                {
                    squad.Stop();
                }
                return;
            }

            squad.UpdatePosition(squad.Position + (direction * step));
        }

        private void ProcessQueuedCommands()
        {
            foreach (BattleSquadState squad in Squads.Where(squad => squad.HasLivingUnits))
            {
                if (squad.HasActiveCommand())
                {
                    continue;
                }

                Command nextCommand;
                if (squad.TryDequeueCommand(out nextCommand))
                {
                    CommandProcessor.ApplyImmediate(squad, nextCommand);
                }
            }
        }

        private void ResolveUnitSeparation()
        {
            for (int i = 0; i < Squads.Count; i++)
            {
                BattleSquadState squadA = Squads[i];
                if (!squadA.HasLivingUnits)
                {
                    continue;
                }

                for (int j = i + 1; j < Squads.Count; j++)
                {
                    BattleSquadState squadB = Squads[j];
                    if (!squadB.HasLivingUnits)
                    {
                        continue;
                    }

                    float minDistance = _combatResolver.GetCollisionRadius(squadA) + _combatResolver.GetCollisionRadius(squadB);
                    Vector2 offset = squadB.Position - squadA.Position;
                    float distance = offset.Length();
                    if (distance >= minDistance || distance <= 0.0001f)
                    {
                        continue;
                    }

                    Vector2 direction = Vector2.Normalize(offset);
                    float penetration = minDistance - distance;
                    Vector2 correction = direction * (penetration * 0.5f);
                    squadA.UpdatePosition(squadA.Position - correction);
                    squadB.UpdatePosition(squadB.Position + correction);
                }
            }
        }
    }
}
