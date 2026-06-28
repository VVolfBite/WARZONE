namespace Warzone.BattleDomain
{
    public sealed class BattleStatistics
    {
        public BattleStatistics(int playerUnitsRemaining, int enemyUnitsRemaining)
        {
            PlayerUnitsRemaining = playerUnitsRemaining;
            EnemyUnitsRemaining = enemyUnitsRemaining;
        }

        public int PlayerUnitsRemaining { get; }
        public int EnemyUnitsRemaining { get; }
    }
}
