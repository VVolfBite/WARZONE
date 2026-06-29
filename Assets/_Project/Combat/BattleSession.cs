using System.Collections.Generic;
using System.Numerics;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleSession
    {
        private readonly List<BattleSquadState> _squads;
        private readonly CommandProcessor _commandProcessor;
        private readonly CombatResolver _combatResolver;
        private readonly List<DamageEvent> _pendingDamageEvents = new List<DamageEvent>();
        private readonly int _seed;
        private float _elapsedTimeSeconds;
        private bool _resultFinalized;
        private int _pendingWaveCount;

        public BattleSession(
            IReadOnlyList<BattleSquadState> squads,
            CommandProcessor commandProcessor,
            CombatResolver combatResolver,
            int seed)
        {
            _squads = new List<BattleSquadState>(squads);
            _commandProcessor = commandProcessor;
            _combatResolver = combatResolver;
            _seed = seed;
        }

        public IReadOnlyList<BattleSquadState> Squads => _squads;
        public MissionOutcome CurrentOutcome { get; private set; } = MissionOutcome.InProgress;
        public bool HasFinished => CurrentOutcome != MissionOutcome.InProgress;

        public void ConfigurePendingWaveCount(int waveCount)
        {
            _pendingWaveCount = waveCount;
        }

        public void ConsumePendingWave()
        {
            if (_pendingWaveCount > 0)
            {
                _pendingWaveCount--;
            }
        }

        public void Tick(float deltaTimeSeconds)
        {
            if (CurrentOutcome != MissionOutcome.InProgress)
            {
                return;
            }

            _elapsedTimeSeconds += deltaTimeSeconds;
            for (int i = 0; i < _squads.Count; i++)
            {
                _squads[i].TickCooldown(deltaTimeSeconds);
            }

            ProcessQueuedCommands();
            UpdateAttackerTargets();
            UpdateMovement(deltaTimeSeconds);
            ResolveUnitSeparation();
            ResolveAttacks();
            CurrentOutcome = DetermineOutcome();
        }

        public IReadOnlyList<DamageEvent> ExecuteCommand(Command command)
        {
            if (CurrentOutcome != MissionOutcome.InProgress)
            {
                return new List<DamageEvent>();
            }

            IReadOnlyList<DamageEvent> events = _commandProcessor.Execute(command, _squads, _combatResolver);
            CurrentOutcome = DetermineOutcome();
            return events;
        }

        public void AddSquads(IEnumerable<BattleSquadState> squads)
        {
            foreach (BattleSquadState squad in squads)
            {
                _squads.Add(squad);
            }

            CurrentOutcome = DetermineOutcome();
        }

        public BattleResult BuildResult()
        {
            _resultFinalized = true;
            List<BattleEntityId> casualties = new List<BattleEntityId>();
            List<UnitOutcome> unitOutcomes = new List<UnitOutcome>();
            int playerUnitsRemaining = 0;
            int enemyUnitsRemaining = 0;

            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                for (int j = 0; j < squad.Units.Count; j++)
                {
                    BattleUnitState unit = squad.Units[j];
                    bool survived = unit.IsAlive;
                    unitOutcomes.Add(new UnitOutcome(unit.EntityId, survived));
                    if (!survived)
                    {
                        casualties.Add(unit.EntityId);
                    }
                }

                if (squad.FactionId == FactionId.Player)
                {
                    playerUnitsRemaining += squad.LivingUnitCount;
                }
                else if (squad.FactionId == FactionId.Enemy)
                {
                    enemyUnitsRemaining += squad.LivingUnitCount;
                }
            }

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

        private void UpdateAttackerTargets()
        {
            AcquireTargetsForFaction(FactionId.Player, FactionId.Enemy, true, true);
            AcquireTargetsForFaction(FactionId.Enemy, FactionId.Player, false, true);
        }

        private void AcquireTargetsForFaction(FactionId attackerFaction, FactionId defenderFaction, bool autoAcquireForIdleSquadsOnly, bool requireAggroRange)
        {
            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                if (squad.FactionId != attackerFaction || !squad.HasLivingUnits)
                {
                    continue;
                }

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

            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
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
            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                if (!squad.HasLivingUnits)
                {
                    continue;
                }

                if (squad.MoveDestination.HasValue)
                {
                    MoveTowardDestination(squad, squad.MoveDestination.Value, deltaTimeSeconds);
                    continue;
                }

                if (!squad.AttackTargetSquadId.HasValue)
                {
                    continue;
                }

                BattleSquadState target = FindLivingSquadById(squad.AttackTargetSquadId.Value);
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
            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                if (!squad.HasLivingUnits || !squad.AttackTargetSquadId.HasValue)
                {
                    continue;
                }

                BattleSquadState target = FindLivingSquadById(squad.AttackTargetSquadId.Value);
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
                for (int j = 0; j < damageEvents.Count; j++)
                {
                    _pendingDamageEvents.Add(damageEvents[j]);
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
            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                if (!squad.HasLivingUnits || squad.HasActiveCommand())
                {
                    continue;
                }

                if (squad.TryDequeueCommand(out Command nextCommand))
                {
                    CommandProcessor.ApplyImmediate(squad, nextCommand);
                }
            }
        }

        private void ResolveUnitSeparation()
        {
            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squadA = _squads[i];
                if (!squadA.HasLivingUnits)
                {
                    continue;
                }

                for (int j = i + 1; j < _squads.Count; j++)
                {
                    BattleSquadState squadB = _squads[j];
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

        private BattleSquadState FindLivingSquadById(int squadId)
        {
            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                if (squad.SquadId == squadId && squad.HasLivingUnits)
                {
                    return squad;
                }
            }

            return null;
        }

        private MissionOutcome DetermineOutcome()
        {
            bool playerAlive = false;
            bool enemyAlive = false;

            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                if (!squad.HasLivingUnits)
                {
                    continue;
                }

                if (squad.FactionId == FactionId.Player)
                {
                    playerAlive = true;
                }
                else if (squad.FactionId == FactionId.Enemy)
                {
                    enemyAlive = true;
                }
            }

            if (!playerAlive)
            {
                return MissionOutcome.Defeat;
            }

            if (enemyAlive || _pendingWaveCount > 0)
            {
                return MissionOutcome.InProgress;
            }

            return MissionOutcome.Victory;
        }
    }
}
