using UnityEngine;

namespace Warzone.Adapters
{
    public sealed class ObstacleVolume : MonoBehaviour
    {
        [SerializeField] private Collider obstacleCollider;

        public Collider ObstacleCollider => obstacleCollider;

        private void Reset()
        {
            obstacleCollider = GetComponent<Collider>();
        }
    }
}
