using System.Collections.Generic;

namespace Warzone.Meta
{
    public sealed class RosterSnapshot
    {
        public RosterSnapshot(IReadOnlyList<OwnedUnitState> deployedUnits)
        {
            DeployedUnits = deployedUnits;
        }

        public IReadOnlyList<OwnedUnitState> DeployedUnits { get; }
    }
}
