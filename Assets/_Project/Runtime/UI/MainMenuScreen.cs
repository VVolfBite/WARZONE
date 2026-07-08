using UnityEngine;

namespace Warzone.Runtime.UI
{
    public sealed class MainMenuScreen : MonoBehaviour, IMainMenuScreen
    {
        private MainMenuViewModel _viewModel;
        private MainMenuScreenController _controller;
        private bool _showSettings;
        private float _masterVolume = 0.8f;
        private float _musicVolume = 0.7f;
        private int _graphicsQuality = 2;

        public void Configure(MainMenuScreenController controller)
        {
            _controller = controller;
            Bind(controller.BuildViewModel());
        }

        public void Bind(MainMenuViewModel viewModel)
        {
            _viewModel = viewModel;
            _masterVolume = viewModel.MasterVolume;
            _musicVolume = viewModel.MusicVolume;
            _graphicsQuality = viewModel.GraphicsQuality;
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
            GUILayout.Space(4f);
            GUILayout.Label(_viewModel.BestScoreSummary, BuildInfoStyle());
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(_viewModel.PrimaryActionLabel, GUILayout.Height(42f)))
            {
                _controller.StartDemo();
            }

            if (GUILayout.Button("Settings", GUILayout.Height(36f)))
            {
                _showSettings = !_showSettings;
            }

            if (GUILayout.Button(_viewModel.TertiaryActionLabel, GUILayout.Height(36f)))
            {
                _showSettings = false;
            }

            if (GUILayout.Button(_viewModel.SecondaryActionLabel, GUILayout.Height(36f)))
            {
                UnityEngine.Application.Quit();
            }

            if (_showSettings)
            {
                GUILayout.Space(12f);
                GUILayout.Label("Settings", BuildSectionStyle());
                GUILayout.Label("Audio / Graphics / Controls");
                GUILayout.Label($"Master Volume: {_masterVolume:0.00}");
                _masterVolume = GUILayout.HorizontalSlider(_masterVolume, 0f, 1f);
                GUILayout.Label($"Music Volume: {_musicVolume:0.00}");
                _musicVolume = GUILayout.HorizontalSlider(_musicVolume, 0f, 1f);
                GUILayout.Label($"Graphics Quality: {_graphicsQuality}");
                _graphicsQuality = Mathf.RoundToInt(GUILayout.HorizontalSlider(_graphicsQuality, 0f, 4f));
                GUILayout.Label("Controls: WASD pan / Edge pan / MMB rotate / RMB command");
                if (GUILayout.Button("Apply", GUILayout.Height(30f)))
                {
                    _controller.SaveSettings(_masterVolume, _musicVolume, _graphicsQuality);
                }
                GUILayout.Space(8f);
                GUILayout.Label("Credits", BuildSectionStyle());
                GUILayout.Label("Warzone RTS prototype by project workspace.");
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

        private static GUIStyle BuildInfoStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 12;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = new Color(0.76f, 0.8f, 0.86f);
            return style;
        }
    }
}



