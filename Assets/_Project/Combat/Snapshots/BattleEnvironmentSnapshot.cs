using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class BattleEnvironmentSnapshot
    {
        public BattleEnvironmentSnapshot(
            bool isNight,
            float globalVisibilityMultiplier,
            float ambientLightLevel,
            IReadOnlyList<EnvironmentalZoneSnapshot> zones)
        {
            IsNight = isNight;
            GlobalVisibilityMultiplier = globalVisibilityMultiplier;
            AmbientLightLevel = ambientLightLevel;
            Zones = zones;
        }

        public bool IsNight { get; private set; }
        public float GlobalVisibilityMultiplier { get; private set; }
        public float AmbientLightLevel { get; private set; }
        public IReadOnlyList<EnvironmentalZoneSnapshot> Zones { get; private set; }
    }
}

