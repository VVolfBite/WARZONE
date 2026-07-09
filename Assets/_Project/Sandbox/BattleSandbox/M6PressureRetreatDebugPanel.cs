using UnityEngine;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M6PressureRetreatDebugPanel : MonoBehaviour
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
            GUILayout.BeginArea(new Rect(12f, 12f, 780f, 960f), GUI.skin.box);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(760f), GUILayout.Height(940f));

            GUILayout.Label("M6 Pressure / Retreat Sandbox");
            if (_context != null)
            {
                GUILayout.Label("Mode: " + _context.ModeLabel);
                GUILayout.Label("Paused: " + _context.IsPaused + " | Selected Squad: " + _context.SelectedSquadId + " | Fire Lines: " + _context.ShowFireLines);
            }

            DrawSectionHeader("Controls");
            GUILayout.Label("LMB Select | RMB Move | D Defend | S Search | E Extract | C Fire Lines | P/Space Pause | R Reset");
            GUILayout.Label("Debug: T Apply Pressure | Y Clear Pressure | G Simulate Incoming Fire");
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
                    " | Result: " + _snapshot.MissionStatus.ResultType);
            }

            int retreatingCount = 0;
            int activeCount = 0;
            int deadCount = 0;
            int extractedCount = 0;
            for (int i = 0; i < _snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = _snapshot.Members[i];
                if (!member.IsAlive)
                {
                    deadCount++;
                }
                else if (member.IsExtracted)
                {
                    extractedCount++;
                }
                else
                {
                    activeCount++;
                }

                if (member.IsRetreating)
                {
                    retreatingCount++;
                }
            }

            GUILayout.Label("Members: active " + activeCount + " | dead " + deadCount + " | extracted " + extractedCount + " | retreating " + retreatingCount);

            DrawSectionHeader("Members");
            for (int i = 0; i < _snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = _snapshot.Members[i];
                string target = member.CurrentTargetEnemyId.HasValue ? member.CurrentTargetEnemyId.Value.Value.ToString() : "-";
                GUILayout.Label(
                    "#" + member.MemberId.Value +
                    " hp " + member.Health + "/" + member.MaxHealth +
                    " pressure " + member.Pressure.ToString("F1") + "/" + member.MaxPressure.ToString("F1") +
                    " suppressed " + member.IsSuppressed +
                    " broken " + member.IsBroken +
                    " retreating " + member.IsRetreating +
                    " retreatTarget " + (member.RetreatTargetPosition.HasValue ? FormatVec2(member.RetreatTargetPosition.Value) : "-") +
                    " pos " + FormatVec2(member.Position) +
                    " intent " + member.CurrentIntent +
                    " target " + target);
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

            DrawSectionHeader("Recent Pressure Events");
            int startIndex = _snapshot.RecentEvents.Count > 12 ? _snapshot.RecentEvents.Count - 12 : 0;
            for (int i = startIndex; i < _snapshot.RecentEvents.Count; i++)
            {
                BattleEventRecord battleEvent = _snapshot.RecentEvents[i];
                if (battleEvent.EventType != BattleEventTypes.PressureChanged &&
                    battleEvent.EventType != BattleEventTypes.MemberSuppressed &&
                    battleEvent.EventType != BattleEventTypes.MemberBroken &&
                    battleEvent.EventType != BattleEventTypes.MemberStartedRetreat)
                {
                    continue;
                }

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
            GUILayout.Label("Pressure / suppression / retreat are first-order combat feedback, not a full morale simulation.");

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
