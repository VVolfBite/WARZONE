namespace Warzone.Combat
{
    public static class FindCoverForMemberQuery
    {
        public static TacticalNodeState Execute(BattleState battleState, BattleMemberState memberState)
        {
            if (battleState == null || memberState == null || !memberState.OccupiedTacticalNodeId.HasValue)
            {
                return null;
            }

            TacticalNodeState nodeState;
            if (!battleState.TryGetTacticalNode(memberState.OccupiedTacticalNodeId.Value, out nodeState))
            {
                return null;
            }

            if (nodeState.NodeType == TacticalNodeType.Cover ||
                nodeState.NodeType == TacticalNodeType.DefensivePosition ||
                nodeState.NodeType == TacticalNodeType.Window)
            {
                return nodeState;
            }

            return null;
        }
    }
}
