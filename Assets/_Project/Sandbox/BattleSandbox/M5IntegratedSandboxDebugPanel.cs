using UnityEngine;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M5IntegratedSandboxDebugPanel : MonoBehaviour
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
            GUILayout.BeginArea(new Rect(12f, 12f, 760f, 940f), GUI.skin.box);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(740f), GUILayout.Height(920f));

            GUILayout.Label("M5 Integrated Sandbox");
            if (_context != null)
            {
                GUILayout.Label("Mode: " + _context.ModeLabel);
                GUILayout.Label("Paused: " + _context.IsPaused + " | Selected Squad: " + _context.SelectedSquadId + " | Fire Lines: " + _context.ShowFireLines);
            }

            DrawSectionHeader("Controls");
            GUILayout.Label("LMB Select | RMB Move | D Defend | S Search | E Extract | C Fire Lines | P/Space Pause | R Reset");
            GUILayout.Label("Pause policy: commands are allowed while paused and execute after resume.");

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
                    "Search: " + _snapshot.MissionStatus.IsSearchObjectiveComplete +
                    " | Eliminate: " + _snapshot.MissionStatus.IsEliminateObjectiveComplete +
                    " | Extract: " + _snapshot.MissionStatus.IsExtractObjectiveComplete +
                    " | BattleComplete: " + _snapshot.MissionStatus.IsBattleComplete +
                    " | Result: " + _snapshot.MissionStatus.ResultType +
                    " | Loot: " + _snapshot.MissionStatus.LootCount);
            }

            if (_snapshot.BattleResult != null)
            {
                GUILayout.Label(
                    "BattleResult: " + _snapshot.BattleResult.CompletionType +
                    " | Casualties: " + _snapshot.BattleResult.CasualtyResult.DeadMemberIds.Count +
                    " | Extracted: " + _snapshot.BattleResult.ExtractionResult.ExtractedMemberIds.Count +
                    " | EnemyKills: " + _snapshot.BattleResult.CasualtyResult.DeadEnemyIds.Count);
            }

            DrawSectionHeader("Squad / Members");
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

            DrawSectionHeader("Enemies");
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

            DrawSectionHeader("Tactical Nodes");
            for (int i = 0; i < _snapshot.TacticalNodes.Count; i++)
            {
                TacticalNodeSnapshot node = _snapshot.TacticalNodes[i];
                GUILayout.Label(
                    "#" + node.NodeId + " " + node.NodeType +
                    " pos " + FormatVec2(node.Position) +
                    " searched " + node.IsSearched +
                    " progress " + node.SearchProgress.ToString("F1") + "/" + node.RequiredSearchSeconds.ToString("F1") +
                    " occupant " + (node.OccupyingMemberId.HasValue ? node.OccupyingMemberId.Value.Value.ToString() : "-"));
            }

            DrawSectionHeader("Obstacles");
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

            DrawSectionHeader("Recent Events");
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

            DrawSectionHeader("Validation Hints");
            if (_context != null)
            {
                GUILayout.Label(_context.ValidationHint);
            }
            GUILayout.Label("This M5 path is an engineering sandbox entry, not a new gameplay layer.");

            GUILayout.EndScrollView();
            GUILayout.EndArea();
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
