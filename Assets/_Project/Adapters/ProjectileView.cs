using UnityEngine;

namespace Warzone.Adapters
{
    public sealed class ProjectileView : MonoBehaviour
    {
        private Vector3 _targetPosition;
        private float _speed;

        public void Launch(Vector3 startPosition, Vector3 targetPosition, float speed, Color color)
        {
            transform.position = startPosition;
            _targetPosition = targetPosition;
            _speed = Mathf.Max(0.1f, speed);

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }

        private void Update()
        {
            Vector3 toTarget = _targetPosition - transform.position;
            float step = _speed * Time.deltaTime;
            if (toTarget.sqrMagnitude <= step * step)
            {
                transform.position = _targetPosition;
                Destroy(gameObject);
                return;
            }

            transform.position += toTarget.normalized * step;
        }
    }
}
