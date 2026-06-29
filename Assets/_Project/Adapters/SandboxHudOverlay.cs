using System.Text;
using UnityEngine;

namespace Warzone.Adapters
{
    public sealed class SandboxHudOverlay : MonoBehaviour
    {
        private readonly StringBuilder _builder = new StringBuilder(512);
        private bool _isPaused;
        private int _activeWaveIndex;
        private int _totalWaveCount;
        private string _objectiveText;
        private string _notificationText;

        public void Bind(bool isPaused, int activeWaveIndex, int totalWaveCount, string objectiveText, string notificationText)
        {
            _isPaused = isPaused;
            _activeWaveIndex = activeWaveIndex;
            _totalWaveCount = totalWaveCount;
            _objectiveText = objectiveText;
            _notificationText = notificationText;
        }

        private void OnGUI()
        {
            DrawTopBar();
            DrawObjectivePanel();
            DrawTeamBar();
            DrawMinimap();
            DrawControlHint();
        }

        private void DrawTopBar()
        {
            GUILayout.BeginArea(new Rect(20f, 20f, Screen.width - 40f, 110f), GUI.skin.box);
            GUILayout.Label("WARZONE POC2");
            GUILayout.Label(_objectiveText ?? "Objective");
            GUILayout.Label(_notificationText ?? string.Empty);
            GUILayout.Label(_isPaused ? "Paused" : "Running");
            GUILayout.EndArea();
        }

        private void DrawObjectivePanel()
        {
            GUILayout.BeginArea(new Rect(Screen.width - 310f, 140f, 290f, 120f), GUI.skin.box);
            GUILayout.Label("Mission");
            GUILayout.Label($"Wave: {_activeWaveIndex}/{_totalWaveCount}");
            GUILayout.Label(_objectiveText ?? "Objective");
            GUILayout.EndArea();
        }

        private void DrawTeamBar()
        {
            GUILayout.BeginArea(new Rect(20f, Screen.height - 120f, 420f, 100f), GUI.skin.box);
            GUILayout.Label("Team");
            GUILayout.Label("1-4: squads");
            GUILayout.Label("Ctrl+1: bind team");
            GUILayout.EndArea();
        }

        private void DrawMinimap()
        {
            GUILayout.BeginArea(new Rect(Screen.width - 210f, Screen.height - 210f, 190f, 190f), GUI.skin.box);
            GUILayout.Label("Minimap");
            GUILayout.Label("Demo view");
            GUILayout.EndArea();
        }

        private void DrawControlHint()
        {
            GUILayout.BeginArea(new Rect(20f, Screen.height - 220f, 260f, 90f), GUI.skin.box);
            GUILayout.Label("LMB select, RMB move/attack");
            GUILayout.Label("Shift queue, Ctrl toggle");
            GUILayout.Label("P pause, +/- speed");
            GUILayout.EndArea();
        }
    }
}
