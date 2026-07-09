namespace Warzone.Combat
{
    public static class BuildingVisibilityRule
    {
        public static BuildingVisibilityResult Evaluate(BattleState battleState, BattleMemberState observer, BattleEnemyState target)
        {
            return EvaluateInternal(battleState, GetNode(battleState, observer != null ? observer.OccupiedTacticalNodeId : null), GetNode(battleState, target != null ? target.OccupiedTacticalNodeId : null));
        }

        public static BuildingVisibilityResult Evaluate(BattleState battleState, BattleEnemyState observer, BattleMemberState target)
        {
            return EvaluateInternal(battleState, GetNode(battleState, observer != null ? observer.OccupiedTacticalNodeId : null), GetNode(battleState, target != null ? target.OccupiedTacticalNodeId : null));
        }

        private static BuildingVisibilityResult EvaluateInternal(BattleState battleState, TacticalNodeState observerNode, TacticalNodeState targetNode)
        {
            if (observerNode == null && targetNode == null)
            {
                return new BuildingVisibilityResult(true);
            }

            if (observerNode != null &&
                targetNode != null &&
                observerNode.BuildingId.HasValue &&
                targetNode.BuildingId.HasValue &&
                observerNode.BuildingId.Value == targetNode.BuildingId.Value)
            {
                return new BuildingVisibilityResult(true, null, IsWindowLike(observerNode) || IsWindowLike(targetNode));
            }

            if (IsInteriorOnly(observerNode) && (targetNode == null || !IsSameBuilding(observerNode, targetNode)))
            {
                return new BuildingVisibilityResult(false, observerNode.BuildingId, false);
            }

            if (IsInteriorOnly(targetNode) && (observerNode == null || !IsSameBuilding(observerNode, targetNode)))
            {
                return new BuildingVisibilityResult(false, targetNode.BuildingId, false);
            }

            bool allowThroughBuildingBlockers = IsWindowLike(observerNode) || IsWindowLike(targetNode);
            return new BuildingVisibilityResult(true, null, allowThroughBuildingBlockers);
        }

        private static TacticalNodeState GetNode(BattleState battleState, int? nodeId)
        {
            if (battleState == null || !nodeId.HasValue)
            {
                return null;
            }

            TacticalNodeState nodeState;
            return battleState.TryGetTacticalNode(nodeId.Value, out nodeState) ? nodeState : null;
        }

        private static bool IsInteriorOnly(TacticalNodeState nodeState)
        {
            return nodeState != null &&
                   nodeState.BuildingId.HasValue &&
                   nodeState.IsInsideBuilding &&
                   !IsWindowLike(nodeState);
        }

        private static bool IsWindowLike(TacticalNodeState nodeState)
        {
            if (nodeState == null)
            {
                return false;
            }

            return nodeState.NodeType == TacticalNodeType.Window ||
                   nodeState.NodeType == TacticalNodeType.Doorway ||
                   nodeState.NodeType == TacticalNodeType.BuildingEntrance ||
                   nodeState.AllowsVisionThrough ||
                   nodeState.AllowsFireThrough ||
                   nodeState.IsEntryPoint;
        }

        private static bool IsSameBuilding(TacticalNodeState firstNodeState, TacticalNodeState secondNodeState)
        {
            return firstNodeState != null &&
                   secondNodeState != null &&
                   firstNodeState.BuildingId.HasValue &&
                   secondNodeState.BuildingId.HasValue &&
                   firstNodeState.BuildingId.Value == secondNodeState.BuildingId.Value;
        }
    }
}
