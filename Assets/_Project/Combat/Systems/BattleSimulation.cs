namespace Warzone.Combat
{
    public sealed class BattleSimulation
    {
        private readonly CommandSystem _commandSystem;
        private readonly SquadPlanningSystem _squadPlanningSystem;
        private readonly FormationSystem _formationSystem;
        private readonly MovementSystem _movementSystem;

        public BattleSimulation(
            CommandSystem commandSystem = null,
            SquadPlanningSystem squadPlanningSystem = null,
            FormationSystem formationSystem = null,
            MovementSystem movementSystem = null)
        {
            _commandSystem = commandSystem ?? new CommandSystem();
            _squadPlanningSystem = squadPlanningSystem ?? new SquadPlanningSystem();
            _formationSystem = formationSystem ?? new FormationSystem();
            _movementSystem = movementSystem ?? new MovementSystem();
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
            battleState.AdvanceTime(deltaTimeSeconds);
            LatestSnapshot = BattleSnapshotFactory.Create(battleState);
            return commandResult;
        }
    }
}
