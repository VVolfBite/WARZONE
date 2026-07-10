using System.Collections.Generic;
using Warzone.Content.Definitions;

namespace Warzone.Application
{
    public sealed class MissionLaunchPlan
    {
        public MissionLaunchPlan(
            string missionId,
            string siteId,
            MissionDefinition missionDefinition,
            MissionSiteContext siteContext,
            IReadOnlyList<int> selectedSquadIds,
            IReadOnlyList<MissionSquadLoadout> squadLoadouts,
            IReadOnlyList<MissionMemberLoadout> memberLoadouts,
            IReadOnlyDictionary<int, string> battleMemberIdToCampaignMemberId,
            IReadOnlyList<MissionObjectiveDefinition> objectiveDefinitions,
            int randomSeed,
            string entryPointId = null)
        {
            MissionId = missionId;
            SiteId = siteId;
            MissionDefinition = missionDefinition;
            SiteContext = siteContext;
            SelectedSquadIds = selectedSquadIds;
            SquadLoadouts = squadLoadouts;
            MemberLoadouts = memberLoadouts;
            BattleMemberIdToCampaignMemberId = battleMemberIdToCampaignMemberId;
            ObjectiveDefinitions = objectiveDefinitions;
            RandomSeed = randomSeed;
            EntryPointId = entryPointId;
        }

        public string MissionId { get; private set; }
        public string SiteId { get; private set; }
        public MissionDefinition MissionDefinition { get; private set; }
        public MissionSiteContext SiteContext { get; private set; }
        public IReadOnlyList<int> SelectedSquadIds { get; private set; }
        public IReadOnlyList<MissionSquadLoadout> SquadLoadouts { get; private set; }
        public IReadOnlyList<MissionMemberLoadout> MemberLoadouts { get; private set; }
        public IReadOnlyDictionary<int, string> BattleMemberIdToCampaignMemberId { get; private set; }
        public IReadOnlyList<MissionObjectiveDefinition> ObjectiveDefinitions { get; private set; }
        public int RandomSeed { get; private set; }
        public string EntryPointId { get; private set; }
    }
}
