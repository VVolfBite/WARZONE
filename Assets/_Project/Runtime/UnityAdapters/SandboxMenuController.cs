using UnityEngine;
using Warzone.Application;
using Warzone.Content.Definitions;
using Warzone.Frontend;
using Warzone.Meta;

namespace Warzone.UnityAdapters
{
    public sealed class SandboxMenuController : MonoBehaviour
    {
        private BattleRuntimeHost _battleRuntimeHost;
        private MainMenuScreen _mainMenuScreen;
        private SandboxMissionStarter _missionStarter;
        private SandboxBattleBootstrap _battleBootstrap;
        private DebriefScreen _debriefScreen;

        public void Configure(
            BattleRuntimeHost battleRuntimeHost,
            MainMenuScreen mainMenuScreen,
            SandboxMissionStarter missionStarter,
            SandboxBattleBootstrap battleBootstrap,
            DebriefScreen debriefScreen)
        {
            _battleRuntimeHost = battleRuntimeHost;
            _mainMenuScreen = mainMenuScreen;
            _missionStarter = missionStarter;
            _battleBootstrap = battleBootstrap;
            _debriefScreen = debriefScreen;

            _mainMenuScreen.Bind(HandlePlayDemo);
            _mainMenuScreen.gameObject.SetActive(true);
            _battleBootstrap.gameObject.SetActive(false);
            _missionStarter.enabled = false;
            if (_debriefScreen != null)
            {
                _debriefScreen.gameObject.SetActive(false);
            }
        }

        private void HandlePlayDemo()
        {
            _mainMenuScreen.gameObject.SetActive(false);
            _battleBootstrap.gameObject.SetActive(true);
            _missionStarter.enabled = true;
            _missionStarter.StartDemoMission();
        }
    }
}
