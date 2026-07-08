using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class BattleSnapshot
    {
        public BattleSnapshot(
            string battleId,
            float elapsedTimeSeconds,
            IReadOnlyList<BattleSquadSnapshot> squads,
            IReadOnlyList<BattleMemberSnapshot> members)
        {
            BattleId = battleId;
            ElapsedTimeSeconds = elapsedTimeSeconds;
            Squads = squads;
            Members = members;
        }

        public string BattleId { get; private set; }
        public float ElapsedTimeSeconds { get; private set; }
        public IReadOnlyList<BattleSquadSnapshot> Squads { get; private set; }
        public IReadOnlyList<BattleMemberSnapshot> Members { get; private set; }
    }
}
