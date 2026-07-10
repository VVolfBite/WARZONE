namespace Warzone.Campaign
{
    public sealed class CampaignMemberState
    {
        public CampaignMemberState(
            string memberId,
            string displayName,
            bool isAlive = true,
            bool isWounded = false,
            bool isAvailable = true,
            int experience = 0,
            int? assignedSquadId = null,
            string carriedWeaponId = null,
            string loadoutId = null,
            string carriedWeaponInstanceId = null,
            int level = 1,
            int missionsCompleted = 0,
            int kills = 0,
            WoundSeverity woundSeverity = WoundSeverity.None,
            int recoveryDaysRemaining = 0,
            bool isRecovering = false,
            int skillPoints = 0,
            float fatigue = 0f,
            string lastInjuryMissionId = null)
        {
            MemberId = memberId;
            DisplayName = displayName;
            IsAlive = isAlive;
            IsWounded = isWounded;
            IsAvailable = isAvailable;
            Experience = experience;
            AssignedSquadId = assignedSquadId;
            CarriedWeaponId = carriedWeaponId;
            LoadoutId = loadoutId;
            CarriedWeaponInstanceId = carriedWeaponInstanceId;
            Level = level < 1 ? 1 : level;
            MissionsCompleted = missionsCompleted < 0 ? 0 : missionsCompleted;
            Kills = kills < 0 ? 0 : kills;
            WoundSeverity = woundSeverity;
            RecoveryDaysRemaining = recoveryDaysRemaining < 0 ? 0 : recoveryDaysRemaining;
            IsRecovering = isRecovering;
            SkillPoints = skillPoints < 0 ? 0 : skillPoints;
            Fatigue = fatigue < 0f ? 0f : fatigue;
            LastInjuryMissionId = lastInjuryMissionId;
            RecalculateLevel();
        }

        public string MemberId { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsAlive { get; private set; }
        public bool IsWounded { get; private set; }
        public bool IsAvailable { get; private set; }
        public int Experience { get; private set; }
        public int Level { get; private set; }
        public int MissionsCompleted { get; private set; }
        public int Kills { get; private set; }
        public WoundSeverity WoundSeverity { get; private set; }
        public int RecoveryDaysRemaining { get; private set; }
        public bool IsRecovering { get; private set; }
        public int SkillPoints { get; private set; }
        public float Fatigue { get; private set; }
        public string LastInjuryMissionId { get; private set; }
        public int? AssignedSquadId { get; private set; }
        public string CarriedWeaponId { get; private set; }
        public string LoadoutId { get; private set; }
        public string CarriedWeaponInstanceId { get; private set; }

        public void MarkDead()
        {
            IsAlive = false;
            IsAvailable = false;
            IsWounded = true;
            IsRecovering = false;
            RecoveryDaysRemaining = 0;
            WoundSeverity = WoundSeverity.Severe;
        }

        public void MarkWounded()
        {
            if (!IsAlive)
            {
                return;
            }

            IsWounded = true;
            IsAvailable = false;
            if (WoundSeverity == WoundSeverity.None)
            {
                WoundSeverity = WoundSeverity.Moderate;
            }
            if (RecoveryDaysRemaining <= 0)
            {
                RecoveryDaysRemaining = 3;
            }
            IsRecovering = true;
        }

        public void SetAvailable(bool isAvailable)
        {
            IsAvailable = isAvailable && IsAlive && !IsRecovering;
        }

        public void SetAssignedSquadId(int? squadId)
        {
            AssignedSquadId = squadId;
        }

        public void SetCarriedWeaponId(string carriedWeaponId)
        {
            CarriedWeaponId = carriedWeaponId;
        }

        public void SetCarriedWeaponInstanceId(string carriedWeaponInstanceId)
        {
            CarriedWeaponInstanceId = carriedWeaponInstanceId;
        }

        public void AddExperience(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Experience += amount;
            RecalculateLevel();
        }

        public void AddMissionCompleted(int amount = 1)
        {
            if (amount <= 0)
            {
                return;
            }

            MissionsCompleted += amount;
        }

        public void AddKill(int amount = 1)
        {
            if (amount <= 0)
            {
                return;
            }

            Kills += amount;
        }

        public void ApplyWound(WoundSeverity woundSeverity, int recoveryDays, string injuryMissionId = null)
        {
            if (!IsAlive)
            {
                return;
            }

            WoundSeverity = woundSeverity;
            IsWounded = woundSeverity != WoundSeverity.None;
            RecoveryDaysRemaining = recoveryDays < 0 ? 0 : recoveryDays;
            IsRecovering = IsWounded && RecoveryDaysRemaining > 0;
            IsAvailable = IsAlive && !IsRecovering;
            LastInjuryMissionId = injuryMissionId;
        }

        public void AdvanceRecoveryDay()
        {
            if (!IsAlive || !IsRecovering || RecoveryDaysRemaining <= 0)
            {
                return;
            }

            RecoveryDaysRemaining--;
            if (RecoveryDaysRemaining <= 0)
            {
                RecoveryDaysRemaining = 0;
                IsRecovering = false;
                IsWounded = false;
                WoundSeverity = WoundSeverity.None;
                IsAvailable = IsAlive;
            }
        }

        public void AddFatigue(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            Fatigue += amount;
        }

        public void ReduceFatigue(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            Fatigue -= amount;
            if (Fatigue < 0f)
            {
                Fatigue = 0f;
            }
        }

        private void RecalculateLevel()
        {
            Level = 1 + (Experience / 100);
            if (Level < 1)
            {
                Level = 1;
            }
        }
    }
}
