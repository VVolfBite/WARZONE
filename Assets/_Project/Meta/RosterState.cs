using System.Collections.Generic;

namespace Warzone.Meta
{
    public sealed class RosterState
    {
        public RosterState(IReadOnlyList<OwnedUnitState> ownedUnits)
        {
            OwnedUnits = ownedUnits;
        }

        public IReadOnlyList<OwnedUnitState> OwnedUnits { get; }
    }
}
