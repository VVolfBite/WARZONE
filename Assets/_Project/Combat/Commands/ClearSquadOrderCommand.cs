namespace Warzone.Combat
{
    public sealed class ClearSquadOrderCommand : BattleCommand
    {
        public ClearSquadOrderCommand(int squadId)
            : base(squadId)
        {
        }

        public override string Name
        {
            get { return "ClearSquadOrder"; }
        }
    }
}
