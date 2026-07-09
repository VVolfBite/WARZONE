using UnityEngine;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M3TacticalMissionDebugPanel : MonoBehaviour
    {
        private BattleSnapshot _snapshot;
        private bool _isPaused;
        private int _selectedSquadId;

        public void Bind(BattleSnapshot snapshot, bool isPaused, int selectedSquadId)
        {
            _snapshot = snapshot;
            _isPaused = isPaused;
            _selectedSquadId = selectedSquadId;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(12f, 12f, 620f, 780f), GUI.skin.box);
            GUILayout.Label("M3 Tactical Mission Slice");
            GUILayout.Label("Paused: " + _isPaused);
            GUILayout.Label("Selected Squad: " + _selectedSquadId);
            GUILayout.Label("Controls: RMB Move | D Defend | S Search | E Extract | P/Space Pause | R Reset");

            if (_snapshot == null)
            {
                GUILayout.Label("No snapshot");
                GUILayout.EndArea();
                return;
            }

            BattleSquadSnapshot selectedSquad = null;
            for (int i = 0; i < _snapshot.Squads.Count; i++)
            {
                if (_snapshot.Squads[i].SquadId == _selectedSquadId)
                {
                    selectedSquad = _snapshot.Squads[i];
                    break;
                }
            }

            if (selectedSquad != null)
            {
                GUILayout.Label("Current Command: " + selectedSquad.CurrentCommand);
            }

            if (_snapshot.MissionStatus != null)
            {
                GUILayout.Label(
                    "Mission: search " + _snapshot.MissionStatus.SearchedPointCount + "/" + _snapshot.MissionStatus.TotalSearchPointCount +
                    " | enemies " + _snapshot.MissionStatus.AliveEnemyCount + "/" + _snapshot.MissionStatus.TotalEnemyCount +
                    " | extracted " + _snapshot.MissionStatus.ExtractedMemberCount + "/" + _snapshot.MissionStatus.TotalAliveMemberCount +
                    " | complete " + _snapshot.MissionStatus.IsObjectiveComplete);
            }

            GUILayout.Space(8f);
            GUILayout.Label("Members");
            for (int i = 0; i < _snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = _snapshot.Members[i];
                string target = member.CurrentTargetEnemyId.HasValue ? member.CurrentTargetEnemyId.Value.Value.ToString() : "-";
                GUILayout.Label(
                    "#" + member.MemberId.Value +
                    " pos " + FormatVec2(member.Position) +
                    " hp " + member.Health + "/" + member.MaxHealth +
                    " alive " + member.IsAlive +
                    " extracted " + member.IsExtracted +
                    " intent " + member.CurrentIntent +
                    " target " + target +
                    " cd " + member.AttackCooldownRemaining.ToString("F2"));
            }

            GUILayout.Space(8f);
            GUILayout.Label("Enemies");
            for (int i = 0; i < _snapshot.Enemies.Count; i++)
            {
                BattleEnemySnapshot enemy = _snapshot.Enemies[i];
                string target = enemy.CurrentTargetMemberId.HasValue ? enemy.CurrentTargetMemberId.Value.Value.ToString() : "-";
                GUILayout.Label(
                    "#" + enemy.EnemyId.Value +
                    " pos " + FormatVec2(enemy.Position) +
                    " hp " + enemy.Health + "/" + enemy.MaxHealth +
                    " alive " + enemy.IsAlive +
                    " target " + target +
                    " cd " + enemy.AttackCooldownRemaining.ToString("F2"));
            }

            GUILayout.Space(8f);
            GUILayout.Label("Tactical Nodes");
            for (int i = 0; i < _snapshot.TacticalNodes.Count; i++)
            {
                TacticalNodeSnapshot node = _snapshot.TacticalNodes[i];
                string occupant = node.OccupyingMemberId.HasValue ? node.OccupyingMemberId.Value.Value.ToString() : "-";
                GUILayout.Label(
                    "#" + node.NodeId +
                    " " + node.NodeType +
                    " pos " + FormatVec2(node.Position) +
                    " progress " + node.SearchProgress.ToString("F1") + "/" + node.RequiredSearchSeconds.ToString("F1") +
                    " searched " + node.IsSearched +
                    " occupant " + occupant);
            }

            GUILayout.Space(8f);
            GUILayout.Label("Recent Events");
            int startIndex = _snapshot.RecentEvents.Count > 10 ? _snapshot.RecentEvents.Count - 10 : 0;
            for (int i = startIndex; i < _snapshot.RecentEvents.Count; i++)
            {
                BattleEventRecord battleEvent = _snapshot.RecentEvents[i];
                GUILayout.Label(
                    battleEvent.EventType +
                    " src " + (battleEvent.MemberId.HasValue ? battleEvent.MemberId.Value.Value.ToString() : "-") +
                    " tgt " + (battleEvent.TargetId.HasValue ? battleEvent.TargetId.Value.Value.ToString() : "-") +
                    " amt " + battleEvent.Amount +
                    " msg " + (battleEvent.Message ?? string.Empty));
            }

            GUILayout.EndArea();
        }

        private static string FormatVec2(Vec2 value)
        {
            return "(" + value.X.ToString("F1") + ", " + value.Y.ToString("F1") + ")";
        }
    }
}
