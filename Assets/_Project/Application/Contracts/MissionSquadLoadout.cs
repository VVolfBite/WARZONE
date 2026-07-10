using System.Collections.Generic;

namespace Warzone.Application
{
    public sealed class MissionSquadLoadout
    {
        public MissionSquadLoadout(
            int squadId,
            string displayName,
            IReadOnlyList<MissionMemberLoadout> members,
            float formationSpacing = 1.5f)
        {
            SquadId = squadId;
            DisplayName = displayName;
            Members = members;
            FormationSpacing = formationSpacing;
        }

        public int SquadId { get; private set; }
        public string DisplayName { get; private set; }
        public IReadOnlyList<MissionMemberLoadout> Members { get; private set; }
        public float FormationSpacing { get; private set; }
    }
}
