using System;
using UnityEngine;

namespace Warzone.Sandbox.UI
{
    public sealed class SandboxHudOverlay : MonoBehaviour
    {
        private SandboxHudSnapshot _snapshot;
        private bool _minimapClicked;
        private Action _resumeAction;
        private Action _restartAction;
        private Action _returnToMenuAction;

        public void Bind(SandboxHudSnapshot snapshot)
        {
            _snapshot = snapshot;
        }

        public void SetPauseActions(Action resumeAction, Action restartAction, Action returnToMenuAction)
        {
            _resumeAction = resumeAction;
            _restartAction = restartAction;
            _returnToMenuAction = returnToMenuAction;
        }

        private void DrawTopBar()
        {
            GUILayout.BeginArea(new Rect(20f, 20f, Screen.width - 40f, 120f), GUI.skin.box);
            GUILayout.Label("WARZONE");
            GUILayout.Label(_snapshot != null ? _snapshot.ObjectiveText : "Objective");
            GUILayout.Label(_snapshot != null && _snapshot.IsPaused ? "Paused" : "Running");
            GUILayout.Label(_snapshot != null ? _snapshot.SpeedText : "Speed x1");
            GUILayout.EndArea();
        }

        private void DrawObjectivePanel()
        {
            GUILayout.BeginArea(new Rect(Screen.width - 310f, 140f, 290f, 120f), GUI.skin.box);
            GUILayout.Label("Mission");
            GUILayout.Label($"Wave: {(_snapshot != null ? _snapshot.ActiveWaveIndex : 0)}/{(_snapshot != null ? _snapshot.TotalWaveCount : 0)}");
            GUILayout.Label(_snapshot != null ? _snapshot.ObjectiveText : "Objective");
            GUILayout.EndArea();
        }

        private void DrawTeamBar()
        {
            GUILayout.BeginArea(new Rect((Screen.width - 620f) * 0.5f, Screen.height - 95f, 620f, 75f), GUI.skin.box);
            GUILayout.Label("Team");
            GUILayout.BeginHorizontal();
            if (_snapshot != null)
            {
                for (int i = 0; i < _snapshot.TeamSlots.Count; i++)
                {
                    SandboxTeamSlotSnapshot slot = _snapshot.TeamSlots[i];
                    Color previous = GUI.color;
                    GUI.color = slot.IsActive ? new Color(0.3f, 0.95f, 0.65f) : (slot.BoundCount > 0 ? new Color(0.85f, 0.85f, 0.45f) : Color.white);
                    GUILayout.Box((slot.SlotIndex + 1).ToString() + "\n" + slot.BoundCount, GUILayout.Width(52f), GUILayout.Height(42f));
                    GUI.color = previous;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void DrawMinimap()
        {
            GUILayout.BeginArea(new Rect(Screen.width - 210f, Screen.height - 210f, 190f, 190f), GUI.skin.box);
            GUILayout.Label("Minimap");
            Rect mapRect = new Rect(12f, 30f, 166f, 146f);
            GUI.Box(mapRect, GUIContent.none);

            Event current = Event.current;
            if (current != null && current.type == EventType.MouseDown && mapRect.Contains(current.mousePosition))
            {
                Vector2 normalized = new Vector2(
                    Mathf.InverseLerp(mapRect.x, mapRect.xMax, current.mousePosition.x),
                    1f - Mathf.InverseLerp(mapRect.y, mapRect.yMax, current.mousePosition.y));
                if (_snapshot != null)
                {
                    _snapshot.MinimapJumpNormalized = normalized;
                    _minimapClicked = true;
                }

                current.Use();
            }

            if (_snapshot != null)
            {
                for (int i = 0; i < _snapshot.MinimapDots.Count; i++)
                {
                    SandboxMinimapDot dot = _snapshot.MinimapDots[i];
                    Rect dotRect = new Rect(
                        mapRect.x + (dot.NormalizedPosition.x * (mapRect.width - 8f)),
                        mapRect.y + ((1f - dot.NormalizedPosition.y) * (mapRect.height - 8f)),
                        dot.IsSelected ? 8f : 6f,
                        dot.IsSelected ? 8f : 6f);
                    Color previous = GUI.color;
                    GUI.color = dot.Color;
                    GUI.DrawTexture(dotRect, Texture2D.whiteTexture);
                    GUI.color = previous;
                }
            }
            GUILayout.EndArea();
        }

        public bool TryConsumeMinimapJump(out Vector2 normalizedPosition)
        {
            if (_snapshot != null && _minimapClicked && _snapshot.MinimapJumpNormalized.HasValue)
            {
                normalizedPosition = _snapshot.MinimapJumpNormalized.Value;
                _minimapClicked = false;
                _snapshot.MinimapJumpNormalized = null;
                return true;
            }

            normalizedPosition = default;
            return false;
        }

        private void DrawControlHint()
        {
            GUILayout.BeginArea(new Rect(20f, Screen.height - 220f, 260f, 90f), GUI.skin.box);
            GUILayout.Label("LMB select, RMB move/attack");
            GUILayout.Label("Shift queue, Ctrl toggle, Q ability");
            GUILayout.Label("P pause, +/- speed");
            GUILayout.EndArea();
        }

        private void DrawNotificationBanner()
        {
            if (_snapshot == null || string.IsNullOrEmpty(_snapshot.NotificationText))
            {
                return;
            }

            string[] lines = _snapshot.NotificationText.Split('\n');
            string latest = lines.Length > 0 ? lines[lines.Length - 1] : _snapshot.NotificationText;
            GUILayout.BeginArea(new Rect((Screen.width - 420f) * 0.5f, 20f, 420f, 42f), GUI.skin.box);
            GUILayout.Label(latest);
            GUILayout.EndArea();
        }

        private void DrawDebugPanel()
        {
            GUILayout.BeginArea(new Rect(20f, 140f, 340f, 120f), GUI.skin.box);
            GUILayout.Label("Debug");
            GUILayout.Label(_snapshot != null ? _snapshot.NotificationText : string.Empty);
            GUILayout.Label(_snapshot != null ? _snapshot.DebugText : string.Empty);
            GUILayout.EndArea();
        }

        private void DrawPauseMenu()
        {
            GUILayout.BeginArea(new Rect((Screen.width - 320f) * 0.5f, (Screen.height - 220f) * 0.5f, 320f, 220f), GUI.skin.box);
            GUILayout.Label("MISSION PAUSED");

            if (GUILayout.Button("Resume", GUILayout.Height(34f)))
            {
                _resumeAction?.Invoke();
            }

            if (GUILayout.Button("Restart Mission", GUILayout.Height(34f)))
            {
                _restartAction?.Invoke();
            }

            if (GUILayout.Button("Main Menu", GUILayout.Height(34f)))
            {
                _returnToMenuAction?.Invoke();
            }

            GUILayout.EndArea();
        }

        private void OnGUI()
        {
            DrawTopBar();
            DrawNotificationBanner();
            DrawObjectivePanel();
            DrawTeamBar();
            DrawMinimap();
            DrawControlHint();
            DrawDebugPanel();

            if (_snapshot != null && _snapshot.IsPaused)
            {
                DrawPauseMenu();
            }
        }
    }
}



