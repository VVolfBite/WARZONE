using UnityEngine;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M2CombatDebugPanel : MonoBehaviour
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
            GUILayout.BeginArea(new Rect(12f, 12f, 520f, 680f), GUI.skin.box);
            GUILayout.Label("M2 Minimal Combat Slice");
            GUILayout.Label("Paused: " + _isPaused);
            GUILayout.Label("Selected Squad: " + _selectedSquadId);

            if (_snapshot == null)
            {
                GUILayout.Label("No snapshot");
                GUILayout.EndArea();
                return;
            }

            int aliveMembers = 0;
            for (int i = 0; i < _snapshot.Members.Count; i++)
            {
                if (_snapshot.Members[i].IsAlive)
                {
                    aliveMembers++;
                }
            }

            int aliveEnemies = 0;
            for (int i = 0; i < _snapshot.Enemies.Count; i++)
            {
                if (_snapshot.Enemies[i].IsAlive)
                {
                    aliveEnemies++;
                }
            }

            GUILayout.Label("Members: " + aliveMembers + "/" + _snapshot.Members.Count);
            GUILayout.Label("Enemies: " + aliveEnemies + "/" + _snapshot.Enemies.Count);

            GUILayout.Space(8f);
            GUILayout.Label("Members");
            for (int i = 0; i < _snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = _snapshot.Members[i];
                string target = member.CurrentTargetEnemyId.HasValue ? member.CurrentTargetEnemyId.Value.Value.ToString() : "-";
                GUILayout.Label(
                    "#" + member.MemberId.Value +
                    " pos " + FormatVector(member.Position) +
                    " hp " + member.Health + "/" + member.MaxHealth +
                    " target " + target +
                    " cd " + member.AttackCooldownRemaining.ToString("F2") +
                    " intent " + member.CurrentIntent);
            }

            GUILayout.Space(8f);
            GUILayout.Label("Enemies");
            for (int i = 0; i < _snapshot.Enemies.Count; i++)
            {
                BattleEnemySnapshot enemy = _snapshot.Enemies[i];
                GUILayout.Label(
                    "#" + enemy.EnemyId.Value +
                    " pos " + FormatVector(enemy.Position) +
                    " hp " + enemy.Health + "/" + enemy.MaxHealth +
                    " alive " + enemy.IsAlive);
            }

            GUILayout.Space(8f);
            GUILayout.Label("Recent Events");
            int startIndex = _snapshot.RecentEvents.Count > 8 ? _snapshot.RecentEvents.Count - 8 : 0;
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

        private static string FormatVector(Vec2 value)
        {
            return "(" + value.X.ToString("F2") + ", " + value.Y.ToString("F2") + ")";
        }
    }
}
