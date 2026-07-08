using System.Collections.Generic;
using System.Numerics;

namespace Warzone.Combat
{
    public sealed class FormationSystem
    {
        public void Execute(BattleState battleState)
        {
            if (battleState == null)
            {
                return;
            }

            foreach (BattleSquadState squadState in battleState.SquadsById.Values)
            {
                AssignIntents(battleState, squadState);
            }
        }

        private static void AssignIntents(BattleState battleState, BattleSquadState squadState)
        {
            List<BattleMemberState> livingMembers = new List<BattleMemberState>();
            for (int i = 0; i < squadState.MemberIds.Count; i++)
            {
                BattleMemberState memberState;
                if (!battleState.TryGetMember(squadState.MemberIds[i], out memberState) || !memberState.IsAlive)
                {
                    continue;
                }

                livingMembers.Add(memberState);
            }

            if (livingMembers.Count == 0)
            {
                return;
            }

            Vector2 direction = squadState.DesiredPosition - squadState.Position;
            if (direction.LengthSquared() <= 0.0001f)
            {
                direction = new Vector2(0f, 1f);
            }
            else
            {
                direction = Vector2.Normalize(direction);
            }

            Vector2 right = new Vector2(-direction.Y, direction.X);
            float halfWidth = (livingMembers.Count - 1) * squadState.FormationSpacing * 0.5f;

            for (int i = 0; i < livingMembers.Count; i++)
            {
                BattleMemberState memberState = livingMembers[i];
                Vector2 target = squadState.DesiredPosition + (right * ((i * squadState.FormationSpacing) - halfWidth));
                ApplyIntent(memberState, target);
                battleState.EventBuffer.Add(new BattleEventRecord("MemberIntentAssigned", squadState.SquadId, memberState.MemberId, target.ToString()));
            }
        }

        private static void ApplyIntent(BattleMemberState memberState, Vector2 targetPosition)
        {
            if (memberState.CurrentIntent == null)
            {
                memberState.SetIntent(new MemberIntent(MemberIntentType.MoveToPosition, targetPosition));
                return;
            }

            if (memberState.CurrentIntent.IntentType != MemberIntentType.MoveToPosition)
            {
                memberState.SetIntent(new MemberIntent(MemberIntentType.MoveToPosition, targetPosition));
                return;
            }

            Vector2 delta = memberState.CurrentIntent.TargetPosition - targetPosition;
            if (delta.LengthSquared() > 0.0001f)
            {
                memberState.CurrentIntent.UpdateTarget(targetPosition);
            }
        }
    }
}
