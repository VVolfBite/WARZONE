using UnityEngine;
using Warzone.Application;
using Warzone.Combat;
using Warzone.Runtime.UI;
using Warzone.Campaign;
using Warzone.Runtime.Persistence;

namespace Warzone.Sandbox.Presentation
{
    public sealed class SandboxMissionCompletionPresenter : MonoBehaviour
    {
        private readonly DebriefScreenController _controller = new DebriefScreenController();
        private readonly ISaveService _saveService = new JsonSaveService();
        private BattleRuntimeHost _battleRuntimeHost;
        private MissionFlow _missionFlow;
        private IDebriefScreen _debriefScreen;

        public void Configure(BattleRuntimeHost battleRuntimeHost, MissionFlow missionFlow, IDebriefScreen debriefScreen)
        {
            _battleRuntimeHost = battleRuntimeHost;
            _missionFlow = missionFlow;
            _debriefScreen = debriefScreen;
            _battleRuntimeHost.BattleFinished += HandleBattleFinished;
        }

        private void OnDestroy()
        {
            if (_battleRuntimeHost != null)
            {
                _battleRuntimeHost.BattleFinished -= HandleBattleFinished;
            }
        }

        private void HandleBattleFinished(BattleResult battleResult)
        {
            MetaSettlementResult settlement = _missionFlow.CompleteMission(battleResult);
            DebriefViewModel viewModel = _controller.BuildViewModel(
                settlement.MissionCompleted,
                settlement.UnitsLost,
                settlement.UnitsKept,
                battleResult.ElapsedTimeSeconds);
            _saveService.SaveBestScore("mission.sandbox", battleResult.BattleStatistics);

            _debriefScreen.Show(viewModel);
            Debug.Log("Battle finished. Victory=" + viewModel.IsVictory + ", UnitsLost=" + viewModel.UnitsLost + ", UnitsKept=" + viewModel.UnitsKept + ", Time=" + viewModel.ElapsedTimeSeconds.ToString("F1") + "s");
        }
    }
}




