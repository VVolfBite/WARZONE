using UnityEngine;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M1MemberSquadDebugPanel : MonoBehaviour
    {
        private BattleSnapshot _snapshot;
        private bool _isPaused;
        private bool _isRunning;
        private int _selectedSquadId;

        public void Bind(BattleSnapshot snapshot, bool isPaused, bool isRunning, int selectedSquadId)
        {
            _snapshot = snapshot;
            _isPaused = isPaused;
            _isRunning = isRunning;
            _selectedSquadId = selectedSquadId;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(12f, 12f, 420f, 480f), GUI.skin.box);
            GUILayout.Label("M1 Member Squad Sandbox");
            GUILayout.Label("Running: " + _isRunning);
            GUILayout.Label("Paused: " + _isPaused);
            GUILayout.Label("Selected Squad: " + _selectedSquadId);

            if (_snapshot == null)
            {
                GUILayout.Label("No snapshot");
                GUILayout.EndArea();
                return;
            }

            GUILayout.Label("Battle: " + _snapshot.BattleId);
            GUILayout.Label("Elapsed: " + _snapshot.ElapsedTimeSeconds.ToString("F2"));

            for (int i = 0; i < _snapshot.Squads.Count; i++)
            {
                BattleSquadSnapshot squad = _snapshot.Squads[i];
                GUILayout.Space(6f);
                GUILayout.Label("Squad " + squad.SquadId + " | members " + squad.AliveMemberCount + "/" + squad.MemberCount);
                GUILayout.Label("Command: " + squad.CurrentCommand + " | stance: " + squad.Stance);
                GUILayout.Label("Center: " + FormatVector(squad.Position) + " -> " + FormatVector(squad.DesiredPosition));
            }

            GUILayout.Space(8f);
            GUILayout.Label("Members");
            for (int i = 0; i < _snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = _snapshot.Members[i];
                GUILayout.Label(
                    "#" + member.MemberId.Value +
                    " squad " + member.SquadId +
                    " hp " + member.Health + "/" + member.MaxHealth +
                    " pos " + FormatVector(member.Position) +
                    " intent " + member.CurrentIntent +
                    " arrived " + member.HasReachedTarget);
            }

            GUILayout.EndArea();
        }

        private static string FormatVector(Vec2 vector)
        {
            return "(" + vector.X.ToString("F2") + ", " + vector.Y.ToString("F2") + ")";
        }
    }
}

