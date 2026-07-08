namespace Warzone.Campaign
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

        public string OwnedUnitId { get; private set; }
        public string DefinitionId { get; private set; }
        public int Experience { get; private set; }
    }
}




