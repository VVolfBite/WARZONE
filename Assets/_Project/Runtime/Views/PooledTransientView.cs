using UnityEngine;

namespace Warzone.Runtime.Views
{
    public abstract class PooledTransientView : MonoBehaviour
    {
        public bool IsActive { get; private set; }

        public virtual void Activate()
        {
            IsActive = true;
            gameObject.SetActive(true);
        }

        public virtual void Deactivate()
        {
            IsActive = false;
            gameObject.SetActive(false);
        }
    }
}



