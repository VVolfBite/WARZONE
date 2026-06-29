using UnityEngine;

namespace Warzone.Controls
{
    public sealed class MainMenuScreen : MonoBehaviour, IMainMenuScreen
    {
        private MainMenuViewModel _viewModel;
        private MainMenuScreenController _controller;
        private bool _showSettings;

        public void Configure(MainMenuScreenController controller)
        {
            _controller = controller;
            Bind(controller.BuildViewModel());
        }

        public void Bind(MainMenuViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void OnGUI()
        {
            if (_viewModel == null)
            {
                return;
            }

            float width = 360f;
            float height = 280f;
            Rect area = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);

            GUILayout.BeginArea(area, GUI.skin.box);
            GUILayout.Space(12f);
            GUILayout.Label(_viewModel.Title, BuildTitleStyle());
            GUILayout.Space(8f);
            GUILayout.Label(_viewModel.Subtitle, BuildSubtitleStyle());
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(_viewModel.PrimaryActionLabel, GUILayout.Height(42f)))
            {
                _controller.StartDemo();
            }

            if (GUILayout.Button("Settings", GUILayout.Height(36f)))
            {
                _showSettings = !_showSettings;
            }

            if (GUILayout.Button(_viewModel.SecondaryActionLabel, GUILayout.Height(36f)))
            {
                Application.Quit();
            }

            if (_showSettings)
            {
                GUILayout.Space(12f);
                GUILayout.Label("Settings", BuildSectionStyle());
                GUILayout.Label("Audio / Graphics / Controls");
                GUILayout.Label("These options are placeholders in POC3.");
            }

            GUILayout.Space(10f);
            GUILayout.EndArea();
        }

        private static GUIStyle BuildTitleStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 34;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            return style;
        }

        private static GUIStyle BuildSubtitleStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 16;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = new Color(0.8f, 0.85f, 0.9f);
            return style;
        }

        private static GUIStyle BuildSectionStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 14;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleLeft;
            return style;
        }
    }
}
