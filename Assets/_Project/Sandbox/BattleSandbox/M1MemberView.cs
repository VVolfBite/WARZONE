using UnityEngine;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M1MemberView : MonoBehaviour
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

        public void SetSelected(bool isSelected)
        {
            if (cachedRenderer == null)
            {
                return;
            }

            cachedRenderer.material.color = isSelected ? new Color(0.2f, 0.85f, 1f) : new Color(0.25f, 0.55f, 0.75f);
        }
    }
}

