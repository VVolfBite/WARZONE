using NUnit.Framework;
using Warzone.Combat;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Tests.Combat
{
    public sealed class U1PlaytestFeedbackCombatTests
    {
        [Test]
        public void MovementBlockingRule_WallBlocksMovement()
        {
            BattleState battleState = CreateSingleMemberBattle();
            battleState.AddObstacle(CreateObstacle(10, TacticalObstacleType.Wall, new Vec2(2f, 0f), true));
            BattleMemberState member = battleState.MembersById[new BattleEntityId(1)];
            member.SetIntent(new MemberIntent(MemberIntentType.MoveToPosition, new Vec2(5f, 0f)));

            new MovementSystem().Execute(battleState, 2f);

            Assert.That(member.Position.X, Is.LessThan(2f));
            Assert.That(member.CurrentIntent.IsCompleted, Is.False);
            Assert.That(ContainsEvent(battleState, BattleEventTypes.MovementBlocked), Is.True);
        }

        [Test]
        public void MovementBlockingRule_BuildingBlockerBlocksMovement()
        {
            BattleState battleState = CreateSingleMemberBattle();
            battleState.AddObstacle(CreateObstacle(11, TacticalObstacleType.BuildingBlocker, new Vec2(2f, 0f), true));
            BattleMemberState member = battleState.MembersById[new BattleEntityId(1)];
            member.SetIntent(new MemberIntent(MemberIntentType.MoveToPosition, new Vec2(5f, 0f)));

            new MovementSystem().Execute(battleState, 2f);

            Assert.That(member.CurrentIntent.IsCompleted, Is.False);
            Assert.That(ContainsEvent(battleState, BattleEventTypes.MovementBlocked), Is.True);
        }

        [Test]
        public void MovementBlockingRule_NonBlockingLowCoverDoesNotBlockMovement()
        {
            BattleState battleState = CreateSingleMemberBattle();
            battleState.AddObstacle(CreateObstacle(12, TacticalObstacleType.LowCover, new Vec2(2f, 0f), false));
            BattleMemberState member = battleState.MembersById[new BattleEntityId(1)];
            member.SetIntent(new MemberIntent(MemberIntentType.MoveToPosition, new Vec2(5f, 0f)));

            new MovementSystem().Execute(battleState, 2f);

            Assert.That(member.Position, Is.EqualTo(new Vec2(5f, 0f)));
            Assert.That(member.CurrentIntent.IsCompleted, Is.True);
            Assert.That(ContainsEvent(battleState, BattleEventTypes.MovementBlocked), Is.False);
        }

        [Test]
        public void MovementBlockingRule_UnblockedMovementSucceeds()
        {
            BattleState battleState = CreateSingleMemberBattle();
            battleState.AddObstacle(CreateObstacle(13, TacticalObstacleType.Wall, new Vec2(2f, 3f), true));
            BattleMemberState member = battleState.MembersById[new BattleEntityId(1)];
            member.SetIntent(new MemberIntent(MemberIntentType.MoveToPosition, new Vec2(5f, 0f)));

            new MovementSystem().Execute(battleState, 2f);

            Assert.That(member.CurrentIntent.IsCompleted, Is.True);
            Assert.That(ContainsEvent(battleState, BattleEventTypes.MovementBlocked), Is.False);
        }

        [Test]
        public void FormationSystem_RepeatedSameCommandDoesNotSpamReservationEvents()
        {
            BattleState battleState = TestCombatContentFactory.CreateBuildingTacticsBattleState();
            ProcessAndPlan(battleState, new DefendBuildingCommand(1, 100));
            int firstReservationCount = CountEvents(battleState, BattleEventTypes.TacticalNodeReserved);

            new SquadPlanningSystem().Execute(battleState);
            new FormationSystem().Execute(battleState);

            Assert.That(CountEvents(battleState, BattleEventTypes.TacticalNodeReserved), Is.EqualTo(firstReservationCount));
        }

        [Test]
        public void FormationSystem_SameCommandKeepsSameMemberTarget()
        {
            BattleState battleState = TestCombatContentFactory.CreateBuildingTacticsBattleState();
            ProcessAndPlan(battleState, new DefendBuildingCommand(1, 100));
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            Vec2 firstTarget = member.CurrentIntent.TargetPosition;
            int? firstNode = member.CurrentIntent.TacticalNodeId;

            new SquadPlanningSystem().Execute(battleState);
            new FormationSystem().Execute(battleState);

            Assert.That(member.CurrentIntent.TargetPosition, Is.EqualTo(firstTarget));
            Assert.That(member.CurrentIntent.TacticalNodeId, Is.EqualTo(firstNode));
        }

        [Test]
        public void BattleSnapshot_IncludesMemberIntentTarget()
        {
            BattleState battleState = CreateSingleMemberBattle();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(1)];
            member.SetIntent(new MemberIntent(MemberIntentType.MoveToPosition, new Vec2(3f, 2f)));

            BattleSnapshot snapshot = BattleSnapshotFactory.Create(battleState);

            Assert.That(snapshot.Members[0].MoveTarget, Is.EqualTo((Vec2?)new Vec2(3f, 2f)));
        }

        private static BattleState CreateSingleMemberBattle()
        {
            return new BattleStateFactory().CreateMemberSquadBattle("u1", 1, FactionId.Player, Vec2.Zero, 1, 1f, movementSpeed: 4f);
        }

        private static TacticalObstacleState CreateObstacle(int id, TacticalObstacleType type, Vec2 position, bool blocksMovement)
        {
            return new TacticalObstacleState(id, type, position, 0.5f, blocksMovement, true, true, true, 0.5f, 0.2f);
        }

        private static void ProcessAndPlan(BattleState battleState, BattleCommand command)
        {
            CommandSystem commandSystem = new CommandSystem();
            commandSystem.Enqueue(battleState, command);
            commandSystem.Process(battleState);
            new SquadPlanningSystem().Execute(battleState);
            new FormationSystem().Execute(battleState);
        }

        private static bool ContainsEvent(BattleState battleState, string eventType)
        {
            return CountEvents(battleState, eventType) > 0;
        }

        private static int CountEvents(BattleState battleState, string eventType)
        {
            int count = 0;
            for (int i = 0; i < battleState.RecentEvents.Count; i++)
            {
                if (battleState.RecentEvents[i].EventType == eventType)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
