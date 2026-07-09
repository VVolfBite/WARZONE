using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class SearchSystem
    {
        public void Execute(BattleState battleState, float deltaTimeSeconds)
        {
            if (battleState == null)
            {
                return;
            }

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (memberState == null || !memberState.CanAct || memberState.CurrentIntent == null)
                {
                    continue;
                }

                if (memberState.CurrentIntent.IntentType != MemberIntentType.SearchPoint || !memberState.CurrentIntent.TacticalNodeId.HasValue)
                {
                    continue;
                }

                TacticalNodeState nodeState;
                if (!battleState.TryGetTacticalNode(memberState.CurrentIntent.TacticalNodeId.Value, out nodeState) || nodeState.NodeType != TacticalNodeType.SearchPoint)
                {
                    continue;
                }

                if (nodeState.IsSearched)
                {
                    memberState.SetIntent(new MemberIntent(MemberIntentType.HoldPosition, nodeState.Position, true, nodeState.NodeId));
                    continue;
                }

                float distance = Vec2.Distance(memberState.Position, nodeState.Position);
                if (distance > nodeState.Radius && !memberState.CurrentIntent.IsCompleted)
                {
                    continue;
                }

                if (!nodeState.SearchStarted)
                {
                    nodeState.MarkSearchStarted();
                    battleState.AddEvent(new BattleEventRecord(BattleEventTypes.SearchStarted, memberState.SquadId, memberState.MemberId, nodeState.NodeId.ToString()));
                }

                nodeState.AdvanceSearch(deltaTimeSeconds);
                if (nodeState.SearchProgress < nodeState.RequiredSearchSeconds)
                {
                    continue;
                }

                nodeState.MarkSearched();
                memberState.SetIntent(new MemberIntent(MemberIntentType.HoldPosition, nodeState.Position, true, nodeState.NodeId));
                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.SearchCompleted, memberState.SquadId, memberState.MemberId, nodeState.NodeId.ToString()));

                if (!nodeState.LootDiscovered)
                {
                    nodeState.MarkLootDiscovered();
                    battleState.AddEvent(new BattleEventRecord(BattleEventTypes.LootDiscovered, memberState.SquadId, memberState.MemberId, "supplies:1", null, 1));
                }
            }
        }
    }
}
