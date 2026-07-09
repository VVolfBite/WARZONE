namespace Warzone.Combat
{
    public sealed class BattleLootResult
    {
        public BattleLootResult(int lootCount)
        {
            LootCount = lootCount;
        }

        public int LootCount { get; private set; }
    }
}
