using System.Collections.Generic;
using UnityEngine;
using Warzone.Combat;

namespace Warzone.Runtime.Views
{
    public sealed class BattleEntityViewRegistry : MonoBehaviour
    {
        private readonly Dictionary<BattleEntityId, UnitEntityBinder> _binders = new Dictionary<BattleEntityId, UnitEntityBinder>();

        public void Register(UnitEntityBinder binder)
        {
            _binders[binder.EntityId] = binder;
        }

        public bool TryGet(BattleEntityId entityId, out UnitEntityBinder binder)
        {
            return _binders.TryGetValue(entityId, out binder);
        }

        public void Unregister(UnitEntityBinder binder)
        {
            if (binder == null)
            {
                return;
            }

            if (_binders.TryGetValue(binder.EntityId, out UnitEntityBinder existing) && existing == binder)
            {
                _binders.Remove(binder.EntityId);
            }
        }
    }
}



