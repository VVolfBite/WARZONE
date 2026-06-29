using UnityEngine;

namespace Warzone.Adapters
{
    public sealed class CommandMarkerView : MonoBehaviour
    {
        [SerializeField] private float lifetimeSeconds = 0.75f;

        private float _timeRemaining;

        private void OnEnable()
        {
            _timeRemaining = lifetimeSeconds;
        }

        private void Update()
        {
            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            transform.Rotate(Vector3.up, 180f * Time.deltaTime, Space.World);
        }
    }
}
