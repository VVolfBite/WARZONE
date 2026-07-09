using UnityEngine;
using Warzone.Combat;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M4ObstacleView : MonoBehaviour
    {
        [SerializeField] private int obstacleId;
        [SerializeField] private TacticalObstacleType obstacleType;
        [SerializeField] private Renderer cachedRenderer;

        public int ObstacleId
        {
            get { return obstacleId; }
        }

        public void Initialize(int newObstacleId, TacticalObstacleType newObstacleType, Renderer renderer)
        {
            obstacleId = newObstacleId;
            obstacleType = newObstacleType;
            cachedRenderer = renderer;
            ApplyColor();
        }

        private void ApplyColor()
        {
            if (cachedRenderer == null)
            {
                return;
            }

            switch (obstacleType)
            {
                case TacticalObstacleType.LowCover:
                    cachedRenderer.material.color = new Color(0.56f, 0.48f, 0.32f);
                    break;
                case TacticalObstacleType.HighCover:
                    cachedRenderer.material.color = new Color(0.38f, 0.38f, 0.42f);
                    break;
                case TacticalObstacleType.Wall:
                case TacticalObstacleType.BuildingBlocker:
                    cachedRenderer.material.color = new Color(0.28f, 0.28f, 0.3f);
                    break;
                case TacticalObstacleType.Window:
                    cachedRenderer.material.color = new Color(0.35f, 0.75f, 0.95f);
                    break;
                default:
                    cachedRenderer.material.color = new Color(0.55f, 0.55f, 0.55f);
                    break;
            }
        }
    }
}
