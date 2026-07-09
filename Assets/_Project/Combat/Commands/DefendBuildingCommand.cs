namespace Warzone.Combat
{
    public sealed class DefendBuildingCommand : BattleCommand
    {
        public DefendBuildingCommand(int squadId, int buildingId)
            : base(squadId)
        {
            BuildingId = buildingId;
        }

        public override string Name
        {
            get { return "DefendBuilding"; }
        }

        public int BuildingId { get; private set; }
    }
}
