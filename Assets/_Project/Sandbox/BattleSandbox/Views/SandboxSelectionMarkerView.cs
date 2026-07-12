using UnityEngine;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class SandboxSelectionMarkerView : MonoBehaviour
    {
        [SerializeField] private int squadId;
        [SerializeField] private Renderer cachedRenderer;

        public int SquadId
        {
            get { return squadId; }
        }

        public void Initialize(int newSquadId, Renderer renderer)
        {
            squadId = newSquadId;
            cachedRenderer = renderer;
        }

        public void ApplySelected(bool isSelected)
        {
            if (cachedRenderer == null)
            {
                return;
            }

            cachedRenderer.material.color = isSelected ? new Color(0.95f, 0.95f, 0.25f) : new Color(0.65f, 0.65f, 0.2f);
            transform.localScale = isSelected ? new Vector3(1.05f, 0.08f, 1.05f) : new Vector3(0.75f, 0.08f, 0.75f);
        }
    }
}
