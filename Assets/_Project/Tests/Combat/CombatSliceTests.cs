using NUnit.Framework;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Tests.Combat
{
    public sealed class CombatSliceTests
    {
        [Test]
        public void MoveSquadCommand_StillAssignsMemberIntents()
        {
            BattleState battleState = CreateBattleState();
            BattleSimulation simulation = CreateSimulation();
            simulation.Enqueue(battleState, new MoveSquadCommand(1, new Vec2(4f, 0f)));

            simulation.Tick(battleState, 0f);

            foreach (BattleEntityId memberId in battleState.SquadsById[1].MemberIds)
            {
                Assert.That(battleState.MembersById[memberId].CurrentIntent, Is.Not.Null);
            }
        }

        [Test]
        public void MovementSystem_StillMovesMembers()
        {
            BattleState battleState = CreateBattleState();
            BattleSimulation simulation = CreateSimulation();
            BattleMemberState memberState = battleState.MembersById[battleState.SquadsById[1].MemberIds[0]];
            Vec2 start = memberState.Position;
            simulation.Enqueue(battleState, new MoveSquadCommand(1, new Vec2(5f, 0f)));

            simulation.Tick(battleState, 0.5f);

            Assert.That(memberState.Position, Is.Not.EqualTo(start));
        }

        [Test]
        public void Movement_DoesNotMoveDeadMembers()
        {
            BattleState battleState = CreateBattleState();
            BattleMemberState memberState = battleState.MembersById[battleState.SquadsById[1].MemberIds[0]];
            memberState.ApplyDamage(200);
            memberState.SetIntent(new MemberIntent(MemberIntentType.MoveToPosition, new Vec2(4f, 0f)));
            Vec2 start = memberState.Position;

            new MovementSystem().Execute(battleState, 1f);

            Assert.That(memberState.Position, Is.EqualTo(start));
        }

        [Test]
        public void Member_SeesEnemyWithinDetectionRange()
        {
            BattleState battleState = CreateBattleState();
            BattleMemberState memberState = battleState.MembersById[battleState.SquadsById[1].MemberIds[0]];

            new PerceptionSystem().Execute(battleState);

            Assert.That(memberState.VisibleEnemyIds.Count, Is.GreaterThan(0));
        }

        [Test]
        public void Member_DoesNotSeeEnemyOutsideDetectionRange()
        {
            BattleState battleState = CreateBattleState();
            BattleEnemyState enemyState = battleState.EnemiesById[new BattleEntityId(9101)];
            enemyState.UpdatePosition(new Vec2(40f, 0f));
            BattleMemberState memberState = battleState.MembersById[battleState.SquadsById[1].MemberIds[0]];

            new PerceptionSystem().Execute(battleState);

            Assert.That(memberState.VisibleEnemyIds.Count, Is.EqualTo(0));
        }

        [Test]
        public void TargetSelection_ChoosesNearestAliveEnemy()
        {
            BattleState battleState = CreateBattleState();
            new PerceptionSystem().Execute(battleState);

            new TargetSelectionSystem().Execute(battleState);

            BattleMemberState memberState = battleState.MembersById[battleState.SquadsById[1].MemberIds[0]];
            Assert.That(memberState.CurrentTargetEnemyId.HasValue, Is.True);
            Assert.That(memberState.CurrentTargetEnemyId.Value, Is.EqualTo(new BattleEntityId(9101)));
        }

        [Test]
        public void DeadEnemy_IsNotSelected()
        {
            BattleState battleState = CreateBattleState();
            BattleEnemyState enemyState = battleState.EnemiesById[new BattleEntityId(9101)];
            enemyState.ApplyDamage(999);
            new PerceptionSystem().Execute(battleState);

            new TargetSelectionSystem().Execute(battleState);

            BattleMemberState memberState = battleState.MembersById[battleState.SquadsById[1].MemberIds[0]];
            Assert.That(memberState.CurrentTargetEnemyId.HasValue, Is.True);
            Assert.That(memberState.CurrentTargetEnemyId.Value, Is.Not.EqualTo(new BattleEntityId(9101)));
        }

        [Test]
        public void MemberWithWeapon_FiresAtValidTarget()
        {
            BattleState battleState = CreateBattleState();
            ContentCatalog catalog = CreateCatalog();
            PrepareCombatTarget(battleState);

            new PerceptionSystem().Execute(battleState);
            new TargetSelectionSystem().Execute(battleState);
            new FireSystem(catalog).Execute(battleState, 0.1f);

            Assert.That(battleState.PendingDamageRequests.Count, Is.GreaterThan(0));
        }

        [Test]
        public void FireSystem_RespectsCooldown()
        {
            BattleState battleState = CreateBattleState();
            ContentCatalog catalog = CreateCatalog();
            PrepareCombatTarget(battleState);
            BattleMemberState memberState = battleState.MembersById[battleState.SquadsById[1].MemberIds[0]];

            new PerceptionSystem().Execute(battleState);
            new TargetSelectionSystem().Execute(battleState);
            FireSystem fireSystem = new FireSystem(catalog);
            fireSystem.Execute(battleState, 0.1f);
            int firstShotCount = battleState.PendingDamageRequests.Count;
            fireSystem.Execute(battleState, 0.1f);

            Assert.That(firstShotCount, Is.GreaterThan(0));
            Assert.That(battleState.PendingDamageRequests.Count, Is.EqualTo(firstShotCount));
            Assert.That(memberState.AttackCooldownRemaining, Is.GreaterThan(0f));
        }

        [Test]
        public void DamageSystem_ReducesEnemyHealth()
        {
            BattleState battleState = CreateBattleState();
            ContentCatalog catalog = CreateCatalog();
            PrepareCombatTarget(battleState);
            BattleEnemyState enemyState = battleState.EnemiesById[new BattleEntityId(9101)];
            int startHealth = enemyState.Health;

            new PerceptionSystem().Execute(battleState);
            new TargetSelectionSystem().Execute(battleState);
            new FireSystem(catalog).Execute(battleState, 0.1f);
            new DamageSystem().Execute(battleState);

            Assert.That(enemyState.Health, Is.LessThan(startHealth));
        }

        [Test]
        public void Enemy_IsMarkedDeadAndProducesKilledEvent()
        {
            BattleState battleState = CreateBattleState();
            ContentCatalog catalog = CreateCatalog();
            PrepareCombatTarget(battleState);
            BattleEnemyState enemyState = battleState.EnemiesById[new BattleEntityId(9101)];

            for (int i = 0; i < 4 && enemyState.IsAlive; i++)
            {
                new PerceptionSystem().Execute(battleState);
                new TargetSelectionSystem().Execute(battleState);
                new FireSystem(catalog).Execute(battleState, 1f);
                new DamageSystem().Execute(battleState);
            }

            Assert.That(enemyState.IsAlive, Is.False);
            Assert.That(ContainsEvent(battleState, BattleEventTypes.EnemyKilled), Is.True);
        }

        [Test]
        public void DeadEnemy_IsIgnoredByFutureTargetSelection()
        {
            BattleState battleState = CreateBattleState();
            BattleEnemyState enemyState = battleState.EnemiesById[new BattleEntityId(9101)];
            enemyState.ApplyDamage(999);
            new PerceptionSystem().Execute(battleState);
            new TargetSelectionSystem().Execute(battleState);

            BattleMemberState memberState = battleState.MembersById[battleState.SquadsById[1].MemberIds[0]];
            Assert.That(memberState.CurrentTargetEnemyId.Value, Is.Not.EqualTo(enemyState.EnemyId));
        }

        [Test]
        public void BattleSnapshot_IncludesEnemies_AndMemberCombatState()
        {
            BattleState battleState = CreateBattleState();
            BattleSimulation simulation = CreateSimulation();
            PrepareCombatTarget(battleState);

            simulation.Tick(battleState, 0.5f);
            BattleSnapshot snapshot = simulation.LatestSnapshot;

            Assert.That(snapshot.Enemies.Count, Is.EqualTo(3));
            Assert.That(snapshot.Enemies[0].MaxHealth, Is.EqualTo(45));
            Assert.That(snapshot.Members[0].WeaponId, Is.EqualTo("sandbox.rifle"));
            Assert.That(snapshot.Members[0].AttackCooldownRemaining, Is.GreaterThanOrEqualTo(0f));
        }

        private static void PrepareCombatTarget(BattleState battleState)
        {
            battleState.EnemiesById[new BattleEntityId(9101)].UpdatePosition(new Vec2(1.5f, 0f));
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

        private static BattleSimulation CreateSimulation()
        {
            return new BattleSimulation(CreateCatalog());
        }

        private static ContentCatalog CreateCatalog()
        {
            return TestCombatContentFactory.CreateCatalog();
        }

        private static BattleState CreateBattleState()
        {
            BattleStateFactory battleStateFactory = new BattleStateFactory();
            BattleState battleState = battleStateFactory.CreateMemberSquadBattle(
                "test.m2",
                1,
                FactionId.Player,
                new Vec2(-6f, -4f),
                4,
                1.4f,
                200,
                "sandbox.rifleman",
                "sandbox.rifle",
                100,
                4f,
                12f,
                10f);

            battleState.AddEnemy(battleStateFactory.CreateEnemy(9101, "sandbox.raider", FactionId.Enemy, new Vec2(6f, 0f), 45, 0f, 10f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9102, "sandbox.raider", FactionId.Enemy, new Vec2(9f, 2f), 45, 0f, 10f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9103, "sandbox.raider", FactionId.Enemy, new Vec2(8f, -3f), 45, 0f, 10f));
            return battleState;
        }
    }
}
