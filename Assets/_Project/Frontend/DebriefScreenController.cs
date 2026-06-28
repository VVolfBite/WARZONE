namespace Warzone.Frontend
{
    public sealed class DebriefScreenController
    {
        public DebriefViewModel BuildViewModel(bool isVictory, int unitsLost, float elapsedTimeSeconds)
        {
            return new DebriefViewModel
            {
                IsVictory = isVictory,
                UnitsLost = unitsLost,
                ElapsedTimeSeconds = elapsedTimeSeconds,
            };
        }
    }
}
