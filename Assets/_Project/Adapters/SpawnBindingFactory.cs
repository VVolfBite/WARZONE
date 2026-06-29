using UnityEngine;
using Warzone.Combat;

namespace Warzone.Adapters
{
    public sealed class SpawnBindingFactory : MonoBehaviour
    {
        [SerializeField] private UnitEntityBinder unitPrefab;

        public UnitEntityBinder Spawn(BattleEntityId entityId, Vector3 position)
        {
            UnitEntityBinder binder = Instantiate(unitPrefab, position, Quaternion.identity);
            binder.Bind(entityId);
            return binder;
        }
    }
}
