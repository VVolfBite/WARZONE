using UnityEngine;

namespace Warzone.Controls
{
    public sealed class DebriefScreen : MonoBehaviour, IDebriefScreen
    {
        private DebriefViewModel _viewModel;

        public void Show(DebriefViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        private void OnGUI()
        {
            if (_viewModel == null)
            {
                return;
            }

            GUILayout.BeginArea(new Rect(Screen.width - 280f, 20f, 260f, 140f), GUI.skin.box);
            GUILayout.Label(_viewModel.IsVictory ? "Victory" : "Defeat");
            GUILayout.Label($"Units Lost: {_viewModel.UnitsLost}");
            GUILayout.Label($"Units Kept: {_viewModel.UnitsKept}");
            GUILayout.Label($"Elapsed: {_viewModel.ElapsedTimeSeconds:F1}s");
            GUILayout.EndArea();
        }
    }
}
