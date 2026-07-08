using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Content
{
    public sealed class ContentCatalog
    {
        public ContentCatalog(
            IReadOnlyDictionary<string, UnitDefinition> units,
            IReadOnlyDictionary<string, MissionDefinition> missions,
            IReadOnlyDictionary<string, AbilityDefinition> abilities = null)
        {
            Units = units;
            Missions = missions;
            Abilities = abilities ?? new Dictionary<string, AbilityDefinition>();
        }

        public IReadOnlyDictionary<string, UnitDefinition> Units { get; private set; }
        public IReadOnlyDictionary<string, MissionDefinition> Missions { get; private set; }
        public IReadOnlyDictionary<string, AbilityDefinition> Abilities { get; private set; }
    }
}



