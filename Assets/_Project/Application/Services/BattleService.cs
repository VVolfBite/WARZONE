using Warzone.Combat;
using Warzone.Content;

namespace Warzone.Application.Services
{
    public sealed class BattleService
    {
        private readonly BattleSimulation _simulation;
        private readonly BattleResultSystem _battleResultSystem;

        public BattleService(ContentCatalog contentCatalog)
        {
            _simulation = new BattleSimulation(contentCatalog);
            _battleResultSystem = new BattleResultSystem(contentCatalog);
        }

        public BattleState ActiveBattleState { get; private set; }

        public void Start(BattleState battleState)
        {
            ActiveBattleState = battleState;
            if (ActiveBattleState != null)
            {
                _battleResultSystem.UpdateMissionStatus(ActiveBattleState);
            }
        }

        public void Tick(float deltaTimeSeconds)
        {
            if (ActiveBattleState == null)
            {
                return;
            }

            _simulation.Tick(ActiveBattleState, deltaTimeSeconds);
        }

        public BattleSnapshot GetSnapshot()
        {
            if (ActiveBattleState == null)
            {
                return null;
            }

            return _simulation.LatestSnapshot ?? BattleSnapshotFactory.Create(ActiveBattleState);
        }

        public BattleMissionStatusSnapshot GetMissionStatus()
        {
            return ActiveBattleState != null ? ActiveBattleState.CurrentMissionStatus : null;
        }

        public BattleResult GetBattleResult()
        {
            return ActiveBattleState != null ? ActiveBattleState.CurrentBattleResult : null;
        }

        public bool IsBattleComplete()
        {
            return ActiveBattleState != null &&
                   ActiveBattleState.CurrentMissionStatus != null &&
                   ActiveBattleState.CurrentMissionStatus.IsBattleComplete;
        }

        public void Enqueue(BattleCommand command)
        {
            if (ActiveBattleState == null)
            {
                return;
            }

            _simulation.Enqueue(ActiveBattleState, command);
        }
    }
}
