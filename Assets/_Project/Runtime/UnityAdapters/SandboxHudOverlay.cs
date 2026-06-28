using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Warzone.BattleDomain;
using Warzone.Content;
using Warzone.Content.Definitions;

namespace Warzone.UnityAdapters
{
    public sealed class SandboxHudOverlay : MonoBehaviour
    {
        private readonly StringBuilder _builder = new StringBuilder(256);
        private bool _isPaused;

        public void Bind(bool isPaused)
        {
            _isPaused = isPaused;
        }

        private void OnGUI()
        {
            _builder.Length = 0;
            _builder.AppendLine("Sandbox HUD");
            _builder.AppendLine("LMB: select / drag box");
            _builder.AppendLine("RMB: move / attack");
            _builder.AppendLine("Shift: add select + queue command");
            _builder.AppendLine("Ctrl: toggle selection");
            _builder.AppendLine("P: pause battle time");
            _builder.Append("State: ");
            _builder.AppendLine(_isPaused ? "Paused" : "Running");

            GUILayout.BeginArea(new Rect(20f, 20f, 320f, 140f), GUI.skin.box);
            GUILayout.Label(_builder.ToString());
            GUILayout.EndArea();
        }
    }
}
