using System.Collections.Generic;

namespace Warzone.Sandbox.Selection
{
    public sealed class SandboxSelectionService
    {
        private readonly HashSet<int> _selectedSquadIds = new HashSet<int>();
        private readonly Dictionary<int, List<int>> _teamBindings = new Dictionary<int, List<int>>();

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

        public void BindTeam(int slotIndex)
        {
            List<int> ordered = BuildOrderedSelection();
            if (ordered.Count == 0)
            {
                return;
            }

            _teamBindings[slotIndex] = ordered;
        }

        public bool TrySelectTeam(int slotIndex)
        {
            if (!_teamBindings.TryGetValue(slotIndex, out List<int> binding) || binding == null || binding.Count == 0)
            {
                return false;
            }

            _selectedSquadIds.Clear();
            for (int i = 0; i < binding.Count; i++)
            {
                _selectedSquadIds.Add(binding[i]);
            }

            return true;
        }

        public int GetBoundCount(int slotIndex)
        {
            return _teamBindings.TryGetValue(slotIndex, out List<int> binding) && binding != null ? binding.Count : 0;
        }

        public bool IsTeamActive(int slotIndex)
        {
            if (!_teamBindings.TryGetValue(slotIndex, out List<int> binding) || binding == null || binding.Count == 0)
            {
                return false;
            }

            if (binding.Count != _selectedSquadIds.Count)
            {
                return false;
            }

            for (int i = 0; i < binding.Count; i++)
            {
                if (!_selectedSquadIds.Contains(binding[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}



