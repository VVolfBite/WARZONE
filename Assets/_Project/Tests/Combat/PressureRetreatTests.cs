using NUnit.Framework;
using Warzone.Combat;
using Warzone.Core.Math;
using Warzone.Content;

namespace Warzone.Tests.Combat
{
    public sealed class PressureRetreatTests
    {
        [Test]
        public void Damage_IncreasesPressure()
        {
            BattleState battleState = CreateBattleState();
            BattleMemberState member = GetFirstMember(battleState);

            battleState.AddEvent(new BattleEventRecord(
                BattleEventTypes.DamageApplied,
                member.SquadId,
                new BattleEntityId(9401),
                "sandbox.raider",
                member.MemberId,
                10));

            new PressureSystem().Execute(battleState, 0f);

            Assert.That(member.Pressure, Is.EqualTo(PressureGainRule.FromDamage(10)).Within(0.01f));
        }

        [Test]
        public void Pressure_RecoversOverTimeWithoutIncomingFire()
        {
            BattleState battleState = CreateBattleState();
            BattleMemberState member = GetFirstMember(battleState);
            member.SetPressure(40f);

            new PressureSystem().Execute(battleState, 1f);

            Assert.That(member.Pressure, Is.LessThan(40f));
        }

        [Test]
        public void DeadOrExtractedMembers_DoNotGainActivePressure()
        {
            BattleState battleState = CreateBattleState();
            BattleMemberState deadMember = GetFirstMember(battleState);
            BattleMemberState extractedMember = GetSecondMember(battleState);
            deadMember.ApplyDamage(deadMember.Health);
            extractedMember.MarkExtracted();

            battleState.AddEvent(new BattleEventRecord(BattleEventTypes.WeaponFired, 1, new BattleEntityId(9401), "incoming", deadMember.MemberId, 0));
            battleState.AddEvent(new BattleEventRecord(BattleEventTypes.WeaponFired, 1, new BattleEntityId(9401), "incoming", extractedMember.MemberId, 0));

            new PressureSystem().Execute(battleState, 0f);

            Assert.That(deadMember.Pressure, Is.EqualTo(0f));
            Assert.That(extractedMember.Pressure, Is.EqualTo(0f));
        }

        [Test]
        public void SuppressedMember_HasMovementAndFirePenalty()
        {
            BattleState battleState = CreateBattleState();
            BattleMemberState member = GetFirstMember(battleState);
            member.SetPressure(member.MaxPressure * 0.5f);
            PressureSystem.UpdateStatusFlags(battleState, member);

            Assert.That(member.IsSuppressed, Is.True);
            Assert.That(SuppressionRule.ApplyMovementPenalty(member.MovementSpeed, member.IsSuppressed), Is.EqualTo(member.MovementSpeed * SuppressionRule.MovementSpeedMultiplier).Within(0.001f));
            Assert.That(SuppressionRule.ApplyAttackCooldownPenalty(1f, member.IsSuppressed), Is.EqualTo(SuppressionRule.AttackCooldownMultiplier).Within(0.001f));
        }

        [Test]
        public void BrokenMember_EntersRetreatingState()
        {
            BattleState battleState = CreateBattleState();
            BattleMemberState member = GetFirstMember(battleState);
            member.SetPressure(member.MaxPressure);
            PressureSystem.UpdateStatusFlags(battleState, member);

            new RetreatSystem().Execute(battleState);

            Assert.That(member.IsBroken, Is.True);
            Assert.That(member.IsRetreating, Is.True);
            Assert.That(member.CurrentIntent, Is.Not.Null);
            Assert.That(member.CurrentIntent.IntentType, Is.EqualTo(MemberIntentType.Retreat));
        }

        [Test]
        public void RetreatTarget_UsesNearestRallyOrExtractionPoint()
        {
            BattleState battleState = CreateBattleState();
            BattleStateFactory factory = new BattleStateFactory();
            battleState.AddTacticalNode(factory.CreateTacticalNode(700, TacticalNodeType.RallyPoint, new Vec2(-3f, 0f), 1f));
            battleState.AddTacticalNode(factory.CreateTacticalNode(701, TacticalNodeType.ExtractionPoint, new Vec2(15f, 0f), 1f));

            BattleMemberState member = GetFirstMember(battleState);
            member.UpdatePosition(new Vec2(0f, 0f));
            member.SetPressure(member.MaxPressure);
            PressureSystem.UpdateStatusFlags(battleState, member);

            new RetreatSystem().Execute(battleState);

            Assert.That(member.RetreatTargetPosition.HasValue, Is.True);
            Assert.That(member.RetreatTargetPosition.Value, Is.EqualTo(new Vec2(-3f, 0f)));
        }

        [Test]
        public void RetreatTarget_FallsBackAwayFromNearestEnemyWhenNoSafeNodeExists()
        {
            BattleState battleState = CreateBattleState();
            BattleMemberState member = GetFirstMember(battleState);
            member.UpdatePosition(new Vec2(0f, 0f));
            member.SetPressure(member.MaxPressure);
            PressureSystem.UpdateStatusFlags(battleState, member);
            battleState.AddEnemy(new BattleEnemyState(new BattleEntityId(9500), "sandbox.raider", Content.Definitions.FactionId.Enemy, new Vec2(2f, 0f), 50, 50, 2f, 10f, 6f));

            new RetreatSystem().Execute(battleState);

            Assert.That(member.RetreatTargetPosition.HasValue, Is.True);
            Assert.That(member.RetreatTargetPosition.Value.X, Is.LessThan(0f));
        }

        [Test]
        public void RetreatingMember_IgnoresNormalDefendAssignment()
        {
            BattleState battleState = TestCombatContentFactory.CreateSpatialBattleState();
            BattleMemberState member = GetFirstMember(battleState);
            member.SetPressure(member.MaxPressure);
            PressureSystem.UpdateStatusFlags(battleState, member);
            new RetreatSystem().Execute(battleState);
            Vec2 retreatTarget = member.CurrentIntent.TargetPosition;

            BattleSquadState squad = battleState.SquadsById[1];
            squad.SetCurrentOrder(new DefendAreaCommand(1, new Vec2(1f, 2f), 3f));
            new FormationSystem().Execute(battleState);

            Assert.That(member.CurrentIntent.IntentType, Is.EqualTo(MemberIntentType.Retreat));
            Assert.That(member.CurrentIntent.TargetPosition, Is.EqualTo(retreatTarget));
        }

        [Test]
        public void EnemyAttacks_CanProducePressureChanges()
        {
            ContentCatalog catalog = TestCombatContentFactory.CreateSpatialCombatCatalog();
            BattleState battleState = TestCombatContentFactory.CreateSpatialBattleState();
            BattleSimulation simulation = new BattleSimulation(catalog);
            BattleMemberState member = GetFirstMember(battleState);

            member.UpdatePosition(new Vec2(6.5f, 2.4f));
            simulation.Tick(battleState, 0.5f);

            Assert.That(member.Pressure, Is.GreaterThan(0f));
        }

        [Test]
        public void SustainedPressure_ProducesRetreatEventAndSnapshotFields()
        {
            BattleState battleState = CreateBattleState();
            BattleStateFactory factory = new BattleStateFactory();
            battleState.AddTacticalNode(factory.CreateTacticalNode(710, TacticalNodeType.RallyPoint, new Vec2(-4f, 0f), 1f));
            BattleMemberState member = GetFirstMember(battleState);

            member.SetPressure(member.MaxPressure);
            PressureSystem.UpdateStatusFlags(battleState, member);
            new RetreatSystem().Execute(battleState);

            BattleSnapshot snapshot = BattleSnapshotFactory.Create(battleState);

            Assert.That(ContainsEvent(battleState, BattleEventTypes.MemberStartedRetreat), Is.True);
            Assert.That(snapshot.Members[0].Pressure, Is.EqualTo(member.Pressure).Within(0.01f));
            Assert.That(snapshot.Members[0].IsBroken, Is.True);
            Assert.That(snapshot.Members[0].IsRetreating, Is.True);
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

        private static BattleState CreateBattleState()
        {
            BattleStateFactory factory = new BattleStateFactory();
            return factory.CreateMemberSquadBattle("pressure-test", 1, Content.Definitions.FactionId.Player, Vec2.Zero, 4, 1.5f);
        }

        private static BattleMemberState GetFirstMember(BattleState battleState)
        {
            return battleState.MembersById[battleState.SquadsById[1].MemberIds[0]];
        }

        private static BattleMemberState GetSecondMember(BattleState battleState)
        {
            return battleState.MembersById[battleState.SquadsById[1].MemberIds[1]];
        }
    }
}
