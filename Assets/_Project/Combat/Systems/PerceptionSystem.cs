using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class PerceptionSystem
    {
        public void Execute(BattleState battleState)
        {
            if (battleState == null)
            {
                return;
            }

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (!memberState.CanAct)
                {
                    memberState.SetVisibleEnemies(null);
                    continue;
                }

                List<BattleEnemyState> visibleEnemies = FindVisibleEnemiesQuery.Execute(battleState, memberState);
                List<BattleEntityId> visibleEnemyIds = new List<BattleEntityId>(visibleEnemies.Count);
                for (int i = 0; i < visibleEnemies.Count; i++)
                {
                    visibleEnemyIds.Add(visibleEnemies[i].EnemyId);
                }

                memberState.SetVisibleEnemies(visibleEnemyIds);
            }
        }
    }
}
