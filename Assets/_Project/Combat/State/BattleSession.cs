using System.Collections.Generic;
using System.Numerics;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleSession
    {
        // TODO(legacy): This prototype session still models combat around squad-level positions.
        // Keep it compiling for Sandbox validation, but do not extend it as the long-term combat model.
        private readonly List<BattleSquadState> _squads;
        private readonly CommandProcessor _commandProcessor;
        private readonly CombatResolver _combatResolver;
        private readonly AbilitySystem _abilitySystem;
        private readonly AiSystem _aiSystem = new AiSystem();
        private readonly StatusEffectSystem _statusEffectSystem = new StatusEffectSystem();
        private readonly TerrainMap _terrainMap;
        private readonly List<DamageEvent> _pendingDamageEvents = new List<DamageEvent>();
        private readonly Dictionary<string, StatusEffectDefinition> _statusEffectLibrary = new Dictionary<string, StatusEffectDefinition>
        {
            { "effect.support.heal", new StatusEffectDefinition("effect.support.heal", "Healing Aura", 2f, 0.5f, 0, 1, 1f, 1f) },
            { "effect.zombie.toxic", new StatusEffectDefinition("effect.zombie.toxic", "Toxic Cloud", 4f, 1f, 1, 0, 0.8f, 0.9f) },
            { "effect.suppression", new StatusEffectDefinition("effect.suppression", "Suppressed", 3f, 1f, 0, 0, 0.75f, 0.85f) },
            { "effect.warlord.rage", new StatusEffectDefinition("effect.warlord.rage", "Rage", 5f, 1f, 0, 0, 1.15f, 1.2f) }
        };
        private readonly int _seed;
        private float _elapsedTimeSeconds;
        private bool _resultFinalized;
        private int _pendingWaveCount;

        public BattleSession(
            IReadOnlyList<BattleSquadState> squads,
            CommandProcessor commandProcessor,
            CombatResolver combatResolver,
            int seed,
            TerrainMap terrainMap = null)
        {
            _squads = new List<BattleSquadState>(squads);
            _commandProcessor = commandProcessor;
            _combatResolver = combatResolver;
            _abilitySystem = new AbilitySystem(combatResolver.ContentCatalog);
            _seed = seed;
            _terrainMap = terrainMap;
            CurrentOutcome = MissionOutcome.InProgress;
            ApplyDefaultStatusEffects();
        }

        public IReadOnlyList<BattleSquadState> Squads
        {
            get { return _squads; }
        }

        public MissionOutcome CurrentOutcome { get; private set; }

        public bool HasFinished
        {
            get { return CurrentOutcome != MissionOutcome.InProgress; }
        }

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
            _aiSystem.Tick(this, deltaTimeSeconds);
            _statusEffectSystem.Tick(this, deltaTimeSeconds);
            UpdateAttackerTargets();
            UpdateMovement(deltaTimeSeconds);
            ResolveAttacks();
            ResolveOutcome();
        }

        public IReadOnlyList<DamageEvent> ExecuteCommand(Command command)
        {
            if (CurrentOutcome != MissionOutcome.InProgress)
            {
                return new List<DamageEvent>();
            }

            if (command == null)
            {
                return new List<DamageEvent>();
            }

            BattleSquadState sourceSquad = FindLivingSquadById(command.SourceSquadId);
            if (sourceSquad == null)
            {
                return new List<DamageEvent>();
            }

            IReadOnlyList<DamageEvent> events = _commandProcessor.Execute(command, _squads, _combatResolver);
            for (int i = 0; i < events.Count; i++)
            {
                _pendingDamageEvents.Add(events[i]);
            }

            return events;
        }

        public void AddSquads(IEnumerable<BattleSquadState> squads)
        {
            foreach (BattleSquadState squad in squads)
            {
                _squads.Add(squad);
            }
        }

        public BattleResult BuildResult()
        {
            List<UnitOutcome> unitOutcomes = new List<UnitOutcome>();
            List<BattleEntityId> casualties = new List<BattleEntityId>();
            int playerUnitsRemaining = 0;
            int enemyUnitsRemaining = 0;

            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                for (int j = 0; j < squad.Units.Count; j++)
                {
                    BattleUnitState unit = squad.Units[j];
                    bool survived = unit.IsAlive;
                    unitOutcomes.Add(new UnitOutcome(unit.EntityId, unit.DefinitionId, survived));

                    if (!survived)
                    {
                        casualties.Add(unit.EntityId);
                    }
                    else if (unit.FactionId == FactionId.Player)
                    {
                        playerUnitsRemaining++;
                    }
                    else if (unit.FactionId == FactionId.Enemy)
                    {
                        enemyUnitsRemaining++;
                    }
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
            if (_resultFinalized || CurrentOutcome == MissionOutcome.InProgress)
            {
                result = null;
                return false;
            }

            _resultFinalized = true;
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

        public UnitDefinition GetPrimaryDefinition(BattleSquadState squad)
        {
            return _combatResolver.GetPrimaryDefinition(squad);
        }

        public BattleSquadState FindSquadById(int squadId)
        {
            for (int i = 0; i < _squads.Count; i++)
            {
                if (_squads[i].SquadId == squadId)
                {
                    return _squads[i];
                }
            }

            return null;
        }

        public void ApplyStatusEffect(BattleUnitState unit, StatusEffectDefinition definition)
        {
            _statusEffectSystem.ApplyEffect(unit, definition);
        }

        public int GetMaxHealth(BattleUnitState unit)
        {
            if (unit == null)
            {
                return 0;
            }

            UnitDefinition definition;
            if (!_combatResolver.ContentCatalog.Units.TryGetValue(unit.DefinitionId, out definition))
            {
                return unit.CurrentHealth;
            }

            return definition.MaxHealth;
        }

        private void ApplyDefaultStatusEffects()
        {
            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                for (int j = 0; j < squad.Units.Count; j++)
                {
                    BattleUnitState unit = squad.Units[j];
                    UnitDefinition definition;
                    if (!_combatResolver.ContentCatalog.Units.TryGetValue(unit.DefinitionId, out definition))
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(definition.DefaultStatusEffectId))
                    {
                        continue;
                    }

                    StatusEffectDefinition effectDefinition;
                    if (_statusEffectLibrary.TryGetValue(definition.DefaultStatusEffectId, out effectDefinition))
                    {
                        _statusEffectSystem.ApplyEffect(unit, effectDefinition);
                    }
                }
            }
        }

        private void ProcessQueuedCommands()
        {
            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                if (squad.HasActiveCommand())
                {
                    continue;
                }

                Command queuedCommand;
                if (!squad.TryDequeueCommand(out queuedCommand))
                {
                    continue;
                }

                CommandProcessor.ApplyImmediate(squad, queuedCommand);
            }
        }

        private void UpdateAttackerTargets()
        {
            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                if (!squad.HasLivingUnits)
                {
                    continue;
                }

                FactionId defenderFaction = squad.FactionId == FactionId.Player ? FactionId.Enemy : FactionId.Player;
                BattleSquadState target = FindNearestLivingEnemy(squad, defenderFaction);
                if (target == null)
                {
                    continue;
                }

                float aggroRange = _combatResolver.GetAggroRange(squad);
                if (CombatResolver.GetDistance(squad, target) <= aggroRange)
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
                    nearestDistance = distance;
                    nearest = squad;
                }
            }

            return nearest;
        }

        private void UpdateMovement(float deltaTimeSeconds)
        {
            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                if (!squad.MoveDestination.HasValue)
                {
                    continue;
                }

                MoveTowardDestination(squad, squad.MoveDestination.Value, deltaTimeSeconds);
            }
        }

        private void ResolveAttacks()
        {
            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                if (!squad.AttackTargetSquadId.HasValue || squad.AttackCooldownRemaining > 0f)
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
                if (distance > _combatResolver.GetAttackRange(squad))
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
            Vector2 toDestination = destination - squad.Position;
            float distance = toDestination.Length();
            if (distance <= 0.05f)
            {
                squad.Stop();
                return;
            }

            Vector2 direction = Vector2.Normalize(toDestination);
            float moveDistance = _combatResolver.GetMoveSpeed(squad) * deltaTimeSeconds;
            if (moveDistance >= distance)
            {
                squad.UpdatePosition(destination);
                squad.Stop();
                return;
            }

            squad.UpdatePosition(squad.Position + (direction * moveDistance));
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

        private void ResolveOutcome()
        {
            CurrentOutcome = DetermineOutcome();
        }

        public bool TryGetStatusEffectDefinition(string effectId, out StatusEffectDefinition definition)
        {
            return _statusEffectLibrary.TryGetValue(effectId, out definition);
        }

        public void SyncStatusEffects()
        {
        }

        private MissionOutcome DetermineOutcome()
        {
            bool hasLivingPlayer = false;
            bool hasLivingEnemy = false;

            for (int i = 0; i < _squads.Count; i++)
            {
                BattleSquadState squad = _squads[i];
                if (!squad.HasLivingUnits)
                {
                    continue;
                }

                if (squad.FactionId == FactionId.Player)
                {
                    hasLivingPlayer = true;
                }
                else if (squad.FactionId == FactionId.Enemy)
                {
                    hasLivingEnemy = true;
                }
            }

            if (!hasLivingPlayer)
            {
                return MissionOutcome.Defeat;
            }

            if (hasLivingEnemy || _pendingWaveCount > 0)
            {
                return MissionOutcome.InProgress;
            }

            return MissionOutcome.Victory;
        }
    }
}

