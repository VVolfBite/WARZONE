using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Content.Validation
{
    public sealed class ContentCatalogValidator
    {
        public ContentValidationResult Validate(ContentCatalog catalog)
        {
            ContentValidationResult result = new ContentValidationResult();
            if (catalog == null)
            {
                result.Add(new ContentValidationIssue("catalog.missing", "Content catalog is missing."));
                return result;
            }

            ValidateDuplicateIds(catalog, result);
            ValidateMissions(catalog, result);
            ValidateLootProfiles(catalog, result);
            ValidateSites(catalog, result);
            ValidateOutposts(catalog, result);
            ValidateWeapons(catalog, result);
            ValidateEnemies(catalog, result);
            ValidateItems(catalog, result);
            ValidateResources(catalog, result);
            ValidateEnvironment(catalog, result);
            ValidateVision(catalog, result);
            ValidateBaseModules(catalog, result);

            return result;
        }

        private static void ValidateDuplicateIds(ContentCatalog catalog, ContentValidationResult result)
        {
            foreach (string duplicateId in catalog.DuplicateRegistrationIds)
            {
                result.Add(new ContentValidationIssue("catalog.duplicate", "Duplicate content id registered: " + duplicateId, duplicateId, ContentValidationSeverity.Warning));
            }
        }

        private static void ValidateMissions(ContentCatalog catalog, ContentValidationResult result)
        {
            foreach (KeyValuePair<string, MissionDefinition> entry in catalog.Missions)
            {
                MissionDefinition mission = entry.Value;
                if (mission == null)
                {
                    continue;
                }

                if (mission.Objectives == null || mission.Objectives.Count == 0)
                {
                    result.Add(new ContentValidationIssue("mission.objectives.missing", "Mission has no objectives.", mission.Id));
                }

                MissionRewardDefinition reward = mission.Reward;
                if (reward == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(reward.LootProfileId) && !catalog.LootProfiles.ContainsKey(reward.LootProfileId))
                {
                    result.Add(new ContentValidationIssue("mission.reward.profile.missing", "Mission references missing loot profile: " + reward.LootProfileId, mission.Id));
                }

                ValidateResourceRewards(catalog, reward.LootReward != null ? reward.LootReward.ResourceRewards : null, result, mission.Id);
            }
        }

        private static void ValidateLootProfiles(ContentCatalog catalog, ContentValidationResult result)
        {
            foreach (KeyValuePair<string, LootProfileDefinition> entry in catalog.LootProfiles)
            {
                LootProfileDefinition profile = entry.Value;
                if (profile == null)
                {
                    continue;
                }

                ValidateResourceRewards(catalog, profile.ResourceRewards, result, profile.Id);
                ValidateItemRewards(catalog, profile.ItemRewards, result, profile.Id);
                ValidateWeaponRewards(catalog, profile.WeaponRewards, result, profile.Id);
            }
        }

        private static void ValidateSites(ContentCatalog catalog, ContentValidationResult result)
        {
            foreach (KeyValuePair<string, SiteDefinition> entry in catalog.Sites)
            {
                SiteDefinition site = entry.Value;
                if (site == null)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(site.DefaultOutpostId) && !catalog.Outposts.ContainsKey(site.DefaultOutpostId))
                {
                    result.Add(new ContentValidationIssue("site.default_outpost.missing", "Site references missing outpost definition: " + site.DefaultOutpostId, site.Id));
                }
            }
        }

        private static void ValidateOutposts(ContentCatalog catalog, ContentValidationResult result)
        {
            foreach (KeyValuePair<string, OutpostDefinition> entry in catalog.Outposts)
            {
                OutpostDefinition outpost = entry.Value;
                if (outpost == null)
                {
                    continue;
                }

                ValidateCostMap(catalog, outpost.EstablishCostResources, result, outpost.Id, "outpost.cost");
                ValidateCostMap(catalog, outpost.DailyMaintenanceResources, result, outpost.Id, "outpost.maintenance");
            }
        }

        private static void ValidateWeapons(ContentCatalog catalog, ContentValidationResult result)
        {
            foreach (KeyValuePair<string, WeaponDefinition> entry in catalog.Weapons)
            {
                WeaponDefinition weapon = entry.Value;
                if (weapon == null || string.IsNullOrEmpty(weapon.Id))
                {
                    result.Add(new ContentValidationIssue("weapon.invalid", "Weapon definition is missing an id."));
                }
            }
        }

        private static void ValidateEnemies(ContentCatalog catalog, ContentValidationResult result)
        {
            foreach (KeyValuePair<string, EnemyDefinition> entry in catalog.Enemies)
            {
                EnemyDefinition enemy = entry.Value;
                if (enemy == null || string.IsNullOrEmpty(enemy.Id))
                {
                    result.Add(new ContentValidationIssue("enemy.invalid", "Enemy definition is missing an id."));
                }
            }
        }

        private static void ValidateItems(ContentCatalog catalog, ContentValidationResult result)
        {
            foreach (KeyValuePair<string, ItemDefinition> entry in catalog.Items)
            {
                ItemDefinition item = entry.Value;
                if (item == null || string.IsNullOrEmpty(item.Id))
                {
                    result.Add(new ContentValidationIssue("item.invalid", "Item definition is missing an id."));
                }
            }
        }

        private static void ValidateResources(ContentCatalog catalog, ContentValidationResult result)
        {
            foreach (KeyValuePair<string, ResourcePackageDefinition> entry in catalog.ResourcePackages)
            {
                ResourcePackageDefinition resource = entry.Value;
                if (resource == null || string.IsNullOrEmpty(resource.Id))
                {
                    result.Add(new ContentValidationIssue("resource.invalid", "Resource package definition is missing an id."));
                }
            }
        }

        private static void ValidateEnvironment(ContentCatalog catalog, ContentValidationResult result)
        {
            foreach (KeyValuePair<string, EnvironmentalZoneDefinition> entry in catalog.EnvironmentalZones)
            {
                EnvironmentalZoneDefinition zone = entry.Value;
                if (zone == null || string.IsNullOrEmpty(zone.Id))
                {
                    result.Add(new ContentValidationIssue("environment.invalid", "Environmental zone definition is missing an id."));
                }
            }
        }

        private static void ValidateVision(ContentCatalog catalog, ContentValidationResult result)
        {
            foreach (KeyValuePair<string, VisionEquipmentDefinition> entry in catalog.VisionEquipment)
            {
                VisionEquipmentDefinition equipment = entry.Value;
                if (equipment == null || string.IsNullOrEmpty(equipment.Id))
                {
                    result.Add(new ContentValidationIssue("vision.invalid", "Vision equipment definition is missing an id."));
                }
            }
        }

        private static void ValidateBaseModules(ContentCatalog catalog, ContentValidationResult result)
        {
            foreach (KeyValuePair<string, BaseModuleDefinition> entry in catalog.BaseModules)
            {
                BaseModuleDefinition module = entry.Value;
                if (module == null || string.IsNullOrEmpty(module.Id))
                {
                    result.Add(new ContentValidationIssue("base_module.invalid", "Base module definition is missing an id."));
                    continue;
                }

                ValidateCostMap(catalog, module.DailyResourceCosts, result, module.Id, "base_module.cost");
            }
        }

        private static void ValidateResourceRewards(ContentCatalog catalog, IReadOnlyList<ResourceRewardDefinition> rewards, ContentValidationResult result, string ownerId)
        {
            if (rewards == null)
            {
                return;
            }

            for (int i = 0; i < rewards.Count; i++)
            {
                ResourceRewardDefinition reward = rewards[i];
                if (reward == null || string.IsNullOrEmpty(reward.ResourceId) || reward.Amount <= 0)
                {
                    result.Add(new ContentValidationIssue("reward.resource.invalid", "Resource reward is invalid.", ownerId));
                    continue;
                }

                if (!catalog.ResourcePackages.ContainsKey(reward.ResourceId))
                {
                    result.Add(new ContentValidationIssue("reward.resource.missing", "Resource reward references missing resource package: " + reward.ResourceId, ownerId));
                }
            }
        }

        private static void ValidateItemRewards(ContentCatalog catalog, IReadOnlyList<ItemRewardDefinition> rewards, ContentValidationResult result, string ownerId)
        {
            if (rewards == null)
            {
                return;
            }

            for (int i = 0; i < rewards.Count; i++)
            {
                ItemRewardDefinition reward = rewards[i];
                if (reward == null || string.IsNullOrEmpty(reward.ItemId) || reward.Count <= 0)
                {
                    result.Add(new ContentValidationIssue("reward.item.invalid", "Item reward is invalid.", ownerId));
                    continue;
                }

                if (!catalog.Items.ContainsKey(reward.ItemId))
                {
                    result.Add(new ContentValidationIssue("reward.item.missing", "Item reward references missing item definition: " + reward.ItemId, ownerId));
                }
            }
        }

        private static void ValidateWeaponRewards(ContentCatalog catalog, IReadOnlyList<WeaponRewardDefinition> rewards, ContentValidationResult result, string ownerId)
        {
            if (rewards == null)
            {
                return;
            }

            for (int i = 0; i < rewards.Count; i++)
            {
                WeaponRewardDefinition reward = rewards[i];
                if (reward == null || string.IsNullOrEmpty(reward.WeaponDefinitionId) || reward.Count <= 0)
                {
                    result.Add(new ContentValidationIssue("reward.weapon.invalid", "Weapon reward is invalid.", ownerId));
                    continue;
                }

                if (!catalog.Weapons.ContainsKey(reward.WeaponDefinitionId))
                {
                    result.Add(new ContentValidationIssue("reward.weapon.missing", "Weapon reward references missing weapon definition: " + reward.WeaponDefinitionId, ownerId));
                }
            }
        }

        private static void ValidateCostMap(ContentCatalog catalog, IReadOnlyDictionary<string, int> costs, ContentValidationResult result, string ownerId, string codePrefix)
        {
            if (costs == null)
            {
                return;
            }

            foreach (KeyValuePair<string, int> cost in costs)
            {
                if (string.IsNullOrEmpty(cost.Key) || cost.Value < 0)
                {
                    result.Add(new ContentValidationIssue(codePrefix + ".invalid", "Invalid cost entry.", ownerId));
                    continue;
                }

                if (!catalog.ResourcePackages.ContainsKey(cost.Key))
                {
                    result.Add(new ContentValidationIssue(codePrefix + ".missing", "Cost references missing resource package: " + cost.Key, ownerId));
                }
            }
        }
    }
}
