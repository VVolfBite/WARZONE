using Warzone.Combat;
using Warzone.Content;

namespace Warzone.Application.Services
{
    public sealed class BattleService
    {
        private readonly BattleSimulation _simulation;

        public BattleService(ContentCatalog contentCatalog)
        {
            _simulation = new BattleSimulation(contentCatalog);
        }

        public BattleState ActiveBattleState { get; private set; }

        public void Start(BattleState battleState)
        {
            ActiveBattleState = battleState;
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
