using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class BattleEnvironmentState
    {
        private readonly Dictionary<int, EnvironmentalZoneState> _zonesById = new Dictionary<int, EnvironmentalZoneState>();

        public BattleEnvironmentState(bool isNight = false, float globalVisibilityMultiplier = 1f, float ambientLightLevel = 1f)
        {
            IsNight = isNight;
            GlobalVisibilityMultiplier = globalVisibilityMultiplier;
            AmbientLightLevel = ambientLightLevel;
        }

        public bool IsNight { get; private set; }
        public float GlobalVisibilityMultiplier { get; private set; }
        public float AmbientLightLevel { get; private set; }

        public IReadOnlyDictionary<int, EnvironmentalZoneState> ZonesById
        {
            get { return _zonesById; }
        }

        public void SetNight(bool isNight)
        {
            IsNight = isNight;
        }

        public void SetGlobalVisibilityMultiplier(float multiplier)
        {
            GlobalVisibilityMultiplier = multiplier < 0f ? 0f : multiplier;
        }

        public void SetAmbientLightLevel(float ambientLightLevel)
        {
            AmbientLightLevel = ambientLightLevel < 0f ? 0f : ambientLightLevel;
        }

        public void AddZone(EnvironmentalZoneState zoneState)
        {
            if (zoneState == null)
            {
                return;
            }

            _zonesById[zoneState.ZoneId] = zoneState;
        }

        public bool TryGetZone(int zoneId, out EnvironmentalZoneState zoneState)
        {
            return _zonesById.TryGetValue(zoneId, out zoneState);
        }

        public int GetNextZoneId()
        {
            int nextId = 1;
            foreach (int zoneId in _zonesById.Keys)
            {
                if (zoneId >= nextId)
                {
                    nextId = zoneId + 1;
                }
            }

            return nextId;
        }
    }
}

