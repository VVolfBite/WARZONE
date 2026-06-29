using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Content.Authoring
{
    public static class ContentCatalogBuilder
    {
        public static ContentCatalog Build(ContentCatalogAsset asset)
        {
            Dictionary<string, UnitDefinition> units = new Dictionary<string, UnitDefinition>();
            Dictionary<string, MissionDefinition> missions = new Dictionary<string, MissionDefinition>();

            if (asset != null)
            {
                foreach (UnitDefinitionAsset unitAsset in asset.UnitDefinitions)
                {
                    if (unitAsset == null || unitAsset.Weapon == null)
                    {
                        continue;
                    }

                    units[unitAsset.Id] = new UnitDefinition(
                        unitAsset.Id,
                        unitAsset.DisplayName,
                        unitAsset.FactionId,
                        unitAsset.MaxHealth,
                        unitAsset.MoveSpeed,
                        new WeaponDefinition(
                            unitAsset.Weapon.Id,
                            unitAsset.Weapon.Range,
                        unitAsset.Weapon.AttackIntervalSeconds,
                        unitAsset.Weapon.DamagePerHit,
                        unitAsset.Weapon.ProjectileSpeed,
                        unitAsset.Weapon.DamageType),
                        unitAsset.AggroRange,
                        unitAsset.CollisionRadius,
                        unitAsset.ArmorType,
                        unitAsset.DefaultStatusEffect != null ? unitAsset.DefaultStatusEffect.Id : null);
                }

                foreach (MissionDefinitionAsset missionAsset in asset.MissionDefinitions)
                {
                    if (missionAsset == null)
                    {
                        continue;
                    }

                    missions[missionAsset.Id] = new MissionDefinition(
                        missionAsset.Id,
                        missionAsset.DisplayName,
                        missionAsset.PlayerSquadCount,
                        missionAsset.EnemySquadCount);
                }
            }

            return new ContentCatalog(units, missions);
        }
    }
}
