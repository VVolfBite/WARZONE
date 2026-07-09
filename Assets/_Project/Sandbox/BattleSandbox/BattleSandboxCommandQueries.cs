using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public static class BattleSandboxCommandQueries
    {
        public static BattleSquadSnapshot FindSelectedSquad(BattleSnapshot snapshot, int selectedSquadId)
        {
            if (snapshot == null)
            {
                return null;
            }

            for (int i = 0; i < snapshot.Squads.Count; i++)
            {
                if (snapshot.Squads[i].SquadId == selectedSquadId)
                {
                    return snapshot.Squads[i];
                }
            }

            return null;
        }

        public static TacticalNodeSnapshot FindNearestNode(BattleSnapshot snapshot, Vec2 position, TacticalNodeType nodeType, bool onlyIncompleteSearch)
        {
            if (snapshot == null)
            {
                return null;
            }

            TacticalNodeSnapshot nearest = null;
            float nearestDistance = float.MaxValue;
            for (int i = 0; i < snapshot.TacticalNodes.Count; i++)
            {
                TacticalNodeSnapshot node = snapshot.TacticalNodes[i];
                if (node.NodeType != nodeType || !node.IsEnabled)
                {
                    continue;
                }

                if (onlyIncompleteSearch && nodeType == TacticalNodeType.SearchPoint && node.IsSearched)
                {
                    continue;
                }

                float distance = Vec2.Distance(position, node.Position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = node;
                }
            }

            return nearest;
        }
    }
}
