using Warzone.Combat;
using Warzone.Meta;

namespace Warzone.Application
{
    public sealed class MissionFlow : IMissionFlow
    {
        private readonly GameFlow _gameFlow;
        private readonly IBattleRuntimeHost _battleRuntimeHost;
        private readonly ProgressionService _progressionService;

        public MissionFlow(
            GameFlow gameFlow,
            IBattleRuntimeHost battleRuntimeHost,
            ProgressionService progressionService)
        {
            _gameFlow = gameFlow;
            _battleRuntimeHost = battleRuntimeHost;
            _progressionService = progressionService;
        }

        public void StartMission(MissionStartRequest request)
        {
            _gameFlow.EnterMission();
            _battleRuntimeHost.StartBattle(request);
        }

        public MetaSettlementResult CompleteMission(BattleResult battleResult)
        {
            _battleRuntimeHost.FinishBattle(battleResult);
            _gameFlow.ExitMission();
            return _progressionService.ApplyBattleResult(battleResult);
        }
    }
}
