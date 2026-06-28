using UnityEngine;
using Warzone.BattleDomain;

namespace Warzone.UnityAdapters
{
    public sealed class UnitEntityBinder : MonoBehaviour
    {
        [SerializeField] private int entityId;
        private BattleEntityViewRegistry _registry;

        public BattleEntityId EntityId => new BattleEntityId(entityId);
        public Transform VisualRoot => transform;

        public void Bind(BattleEntityId battleEntityId)
        {
            entityId = battleEntityId.Value;
        }

        public void AttachRegistry(BattleEntityViewRegistry registry)
        {
            _registry = registry;
            if (_registry != null)
            {
                _registry.Register(this);
            }
        }

        private void OnDestroy()
        {
            if (_registry != null)
            {
                _registry.Unregister(this);
            }
        }
    }
}
