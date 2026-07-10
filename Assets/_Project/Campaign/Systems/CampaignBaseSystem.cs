using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Campaign
{
    public sealed class CampaignBaseSystem
    {
        public CampaignBaseState CreateStartingBase(string baseId, string displayName, string siteId)
        {
            return CreateStartingBase(baseId, displayName, siteId, null);
        }

        public CampaignBaseState CreateStartingBase(string baseId, string displayName, string siteId, IReadOnlyList<BaseModuleDefinition> moduleDefinitions)
        {
            CampaignBaseState baseState = new CampaignBaseState(baseId, displayName, siteId, 150, true);

            if (moduleDefinitions != null && moduleDefinitions.Count > 0)
            {
                for (int i = 0; i < moduleDefinitions.Count; i++)
                {
                    BaseModuleDefinition moduleDefinition = moduleDefinitions[i];
                    if (moduleDefinition == null || string.IsNullOrEmpty(moduleDefinition.Id))
                    {
                        continue;
                    }

                    baseState.AddModule(CreateModule(
                        "base." + moduleDefinition.Id,
                        moduleDefinition.DisplayName,
                        moduleDefinition.ProvidedCapabilities,
                        moduleDefinition.DailyResourceCosts));
                }

                return baseState;
            }

            baseState.AddModule(CreateModule("base.storage", "Storage Wing", new[] { "storage" }, new Dictionary<string, int> { { "building_material", 1 } }));
            baseState.AddModule(CreateModule("base.infirmary", "Infirmary", new[] { "infirmary" }, new Dictionary<string, int> { { "medicine", 1 } }));
            baseState.AddModule(CreateModule("base.workshop", "Workshop", new[] { "workshop" }, new Dictionary<string, int> { { "building_material", 1 }, { "ammo", 1 } }));
            baseState.AddModule(CreateModule("base.training", "Training Yard", new[] { "training" }, new Dictionary<string, int> { { "food", 1 } }));
            baseState.AddModule(CreateModule("base.watchtower", "Watchtower", new[] { "watchtower" }, new Dictionary<string, int> { { "ammo", 1 } }));

            return baseState;
        }

        public void AddModule(CampaignState campaignState, CampaignBaseModuleState moduleState)
        {
            if (campaignState == null || moduleState == null)
            {
                return;
            }

            if (campaignState.MainBase == null)
            {
                campaignState.SetMainBase(new CampaignBaseState("base.main", "Main Base", null));
            }

            campaignState.MainBase.AddModule(moduleState);
        }

        public bool RemoveModule(CampaignState campaignState, string moduleId)
        {
            if (campaignState == null || campaignState.MainBase == null)
            {
                return false;
            }

            return campaignState.MainBase.RemoveModule(moduleId);
        }

        public bool HasCapability(CampaignState campaignState, string capabilityId)
        {
            if (campaignState == null || campaignState.MainBase == null)
            {
                return false;
            }

            return campaignState.MainBase.HasCapability(capabilityId);
        }

        public void SetOperational(CampaignState campaignState, bool isOperational, string warning = null)
        {
            if (campaignState == null || campaignState.MainBase == null)
            {
                return;
            }

            campaignState.MainBase.SetOperational(isOperational, warning);
        }

        private static CampaignBaseModuleState CreateModule(
            string moduleId,
            string displayName,
            IEnumerable<string> capabilities,
            IReadOnlyDictionary<string, int> dailyResourceCosts)
        {
            CampaignBaseModuleState moduleState = new CampaignBaseModuleState(moduleId, displayName, true, new List<string>(capabilities), dailyResourceCosts);
            return moduleState;
        }
    }
}
