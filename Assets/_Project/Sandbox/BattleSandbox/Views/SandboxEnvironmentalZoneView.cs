using UnityEngine;
using Warzone.Combat;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class SandboxEnvironmentalZoneView : MonoBehaviour
    {
        private Renderer _renderer;
        private int _zoneId;
        private EnvironmentalZoneType _zoneType;

        public void Initialize(int zoneId, EnvironmentalZoneType zoneType, Renderer renderer)
        {
            _zoneId = zoneId;
            _zoneType = zoneType;
            _renderer = renderer;
        }

        public void ApplySnapshot(EnvironmentalZoneSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            transform.position = new Vector3(snapshot.Position.X, 0.05f, snapshot.Position.Y);
            transform.localScale = new Vector3(snapshot.Radius * 2f, 0.1f, snapshot.Radius * 2f);

            if (_renderer != null)
            {
                _renderer.material.color = BuildColor(snapshot.ZoneType, snapshot.IsActive);
            }
        }

        private static Color BuildColor(EnvironmentalZoneType zoneType, bool isActive)
        {
            Color color;
            switch (zoneType)
            {
                case EnvironmentalZoneType.Smoke:
                    color = new Color(0.55f, 0.55f, 0.6f, 0.7f);
                    break;
                case EnvironmentalZoneType.Fire:
                    color = new Color(0.95f, 0.4f, 0.1f, 0.8f);
                    break;
                case EnvironmentalZoneType.Toxic:
                    color = new Color(0.25f, 0.7f, 0.2f, 0.75f);
                    break;
                case EnvironmentalZoneType.Light:
                    color = new Color(0.95f, 0.9f, 0.2f, 0.65f);
                    break;
                case EnvironmentalZoneType.Darkness:
                    color = new Color(0.15f, 0.15f, 0.2f, 0.8f);
                    break;
                default:
                    color = Color.white;
                    break;
            }

            if (!isActive)
            {
                color *= 0.45f;
            }

            return color;
        }
    }
}
