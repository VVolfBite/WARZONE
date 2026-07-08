using UnityEngine;

namespace Warzone.Runtime.Views
{
    public sealed class CommandMarkerView : PooledTransientView
    {
        [SerializeField] private float lifetimeSeconds = 0.75f;

        private float _timeRemaining;
        private System.Action<CommandMarkerView> _release;

        public void Initialize(System.Action<CommandMarkerView> release)
        {
            _release = release;
            _timeRemaining = lifetimeSeconds;
            Activate();
        }

        private void Update()
        {
            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining <= 0f)
            {
                _release?.Invoke(this);
                return;
            }

            transform.Rotate(Vector3.up, 180f * Time.deltaTime, Space.World);
        }
    }
}



