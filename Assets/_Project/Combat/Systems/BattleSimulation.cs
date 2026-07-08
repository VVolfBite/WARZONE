using Warzone.Content;

namespace Warzone.Combat
{
    public sealed class BattleSimulation
    {
        private readonly CommandSystem _commandSystem;
        private readonly SquadPlanningSystem _squadPlanningSystem;
        private readonly FormationSystem _formationSystem;
        private readonly MovementSystem _movementSystem;
        private readonly PerceptionSystem _perceptionSystem;
        private readonly TargetSelectionSystem _targetSelectionSystem;
        private readonly FireSystem _fireSystem;
        private readonly DamageSystem _damageSystem;
        private readonly DeathCleanupSystem _deathCleanupSystem;

        public BattleSimulation()
            : this(new ContentCatalog(null, null))
        {
        }

        public BattleSimulation(
            ContentCatalog contentCatalog,
            CommandSystem commandSystem = null,
            SquadPlanningSystem squadPlanningSystem = null,
            FormationSystem formationSystem = null,
            MovementSystem movementSystem = null,
            PerceptionSystem perceptionSystem = null,
            TargetSelectionSystem targetSelectionSystem = null,
            FireSystem fireSystem = null,
            DamageSystem damageSystem = null,
            DeathCleanupSystem deathCleanupSystem = null)
        {
            _commandSystem = commandSystem ?? new CommandSystem();
            _squadPlanningSystem = squadPlanningSystem ?? new SquadPlanningSystem();
            _formationSystem = formationSystem ?? new FormationSystem();
            _movementSystem = movementSystem ?? new MovementSystem();
            _perceptionSystem = perceptionSystem ?? new PerceptionSystem();
            _targetSelectionSystem = targetSelectionSystem ?? new TargetSelectionSystem();
            _fireSystem = fireSystem ?? new FireSystem(contentCatalog);
            _damageSystem = damageSystem ?? new DamageSystem();
            _deathCleanupSystem = deathCleanupSystem ?? new DeathCleanupSystem();
        }

        public BattleSnapshot LatestSnapshot { get; private set; }

        public void Enqueue(BattleState battleState, BattleCommand command)
        {
            _commandSystem.Enqueue(battleState, command);
        }

        public BattleCommandProcessResult Tick(BattleState battleState, float deltaTimeSeconds)
        {
            BattleCommandProcessResult commandResult = _commandSystem.Process(battleState);
            _squadPlanningSystem.Execute(battleState);
            _formationSystem.Execute(battleState);
            _movementSystem.Execute(battleState, deltaTimeSeconds);
            _perceptionSystem.Execute(battleState);
            _targetSelectionSystem.Execute(battleState);
            _fireSystem.Execute(battleState, deltaTimeSeconds);
            _damageSystem.Execute(battleState);
            _deathCleanupSystem.Execute(battleState);
            battleState.AdvanceTime(deltaTimeSeconds);
            LatestSnapshot = BattleSnapshotFactory.Create(battleState);
            return commandResult;
        }
    }
}
