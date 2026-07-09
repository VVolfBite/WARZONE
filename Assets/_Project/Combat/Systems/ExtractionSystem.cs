using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class ExtractionSystem
    {
        public void Execute(BattleState battleState)
        {
            if (battleState == null)
            {
                return;
            }

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (memberState == null || !memberState.IsAlive || memberState.IsExtracted || memberState.CurrentIntent == null)
                {
                    continue;
                }

                if (memberState.CurrentIntent.IntentType != MemberIntentType.Extract || !memberState.CurrentIntent.TacticalNodeId.HasValue)
                {
                    continue;
                }

                TacticalNodeState nodeState;
                if (!battleState.TryGetTacticalNode(memberState.CurrentIntent.TacticalNodeId.Value, out nodeState) || nodeState.NodeType != TacticalNodeType.ExtractionPoint)
                {
                    continue;
                }

                if (Vec2.Distance(memberState.Position, nodeState.Position) > nodeState.Radius && !memberState.CurrentIntent.IsCompleted)
                {
                    continue;
                }

                memberState.MarkExtracted();
                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.MemberExtracted, memberState.SquadId, memberState.MemberId, nodeState.NodeId.ToString()));
            }

            foreach (BattleSquadState squadState in battleState.SquadsById.Values)
            {
                if (squadState == null || squadState.IsExtracted)
                {
                    continue;
                }

                bool allResolved = true;
                for (int i = 0; i < squadState.MemberIds.Count; i++)
                {
                    BattleMemberState memberState;
                    if (!battleState.TryGetMember(squadState.MemberIds[i], out memberState))
                    {
                        continue;
                    }

                    if (memberState.IsAlive && !memberState.IsExtracted)
                    {
                        allResolved = false;
                        break;
                    }
                }

                if (!allResolved)
                {
                    continue;
                }

                squadState.MarkExtracted();
                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.SquadExtracted, squadState.SquadId, message: squadState.SquadId.ToString()));
            }
        }
    }
}
