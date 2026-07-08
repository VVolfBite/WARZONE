using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class BattleCommandQueue
    {
        private readonly Queue<BattleCommand> _pendingCommands = new Queue<BattleCommand>();

        public int Count
        {
            get { return _pendingCommands.Count; }
        }

        public void Enqueue(BattleCommand command)
        {
            if (command == null)
            {
                return;
            }

            _pendingCommands.Enqueue(command);
        }

        public bool TryDequeue(out BattleCommand command)
        {
            if (_pendingCommands.Count > 0)
            {
                command = _pendingCommands.Dequeue();
                return true;
            }

            command = null;
            return false;
        }
    }
}

