namespace Warzone.Combat
{
    public sealed class SearchBuildingCommand : BattleCommand
    {
        public SearchBuildingCommand(int squadId, int buildingId)
            : base(squadId)
        {
            BuildingId = buildingId;
        }

        public override string Name
        {
            get { return "SearchBuilding"; }
        }

        public int BuildingId { get; private set; }
    }
}
