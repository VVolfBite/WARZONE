using UnityEngine;

namespace Warzone.Frontend
{
    public sealed class MainMenuScreen : MonoBehaviour
    {
        private System.Action _onPlayDemo;

        public void Bind(System.Action onPlayDemo)
        {
            _onPlayDemo = onPlayDemo;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect((Screen.width * 0.5f) - 140f, (Screen.height * 0.5f) - 90f, 280f, 180f), GUI.skin.box);
            GUILayout.Label("Warzone Prototype");
            GUILayout.Space(12f);
            GUILayout.Label("Small-scale RTS sandbox");
            GUILayout.Space(18f);
            if (GUILayout.Button("Play Demo", GUILayout.Height(42f)))
            {
                _onPlayDemo?.Invoke();
            }

            GUILayout.Space(8f);
            GUILayout.Label("WASD / Edge Pan / Mouse Wheel / MMB Rotate");
            GUILayout.EndArea();
        }
    }
}
