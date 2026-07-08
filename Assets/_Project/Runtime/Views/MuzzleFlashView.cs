using UnityEngine;

namespace Warzone.Runtime.Views
{
    public sealed class MuzzleFlashView : PooledTransientView
    {
        private float _remainingLifetime;
        private System.Action<MuzzleFlashView> _release;

        public void Initialize(System.Action<MuzzleFlashView> release)
        {
            _release = release;
            _remainingLifetime = 0.08f;
            Activate();
        }

        private void Update()
        {
            _remainingLifetime -= Time.deltaTime;
            if (_remainingLifetime <= 0f)
            {
                _release?.Invoke(this);
                return;
            }

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 12f);
        }
    }
}



