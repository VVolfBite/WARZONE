using System.Collections.Generic;
using UnityEngine;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.Input
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
            List<Vec2> destinations = BuildFormationDestinations(new Vec2(worldPoint.x, worldPoint.z), orderedSquadIds.Count);
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

        public void IssueFormationMove(BattleSession battleSession, IReadOnlyList<int> orderedSquadIds, UnityEngine.Vector3 worldPoint, int formationIndex, bool queue)
        {
            List<Vec2> destinations = BuildFormationDestinations(new Vec2(worldPoint.x, worldPoint.z), orderedSquadIds.Count, formationIndex);
            for (int i = 0; i < orderedSquadIds.Count; i++)
            {
                battleSession.ExecuteCommand(new Command(
                    CommandType.Move,
                    orderedSquadIds[i],
                    destination: destinations[i],
                    queue: queue));
            }
        }

        private static List<Vec2> BuildFormationDestinations(Vec2 center, int count, int formationIndex = 0)
        {
            List<Vec2> destinations = new List<Vec2>(count);
            if (count <= 0)
            {
                return destinations;
            }

            if (count == 1)
            {
                destinations.Add(center);
                return destinations;
            }

            if (formationIndex == 1)
            {
                const float lineSpacing = 2f;
                float lineWidth = (count - 1) * lineSpacing;
                for (int i = 0; i < count; i++)
                {
                    float offsetX = (i * lineSpacing) - (lineWidth * 0.5f);
                    destinations.Add(new Vec2(center.X + offsetX, center.Y));
                }

                return destinations;
            }

            if (formationIndex == 2)
            {
                const float columnSpacing = 2f;
                float columnHeight = (count - 1) * columnSpacing;
                for (int i = 0; i < count; i++)
                {
                    float offsetY = (i * columnSpacing) - (columnHeight * 0.5f);
                    destinations.Add(new Vec2(center.X, center.Y + offsetY));
                }

                return destinations;
            }

            if (formationIndex == 3)
            {
                float radius = 2.2f;
                for (int i = 0; i < count; i++)
                {
                    float angle = ((float)i / count) * Mathf.PI * 2f;
                    destinations.Add(new Vec2(
                        center.X + (Mathf.Cos(angle) * radius),
                        center.Y + (Mathf.Sin(angle) * radius)));
                }

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
                destinations.Add(new Vec2(center.X + offsetX, center.Y + offsetY));
            }

            return destinations;
        }
    }
}



