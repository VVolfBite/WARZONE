using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Content
{
    public sealed class ContentCatalog
    {
        public ContentCatalog(
            IReadOnlyDictionary<string, UnitDefinition> units,
            IReadOnlyDictionary<string, MissionDefinition> missions)
        {
            Units = units;
            Missions = missions;
        }

        public IReadOnlyDictionary<string, UnitDefinition> Units { get; }
        public IReadOnlyDictionary<string, MissionDefinition> Missions { get; }
    }
}
