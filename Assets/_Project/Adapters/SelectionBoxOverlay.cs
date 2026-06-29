using UnityEngine;

namespace Warzone.Adapters
{
    public sealed class SelectionBoxOverlay : MonoBehaviour
    {
        private bool _active;
        private Rect _selectionRect;

        public void SetSelection(Rect selectionRect, bool active)
        {
            _selectionRect = selectionRect;
            _active = active;
        }

        private void OnGUI()
        {
            if (!_active)
            {
                return;
            }

            Color oldColor = GUI.color;
            GUI.color = new Color(0.2f, 0.8f, 1f, 0.2f);
            GUI.DrawTexture(_selectionRect, Texture2D.whiteTexture);
            GUI.color = new Color(0.2f, 0.8f, 1f, 0.9f);
            DrawBorder(_selectionRect, 2f);
            GUI.color = oldColor;
        }

        private static void DrawBorder(Rect rect, float thickness)
        {
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);
        }
    }
}
