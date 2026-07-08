namespace Warzone.Combat
{
    public abstract class BattleCommand
    {
        protected BattleCommand(int squadId)
        {
            SquadId = squadId;
        }

        public int SquadId { get; private set; }
        public abstract string Name { get; }
    }
}
