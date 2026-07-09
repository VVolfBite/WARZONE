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
            IReadOnlyList<TacticalNodeSnapshot> tacticalNodes,
            IReadOnlyList<TacticalObstacleSnapshot> obstacles,
            IReadOnlyList<BuildingSnapshot> buildings,
            BattleEnvironmentSnapshot environment,
            BattleMissionStatusSnapshot missionStatus,
            BattleResult battleResult,
            IReadOnlyList<BattleEventRecord> recentEvents)
        {
            BattleId = battleId;
            ElapsedTimeSeconds = elapsedTimeSeconds;
            Squads = squads;
            Members = members;
            Enemies = enemies;
            TacticalNodes = tacticalNodes;
            Obstacles = obstacles;
            Buildings = buildings;
            Environment = environment;
            MissionStatus = missionStatus;
            BattleResult = battleResult;
            RecentEvents = recentEvents;
        }

        public string BattleId { get; private set; }
        public float ElapsedTimeSeconds { get; private set; }
        public IReadOnlyList<BattleSquadSnapshot> Squads { get; private set; }
        public IReadOnlyList<BattleMemberSnapshot> Members { get; private set; }
        public IReadOnlyList<BattleEnemySnapshot> Enemies { get; private set; }
        public IReadOnlyList<TacticalNodeSnapshot> TacticalNodes { get; private set; }
        public IReadOnlyList<TacticalObstacleSnapshot> Obstacles { get; private set; }
        public IReadOnlyList<BuildingSnapshot> Buildings { get; private set; }
        public BattleEnvironmentSnapshot Environment { get; private set; }
        public BattleMissionStatusSnapshot MissionStatus { get; private set; }
        public BattleResult BattleResult { get; private set; }
        public IReadOnlyList<BattleEventRecord> RecentEvents { get; private set; }
    }
}
