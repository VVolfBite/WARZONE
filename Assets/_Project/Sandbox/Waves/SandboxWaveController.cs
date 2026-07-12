using System.Collections.Generic;
using Warzone.Combat;
using Warzone.Content.Definitions;
using Warzone.Runtime.Audio;

namespace Warzone.Sandbox.Waves
{
    public sealed class SandboxWaveController
    {
        private readonly Queue<string> _notifications;
        private readonly AudioService _audioService;
        private readonly List<SandboxWaveSpawnPlan> _wavePlans = new List<SandboxWaveSpawnPlan>();
        private int _activeWaveIndex;
        private float _nextWaveCountdown;
        private bool _waitingForNextWave;

        public SandboxWaveController(Queue<string> notifications, AudioService audioService = null)
        {
            _notifications = notifications;
            _audioService = audioService;
        }

        public int ActiveWaveIndex => _activeWaveIndex;
        public int TotalWaveCount => _wavePlans.Count;

        public void Reset(IReadOnlyList<SandboxWaveSpawnPlan> wavePlans, BattleSession battleSession)
        {
            _wavePlans.Clear();
            for (int i = 0; i < wavePlans.Count; i++)
            {
                _wavePlans.Add(wavePlans[i]);
            }

            _activeWaveIndex = 0;
            _nextWaveCountdown = 0f;
            _waitingForNextWave = false;
            battleSession.ConfigurePendingWaveCount(_wavePlans.Count);
        }

        public IReadOnlyList<BattleSquadState> SpawnInitialWave(BattleSession battleSession)
        {
            return TrySpawnNextWave(battleSession, true);
        }

        public IReadOnlyList<BattleSquadState> Tick(BattleSession battleSession, float deltaTimeSeconds)
        {
            if (_activeWaveIndex >= _wavePlans.Count)
            {
                return null;
            }

            if (HasLivingEnemy(battleSession))
            {
                _waitingForNextWave = false;
                _nextWaveCountdown = 0f;
                return null;
            }

            if (!_waitingForNextWave)
            {
                _waitingForNextWave = true;
                _nextWaveCountdown = _wavePlans[_activeWaveIndex].DelaySeconds;
                _notifications.Enqueue("Wave " + _wavePlans[_activeWaveIndex].WaveIndex + " incoming");
                _audioService?.PlayWaveAlert();
            }

            _nextWaveCountdown -= deltaTimeSeconds;
            if (_nextWaveCountdown > 0f)
            {
                return null;
            }

            return TrySpawnNextWave(battleSession, false);
        }

        private IReadOnlyList<BattleSquadState> TrySpawnNextWave(BattleSession battleSession, bool forceImmediate)
        {
            if (_activeWaveIndex >= _wavePlans.Count)
            {
                return null;
            }

            SandboxWaveSpawnPlan plan = _wavePlans[_activeWaveIndex];
            if (!forceImmediate)
            {
                _waitingForNextWave = false;
                _nextWaveCountdown = 0f;
            }

            battleSession.ConsumePendingWave();
            battleSession.AddSquads(plan.Squads);
            _notifications.Enqueue("Wave " + plan.WaveIndex + " deployed");
            _activeWaveIndex++;
            return plan.Squads;
        }

        private static bool HasLivingEnemy(BattleSession battleSession)
        {
            for (int i = 0; i < battleSession.Squads.Count; i++)
            {
                BattleSquadState squad = battleSession.Squads[i];
                if (squad.FactionId == FactionId.Enemy && squad.HasLivingUnits)
                {
                    return true;
                }
            }

            return false;
        }
    }
}



