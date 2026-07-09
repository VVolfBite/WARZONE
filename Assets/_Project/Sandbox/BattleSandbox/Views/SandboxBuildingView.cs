using UnityEngine;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class SandboxBuildingView : MonoBehaviour
    {
        [SerializeField] private int buildingId;
        [SerializeField] private Renderer cachedRenderer;

        public int BuildingId
        {
            get { return buildingId; }
        }

        public void Initialize(int newBuildingId, Renderer renderer)
        {
            buildingId = newBuildingId;
            cachedRenderer = renderer;
            if (cachedRenderer != null)
            {
                cachedRenderer.material.color = new Color(0.44f, 0.42f, 0.4f);
            }
        }
    }
}
