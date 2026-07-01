using UnityEngine;

namespace Warzone.Adapters
{
    public sealed class DamageNumberView : MonoBehaviour
    {
        private Camera _camera;
        private string _text;
        private Color _color;
        private float _remainingLifetime;
        private Vector3 _velocity;

        public void Initialize(Camera camera, string text, Color color)
        {
            _camera = camera;
            _text = text;
            _color = color;
            _remainingLifetime = 0.9f;
            _velocity = new Vector3(Random.Range(-0.2f, 0.2f), 0.9f, 0f);
        }

        private void Update()
        {
            _remainingLifetime -= Time.deltaTime;
            if (_remainingLifetime <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            transform.position += _velocity * Time.deltaTime;
        }

        private void OnGUI()
        {
            if (_camera == null)
            {
                return;
            }

            Vector3 screenPosition = _camera.WorldToScreenPoint(transform.position);
            if (screenPosition.z <= 0f)
            {
                return;
            }

            Color previous = GUI.color;
            GUI.color = new Color(_color.r, _color.g, _color.b, Mathf.Clamp01(_remainingLifetime));
            Rect rect = new Rect(screenPosition.x - 18f, Screen.height - screenPosition.y - 10f, 36f, 20f);
            GUI.Label(rect, _text, BuildStyle());
            GUI.color = previous;
        }

        private static GUIStyle BuildStyle()
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 16;
            return style;
        }
    }
}
