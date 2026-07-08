namespace Warzone.Content.Definitions
{
    public sealed class MissionDefinition
    {
        public MissionDefinition(
            string id,
            string displayName,
            int playerSquadCount,
            int enemySquadCount)
        {
            Id = id;
            DisplayName = displayName;
            PlayerSquadCount = playerSquadCount;
            EnemySquadCount = enemySquadCount;
        }

        public string Id { get; private set; }
        public string DisplayName { get; private set; }
        public int PlayerSquadCount { get; private set; }
        public int EnemySquadCount { get; private set; }
    }
}



