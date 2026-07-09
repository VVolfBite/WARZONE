using UnityEngine;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class SandboxEnemyView : MonoBehaviour
    {
        [SerializeField] private int enemyId;
        [SerializeField] private Renderer cachedRenderer;
        [SerializeField] private bool isDead;

        public int EnemyId
        {
            get { return enemyId; }
        }

        public void Initialize(int newEnemyId, Renderer renderer)
        {
            enemyId = newEnemyId;
            cachedRenderer = renderer;
            isDead = false;
        }

        public void ApplyState(bool alive)
        {
            if (cachedRenderer == null)
            {
                return;
            }

            if (alive)
            {
                cachedRenderer.material.color = new Color(0.76f, 0.28f, 0.24f);
                transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                isDead = false;
                return;
            }

            if (isDead)
            {
                return;
            }

            cachedRenderer.material.color = new Color(0.22f, 0.22f, 0.22f);
            transform.localScale = new Vector3(0.45f, 0.2f, 0.45f);
            isDead = true;
        }
    }
}
