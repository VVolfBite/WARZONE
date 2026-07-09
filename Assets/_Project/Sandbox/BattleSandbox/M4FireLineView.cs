using System.Collections.Generic;
using UnityEngine;
using Warzone.Combat;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M4FireLineView : MonoBehaviour
    {
        public void Render(
            BattleSnapshot snapshot,
            Dictionary<BattleEntityId, M1MemberView> memberViews,
            Dictionary<BattleEntityId, M2EnemyView> enemyViews,
            bool isEnabled)
        {
            if (!isEnabled || snapshot == null)
            {
                return;
            }

            for (int i = 0; i < snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = snapshot.Members[i];
                if (!member.IsAlive || member.IsExtracted || !member.CurrentTargetEnemyId.HasValue)
                {
                    continue;
                }

                M1MemberView memberView;
                M2EnemyView enemyView;
                if (!memberViews.TryGetValue(member.MemberId, out memberView) ||
                    !enemyViews.TryGetValue(member.CurrentTargetEnemyId.Value, out enemyView) ||
                    memberView == null ||
                    enemyView == null)
                {
                    continue;
                }

                Debug.DrawLine(memberView.transform.position, enemyView.transform.position, new Color(0.2f, 0.9f, 1f));
            }

            for (int i = 0; i < snapshot.Enemies.Count; i++)
            {
                BattleEnemySnapshot enemy = snapshot.Enemies[i];
                if (!enemy.IsAlive || !enemy.CurrentTargetMemberId.HasValue)
                {
                    continue;
                }

                M1MemberView memberView;
                M2EnemyView enemyView;
                if (!memberViews.TryGetValue(enemy.CurrentTargetMemberId.Value, out memberView) ||
                    !enemyViews.TryGetValue(enemy.EnemyId, out enemyView) ||
                    memberView == null ||
                    enemyView == null)
                {
                    continue;
                }

                Debug.DrawLine(enemyView.transform.position, memberView.transform.position, new Color(1f, 0.4f, 0.3f));
            }
        }
    }
}
