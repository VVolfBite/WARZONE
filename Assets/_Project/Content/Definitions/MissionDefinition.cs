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

        public string Id { get; }
        public string DisplayName { get; }
        public int PlayerSquadCount { get; }
        public int EnemySquadCount { get; }
    }
}
