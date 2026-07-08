namespace Warzone.Runtime.UI
{
    public sealed class DebriefScreenController
    {
        public DebriefViewModel BuildViewModel(bool isVictory, int unitsLost, int unitsKept, float elapsedTimeSeconds)
        {
            return new DebriefViewModel
            {
                IsVictory = isVictory,
                UnitsLost = unitsLost,
                UnitsKept = unitsKept,
                ElapsedTimeSeconds = elapsedTimeSeconds,
                Rating = BuildRating(isVictory, unitsLost, elapsedTimeSeconds),
                Summary = BuildSummary(isVictory, unitsLost, unitsKept),
            };
        }

        private static string BuildRating(bool isVictory, int unitsLost, float elapsedTimeSeconds)
        {
            if (!isVictory)
            {
                return "C";
            }

            if (unitsLost == 0 && elapsedTimeSeconds <= 90f)
            {
                return "S";
            }

            if (unitsLost <= 1 && elapsedTimeSeconds <= 140f)
            {
                return "A";
            }

            return unitsLost <= 2 ? "B" : "C";
        }

        private static string BuildSummary(bool isVictory, int unitsLost, int unitsKept)
        {
            if (isVictory)
            {
                return "Objective secured. Remaining squads extracted: " + unitsKept + ".";
            }

            return "Strike team wiped or mission failed. Units lost: " + unitsLost + ".";
        }
    }
}



