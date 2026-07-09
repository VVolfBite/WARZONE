namespace Warzone.Combat
{
    public sealed class SearchPointCommand : BattleCommand
    {
        public SearchPointCommand(int squadId, int nodeId)
            : base(squadId)
        {
            NodeId = nodeId;
        }

        public override string Name
        {
            get { return "SearchPoint"; }
        }

        public int NodeId { get; private set; }
    }
}
