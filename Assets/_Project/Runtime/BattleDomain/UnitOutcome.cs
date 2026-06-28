namespace Warzone.BattleDomain
{
    public sealed class UnitOutcome
    {
        public UnitOutcome(BattleEntityId entityId, bool survived)
        {
            EntityId = entityId;
            Survived = survived;
        }

        public BattleEntityId EntityId { get; }
        public bool Survived { get; }
    }
}
