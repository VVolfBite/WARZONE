using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class BattleCasualtyResult
    {
        public BattleCasualtyResult(IReadOnlyList<BattleEntityId> deadMemberIds, IReadOnlyList<BattleEntityId> deadEnemyIds)
        {
            DeadMemberIds = deadMemberIds;
            DeadEnemyIds = deadEnemyIds;
        }

        public IReadOnlyList<BattleEntityId> DeadMemberIds { get; private set; }
        public IReadOnlyList<BattleEntityId> DeadEnemyIds { get; private set; }
    }
}
