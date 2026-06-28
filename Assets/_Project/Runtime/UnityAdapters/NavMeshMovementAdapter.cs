using UnityEngine;
using UnityEngine.AI;

namespace Warzone.UnityAdapters
{
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class NavMeshMovementAdapter : MonoBehaviour
    {
        private NavMeshAgent _agent;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void MoveTo(Vector3 destination)
        {
            _agent.SetDestination(destination);
        }
    }
}
