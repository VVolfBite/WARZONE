namespace Warzone.Meta
{
    public sealed class OwnedUnitState
    {
        public OwnedUnitState(
            string ownedUnitId,
            string definitionId,
            int experience)
        {
            OwnedUnitId = ownedUnitId;
            DefinitionId = definitionId;
            Experience = experience;
        }

        public string OwnedUnitId { get; }
        public string DefinitionId { get; }
        public int Experience { get; }
    }
}
