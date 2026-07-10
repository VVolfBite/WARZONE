using System.Collections.Generic;

namespace Warzone.Application.Save
{
    public sealed class InMemorySaveGameRepository : ISaveGameRepository
    {
        private readonly Dictionary<string, string> _slots = new Dictionary<string, string>();

        public bool Save(string slotId, string data, out string reason)
        {
            reason = null;

            if (string.IsNullOrEmpty(slotId))
            {
                reason = "Slot id is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(data))
            {
                reason = "Save data is empty.";
                return false;
            }

            _slots[slotId] = data;
            return true;
        }

        public bool TryLoad(string slotId, out string data, out string reason)
        {
            reason = null;
            data = null;

            if (string.IsNullOrEmpty(slotId))
            {
                reason = "Slot id is required.";
                return false;
            }

            if (!_slots.TryGetValue(slotId, out data) || string.IsNullOrWhiteSpace(data))
            {
                reason = "Slot not found.";
                data = null;
                return false;
            }

            return true;
        }
    }
}
