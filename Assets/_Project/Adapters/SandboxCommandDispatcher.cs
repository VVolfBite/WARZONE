using System.Collections.Generic;
using UnityEngine;
using Warzone.Combat;

namespace Warzone.Adapters
{
    public sealed class SandboxCommandDispatcher
    {
        public void IssueAttack(BattleSession battleSession, IReadOnlyCollection<int> selectedSquadIds, int targetSquadId, bool queue)
        {
            foreach (int squadId in selectedSquadIds)
            {
                battleSession.ExecuteCommand(new Command(CommandType.Attack, squadId, targetSquadId, queue: queue));
            }
        }

        public void IssueMove(BattleSession battleSession, IReadOnlyList<int> orderedSquadIds, UnityEngine.Vector3 worldPoint, bool queue)
        {
            List<System.Numerics.Vector2> destinations = BuildFormationDestinations(new System.Numerics.Vector2(worldPoint.x, worldPoint.z), orderedSquadIds.Count);
            for (int i = 0; i < orderedSquadIds.Count; i++)
            {
                battleSession.ExecuteCommand(new Command(
                    CommandType.Move,
                    orderedSquadIds[i],
                    destination: destinations[i],
                    queue: queue));
            }
        }

        public void IssueUseAbility(BattleSession battleSession, IReadOnlyCollection<int> selectedSquadIds, string abilityId)
        {
            foreach (int squadId in selectedSquadIds)
            {
                battleSession.ExecuteCommand(new Command(
                    CommandType.UseAbility,
                    squadId,
                    abilityId: abilityId));
            }
        }

        private static List<System.Numerics.Vector2> BuildFormationDestinations(System.Numerics.Vector2 center, int count)
        {
            List<System.Numerics.Vector2> destinations = new List<System.Numerics.Vector2>(count);
            if (count <= 0)
            {
                return destinations;
            }

            if (count == 1)
            {
                destinations.Add(center);
                return destinations;
            }

            int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
            int rows = Mathf.CeilToInt((float)count / columns);
            const float spacing = 1.75f;
            float width = (columns - 1) * spacing;
            float height = (rows - 1) * spacing;

            for (int i = 0; i < count; i++)
            {
                int row = i / columns;
                int column = i % columns;
                float offsetX = (column * spacing) - (width * 0.5f);
                float offsetY = (row * spacing) - (height * 0.5f);
                destinations.Add(new System.Numerics.Vector2(center.X + offsetX, center.Y + offsetY));
            }

            return destinations;
        }
    }
}
