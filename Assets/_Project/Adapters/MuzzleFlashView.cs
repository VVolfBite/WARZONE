using UnityEngine;

namespace Warzone.Adapters
{
    public sealed class MuzzleFlashView : MonoBehaviour
    {
        private float _remainingLifetime;

        private void OnEnable()
        {
            _remainingLifetime = 0.08f;
        }

        private void Update()
        {
            _remainingLifetime -= Time.deltaTime;
            if (_remainingLifetime <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 12f);
        }
    }
}
