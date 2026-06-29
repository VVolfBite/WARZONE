using UnityEngine;
using Warzone.Combat;
using Warzone.Controls;

namespace Warzone.Adapters
{
    public sealed class SandboxDebriefListener : MonoBehaviour
    {
        private readonly DebriefScreenController _controller = new DebriefScreenController();
        private BattleRuntimeHost _battleRuntimeHost;
        private DebriefScreen _debriefScreen;

        public void Configure(BattleRuntimeHost battleRuntimeHost, DebriefScreen debriefScreen)
        {
            _battleRuntimeHost = battleRuntimeHost;
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
            DebriefViewModel viewModel = _controller.BuildViewModel(
                battleResult.MissionOutcome == MissionOutcome.Victory,
                battleResult.Casualties.Count,
                battleResult.ElapsedTimeSeconds);

            _debriefScreen.Show(viewModel);
            Debug.Log($"Battle finished. Victory={viewModel.IsVictory}, UnitsLost={viewModel.UnitsLost}, Time={viewModel.ElapsedTimeSeconds:F1}s");
        }
    }
}
