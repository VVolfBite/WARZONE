namespace Warzone.Combat
{
    public sealed class ExtractSquadCommand : BattleCommand
    {
        public ExtractSquadCommand(int squadId, int extractionNodeId)
            : base(squadId)
        {
            ExtractionNodeId = extractionNodeId;
        }

        public override string Name
        {
            get { return "ExtractSquad"; }
        }

        public int ExtractionNodeId { get; private set; }
    }
}
