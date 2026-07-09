using NUnit.Framework;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Tests.Combat
{
    public sealed class EnvironmentCombatTests
    {
        [Test]
        public void EventBuffer_PeekDoesNotClear_AndPressureSystemDoesNotDrain()
        {
            BattleState battleState = CreateEnvironmentBattleState();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            battleState.AddEvent(new BattleEventRecord(BattleEventTypes.DamageApplied, member.SquadId, new BattleEntityId(9401), "test", member.MemberId, 10));

            Assert.That(battleState.EventBuffer.Peek().Count, Is.EqualTo(1));
            new PressureSystem().Execute(battleState, 0f);

            Assert.That(battleState.EventBuffer.Events.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(member.Pressure, Is.GreaterThan(0f));
        }

        [Test]
        public void BattleSnapshot_StillContainsRecentEventsAfterPressureSystemExecutes()
        {
            BattleState battleState = CreateEnvironmentBattleState();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            battleState.AddEvent(new BattleEventRecord(BattleEventTypes.WeaponFired, member.SquadId, new BattleEntityId(9401), "test", member.MemberId, 0));
            battleState.AddEvent(new BattleEventRecord(BattleEventTypes.DamageApplied, member.SquadId, new BattleEntityId(9401), "test", member.MemberId, 12));

            new PressureSystem().Execute(battleState, 0f);
            BattleSnapshot snapshot = BattleSnapshotFactory.Create(battleState);

            Assert.That(snapshot.RecentEvents.Count, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void BattleSimulation_EventLifecycleKeepsRecentHistoryVisibleAndClearsFrameBuffer()
        {
            ContentCatalog catalog = CreateEnvironmentCatalog();
            BattleState battleState = CreateEnvironmentBattleState();
            BattleSimulation simulation = new BattleSimulation(catalog);

            simulation.Enqueue(battleState, new MoveSquadCommand(1, new Vec2(-12f, -5f)));
            simulation.Tick(battleState, 0.25f);

            Assert.That(battleState.EventBuffer.Events.Count, Is.EqualTo(0));
            Assert.That(simulation.LatestSnapshot.RecentEvents.Count, Is.GreaterThan(0));
        }

        [Test]
        public void CanCreateZones_DurationTicks_AndSnapshotIncludesEnvironment()
        {
            BattleState battleState = CreateEnvironmentBattleState();
            EnvironmentalZoneState smokeZone = battleState.EnvironmentState.ZonesById[1000];
            float startDuration = smokeZone.DurationRemaining;

            new EnvironmentalZoneSystem().Execute(battleState, 1f);
            BattleSnapshot snapshot = BattleSnapshotFactory.Create(battleState);

            Assert.That(smokeZone.DurationRemaining, Is.LessThan(startDuration));
            Assert.That(snapshot.Environment, Is.Not.Null);
            Assert.That(snapshot.Environment.Zones.Count, Is.GreaterThanOrEqualTo(5));
        }

        [Test]
        public void InactiveZone_DoesNotAffectPerception()
        {
            BattleState battleState = CreateEnvironmentBattleState();
            battleState.EnvironmentState.ZonesById[1000].SetActive(false);
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            BattleEnemyState enemy = battleState.EnemiesById[new BattleEntityId(9401)];
            member.UpdatePosition(new Vec2(0f, 0f));
            enemy.UpdatePosition(new Vec2(2f, 0f));

            new PerceptionSystem().Execute(battleState);

            Assert.That(member.VisibleEnemyIds.Count, Is.EqualTo(1));
        }

        [Test]
        public void SmokeBetweenObserverAndTarget_BlocksSight_ButSmokeOutsideLineDoesNot()
        {
            BattleState battleState = CreateEnvironmentBattleState();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            BattleEnemyState enemy = battleState.EnemiesById[new BattleEntityId(9401)];
            member.UpdatePosition(new Vec2(-4f, -2f));
            enemy.UpdatePosition(new Vec2(1f, -2f));

            new PerceptionSystem().Execute(battleState);
            Assert.That(member.VisibleEnemyIds.Count, Is.EqualTo(0));

            enemy.UpdatePosition(new Vec2(-4f, 2f));
            new PerceptionSystem().Execute(battleState);
            Assert.That(member.VisibleEnemyIds.Count, Is.EqualTo(1));
        }

        [Test]
        public void NightReducesEffectiveDetectionRange_AndNightVisionMitigatesPenalty()
        {
            BattleState battleState = CreateEnvironmentBattleState();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            member.SetNightVisionLevel(0);
            float withoutNv = EnvironmentalVisibilityRule.GetEffectiveDetectionRange(
                battleState.EnvironmentState,
                member.Position,
                member.DetectionRange,
                member.NightVisionLevel,
                new Vec2(0f, 0f),
                false);

            member.SetNightVisionLevel(1);
            float withNv = EnvironmentalVisibilityRule.GetEffectiveDetectionRange(
                battleState.EnvironmentState,
                member.Position,
                member.DetectionRange,
                member.NightVisionLevel,
                new Vec2(0f, 0f),
                false);

            Assert.That(withoutNv, Is.LessThan(member.DetectionRange));
            Assert.That(withNv, Is.GreaterThan(withoutNv));
        }

        [Test]
        public void LightZone_MakesTargetEasierToDetectAtNight()
        {
            BattleState battleState = CreateEnvironmentBattleState();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            float baseRange = EnvironmentalVisibilityRule.GetEffectiveDetectionRange(
                battleState.EnvironmentState,
                member.Position,
                member.DetectionRange,
                0,
                new Vec2(5f, 5f),
                false);
            float litRange = EnvironmentalVisibilityRule.GetEffectiveDetectionRange(
                battleState.EnvironmentState,
                member.Position,
                member.DetectionRange,
                0,
                new Vec2(11.1f, 1.2f),
                false);

            Assert.That(litRange, Is.GreaterThan(baseRange));
        }

        [Test]
        public void EnemyWithoutNightVision_FailsToDetectTargetBeyondReducedRange()
        {
            BattleState battleState = CreateEnvironmentBattleState();
            BattleEnemyState enemy = battleState.EnemiesById[new BattleEntityId(9401)];
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            enemy.SetNightVisionLevel(0);
            enemy.UpdatePosition(new Vec2(0f, 0f));
            member.UpdatePosition(new Vec2(7f, 0f));

            new EnemyAwarenessSystem().Execute(battleState);

            Assert.That(enemy.CurrentTargetMemberId.HasValue, Is.False);
        }

        [Test]
        public void FireAndToxicZones_ApplyDamageOrPressure_AndIgnoreDeadOrExtractedUnits()
        {
            BattleState battleState = CreateEnvironmentBattleState();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            BattleMemberState extractedMember = battleState.MembersById[new BattleEntityId(501)];
            BattleEnemyState enemy = battleState.EnemiesById[new BattleEntityId(9401)];
            BattleEnemyState deadEnemy = battleState.EnemiesById[new BattleEntityId(9402)];
            member.UpdatePosition(new Vec2(3.4f, -0.8f));
            extractedMember.MarkExtracted();
            extractedMember.UpdatePosition(new Vec2(3.4f, -0.8f));
            enemy.UpdatePosition(new Vec2(10.2f, 1.2f));
            deadEnemy.ApplyDamage(999);
            deadEnemy.UpdatePosition(new Vec2(10.2f, 1.2f));
            int startMemberHealth = member.Health;
            int extractedMemberHealth = extractedMember.Health;
            int startEnemyHealth = enemy.Health;
            int deadEnemyHealth = deadEnemy.Health;

            EnvironmentalZoneSystem environmentalSystem = new EnvironmentalZoneSystem();
            environmentalSystem.Execute(battleState, 1f);
            new DamageSystem().Execute(battleState);

            Assert.That(member.Health, Is.LessThan(startMemberHealth));
            Assert.That(extractedMember.Health, Is.EqualTo(extractedMemberHealth));
            Assert.That(enemy.Health, Is.LessThan(startEnemyHealth));
            Assert.That(deadEnemy.Health, Is.EqualTo(deadEnemyHealth));
            Assert.That(ContainsEvent(battleState, BattleEventTypes.FireDamageApplied), Is.True);
            Assert.That(ContainsEvent(battleState, BattleEventTypes.ToxicDamageApplied), Is.True);
        }

        [Test]
        public void TargetSelectionAndFire_IgnoreEnemyHiddenBySmoke()
        {
            BattleState battleState = CreateEnvironmentBattleState();
            ContentCatalog catalog = CreateEnvironmentCatalog();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            BattleEnemyState enemy = battleState.EnemiesById[new BattleEntityId(9401)];
            member.UpdatePosition(new Vec2(-4f, -2f));
            enemy.UpdatePosition(new Vec2(1f, -2f));

            new TargetSelectionSystem().Execute(battleState);
            Assert.That(member.CurrentTargetEnemyId.HasValue, Is.False);

            member.SetCurrentTargetEnemy(enemy.EnemyId);
            new FireSystem(catalog).Execute(battleState, 0.5f);

            Assert.That(battleState.PendingDamageRequests.Count, Is.EqualTo(0));
            Assert.That(ContainsEvent(battleState, BattleEventTypes.SmokeLineOfSightBlocked), Is.True);
        }

        [Test]
        public void ExistingWallRule_StillBlocksFire()
        {
            BattleState battleState = CreateEnvironmentBattleState();
            FireLineResult fireLine = FireLineRule.Evaluate(battleState, new Vec2(5f, 1.5f), new Vec2(10.5f, 1.5f));
            Assert.That(fireLine.CanFire, Is.False);
        }

        [Test]
        public void Smoke_DoesNotCountAsPhysicalWallForFireLineRule()
        {
            BattleState battleState = CreateEnvironmentBattleState();
            FireLineResult fireLine = FireLineRule.Evaluate(battleState, new Vec2(-4f, -2f), new Vec2(1f, -2f));

            Assert.That(fireLine.CanFire, Is.True);
            Assert.That(fireLine.BlockingObstacleId.HasValue, Is.False);
        }

        private static bool ContainsEvent(BattleState battleState, string eventType)
        {
            for (int i = 0; i < battleState.RecentEvents.Count; i++)
            {
                if (battleState.RecentEvents[i].EventType == eventType)
                {
                    return true;
                }
            }

            return false;
        }

        private static BattleState CreateEnvironmentBattleState()
        {
            return TestCombatContentFactory.CreateEnvironmentBattleState();
        }

        private static ContentCatalog CreateEnvironmentCatalog()
        {
            return TestCombatContentFactory.CreateEnvironmentCombatCatalog();
        }
    }
}
