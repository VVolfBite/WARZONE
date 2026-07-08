namespace Warzone.Combat
{
    public sealed class BattleStatistics
    {
        public BattleStatistics(int playerUnitsRemaining, int enemyUnitsRemaining)
        {
            PlayerUnitsRemaining = playerUnitsRemaining;
            EnemyUnitsRemaining = enemyUnitsRemaining;
        }

        public int PlayerUnitsRemaining { get; private set; }
        public int EnemyUnitsRemaining { get; private set; }
    }
}



