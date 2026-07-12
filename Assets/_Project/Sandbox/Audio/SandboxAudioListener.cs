using UnityEngine;
using Warzone.Application;
using Warzone.Combat;
using Warzone.Runtime.Audio;
using Warzone.Runtime.Bootstrap;

namespace Warzone.Sandbox.Audio
{
    public sealed class SandboxAudioListener : MonoBehaviour
    {
        private BattleRuntimeHost _battleRuntimeHost;
        private AudioService _audioService;

        public void Configure(BattleRuntimeHost battleRuntimeHost, AudioService audioService)
        {
            _battleRuntimeHost = battleRuntimeHost;
            _audioService = audioService;
            _battleRuntimeHost.BattleStarted += HandleBattleStarted;
            _battleRuntimeHost.BattleFinished += HandleBattleFinished;
        }

        private void OnDestroy()
        {
            if (_battleRuntimeHost != null)
            {
                _battleRuntimeHost.BattleStarted -= HandleBattleStarted;
                _battleRuntimeHost.BattleFinished -= HandleBattleFinished;
            }
        }

        private void HandleBattleStarted(MissionStartRequest request)
        {
            _audioService?.PlayBattleStart();
        }

        private void HandleBattleFinished(BattleResult result)
        {
            if (result.MissionOutcome == MissionOutcome.Victory)
            {
                _audioService?.PlayVictory();
            }
            else if (result.MissionOutcome == MissionOutcome.Defeat)
            {
                _audioService?.PlayDefeat();
            }
        }
    }
}



