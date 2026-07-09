using UnityEngine;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M4SpatialCombatDebugPanel : MonoBehaviour
    {
        private BattleSnapshot _snapshot;
        private bool _isPaused;
        private int _selectedSquadId;
        private bool _showDebugLines;

        public void Bind(BattleSnapshot snapshot, bool isPaused, int selectedSquadId, bool showDebugLines)
        {
            _snapshot = snapshot;
            _isPaused = isPaused;
            _selectedSquadId = selectedSquadId;
            _showDebugLines = showDebugLines;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(12f, 12f, 720f, 900f), GUI.skin.box);
            GUILayout.Label("M4 Spatial Combat Slice");
            GUILayout.Label("Paused: " + _isPaused + " | Selected Squad: " + _selectedSquadId + " | Fire Lines: " + _showDebugLines);
            GUILayout.Label("Controls: RMB Move | D Defend | S Search | E Extract | C Lines | P/Space Pause | R Reset");

            if (_snapshot == null)
            {
                GUILayout.Label("No snapshot");
                GUILayout.EndArea();
                return;
            }

            if (_snapshot.MissionStatus != null)
            {
                GUILayout.Label(
                    "Mission: search " + _snapshot.MissionStatus.IsSearchObjectiveComplete +
                    " | eliminate " + _snapshot.MissionStatus.IsEliminateObjectiveComplete +
                    " | extract " + _snapshot.MissionStatus.IsExtractObjectiveComplete +
                    " | battleComplete " + _snapshot.MissionStatus.IsBattleComplete +
                    " | result " + _snapshot.MissionStatus.ResultType +
                    " | loot " + _snapshot.MissionStatus.LootCount);
            }

            if (_snapshot.BattleResult != null)
            {
                GUILayout.Label(
                    "BattleResult: " + _snapshot.BattleResult.CompletionType +
                    " | casualties " + _snapshot.BattleResult.CasualtyResult.DeadMemberIds.Count +
                    " | extracted " + _snapshot.BattleResult.ExtractionResult.ExtractedMemberIds.Count +
                    " | enemyKills " + _snapshot.BattleResult.CasualtyResult.DeadEnemyIds.Count);
            }

            GUILayout.Space(8f);
            GUILayout.Label("Members");
            for (int i = 0; i < _snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = _snapshot.Members[i];
                string target = member.CurrentTargetEnemyId.HasValue ? member.CurrentTargetEnemyId.Value.Value.ToString() : "-";
                GUILayout.Label(
                    "#" + member.MemberId.Value +
                    " hp " + member.Health + "/" + member.MaxHealth +
                    " alive " + member.IsAlive +
                    " extracted " + member.IsExtracted +
                    " coverNode " + (member.OccupiedTacticalNodeId.HasValue ? member.OccupiedTacticalNodeId.Value.ToString() : "-") +
                    " pos " + FormatVec2(member.Position) +
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
                    " hp " + enemy.Health + "/" + enemy.MaxHealth +
                    " alive " + enemy.IsAlive +
                    " pos " + FormatVec2(enemy.Position) +
                    " target " + target +
                    " cd " + enemy.AttackCooldownRemaining.ToString("F2"));
            }

            GUILayout.Space(8f);
            GUILayout.Label("Tactical Nodes");
            for (int i = 0; i < _snapshot.TacticalNodes.Count; i++)
            {
                TacticalNodeSnapshot node = _snapshot.TacticalNodes[i];
                GUILayout.Label(
                    "#" + node.NodeId + " " + node.NodeType +
                    " pos " + FormatVec2(node.Position) +
                    " searched " + node.IsSearched +
                    " progress " + node.SearchProgress.ToString("F1") + "/" + node.RequiredSearchSeconds.ToString("F1"));
            }

            GUILayout.Space(8f);
            GUILayout.Label("Obstacles");
            for (int i = 0; i < _snapshot.Obstacles.Count; i++)
            {
                TacticalObstacleSnapshot obstacle = _snapshot.Obstacles[i];
                GUILayout.Label(
                    "#" + obstacle.ObstacleId + " " + obstacle.ObstacleType +
                    " los " + obstacle.BlocksLineOfSight +
                    " fire " + obstacle.BlocksFire +
                    " cover " + obstacle.ProvidesCover +
                    " reduction " + obstacle.DamageReductionFactor.ToString("F2"));
            }

            GUILayout.Space(8f);
            GUILayout.Label("Recent Events");
            int startIndex = _snapshot.RecentEvents.Count > 12 ? _snapshot.RecentEvents.Count - 12 : 0;
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
