using Warzone.Core.Math;

namespace Warzone.Combat
{
    public sealed class PressureSystem
    {
        public void Execute(BattleState battleState, float deltaTimeSeconds)
        {
            if (battleState == null)
            {
                return;
            }

            ApplyEventPressure(battleState);
            RecoverPressure(battleState, deltaTimeSeconds);
        }

        private static void ApplyEventPressure(BattleState battleState)
        {
            var events = battleState.EventBuffer.Events;
            for (int i = 0; i < events.Count; i++)
            {
                BattleEventRecord battleEvent = events[i];
                if (battleEvent == null)
                {
                    continue;
                }

                if (battleEvent.EventType == BattleEventTypes.DamageApplied && battleEvent.TargetId.HasValue)
                {
                    BattleMemberState memberState;
                    if (battleState.TryGetMember(battleEvent.TargetId.Value, out memberState))
                    {
                        memberState.SetLastDamageSourceEnemy(battleEvent.MemberId);
                        memberState.SetRecentIncomingFire(PressureGainRule.RecentIncomingFireDurationSeconds);
                        ApplyPressureChange(battleState, memberState, PressureGainRule.FromDamage(battleEvent.Amount));
                    }

                    continue;
                }

                if (battleEvent.EventType == BattleEventTypes.WeaponFired && battleEvent.TargetId.HasValue)
                {
                    BattleMemberState memberState;
                    if (battleState.TryGetMember(battleEvent.TargetId.Value, out memberState))
                    {
                        memberState.SetRecentIncomingFire(PressureGainRule.RecentIncomingFireDurationSeconds);
                        ApplyPressureChange(battleState, memberState, PressureGainRule.IncomingFirePressure);
                    }

                    continue;
                }

                if (battleEvent.EventType == BattleEventTypes.MemberKilled && battleEvent.TargetId.HasValue)
                {
                    BattleMemberState deadMemberState;
                    if (!battleState.TryGetMember(battleEvent.TargetId.Value, out deadMemberState))
                    {
                        continue;
                    }

                    foreach (BattleMemberState memberState in battleState.MembersById.Values)
                    {
                        if (memberState == null || !memberState.IsAlive || memberState.IsExtracted || memberState.MemberId == deadMemberState.MemberId)
                        {
                            continue;
                        }

                        if (memberState.SquadId != deadMemberState.SquadId)
                        {
                            continue;
                        }

                        if (Vec2.Distance(memberState.Position, deadMemberState.Position) > 6f)
                        {
                            continue;
                        }

                        ApplyPressureChange(battleState, memberState, PressureGainRule.NearbyDeathPressure);
                    }
                }
            }
        }

        private static void RecoverPressure(BattleState battleState, float deltaTimeSeconds)
        {
            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (memberState == null || !memberState.IsAlive || memberState.IsExtracted)
                {
                    continue;
                }

                memberState.TickIncomingFire(deltaTimeSeconds);

                if (memberState.RecentIncomingFireSeconds <= 0f)
                {
                    float previousPressure = memberState.Pressure;
                    memberState.ReducePressure(PressureRecoveryRule.GetRecoveryAmount(deltaTimeSeconds));
                    if (previousPressure != memberState.Pressure)
                    {
                        battleState.AddEvent(new BattleEventRecord(
                            BattleEventTypes.PressureChanged,
                            memberState.SquadId,
                            memberState.MemberId,
                            "recover",
                            null,
                            (int)(memberState.Pressure * 10f)));
                    }
                }

                if (!memberState.IsBroken)
                {
                    UpdateStatusFlags(battleState, memberState);
                }
            }
        }

        private static void ApplyPressureChange(BattleState battleState, BattleMemberState memberState, float pressureAmount)
        {
            if (memberState == null || pressureAmount <= 0f)
            {
                return;
            }

            float previousPressure = memberState.Pressure;
            memberState.AddPressure(pressureAmount);
            if (previousPressure == memberState.Pressure)
            {
                return;
            }

            battleState.AddEvent(new BattleEventRecord(
                BattleEventTypes.PressureChanged,
                memberState.SquadId,
                memberState.MemberId,
                "gain",
                null,
                (int)(memberState.Pressure * 10f)));

            UpdateStatusFlags(battleState, memberState);
        }

        public static void UpdateStatusFlags(BattleState battleState, BattleMemberState memberState)
        {
            if (memberState == null)
            {
                return;
            }

            bool wasSuppressed = memberState.IsSuppressed;
            bool wasBroken = memberState.IsBroken;

            memberState.SetSuppression(memberState.MaxPressure <= 0f ? 0f : memberState.Pressure / memberState.MaxPressure);

            bool isSuppressed = SuppressionRule.IsSuppressed(memberState) || memberState.IsBroken;
            memberState.SetSuppressed(isSuppressed);
            if (isSuppressed && !wasSuppressed)
            {
                battleState.AddEvent(new BattleEventRecord(
                    BattleEventTypes.MemberSuppressed,
                    memberState.SquadId,
                    memberState.MemberId,
                    "suppressed"));
            }

            bool isBroken = wasBroken || BreakRetreatRule.IsBroken(memberState);
            memberState.SetBroken(isBroken);
            if (isBroken && !wasBroken)
            {
                battleState.AddEvent(new BattleEventRecord(
                    BattleEventTypes.MemberBroken,
                    memberState.SquadId,
                    memberState.MemberId,
                    "broken"));
            }
        }
    }
}
