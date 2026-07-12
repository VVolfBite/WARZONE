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
                GUILayout.Label("Selected Squad: " + _context.SelectedSquadId + " | Fire Lines: " + _context.ShowFireLines + " | Command Plan: " + _context.ShowCommandPlan);
                GUILayout.Label("Pause: Hold P " + _context.IsHoldPaused + " | Space Toggle " + _context.IsTogglePaused + " | Effective " + _context.IsPaused);
            }

            DrawSectionHeader("Controls");
            GUILayout.Label("LMB Select | RMB Move | D Defend Area | S Search Point | E Extract | G Enter Building | H Defend Building | J Search Building");
            GUILayout.Label("Debug: Hold Shift Command Plan | Hold P Pause | Space Toggle Pause | C Fire Lines | N Night | B Smoke | F Fire | L Light | R Reset");

            if (_snapshot == null)
            {
                GUILayout.Label("No snapshot");
                GUILayout.EndScrollView();
                GUILayout.EndArea();
                return;
            }

            DrawSelectedSquadSummary();
            DrawInputDiagnostics();

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

            DrawSectionHeader("Member Details");
            for (int i = 0; i < _snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = _snapshot.Members[i];
                GUILayout.Label(
                    "#" + member.MemberId.Value +
                    " hp " + member.Health + "/" + member.MaxHealth +
                    " inside " + IsInsideBuilding(_snapshot, member.OccupiedTacticalNodeId) +
                    " node " + (member.OccupiedTacticalNodeId.HasValue ? member.OccupiedTacticalNodeId.Value.ToString() : "-") +
                    " intent " + member.CurrentIntent +
                    " moveTarget " + (member.MoveTarget.HasValue ? FormatVec2(member.MoveTarget.Value) : "-") +
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

        private void DrawSelectedSquadSummary()
        {
            BattleSquadSnapshot squad = BattleSandboxCommandQueries.FindSelectedSquad(_snapshot, _context != null ? _context.SelectedSquadId : 0);
            DrawSectionHeader("Selected Squad");
            if (squad == null)
            {
                GUILayout.Label("No selected squad snapshot");
                return;
            }

            int active = 0;
            int dead = 0;
            int extracted = 0;
            int retreating = 0;
            for (int i = 0; i < _snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = _snapshot.Members[i];
                if (member.SquadId != squad.SquadId)
                {
                    continue;
                }

                if (!member.IsAlive)
                {
                    dead++;
                }
                else if (member.IsExtracted)
                {
                    extracted++;
                }
                else
                {
                    active++;
                }

                if (member.IsRetreating)
                {
                    retreating++;
                }
            }

            GUILayout.Label("Current Order: " + squad.CurrentCommand + " | Stance: " + squad.Stance);
            GUILayout.Label("Desired Position: " + FormatVec2(squad.DesiredPosition) + " | Center: " + FormatVec2(squad.Position));
            GUILayout.Label("Active " + active + " | Dead " + dead + " | Extracted " + extracted + " | Retreating " + retreating);

            DrawSectionHeader("Current Command Plan");
            for (int i = 0; i < _snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = _snapshot.Members[i];
                if (member.SquadId != squad.SquadId)
                {
                    continue;
                }

                GUILayout.Label("#" + member.MemberId.Value + " " + member.CurrentIntent + " -> " + (member.MoveTarget.HasValue ? FormatVec2(member.MoveTarget.Value) : "-"));
            }
        }

        private void DrawInputDiagnostics()
        {
            if (_context == null)
            {
                return;
            }

            DrawSectionHeader("Input Diagnostics");
            GUILayout.Label("Last Input: " + _context.LastInputAction + " | Last Command: " + _context.LastCommandIssued);
            GUILayout.Label("Last Command Position: " + (_context.LastCommandWorldPosition.HasValue ? FormatVec2(_context.LastCommandWorldPosition.Value) : "-") + " | Frame: " + _context.LastCommandFrame + " | Time: " + _context.LastCommandTime.ToString("F2"));
            GUILayout.Label("Raycast Hit: " + _context.LastRaycastHitName + " | Ground Fallback: " + _context.LastRaycastUsedGroundFallback);

            GUILayout.Label("Recent Commands");
            for (int i = 0; i < _context.RecentCommandRecords.Count; i++)
            {
                SquadCommandDebugRecord record = _context.RecentCommandRecords[i];
                GUILayout.Label("Squad " + record.SquadId + " " + record.CommandName + " -> " + (record.DesiredPosition.HasValue ? FormatVec2(record.DesiredPosition.Value) : "-") + " frame " + record.Frame);
            }
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
