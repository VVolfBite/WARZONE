using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class RosterState
    {
        public RosterState(IReadOnlyList<OwnedUnitState> ownedUnits)
        {
            OwnedUnits = ownedUnits;
        }

        public IReadOnlyList<OwnedUnitState> OwnedUnits { get; private set; }
    }
}




