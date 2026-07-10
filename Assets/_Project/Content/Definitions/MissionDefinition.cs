using System.Collections.Generic;

namespace Warzone.Content.Definitions
{
    public sealed class MissionDefinition
    {
        public MissionDefinition(
            string id,
            string displayName,
            int playerSquadCount,
            int enemySquadCount,
            IReadOnlyList<MissionObjectiveDefinition> objectives = null,
            MissionType missionType = MissionType.Tactical,
            MissionDifficulty difficulty = MissionDifficulty.Normal,
            string requiredSiteType = null,
            MissionRewardDefinition reward = null)
        {
            Id = id;
            DisplayName = displayName;
            PlayerSquadCount = playerSquadCount;
            EnemySquadCount = enemySquadCount;
            Objectives = objectives ?? new List<MissionObjectiveDefinition>();
            MissionType = missionType;
            Difficulty = difficulty;
            RequiredSiteType = requiredSiteType;
            Reward = reward ?? new MissionRewardDefinition();
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public int PlayerSquadCount { get; private set; }
        public int EnemySquadCount { get; private set; }
        public IReadOnlyList<MissionObjectiveDefinition> Objectives { get; private set; }
        public MissionType MissionType { get; private set; }
        public MissionDifficulty Difficulty { get; private set; }
        public string RequiredSiteType { get; private set; }
        public MissionRewardDefinition Reward { get; private set; }
    }
}
