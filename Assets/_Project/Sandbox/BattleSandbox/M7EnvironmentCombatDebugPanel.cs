using UnityEngine;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M7EnvironmentCombatDebugPanel : MonoBehaviour
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
            GUILayout.BeginArea(new Rect(12f, 12f, 840f, 980f), GUI.skin.box);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(820f), GUILayout.Height(960f));

            GUILayout.Label("M7 Environment Combat Sandbox");
            if (_context != null)
            {
                GUILayout.Label("Mode: " + _context.ModeLabel);
                GUILayout.Label("Paused: " + _context.IsPaused + " | Selected Squad: " + _context.SelectedSquadId + " | Fire Lines: " + _context.ShowFireLines);
            }

            DrawSectionHeader("Controls");
            GUILayout.Label("LMB Select | RMB Move | D Defend | S Search | E Extract | C Fire Lines | P/Space Pause | R Reset");
            GUILayout.Label("Debug: N Night Toggle | V Toggle Player NV | B Smoke | F Fire | L Light");

            if (_snapshot == null)
            {
                GUILayout.Label("No snapshot");
                GUILayout.EndScrollView();
                GUILayout.EndArea();
                return;
            }

            DrawSectionHeader("Environment");
            if (_snapshot.Environment != null)
            {
                GUILayout.Label("Night: " + _snapshot.Environment.IsNight + " | GlobalVisibility: " + _snapshot.Environment.GlobalVisibilityMultiplier.ToString("F2") + " | AmbientLight: " + _snapshot.Environment.AmbientLightLevel.ToString("F2"));
            }

            DrawSectionHeader("Members");
            for (int i = 0; i < _snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = _snapshot.Members[i];
                string target = member.CurrentTargetEnemyId.HasValue ? member.CurrentTargetEnemyId.Value.Value.ToString() : "-";
                GUILayout.Label(
                    "#" + member.MemberId.Value +
                    " nv " + member.NightVisionLevel +
                    " smokeVis " + member.SmokeVisionLevel +
                    " det " + member.DetectionRange.ToString("F1") +
                    " effDet " + member.EffectiveDetectionRange.ToString("F1") +
                    " target " + target +
                    " pressure " + member.Pressure.ToString("F1") +
                    " suppressed " + member.IsSuppressed +
                    " retreating " + member.IsRetreating +
                    " pos " + FormatVec2(member.Position));
            }

            DrawSectionHeader("Enemies");
            for (int i = 0; i < _snapshot.Enemies.Count; i++)
            {
                BattleEnemySnapshot enemy = _snapshot.Enemies[i];
                string target = enemy.CurrentTargetMemberId.HasValue ? enemy.CurrentTargetMemberId.Value.Value.ToString() : "-";
                GUILayout.Label(
                    "#" + enemy.EnemyId.Value +
                    " nv " + enemy.NightVisionLevel +
                    " det " + enemy.DetectionRange.ToString("F1") +
                    " effDet " + enemy.EffectiveDetectionRange.ToString("F1") +
                    " target " + target +
                    " pos " + FormatVec2(enemy.Position));
            }

            DrawSectionHeader("Zones");
            if (_snapshot.Environment != null)
            {
                for (int i = 0; i < _snapshot.Environment.Zones.Count; i++)
                {
                    EnvironmentalZoneSnapshot zone = _snapshot.Environment.Zones[i];
                    GUILayout.Label(
                        "#" + zone.ZoneId + " " + zone.ZoneType +
                        " pos " + FormatVec2(zone.Position) +
                        " r " + zone.Radius.ToString("F1") +
                        " intensity " + zone.Intensity.ToString("F2") +
                        " duration " + zone.DurationRemaining.ToString("F1") +
                        " active " + zone.IsActive);
                }
            }

            DrawSectionHeader("Recent Events");
            int startIndex = _snapshot.RecentEvents.Count > 14 ? _snapshot.RecentEvents.Count - 14 : 0;
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
