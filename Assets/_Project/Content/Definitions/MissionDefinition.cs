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
            IReadOnlyList<MissionObjectiveDefinition> objectives = null)
        {
            Id = id;
            DisplayName = displayName;
            PlayerSquadCount = playerSquadCount;
            EnemySquadCount = enemySquadCount;
            Objectives = objectives ?? new List<MissionObjectiveDefinition>();
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public int PlayerSquadCount { get; private set; }
        public int EnemySquadCount { get; private set; }
        public IReadOnlyList<MissionObjectiveDefinition> Objectives { get; private set; }
    }
}
