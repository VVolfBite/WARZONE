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

        public void ApplyState(bool isDead, bool isExtracted, bool isSuppressed, bool isBrokenOrRetreating)
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
                cachedRenderer.material.color = new Color(0.45f, 0.8f, 0.45f);
                transform.localScale = new Vector3(0.45f, 0.35f, 0.45f);
                return;
            }

            if (isBrokenOrRetreating)
            {
                cachedRenderer.material.color = new Color(0.92f, 0.48f, 0.18f);
                transform.localScale = new Vector3(0.52f, 0.52f, 0.52f);
                return;
            }

            if (isSuppressed)
            {
                cachedRenderer.material.color = new Color(0.82f, 0.74f, 0.2f);
                transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
                return;
            }

            cachedRenderer.material.color = new Color(0.25f, 0.55f, 0.75f);
            transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        }
    }
}
