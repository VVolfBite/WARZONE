namespace Warzone.Combat
{
    public static class BuildingFireRule
    {
        private const float WindowTargetDamageMultiplier = 0.7f;

        public static BuildingFireResult Evaluate(BattleState battleState, BattleMemberState shooter, BattleEnemyState target)
        {
            return EvaluateInternal(battleState, shooter != null ? shooter.OccupiedTacticalNodeId : null, target != null ? target.OccupiedTacticalNodeId : null);
        }

        public static BuildingFireResult Evaluate(BattleState battleState, BattleEnemyState shooter, BattleMemberState target)
        {
            return EvaluateInternal(battleState, shooter != null ? shooter.OccupiedTacticalNodeId : null, target != null ? target.OccupiedTacticalNodeId : null);
        }

        private static BuildingFireResult EvaluateInternal(BattleState battleState, int? shooterNodeId, int? targetNodeId)
        {
            TacticalNodeState shooterNode = GetNode(battleState, shooterNodeId);
            TacticalNodeState targetNode = GetNode(battleState, targetNodeId);
            BuildingVisibilityResult visibilityResult = EvaluateVisibility(shooterNode, targetNode);
            if (!visibilityResult.HasVisibility)
            {
                return new BuildingFireResult(false, visibilityResult.BlockingBuildingId, visibilityResult.AllowThroughBuildingBlockers);
            }

            float targetDamageMultiplier = IsWindowLike(targetNode) ? WindowTargetDamageMultiplier : 1f;
            return new BuildingFireResult(true, null, visibilityResult.AllowThroughBuildingBlockers, targetDamageMultiplier);
        }

        private static BuildingVisibilityResult EvaluateVisibility(TacticalNodeState shooterNode, TacticalNodeState targetNode)
        {
            if (shooterNode != null &&
                targetNode != null &&
                shooterNode.BuildingId.HasValue &&
                targetNode.BuildingId.HasValue &&
                shooterNode.BuildingId.Value == targetNode.BuildingId.Value)
            {
                return new BuildingVisibilityResult(true, null, IsWindowLike(shooterNode) || IsWindowLike(targetNode));
            }

            if (IsInteriorOnly(shooterNode) && (targetNode == null || !IsSameBuilding(shooterNode, targetNode)))
            {
                return new BuildingVisibilityResult(false, shooterNode.BuildingId, false);
            }

            if (IsInteriorOnly(targetNode) && (shooterNode == null || !IsSameBuilding(shooterNode, targetNode)))
            {
                return new BuildingVisibilityResult(false, targetNode.BuildingId, false);
            }

            return new BuildingVisibilityResult(true, null, IsWindowLike(shooterNode) || IsWindowLike(targetNode));
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
