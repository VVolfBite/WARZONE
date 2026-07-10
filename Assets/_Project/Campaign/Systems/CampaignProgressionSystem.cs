using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignProgressionSystem
    {
        public void AddExperience(CampaignMemberState member, int amount)
        {
            if (member == null || !member.IsAlive || amount <= 0)
            {
                return;
            }

            member.AddExperience(amount);
        }

        public void AddMissionCompleted(CampaignMemberState member, int amount = 1)
        {
            if (member == null || !member.IsAlive || amount <= 0)
            {
                return;
            }

            member.AddMissionCompleted(amount);
        }

        public void ApplySettlements(CampaignState campaignState, IReadOnlyList<CampaignExperienceSettlement> experienceSettlements)
        {
            if (campaignState == null || experienceSettlements == null)
            {
                return;
            }

            for (int i = 0; i < experienceSettlements.Count; i++)
            {
                CampaignExperienceSettlement settlement = experienceSettlements[i];
                if (settlement == null || string.IsNullOrEmpty(settlement.CampaignMemberId))
                {
                    continue;
                }

                CampaignMemberState member;
                if (!campaignState.Roster.TryGetMember(settlement.CampaignMemberId, out member))
                {
                    continue;
                }

                AddExperience(member, settlement.ExperienceGained);
                AddMissionCompleted(member, settlement.MissionsCompletedGained);
                if (settlement.KillsGained > 0)
                {
                    member.AddKill(settlement.KillsGained);
                }
            }
        }
    }
}
