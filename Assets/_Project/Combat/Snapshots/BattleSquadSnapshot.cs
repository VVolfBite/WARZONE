using System.Numerics;
using Warzone.Content.Definitions;

namespace Warzone.Combat
{
    public sealed class BattleSquadSnapshot
    {
        public BattleSquadSnapshot(
            int squadId,
            FactionId factionId,
            Vector2 position,
            Vector2 desiredPosition,
            string currentCommand,
            SquadStance stance,
            int memberCount,
            int aliveMemberCount,
            float formationSpacing)
        {
            SquadId = squadId;
            FactionId = factionId;
            Position = position;
            DesiredPosition = desiredPosition;
            CurrentCommand = currentCommand;
            Stance = stance;
            MemberCount = memberCount;
            AliveMemberCount = aliveMemberCount;
            FormationSpacing = formationSpacing;
        }

        public int SquadId { get; private set; }
        public FactionId FactionId { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 DesiredPosition { get; private set; }
        public string CurrentCommand { get; private set; }
        public SquadStance Stance { get; private set; }
        public int MemberCount { get; private set; }
        public int AliveMemberCount { get; private set; }
        public float FormationSpacing { get; private set; }
    }
}
