using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class BattleSnapshot
    {
        public BattleSnapshot(
            string battleId,
            float elapsedTimeSeconds,
            IReadOnlyList<BattleSquadSnapshot> squads,
            IReadOnlyList<BattleMemberSnapshot> members,
            IReadOnlyList<BattleEnemySnapshot> enemies,
            IReadOnlyList<BattleEventRecord> recentEvents)
        {
            BattleId = battleId;
            ElapsedTimeSeconds = elapsedTimeSeconds;
            Squads = squads;
            Members = members;
            Enemies = enemies;
            RecentEvents = recentEvents;
        }

        public string BattleId { get; private set; }
        public float ElapsedTimeSeconds { get; private set; }
        public IReadOnlyList<BattleSquadSnapshot> Squads { get; private set; }
        public IReadOnlyList<BattleMemberSnapshot> Members { get; private set; }
        public IReadOnlyList<BattleEnemySnapshot> Enemies { get; private set; }
        public IReadOnlyList<BattleEventRecord> RecentEvents { get; private set; }
    }
}
