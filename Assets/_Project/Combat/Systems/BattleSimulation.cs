using Warzone.Content;

namespace Warzone.Combat
{
    public sealed class BattleSimulation
    {
        private readonly CommandSystem _commandSystem;
        private readonly SquadPlanningSystem _squadPlanningSystem;
        private readonly FormationSystem _formationSystem;
        private readonly MovementSystem _movementSystem;
        private readonly SearchSystem _searchSystem;
        private readonly ExtractionSystem _extractionSystem;
        private readonly EnemyAwarenessSystem _enemyAwarenessSystem;
        private readonly EnemyBehaviorSystem _enemyBehaviorSystem;
        private readonly PerceptionSystem _perceptionSystem;
        private readonly TargetSelectionSystem _targetSelectionSystem;
        private readonly FireSystem _fireSystem;
        private readonly EnemyFireSystem _enemyFireSystem;
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
            SearchSystem searchSystem = null,
            ExtractionSystem extractionSystem = null,
            EnemyAwarenessSystem enemyAwarenessSystem = null,
            EnemyBehaviorSystem enemyBehaviorSystem = null,
            PerceptionSystem perceptionSystem = null,
            TargetSelectionSystem targetSelectionSystem = null,
            FireSystem fireSystem = null,
            EnemyFireSystem enemyFireSystem = null,
            DamageSystem damageSystem = null,
            DeathCleanupSystem deathCleanupSystem = null)
        {
            _commandSystem = commandSystem ?? new CommandSystem();
            _squadPlanningSystem = squadPlanningSystem ?? new SquadPlanningSystem();
            _formationSystem = formationSystem ?? new FormationSystem();
            _movementSystem = movementSystem ?? new MovementSystem();
            _searchSystem = searchSystem ?? new SearchSystem();
            _extractionSystem = extractionSystem ?? new ExtractionSystem();
            _enemyAwarenessSystem = enemyAwarenessSystem ?? new EnemyAwarenessSystem();
            _enemyBehaviorSystem = enemyBehaviorSystem ?? new EnemyBehaviorSystem();
            _perceptionSystem = perceptionSystem ?? new PerceptionSystem();
            _targetSelectionSystem = targetSelectionSystem ?? new TargetSelectionSystem();
            _fireSystem = fireSystem ?? new FireSystem(contentCatalog);
            _enemyFireSystem = enemyFireSystem ?? new EnemyFireSystem(contentCatalog);
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
            _searchSystem.Execute(battleState, deltaTimeSeconds);
            _extractionSystem.Execute(battleState);
            _enemyAwarenessSystem.Execute(battleState);
            _enemyBehaviorSystem.Execute(battleState, deltaTimeSeconds);
            _perceptionSystem.Execute(battleState);
            _targetSelectionSystem.Execute(battleState);
            _fireSystem.Execute(battleState, deltaTimeSeconds);
            _enemyFireSystem.Execute(battleState, deltaTimeSeconds);
            _damageSystem.Execute(battleState);
            _deathCleanupSystem.Execute(battleState);
            battleState.AdvanceTime(deltaTimeSeconds);
            LatestSnapshot = BattleSnapshotFactory.Create(battleState);
            return commandResult;
        }
    }
}
