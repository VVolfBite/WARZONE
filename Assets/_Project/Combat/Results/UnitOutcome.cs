namespace Warzone.Combat
{
    public sealed class UnitOutcome
    {
        public UnitOutcome(BattleEntityId entityId, string definitionId, bool survived)
        {
            EntityId = entityId;
            DefinitionId = definitionId;
            Survived = survived;
        }

        public BattleEntityId EntityId { get; private set; }
        public string DefinitionId { get; private set; }
        public bool Survived { get; private set; }
    }
}



