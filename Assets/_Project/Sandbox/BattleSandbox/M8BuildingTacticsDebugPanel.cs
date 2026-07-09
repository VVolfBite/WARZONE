using UnityEngine;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M8BuildingTacticsDebugPanel : MonoBehaviour
    {
        private BattleSnapshot _snapshot;
        private BattleSandboxRuntimeContext _context;
        private Vector2 _scrollPosition;

        public void Bind(BattleSnapshot snapshot, BattleSandboxRuntimeContext context)
        {
            _snapshot = snapshot;
            _context = context;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(12f, 12f, 900f, 980f), GUI.skin.box);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(880f), GUILayout.Height(960f));

            GUILayout.Label("M8 Building Tactics Sandbox");
            if (_context != null)
            {
                GUILayout.Label("Mode: " + _context.ModeLabel);
                GUILayout.Label("Paused: " + _context.IsPaused + " | Selected Squad: " + _context.SelectedSquadId + " | Fire Lines: " + _context.ShowFireLines);
            }

            DrawSectionHeader("Controls");
            GUILayout.Label("LMB Select | RMB Move | D Defend Area | S Search Point | E Extract | G Enter Building | H Defend Building | J Search Building");
            GUILayout.Label("Debug: C Fire Lines | N Night | B Smoke | F Fire | L Light | P/Space Pause | R Reset");

            if (_snapshot == null)
            {
                GUILayout.Label("No snapshot");
                GUILayout.EndScrollView();
                GUILayout.EndArea();
                return;
            }

            DrawSectionHeader("Mission");
            if (_snapshot.MissionStatus != null)
            {
                GUILayout.Label(
                    "entered " + _snapshot.MissionStatus.EnteredBuildingCount + "/" + _snapshot.MissionStatus.TotalBuildingObjectiveCount +
                    " | searched " + _snapshot.MissionStatus.SearchedPointCount + "/" + _snapshot.MissionStatus.TotalSearchPointCount +
                    " | enemies " + _snapshot.MissionStatus.AliveEnemyCount + "/" + _snapshot.MissionStatus.TotalEnemyCount +
                    " | extracted " + _snapshot.MissionStatus.ExtractedMemberCount + "/" + _snapshot.MissionStatus.TotalAliveMemberCount +
                    " | loot " + _snapshot.MissionStatus.LootCount +
                    " | complete " + _snapshot.MissionStatus.IsBattleComplete +
                    " | result " + _snapshot.MissionStatus.ResultType);
            }

            DrawSectionHeader("Buildings");
            for (int i = 0; i < _snapshot.Buildings.Count; i++)
            {
                BuildingSnapshot building = _snapshot.Buildings[i];
                int occupiedWindows = CountOccupiedNodes(_snapshot, building.WindowNodeIds);
                int occupiedInteriors = CountOccupiedNodes(_snapshot, building.InteriorNodeIds);
                int searchedNodes = CountSearchedNodes(_snapshot, building.SearchNodeIds);
                GUILayout.Label(
                    "#" + building.BuildingId +
                    " enterable " + building.IsEnterable +
                    " windows " + occupiedWindows + "/" + building.WindowNodeIds.Count +
                    " interiors " + occupiedInteriors + "/" + building.InteriorNodeIds.Count +
                    " searched " + searchedNodes + "/" + building.SearchNodeIds.Count);
            }

            DrawSectionHeader("Members");
            for (int i = 0; i < _snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = _snapshot.Members[i];
                GUILayout.Label(
                    "#" + member.MemberId.Value +
                    " hp " + member.Health + "/" + member.MaxHealth +
                    " inside " + IsInsideBuilding(_snapshot, member.OccupiedTacticalNodeId) +
                    " node " + (member.OccupiedTacticalNodeId.HasValue ? member.OccupiedTacticalNodeId.Value.ToString() : "-") +
                    " intent " + member.CurrentIntent +
                    " target " + (member.CurrentTargetEnemyId.HasValue ? member.CurrentTargetEnemyId.Value.Value.ToString() : "-") +
                    " pressure " + member.Pressure.ToString("F1") +
                    " suppressed " + member.IsSuppressed +
                    " retreating " + member.IsRetreating +
                    " pos " + FormatVec2(member.Position));
            }

            DrawSectionHeader("Recent Events");
            int startIndex = _snapshot.RecentEvents.Count > 14 ? _snapshot.RecentEvents.Count - 14 : 0;
            for (int i = startIndex; i < _snapshot.RecentEvents.Count; i++)
            {
                BattleEventRecord battleEvent = _snapshot.RecentEvents[i];
                GUILayout.Label(battleEvent.EventType + " | " + (battleEvent.Message ?? string.Empty));
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private static int CountOccupiedNodes(BattleSnapshot snapshot, System.Collections.Generic.IReadOnlyList<int> nodeIds)
        {
            int count = 0;
            for (int i = 0; i < nodeIds.Count; i++)
            {
                TacticalNodeSnapshot node = FindNode(snapshot, nodeIds[i]);
                if (node != null && node.OccupyingMemberId.HasValue)
                {
                    count++;
                }
            }

            return count;
        }

        private static int CountSearchedNodes(BattleSnapshot snapshot, System.Collections.Generic.IReadOnlyList<int> nodeIds)
        {
            int count = 0;
            for (int i = 0; i < nodeIds.Count; i++)
            {
                TacticalNodeSnapshot node = FindNode(snapshot, nodeIds[i]);
                if (node != null && node.IsSearched)
                {
                    count++;
                }
            }

            return count;
        }

        private static bool IsInsideBuilding(BattleSnapshot snapshot, int? nodeId)
        {
            TacticalNodeSnapshot node = FindNode(snapshot, nodeId);
            return node != null && node.IsInsideBuilding;
        }

        private static TacticalNodeSnapshot FindNode(BattleSnapshot snapshot, int? nodeId)
        {
            if (snapshot == null || !nodeId.HasValue)
            {
                return null;
            }

            for (int i = 0; i < snapshot.TacticalNodes.Count; i++)
            {
                if (snapshot.TacticalNodes[i].NodeId == nodeId.Value)
                {
                    return snapshot.TacticalNodes[i];
                }
            }

            return null;
        }

        private static void DrawSectionHeader(string text)
        {
            GUILayout.Space(8f);
            GUILayout.Label(text);
        }

        private static string FormatVec2(Vec2 value)
        {
            return "(" + value.X.ToString("F1") + ", " + value.Y.ToString("F1") + ")";
        }
    }
}
