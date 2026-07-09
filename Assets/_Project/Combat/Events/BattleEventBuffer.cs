using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class BattleEventBuffer
    {
        private readonly List<BattleEventRecord> _events = new List<BattleEventRecord>();

        public IReadOnlyList<BattleEventRecord> Events
        {
            get { return _events; }
        }

        public IReadOnlyList<BattleEventRecord> Peek()
        {
            return _events;
        }

        public void Add(BattleEventRecord battleEvent)
        {
            if (battleEvent == null)
            {
                return;
            }

            _events.Add(battleEvent);
        }

        public void Clear()
        {
            _events.Clear();
        }

        [System.Obsolete("Drain is reserved for lifecycle control paths. Systems should observe Events/Peek instead.")]
        public IReadOnlyList<BattleEventRecord> Drain()
        {
            if (_events.Count == 0)
            {
                return new List<BattleEventRecord>();
            }

            List<BattleEventRecord> snapshot = new List<BattleEventRecord>(_events);
            _events.Clear();
            return snapshot;
        }
    }
}

