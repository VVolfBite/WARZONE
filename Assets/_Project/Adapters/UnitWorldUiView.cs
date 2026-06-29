using UnityEngine;

namespace Warzone.Adapters
{
    public sealed class UnitWorldUiView : MonoBehaviour
    {
        private Camera _camera;
        private float _healthNormalized = 1f;
        private bool _showRange;
        private float _rangeRadius;
        private Color _rangeColor;
        private LineRenderer _rangeRenderer;

        public void Initialize(Camera camera)
        {
            _camera = camera;
            EnsureRangeRenderer();
        }

        public void SetHealth(float healthNormalized)
        {
            _healthNormalized = Mathf.Clamp01(healthNormalized);
        }

        public void SetRangeVisible(bool visible, float radius, Color color)
        {
            _showRange = visible;
            _rangeRadius = radius;
            _rangeColor = color;
            EnsureRangeRenderer();
            _rangeRenderer.enabled = visible;
            if (visible)
            {
                UpdateRangeRenderer();
            }
        }

        private void OnGUI()
        {
            if (_camera == null)
            {
                return;
            }

            Vector3 screenPosition = _camera.WorldToScreenPoint(transform.position + (Vector3.up * 1.8f));
            if (screenPosition.z <= 0f)
            {
                return;
            }

            Rect backRect = new Rect(screenPosition.x - 22f, Screen.height - screenPosition.y, 44f, 6f);
            Color previous = GUI.color;
            GUI.color = Color.black;
            GUI.DrawTexture(backRect, Texture2D.whiteTexture);
            GUI.color = Color.Lerp(Color.red, Color.green, _healthNormalized);
            GUI.DrawTexture(new Rect(backRect.x + 1f, backRect.y + 1f, (backRect.width - 2f) * _healthNormalized, backRect.height - 2f), Texture2D.whiteTexture);
            GUI.color = previous;
        }

        private void LateUpdate()
        {
            if (_showRange && _rangeRenderer != null)
            {
                UpdateRangeRenderer();
            }
        }

        private void EnsureRangeRenderer()
        {
            if (_rangeRenderer != null)
            {
                return;
            }

            GameObject ringObject = new GameObject("AttackRangeRing");
            ringObject.transform.SetParent(transform, false);
            ringObject.transform.localPosition = Vector3.up * 0.05f;
            _rangeRenderer = ringObject.AddComponent<LineRenderer>();
            _rangeRenderer.useWorldSpace = false;
            _rangeRenderer.loop = true;
            _rangeRenderer.positionCount = 48;
            _rangeRenderer.startWidth = 0.06f;
            _rangeRenderer.endWidth = 0.06f;
            _rangeRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _rangeRenderer.receiveShadows = false;
            _rangeRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _rangeRenderer.enabled = false;
        }

        private void UpdateRangeRenderer()
        {
            if (_rangeRenderer == null)
            {
                return;
            }

            _rangeRenderer.startColor = _rangeColor;
            _rangeRenderer.endColor = _rangeColor;
            for (int i = 0; i < _rangeRenderer.positionCount; i++)
            {
                float angle = ((float)i / _rangeRenderer.positionCount) * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * _rangeRadius;
                float z = Mathf.Sin(angle) * _rangeRadius;
                _rangeRenderer.SetPosition(i, new Vector3(x, 0f, z));
            }
        }
    }
}
