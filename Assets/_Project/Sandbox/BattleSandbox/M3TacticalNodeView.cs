using UnityEngine;
using Warzone.Combat;

namespace Warzone.Sandbox.BattleSandbox
{
    public class M3TacticalNodeView : MonoBehaviour
    {
        [SerializeField] private int nodeId;
        [SerializeField] private TacticalNodeType nodeType;
        [SerializeField] private Renderer cachedRenderer;

        public int NodeId
        {
            get { return nodeId; }
        }

        public void Initialize(int newNodeId, TacticalNodeType newNodeType, Renderer renderer)
        {
            nodeId = newNodeId;
            nodeType = newNodeType;
            cachedRenderer = renderer;
        }

        public void ApplySnapshot(TacticalNodeSnapshot snapshot)
        {
            if (cachedRenderer == null || snapshot == null)
            {
                return;
            }

            switch (nodeType)
            {
                case TacticalNodeType.Cover:
                    cachedRenderer.material.color = snapshot.OccupyingMemberId.HasValue ? new Color(0.6f, 0.72f, 0.84f) : new Color(0.45f, 0.52f, 0.58f);
                    break;
                case TacticalNodeType.DefensivePosition:
                    cachedRenderer.material.color = snapshot.OccupyingMemberId.HasValue ? new Color(0.74f, 0.83f, 0.55f) : new Color(0.55f, 0.66f, 0.35f);
                    break;
                case TacticalNodeType.SearchPoint:
                    cachedRenderer.material.color = snapshot.IsSearched ? new Color(0.18f, 0.6f, 0.22f) : new Color(0.75f, 0.48f, 0.22f);
                    break;
                case TacticalNodeType.ExtractionPoint:
                    cachedRenderer.material.color = new Color(0.22f, 0.8f, 0.48f);
                    break;
                case TacticalNodeType.EnemyIngress:
                    cachedRenderer.material.color = new Color(0.8f, 0.2f, 0.18f);
                    break;
                default:
                    cachedRenderer.material.color = new Color(0.7f, 0.7f, 0.35f);
                    break;
            }
        }
    }
}
