namespace Warzone.Controls
{
    public sealed class DebriefViewModel
    {
        public bool IsVictory { get; set; }
        public int UnitsLost { get; set; }
        public int UnitsKept { get; set; }
        public float ElapsedTimeSeconds { get; set; }
    }
}
