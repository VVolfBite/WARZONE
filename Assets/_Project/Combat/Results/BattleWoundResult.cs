using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class BattleWoundResult
    {
        public BattleWoundResult(IReadOnlyList<BattleEntityId> woundedMemberIds)
        {
            WoundedMemberIds = woundedMemberIds ?? new List<BattleEntityId>();
        }

        public IReadOnlyList<BattleEntityId> WoundedMemberIds { get; private set; }
    }
}
