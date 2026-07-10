using System.Collections.Generic;

namespace Warzone.Campaign
{
    public sealed class CampaignRecoverySystem
    {
        private readonly CampaignBaseSystem _baseSystem;

        public CampaignRecoverySystem()
            : this(new CampaignBaseSystem())
        {
        }

        public CampaignRecoverySystem(CampaignBaseSystem baseSystem)
        {
            _baseSystem = baseSystem ?? new CampaignBaseSystem();
        }

        public void ApplyWound(CampaignState campaignState, CampaignMemberState member, WoundSeverity woundSeverity, int recoveryDaysRemaining, string missionId = null)
        {
            if (member == null || !member.IsAlive)
            {
                return;
            }

            int recoveryDays = recoveryDaysRemaining > 0 ? recoveryDaysRemaining : ResolveRecoveryDays(woundSeverity);
            if (member.IsWounded && member.RecoveryDaysRemaining > 0)
            {
                recoveryDays = member.RecoveryDaysRemaining;
            }

            recoveryDays = ApplyInfirmaryModifier(campaignState, recoveryDays);
            member.ApplyWound(woundSeverity, recoveryDays, missionId);
        }

        public void AdvanceRecoveryDay(CampaignState campaignState)
        {
            if (campaignState == null || campaignState.Roster == null)
            {
                return;
            }

            foreach (KeyValuePair<string, CampaignMemberState> pair in campaignState.Roster.MembersById)
            {
                CampaignMemberState member = pair.Value;
                if (member == null || !member.IsAlive)
                {
                    continue;
                }

                member.AdvanceRecoveryDay();
            }
        }

        public bool IsMemberAvailable(CampaignMemberState member)
        {
            return member != null && member.IsAlive && !member.IsRecovering;
        }

        public int ApplyInfirmaryModifier(CampaignState campaignState, int recoveryDays)
        {
            int adjusted = recoveryDays;
            if (_baseSystem != null && _baseSystem.HasCapability(campaignState, "infirmary"))
            {
                adjusted = recoveryDays - 1;
            }

            return adjusted < 1 ? 1 : adjusted;
        }

        private int ResolveRecoveryDays(WoundSeverity severity)
        {
            switch (severity)
            {
                case WoundSeverity.Light:
                    return 1;
                case WoundSeverity.Moderate:
                    return 3;
                case WoundSeverity.Severe:
                    return 5;
                default:
                    return 0;
            }
        }
    }
}
