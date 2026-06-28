using System.Collections.Generic;
using System.Linq;
using Warzone.Content.Definitions;

namespace Warzone.BattleDomain
{
    public sealed class MissionRuntime
    {
        public MissionOutcome Evaluate(IReadOnlyList<BattleSquadState> squads)
        {
            bool playerAlive = squads.Any(squad => squad.FactionId == FactionId.Player && squad.HasLivingUnits);
            bool enemyAlive = squads.Any(squad => squad.FactionId == FactionId.Enemy && squad.HasLivingUnits);

            if (playerAlive && enemyAlive)
            {
                return MissionOutcome.InProgress;
            }

            if (playerAlive)
            {
                return MissionOutcome.Victory;
            }

            return MissionOutcome.Defeat;
        }
    }
}
