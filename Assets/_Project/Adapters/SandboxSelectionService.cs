using System.Collections.Generic;

namespace Warzone.Adapters
{
    public sealed class SandboxSelectionService
    {
        private readonly HashSet<int> _selectedSquadIds = new HashSet<int>();

        public IReadOnlyCollection<int> SelectedSquadIds => _selectedSquadIds;
        public int? HoveredEnemySquadId { get; private set; }

        public void Clear()
        {
            _selectedSquadIds.Clear();
            HoveredEnemySquadId = null;
        }

        public void SetHoverEnemy(int? squadId)
        {
            HoveredEnemySquadId = squadId;
        }

        public void SelectExclusive(int squadId)
        {
            _selectedSquadIds.Clear();
            _selectedSquadIds.Add(squadId);
        }

        public void Add(int squadId)
        {
            _selectedSquadIds.Add(squadId);
        }

        public void Toggle(int squadId)
        {
            if (_selectedSquadIds.Contains(squadId))
            {
                _selectedSquadIds.Remove(squadId);
            }
            else
            {
                _selectedSquadIds.Add(squadId);
            }
        }

        public bool Contains(int squadId)
        {
            return _selectedSquadIds.Contains(squadId);
        }

        public List<int> BuildOrderedSelection()
        {
            List<int> ordered = new List<int>(_selectedSquadIds);
            ordered.Sort();
            return ordered;
        }
    }
}
