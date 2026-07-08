using System.Collections.Generic;
using Warzone.Combat;

namespace Warzone.Sandbox.Waves
{
    public sealed class SandboxWaveSpawnPlan
    {
        public SandboxWaveSpawnPlan(int waveIndex, float delaySeconds, IReadOnlyList<BattleSquadState> squads)
        {
            WaveIndex = waveIndex;
            DelaySeconds = delaySeconds;
            Squads = squads;
        }

        public int WaveIndex { get; }
        public float DelaySeconds { get; }
        public IReadOnlyList<BattleSquadState> Squads { get; }
    }
}



