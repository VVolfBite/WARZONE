using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class EnvironmentalZoneSystem
    {
        public void Execute(BattleState battleState, float deltaTimeSeconds)
        {
            if (battleState == null || battleState.EnvironmentState == null)
            {
                return;
            }

            TickZoneDurations(battleState.EnvironmentState, deltaTimeSeconds);
            ApplyMemberEnvironmentalEffects(battleState, deltaTimeSeconds);
            ApplyEnemyEnvironmentalEffects(battleState, deltaTimeSeconds);
        }

        private static void TickZoneDurations(BattleEnvironmentState environmentState, float deltaTimeSeconds)
        {
            foreach (EnvironmentalZoneState zoneState in environmentState.ZonesById.Values)
            {
                if (zoneState != null)
                {
                    zoneState.TickDuration(deltaTimeSeconds);
                }
            }
        }

        private static void ApplyMemberEnvironmentalEffects(BattleState battleState, float deltaTimeSeconds)
        {
            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (memberState == null || !memberState.CanAct)
                {
                    continue;
                }

                List<EnvironmentalZoneState> zones = EnvironmentalVisibilityRule.FindZonesContaining(battleState.EnvironmentState, memberState.Position);
                for (int i = 0; i < zones.Count; i++)
                {
                    ApplyEnvironmentalEffectsToMember(battleState, memberState, zones[i], deltaTimeSeconds);
                }
            }
        }

        private static void ApplyEnemyEnvironmentalEffects(BattleState battleState, float deltaTimeSeconds)
        {
            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                if (enemyState == null || !enemyState.IsAlive)
                {
                    continue;
                }

                List<EnvironmentalZoneState> zones = EnvironmentalVisibilityRule.FindZonesContaining(battleState.EnvironmentState, enemyState.Position);
                for (int i = 0; i < zones.Count; i++)
                {
                    ApplyEnvironmentalEffectsToEnemy(battleState, enemyState, zones[i], deltaTimeSeconds);
                }
            }
        }

        private static void ApplyEnvironmentalEffectsToMember(BattleState battleState, BattleMemberState memberState, EnvironmentalZoneState zoneState, float deltaTimeSeconds)
        {
            int damage = EnvironmentalDamageRule.GetDamagePerTick(zoneState, deltaTimeSeconds);
            if (damage > 0)
            {
                battleState.EnqueueDamage(new PendingDamageRequest(
                    new BattleEntityId(200000 + zoneState.ZoneId),
                    memberState.MemberId,
                    damage,
                    zoneState.ZoneType.ToString(),
                    true));

                battleState.AddEvent(new BattleEventRecord(
                    zoneState.ZoneType == EnvironmentalZoneType.Fire ? BattleEventTypes.FireDamageApplied : BattleEventTypes.ToxicDamageApplied,
                    memberState.SquadId,
                    memberState.MemberId,
                    zoneState.ZoneId.ToString(),
                    memberState.MemberId,
                    damage));
            }

            float pressure = EnvironmentalPressureRule.GetPressurePerTick(zoneState, deltaTimeSeconds);
            if (pressure > 0f)
            {
                memberState.AddPressure(pressure);
                battleState.AddEvent(new BattleEventRecord(
                    BattleEventTypes.PressureChanged,
                    memberState.SquadId,
                    memberState.MemberId,
                    zoneState.ZoneType.ToString(),
                    null,
                    (int)(memberState.Pressure * 10f)));
                PressureSystem.UpdateStatusFlags(battleState, memberState);
            }
        }

        private static void ApplyEnvironmentalEffectsToEnemy(BattleState battleState, BattleEnemyState enemyState, EnvironmentalZoneState zoneState, float deltaTimeSeconds)
        {
            int damage = EnvironmentalDamageRule.GetDamagePerTick(zoneState, deltaTimeSeconds);
            if (damage <= 0)
            {
                return;
            }

            battleState.EnqueueDamage(new PendingDamageRequest(
                new BattleEntityId(200000 + zoneState.ZoneId),
                enemyState.EnemyId,
                damage,
                zoneState.ZoneType.ToString(),
                false));

            battleState.AddEvent(new BattleEventRecord(
                zoneState.ZoneType == EnvironmentalZoneType.Fire ? BattleEventTypes.FireDamageApplied : BattleEventTypes.ToxicDamageApplied,
                null,
                enemyState.EnemyId,
                zoneState.ZoneId.ToString(),
                enemyState.EnemyId,
                damage));
        }
    }
}
