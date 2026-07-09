using System.Collections.Generic;
using Warzone.Core.Math;

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

            ResetTacticalReservations(battleState);
            foreach (BattleSquadState squadState in battleState.SquadsById.Values)
            {
                AssignIntents(battleState, squadState);
            }
        }

        private static void ResetTacticalReservations(BattleState battleState)
        {
            foreach (TacticalNodeState nodeState in battleState.TacticalNodesById.Values)
            {
                if (nodeState != null)
                {
                    nodeState.ClearReservation();
                }
            }
        }

        private static void AssignIntents(BattleState battleState, BattleSquadState squadState)
        {
            List<BattleMemberState> activeMembers = new List<BattleMemberState>();
            for (int i = 0; i < squadState.MemberIds.Count; i++)
            {
                BattleMemberState memberState;
                if (!battleState.TryGetMember(squadState.MemberIds[i], out memberState) || !memberState.CanReceiveOrders)
                {
                    continue;
                }

                activeMembers.Add(memberState);
            }

            if (activeMembers.Count == 0)
            {
                return;
            }

            DefendAreaCommand defendCommand = squadState.CurrentOrder as DefendAreaCommand;
            if (defendCommand != null)
            {
                AssignDefendIntents(battleState, squadState, activeMembers, defendCommand);
                return;
            }

            SearchPointCommand searchCommand = squadState.CurrentOrder as SearchPointCommand;
            if (searchCommand != null)
            {
                AssignSearchIntents(battleState, squadState, activeMembers, searchCommand);
                return;
            }

            ExtractSquadCommand extractCommand = squadState.CurrentOrder as ExtractSquadCommand;
            if (extractCommand != null)
            {
                AssignExtractIntents(battleState, activeMembers, extractCommand);
                return;
            }

            AssignMoveFormation(squadState, activeMembers);
        }

        private static void AssignDefendIntents(BattleState battleState, BattleSquadState squadState, List<BattleMemberState> activeMembers, DefendAreaCommand command)
        {
            for (int i = 0; i < activeMembers.Count; i++)
            {
                BattleMemberState memberState = activeMembers[i];
                TacticalNodeState coverNode = FindAvailableCoverNodeQuery.Execute(battleState, command.AreaCenter, command.Radius);
                if (coverNode != null)
                {
                    coverNode.ReserveFor(memberState.MemberId);
                    ApplyIntent(memberState, MemberIntentType.TakeCover, coverNode.Position, coverNode.NodeId);
                    battleState.AddEvent(new BattleEventRecord(BattleEventTypes.TacticalNodeReserved, squadState.SquadId, memberState.MemberId, coverNode.NodeId.ToString()));
                    battleState.AddEvent(new BattleEventRecord(BattleEventTypes.DefensivePositionAssigned, squadState.SquadId, memberState.MemberId, coverNode.NodeId.ToString()));
                    continue;
                }

                Vec2 fallback = BuildFormationTarget(squadState, activeMembers.Count, i, command.AreaCenter);
                ApplyIntent(memberState, MemberIntentType.MoveToPosition, fallback, null);
            }
        }

        private static void AssignSearchIntents(BattleState battleState, BattleSquadState squadState, List<BattleMemberState> activeMembers, SearchPointCommand command)
        {
            TacticalNodeState searchNode;
            if (!battleState.TryGetTacticalNode(command.NodeId, out searchNode))
            {
                return;
            }

            for (int i = 0; i < activeMembers.Count; i++)
            {
                BattleMemberState memberState = activeMembers[i];
                if (!searchNode.IsSearched && i == 0)
                {
                    searchNode.ReserveFor(memberState.MemberId);
                    ApplyIntent(memberState, MemberIntentType.SearchPoint, searchNode.Position, searchNode.NodeId);
                    battleState.AddEvent(new BattleEventRecord(BattleEventTypes.TacticalNodeReserved, squadState.SquadId, memberState.MemberId, searchNode.NodeId.ToString()));
                    continue;
                }

                TacticalNodeState coverNode = FindAvailableCoverNodeQuery.Execute(battleState, searchNode.Position, 6f);
                if (coverNode != null)
                {
                    coverNode.ReserveFor(memberState.MemberId);
                    ApplyIntent(memberState, MemberIntentType.TakeCover, coverNode.Position, coverNode.NodeId);
                    continue;
                }

                Vec2 fallback = BuildFormationTarget(squadState, activeMembers.Count, i, searchNode.Position);
                ApplyIntent(memberState, MemberIntentType.MoveToPosition, fallback, null);
            }
        }

        private static void AssignExtractIntents(BattleState battleState, List<BattleMemberState> activeMembers, ExtractSquadCommand command)
        {
            TacticalNodeState extractionNode;
            if (!battleState.TryGetTacticalNode(command.ExtractionNodeId, out extractionNode))
            {
                return;
            }

            for (int i = 0; i < activeMembers.Count; i++)
            {
                ApplyIntent(activeMembers[i], MemberIntentType.Extract, extractionNode.Position, extractionNode.NodeId);
            }
        }

        private static void AssignMoveFormation(BattleSquadState squadState, List<BattleMemberState> activeMembers)
        {
            for (int i = 0; i < activeMembers.Count; i++)
            {
                Vec2 target = BuildFormationTarget(squadState, activeMembers.Count, i, squadState.DesiredPosition);
                ApplyIntent(activeMembers[i], MemberIntentType.MoveToPosition, target, null);
            }
        }

        private static Vec2 BuildFormationTarget(BattleSquadState squadState, int memberCount, int memberIndex, Vec2 center)
        {
            Vec2 direction = squadState.DesiredPosition - squadState.Position;
            direction = direction.LengthSquared <= 0.0001f ? new Vec2(0f, 1f) : Vec2.Normalize(direction);
            Vec2 right = new Vec2(-direction.Y, direction.X);
            float halfWidth = (memberCount - 1) * squadState.FormationSpacing * 0.5f;
            return center + (right * ((memberIndex * squadState.FormationSpacing) - halfWidth));
        }

        private static void ApplyIntent(BattleMemberState memberState, MemberIntentType intentType, Vec2 targetPosition, int? tacticalNodeId)
        {
            if (memberState.CurrentIntent == null)
            {
                memberState.SetIntent(new MemberIntent(intentType, targetPosition, false, tacticalNodeId));
                return;
            }

            if (memberState.CurrentIntent.IntentType != intentType)
            {
                memberState.SetIntent(new MemberIntent(intentType, targetPosition, false, tacticalNodeId));
                return;
            }

            Vec2 delta = memberState.CurrentIntent.TargetPosition - targetPosition;
            if (delta.LengthSquared > 0.0001f || memberState.CurrentIntent.TacticalNodeId != tacticalNodeId)
            {
                memberState.CurrentIntent.UpdateTarget(targetPosition, tacticalNodeId);
            }
        }
    }
}
