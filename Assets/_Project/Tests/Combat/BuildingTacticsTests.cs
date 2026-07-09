using NUnit.Framework;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Core.Math;

namespace Warzone.Tests.Combat
{
    public sealed class BuildingTacticsTests
    {
        [Test]
        public void SearchCompletion_WritesMissionRuntimeState()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            member.UpdatePosition(new Vec2(7.5f, 1.4f));
            member.SetIntent(new MemberIntent(MemberIntentType.SearchPoint, new Vec2(7.5f, 1.4f), false, 30));

            new SearchSystem().Execute(battleState, 3.1f);

            Assert.That(battleState.MissionRuntimeState.HasCompletedSearchPoint(30), Is.True);
            Assert.That(battleState.MissionRuntimeState.LootDiscoveredCount, Is.EqualTo(1));
        }

        [Test]
        public void LootCount_RemainsAfterRecentEventsOverflow_AndBattleResultUsesRuntime()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            ContentCatalog catalog = CreateBuildingTacticsCatalog();
            CompleteBattleUsingRuntimeState(battleState);

            for (int i = 0; i < 40; i++)
            {
                battleState.AddEvent(new BattleEventRecord(BattleEventTypes.CommandAccepted, 1, new BattleEntityId(500), "overflow-" + i));
            }

            BattleResultSystem resultSystem = new BattleResultSystem(catalog);
            resultSystem.UpdateMissionStatus(battleState);
            BattleResult battleResult = resultSystem.UpdateBattleResult(battleState);

            Assert.That(battleState.MissionRuntimeState.LootDiscoveredCount, Is.EqualTo(1));
            Assert.That(battleResult, Is.Not.Null);
            Assert.That(battleResult.LootResult.LootCount, Is.EqualTo(1));
        }

        [Test]
        public void ObjectivesRemainCompleted_AfterEventBufferClears()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            ContentCatalog catalog = CreateBuildingTacticsCatalog();
            CompleteBattleUsingRuntimeState(battleState);
            battleState.ClearFrameEvents();

            BattleMissionStatusSnapshot missionStatus = new BattleResultSystem(catalog).UpdateMissionStatus(battleState);

            Assert.That(missionStatus.IsEnterBuildingObjectiveComplete, Is.True);
            Assert.That(missionStatus.IsSearchObjectiveComplete, Is.True);
            Assert.That(missionStatus.IsEliminateObjectiveComplete, Is.True);
            Assert.That(missionStatus.IsExtractObjectiveComplete, Is.True);
        }

        [Test]
        public void EnterBuildingCommand_AssignsMembersToBuildingNodes()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            ProcessAndPlan(battleState, new EnterBuildingCommand(1, 100));

            int assignedCount = 0;
            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (memberState.CurrentIntent == null || !memberState.CurrentIntent.TacticalNodeId.HasValue)
                {
                    continue;
                }

                TacticalNodeState nodeState;
                Assert.That(battleState.TryGetTacticalNode(memberState.CurrentIntent.TacticalNodeId.Value, out nodeState), Is.True);
                Assert.That(nodeState.BuildingId, Is.EqualTo(100));
                assignedCount++;
            }

            Assert.That(assignedCount, Is.EqualTo(4));
        }

        [Test]
        public void DefendBuildingCommand_PrioritizesWindowNodes()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            ProcessAndPlan(battleState, new DefendBuildingCommand(1, 100));

            int windowAssignments = CountAssignmentsToNodes(battleState, 16, 17);
            Assert.That(windowAssignments, Is.EqualTo(2));
        }

        [Test]
        public void DefendBuildingCommand_FallsBackToInteriorWhenWindowsUnavailable()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            battleState.TacticalNodesById[16].SetOccupyingMember(new BattleEntityId(9901));
            battleState.TacticalNodesById[17].SetOccupyingMember(new BattleEntityId(9902));

            ProcessAndPlan(battleState, new DefendBuildingCommand(1, 100));

            int interiorAssignments = CountAssignmentsToNodes(battleState, 26, 27, 28, 29);
            Assert.That(interiorAssignments, Is.GreaterThan(0));
        }

        [Test]
        public void SearchBuildingCommand_TargetsBuildingSearchPoint()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            ProcessAndPlan(battleState, new SearchBuildingCommand(1, 100));

            bool foundSearchAssignment = false;
            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (memberState.CurrentIntent != null &&
                    memberState.CurrentIntent.IntentType == MemberIntentType.SearchPoint &&
                    memberState.CurrentIntent.TacticalNodeId == 30)
                {
                    foundSearchAssignment = true;
                    break;
                }
            }

            Assert.That(foundSearchAssignment, Is.True);
        }

        [Test]
        public void DeadExtractedBrokenMembers_AreNotAssignedBuildingPositions()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            battleState.MembersById[new BattleEntityId(500)].MarkExtracted();
            battleState.MembersById[new BattleEntityId(501)].ApplyDamage(999);
            battleState.MembersById[new BattleEntityId(502)].BeginRetreat(new Vec2(-10f, -3f));

            ProcessAndPlan(battleState, new EnterBuildingCommand(1, 100));

            Assert.That(battleState.MembersById[new BattleEntityId(500)].CurrentIntent, Is.Null);
            Assert.That(battleState.MembersById[new BattleEntityId(501)].CurrentIntent, Is.Null);
            Assert.That(battleState.MembersById[new BattleEntityId(502)].CurrentIntent, Is.Null);
            Assert.That(battleState.MembersById[new BattleEntityId(503)].CurrentIntent, Is.Not.Null);
        }

        [Test]
        public void ExteriorUnit_CannotSeeInteriorNonWindowMember()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            member.UpdatePosition(new Vec2(-10f, -3f));

            new PerceptionSystem().Execute(battleState);

            Assert.That(member.VisibleEnemyIds, Has.No.Member(new BattleEntityId(9401)));
        }

        [Test]
        public void InteriorNonWindowMember_CannotShootExteriorTarget()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            ContentCatalog catalog = CreateBuildingTacticsCatalog();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            BattleEnemyState enemy = battleState.EnemiesById[new BattleEntityId(9403)];
            member.UpdatePosition(new Vec2(6.8f, 1.4f));
            member.SetOccupiedTacticalNode(26);
            member.SetCurrentTargetEnemy(enemy.EnemyId);

            new FireSystem(catalog).Execute(battleState, 0.5f);

            Assert.That(battleState.PendingDamageRequests.Count, Is.EqualTo(0));
            Assert.That(ContainsEvent(battleState, BattleEventTypes.BuildingFireBlocked), Is.True);
        }

        [Test]
        public void WindowMember_CanSeeAndShootExteriorTarget()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            ContentCatalog catalog = CreateBuildingTacticsCatalog();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            BattleEnemyState enemy = battleState.EnemiesById[new BattleEntityId(9403)];
            member.UpdatePosition(new Vec2(6.4f, 2.7f));
            member.SetOccupiedTacticalNode(16);
            enemy.UpdatePosition(new Vec2(11.6f, 2.4f));

            new PerceptionSystem().Execute(battleState);
            Assert.That(member.VisibleEnemyIds, Has.Member(enemy.EnemyId));

            member.SetCurrentTargetEnemy(enemy.EnemyId);
            new FireSystem(catalog).Execute(battleState, 0.5f);

            Assert.That(battleState.PendingDamageRequests.Count, Is.EqualTo(1));
        }

        [Test]
        public void ExteriorEnemy_CanSeeAndShootWindowMember()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            ContentCatalog catalog = CreateBuildingTacticsCatalog();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            BattleEnemyState enemy = battleState.EnemiesById[new BattleEntityId(9403)];
            member.UpdatePosition(new Vec2(6.4f, 2.7f));
            member.SetOccupiedTacticalNode(16);
            enemy.UpdatePosition(new Vec2(11.6f, 2.4f));

            new EnemyAwarenessSystem().Execute(battleState);
            Assert.That(enemy.CurrentTargetMemberId, Is.EqualTo((BattleEntityId?)member.MemberId));

            new EnemyFireSystem(catalog).Execute(battleState, 0.5f);
            Assert.That(battleState.PendingDamageRequests.Count, Is.EqualTo(1));
        }

        [Test]
        public void WindowTarget_ReceivesCoverModifier()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            ContentCatalog catalog = CreateBuildingTacticsCatalog();
            BattleMemberState member = battleState.MembersById[new BattleEntityId(500)];
            BattleEnemyState enemy = battleState.EnemiesById[new BattleEntityId(9403)];
            member.UpdatePosition(new Vec2(6.4f, 2.7f));
            member.SetOccupiedTacticalNode(16);
            enemy.UpdatePosition(new Vec2(11.6f, 2.4f));
            enemy.SetCurrentTargetMember(member.MemberId);
            int startHealth = member.Health;

            new EnemyFireSystem(catalog).Execute(battleState, 0.5f);
            new DamageSystem().Execute(battleState);

            Assert.That(member.Health, Is.LessThan(startHealth));
            Assert.That(startHealth - member.Health, Is.LessThan(12));
        }

        [Test]
        public void BuildingBlocker_BlocksNormalLineOfSight()
        {
            BattleState battleState = CreateBuildingTacticsBattleState();
            LineOfSightResult result = LineOfSightRule.Evaluate(battleState, new Vec2(5.2f, 1.2f), new Vec2(10.4f, 1.2f));

            Assert.That(result.HasLineOfSight, Is.False);
        }

        private static void ProcessAndPlan(BattleState battleState, BattleCommand command)
        {
            CommandSystem commandSystem = new CommandSystem();
            commandSystem.Enqueue(battleState, command);
            commandSystem.Process(battleState);
            new SquadPlanningSystem().Execute(battleState);
            new FormationSystem().Execute(battleState);
        }

        private static int CountAssignmentsToNodes(BattleState battleState, params int[] nodeIds)
        {
            int count = 0;
            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                if (memberState.CurrentIntent == null || !memberState.CurrentIntent.TacticalNodeId.HasValue)
                {
                    continue;
                }

                for (int i = 0; i < nodeIds.Length; i++)
                {
                    if (memberState.CurrentIntent.TacticalNodeId.Value == nodeIds[i])
                    {
                        count++;
                        break;
                    }
                }
            }

            return count;
        }

        private static void CompleteBattleUsingRuntimeState(BattleState battleState)
        {
            battleState.MissionRuntimeState.MarkBuildingEntered(100);
            battleState.MissionRuntimeState.MarkSearchPointCompleted(30);
            battleState.MissionRuntimeState.LootRuntimeState.AddLootFromSearchPoint(30, 1);

            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                enemyState.ApplyDamage(999);
                battleState.MissionRuntimeState.MarkEnemyKilled(enemyState.EnemyId);
            }

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                memberState.MarkExtracted();
                battleState.MissionRuntimeState.MarkMemberExtracted(memberState.MemberId);
            }
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

        private static BattleState CreateBuildingTacticsBattleState()
        {
            return TestCombatContentFactory.CreateBuildingTacticsBattleState();
        }

        private static ContentCatalog CreateBuildingTacticsCatalog()
        {
            return TestCombatContentFactory.CreateBuildingTacticsCatalog();
        }
    }
}
