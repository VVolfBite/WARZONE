namespace Warzone.Combat
{
    public sealed class EnterBuildingCommand : BattleCommand
    {
        public EnterBuildingCommand(int squadId, int buildingId)
            : base(squadId)
        {
            BuildingId = buildingId;
        }

        public override string Name
        {
            get { return "EnterBuilding"; }
        }

        public int BuildingId { get; private set; }
    }
}
