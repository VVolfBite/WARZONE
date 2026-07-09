using UnityEngine;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class SandboxMemberView : MonoBehaviour
    {
        [SerializeField] private int memberId;
        [SerializeField] private int squadId;
        [SerializeField] private Renderer cachedRenderer;

        public int MemberId
        {
            get { return memberId; }
        }

        public int SquadId
        {
            get { return squadId; }
        }

        public void Initialize(int newMemberId, int newSquadId, Renderer renderer)
        {
            memberId = newMemberId;
            squadId = newSquadId;
            cachedRenderer = renderer;
        }

        public void ApplyState(bool isSelected, bool isDead, bool isExtracted)
        {
            if (cachedRenderer == null)
            {
                return;
            }

            if (isDead)
            {
                cachedRenderer.material.color = new Color(0.18f, 0.18f, 0.18f);
                transform.localScale = new Vector3(0.35f, 0.2f, 0.35f);
                return;
            }

            if (isExtracted)
            {
                cachedRenderer.material.color = isSelected ? new Color(0.7f, 1f, 0.7f) : new Color(0.45f, 0.8f, 0.45f);
                transform.localScale = new Vector3(0.45f, 0.35f, 0.45f);
                return;
            }

            cachedRenderer.material.color = isSelected ? new Color(0.2f, 0.85f, 1f) : new Color(0.25f, 0.55f, 0.75f);
            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }
    }
}
