using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class WaveDefinition
    {
        public WaveDefinition(int waveIndex, float delaySeconds, IReadOnlyList<BattleSquadState> squads)
        {
            WaveIndex = waveIndex;
            DelaySeconds = delaySeconds;
            RemainingDelaySeconds = delaySeconds;
            Squads = squads;
        }

        public int WaveIndex { get; private set; }
        public float DelaySeconds { get; private set; }
        public float RemainingDelaySeconds { get; set; }
        public IReadOnlyList<BattleSquadState> Squads { get; private set; }
    }
}



