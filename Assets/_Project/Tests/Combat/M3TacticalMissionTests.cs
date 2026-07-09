using NUnit.Framework;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Core.Math;

namespace Warzone.Tests.Combat
{
    public sealed class M3TacticalMissionTests
    {
        [Test]
        public void TacticalNodes_CanCreateCoverSearchExtractionAndIngress()
        {
            BattleState battleState = CreateBattleState();

            Assert.That(battleState.TacticalNodesById[10].NodeType, Is.EqualTo(TacticalNodeType.Cover));
            Assert.That(battleState.TacticalNodesById[20].NodeType, Is.EqualTo(TacticalNodeType.SearchPoint));
            Assert.That(battleState.TacticalNodesById[21].NodeType, Is.EqualTo(TacticalNodeType.ExtractionPoint));
            Assert.That(battleState.TacticalNodesById[22].NodeType, Is.EqualTo(TacticalNodeType.EnemyIngress));
        }

        [Test]
        public void FindAvailableCoverNode_ReturnsNearestAvailableCover()
        {
            BattleState battleState = CreateBattleState();

            TacticalNodeState first = FindAvailableCoverNodeQuery.Execute(battleState, new Vec2(0f, 0f), 8f);
            first.ReserveFor(new BattleEntityId(999));
            TacticalNodeState second = FindAvailableCoverNodeQuery.Execute(battleState, new Vec2(0f, 0f), 8f);

            Assert.That(first, Is.Not.Null);
            Assert.That(second, Is.Not.Null);
            Assert.That(second.NodeId, Is.Not.EqualTo(first.NodeId));
        }

        [Test]
        public void SearchPoint_StartsIncomplete_AndExtractionPointCanBeFound()
        {
            BattleState battleState = CreateBattleState();

            TacticalNodeState searchNode = FindSearchPointQuery.Execute(battleState, new Vec2(0f, 0f));
            TacticalNodeState extractionNode = FindExtractionPointQuery.Execute(battleState, new Vec2(0f, 0f), 1);

            Assert.That(searchNode.IsSearched, Is.False);
            Assert.That(searchNode.SearchProgress, Is.EqualTo(0f));
            Assert.That(extractionNode, Is.Not.Null);
            Assert.That(extractionNode.NodeType, Is.EqualTo(TacticalNodeType.ExtractionPoint));
        }

        [Test]
        public void DefendAreaCommand_AssignsMembersToCoverNodes_WhenAvailable()
        {
            BattleState battleState = CreateBattleState();
            BattleSimulation simulation = CreateSimulation();

            simulation.Enqueue(battleState, new DefendAreaCommand(1, new Vec2(0f, 0f), 8f));
            simulation.Tick(battleState, 0f);

            int coverAssignments = 0;
            foreach (BattleEntityId memberId in battleState.SquadsById[1].MemberIds)
            {
                BattleMemberState memberState = battleState.MembersById[memberId];
                if (memberState.CanAct && memberState.CurrentIntent != null && memberState.CurrentIntent.IntentType == MemberIntentType.TakeCover)
                {
                    coverAssignments++;
                }
            }

            Assert.That(coverAssignments, Is.GreaterThan(0));
        }

        [Test]
        public void DefendAreaCommand_FallsBackToFormation_WhenNoCoverExists()
        {
            BattleState battleState = CreateBattleState(includeCoverNodes: false);
            BattleSimulation simulation = CreateSimulation();

            simulation.Enqueue(battleState, new DefendAreaCommand(1, new Vec2(0f, 0f), 8f));
            simulation.Tick(battleState, 0f);

            foreach (BattleEntityId memberId in battleState.SquadsById[1].MemberIds)
            {
                BattleMemberState memberState = battleState.MembersById[memberId];
                if (!memberState.CanAct)
                {
                    continue;
                }

                Assert.That(memberState.CurrentIntent, Is.Not.Null);
                Assert.That(memberState.CurrentIntent.IntentType, Is.EqualTo(MemberIntentType.MoveToPosition));
            }
        }

        [Test]
        public void DeadOrExtractedMembers_AreNotAssignedDefendPositions()
        {
            BattleState battleState = CreateBattleState();
            BattleSimulation simulation = CreateSimulation();
            BattleMemberState deadMember = battleState.MembersById[new BattleEntityId(401)];
            BattleMemberState extractedMember = battleState.MembersById[new BattleEntityId(402)];
            deadMember.ApplyDamage(200);
            extractedMember.MarkExtracted();

            simulation.Enqueue(battleState, new DefendAreaCommand(1, new Vec2(0f, 0f), 8f));
            simulation.Tick(battleState, 0f);

            Assert.That(deadMember.CurrentIntent, Is.Null);
            Assert.That(extractedMember.CurrentIntent, Is.Null);
        }

        [Test]
        public void SearchPointCommand_AssignsAtLeastOneMemberToSearch()
        {
            BattleState battleState = CreateBattleState();
            BattleSimulation simulation = CreateSimulation();

            simulation.Enqueue(battleState, new SearchPointCommand(1, 20));
            simulation.Tick(battleState, 0f);

            bool hasSearcher = false;
            foreach (BattleEntityId memberId in battleState.SquadsById[1].MemberIds)
            {
                BattleMemberState memberState = battleState.MembersById[memberId];
                if (memberState.CanAct && memberState.CurrentIntent != null && memberState.CurrentIntent.IntentType == MemberIntentType.SearchPoint)
                {
                    hasSearcher = true;
                    break;
                }
            }

            Assert.That(hasSearcher, Is.True);
        }

        [Test]
        public void SearchSystem_ProgressesOnlyWithinRadius_AndProducesEventsOnce()
        {
            BattleState battleState = CreateBattleState();
            SearchSystem searchSystem = new SearchSystem();
            BattleMemberState memberState = battleState.MembersById[new BattleEntityId(400)];
            memberState.SetIntent(new MemberIntent(MemberIntentType.SearchPoint, new Vec2(6f, 0f), false, 20));

            searchSystem.Execute(battleState, 1f);
            Assert.That(battleState.TacticalNodesById[20].SearchProgress, Is.EqualTo(0f));

            memberState.UpdatePosition(new Vec2(6f, 0f));
            searchSystem.Execute(battleState, 2f);
            searchSystem.Execute(battleState, 2f);

            Assert.That(battleState.TacticalNodesById[20].IsSearched, Is.True);
            Assert.That(CountEvents(battleState, BattleEventTypes.SearchCompleted), Is.EqualTo(1));
            Assert.That(CountEvents(battleState, BattleEventTypes.LootDiscovered), Is.EqualTo(1));
        }

        [Test]
        public void ExtractSquadCommand_AssignsMembersAndExtractionMarksMemberAndSquad()
        {
            BattleState battleState = CreateBattleState();
            BattleSimulation simulation = CreateSimulation();

            simulation.Enqueue(battleState, new ExtractSquadCommand(1, 21));
            simulation.Tick(battleState, 0f);

            foreach (BattleEntityId memberId in battleState.SquadsById[1].MemberIds)
            {
                BattleMemberState memberState = battleState.MembersById[memberId];
                if (memberState.IsAlive)
                {
                    memberState.UpdatePosition(new Vec2(12f, -4f));
                }
            }

            new ExtractionSystem().Execute(battleState);

            Assert.That(battleState.MembersById[new BattleEntityId(400)].IsExtracted, Is.True);
            Assert.That(ContainsEvent(battleState, BattleEventTypes.SquadExtracted), Is.True);
        }

        [Test]
        public void ExtractedMember_NoLongerFires_AndIsNotTargetedByEnemy()
        {
            BattleState battleState = CreateBattleState();
            ContentCatalog catalog = CreateCatalog();
            BattleMemberState memberState = battleState.MembersById[new BattleEntityId(400)];
            BattleEnemyState enemyState = battleState.EnemiesById[new BattleEntityId(9301)];
            memberState.MarkExtracted();
            memberState.SetCurrentTargetEnemy(enemyState.EnemyId);

            new FireSystem(catalog).Execute(battleState, 1f);
            new EnemyAwarenessSystem().Execute(battleState);

            Assert.That(battleState.PendingDamageRequests.Count, Is.EqualTo(0));
            Assert.That(enemyState.CurrentTargetMemberId.HasValue, Is.False);
        }

        [Test]
        public void EnemyDetectsMovesAndFires_WhenMemberIsValid()
        {
            BattleState battleState = CreateBattleState();
            ContentCatalog catalog = CreateCatalog();
            BattleEnemyState enemyState = battleState.EnemiesById[new BattleEntityId(9301)];
            BattleMemberState memberState = battleState.MembersById[new BattleEntityId(400)];
            memberState.UpdatePosition(new Vec2(4f, 0f));
            enemyState.UpdatePosition(new Vec2(10f, 0f));

            new EnemyAwarenessSystem().Execute(battleState);
            Vec2 start = enemyState.Position;
            new EnemyBehaviorSystem().Execute(battleState, 1f);

            Assert.That(enemyState.CurrentTargetMemberId.HasValue, Is.True);
            Assert.That(enemyState.Position, Is.Not.EqualTo(start));

            enemyState.UpdatePosition(new Vec2(8f, 0f));
            new EnemyFireSystem(catalog).Execute(battleState, 1f);
            int firstShotCount = battleState.PendingDamageRequests.Count;
            new EnemyFireSystem(catalog).Execute(battleState, 0.1f);

            Assert.That(firstShotCount, Is.EqualTo(1));
            Assert.That(battleState.PendingDamageRequests.Count, Is.EqualTo(1));
        }

        [Test]
        public void EnemyIgnoresDeadOrExtractedMembers()
        {
            BattleState battleState = CreateBattleState();
            BattleEnemyState enemyState = battleState.EnemiesById[new BattleEntityId(9301)];
            battleState.MembersById[new BattleEntityId(400)].ApplyDamage(200);
            battleState.MembersById[new BattleEntityId(401)].MarkExtracted();

            new EnemyAwarenessSystem().Execute(battleState);

            Assert.That(enemyState.CurrentTargetMemberId.HasValue, Is.True);
            Assert.That(enemyState.CurrentTargetMemberId.Value, Is.Not.EqualTo(new BattleEntityId(400)));
            Assert.That(enemyState.CurrentTargetMemberId.Value, Is.Not.EqualTo(new BattleEntityId(401)));
        }

        [Test]
        public void EnemyDamage_ReducesMemberHealth_AndCanKillMember()
        {
            BattleState battleState = CreateBattleState();
            battleState.EnqueueDamage(new PendingDamageRequest(new BattleEntityId(9301), new BattleEntityId(400), 50, "sandbox.raider", true));
            battleState.EnqueueDamage(new PendingDamageRequest(new BattleEntityId(9301), new BattleEntityId(400), 60, "sandbox.raider", true));

            new DamageSystem().Execute(battleState);

            BattleMemberState memberState = battleState.MembersById[new BattleEntityId(400)];
            Assert.That(memberState.IsAlive, Is.False);
            Assert.That(ContainsEvent(battleState, BattleEventTypes.MemberKilled), Is.True);
        }

        [Test]
        public void DeadMember_StopsMovingFiringAndSearching()
        {
            BattleState battleState = CreateBattleState();
            ContentCatalog catalog = CreateCatalog();
            BattleMemberState memberState = battleState.MembersById[new BattleEntityId(400)];
            Vec2 start = memberState.Position;
            memberState.ApplyDamage(200);
            memberState.SetIntent(new MemberIntent(MemberIntentType.SearchPoint, new Vec2(6f, 0f), false, 20));
            memberState.SetCurrentTargetEnemy(new BattleEntityId(9301));

            new MovementSystem().Execute(battleState, 1f);
            new SearchSystem().Execute(battleState, 1f);
            new FireSystem(catalog).Execute(battleState, 1f);

            Assert.That(memberState.Position, Is.EqualTo(start));
            Assert.That(battleState.TacticalNodesById[20].SearchProgress, Is.EqualTo(0f));
            Assert.That(battleState.PendingDamageRequests.Count, Is.EqualTo(0));
        }

        [Test]
        public void Snapshot_IncludesNodesSearchProgressExtractedMembersEnemyCooldownAndEvents()
        {
            BattleState battleState = CreateBattleState();
            ContentCatalog catalog = CreateCatalog();
            BattleMemberState memberState = battleState.MembersById[new BattleEntityId(400)];
            BattleEnemyState enemyState = battleState.EnemiesById[new BattleEntityId(9301)];
            memberState.UpdatePosition(new Vec2(6f, 0f));
            memberState.SetIntent(new MemberIntent(MemberIntentType.SearchPoint, new Vec2(6f, 0f), false, 20));
            new SearchSystem().Execute(battleState, 3f);
            memberState.MarkExtracted();
            enemyState.SetCurrentTargetMember(new BattleEntityId(401));
            enemyState.ResetAttackCooldown(0.5f);
            battleState.AddEvent(new BattleEventRecord(BattleEventTypes.MemberExtracted, 1, memberState.MemberId));

            BattleSnapshot snapshot = BattleSnapshotFactory.Create(battleState);

            Assert.That(snapshot.TacticalNodes.Count, Is.GreaterThan(0));
            Assert.That(snapshot.TacticalNodes[0].NodeId, Is.GreaterThan(0));
            Assert.That(FindNode(snapshot, 20).IsSearched, Is.True);
            Assert.That(FindMember(snapshot, new BattleEntityId(400)).IsExtracted, Is.True);
            Assert.That(FindEnemy(snapshot, new BattleEntityId(9301)).CurrentTargetMemberId.HasValue, Is.True);
            Assert.That(FindEnemy(snapshot, new BattleEntityId(9301)).AttackCooldownRemaining, Is.EqualTo(0.5f));
            Assert.That(snapshot.RecentEvents.Count, Is.GreaterThan(0));
        }

        private static BattleSimulation CreateSimulation()
        {
            return new BattleSimulation(CreateCatalog());
        }

        private static ContentCatalog CreateCatalog()
        {
            return TestCombatContentFactory.CreateTacticalCatalog();
        }

        private static BattleState CreateBattleState(bool includeCoverNodes = true)
        {
            BattleStateFactory battleStateFactory = new BattleStateFactory();
            BattleState battleState = battleStateFactory.CreateMemberSquadBattle(
                "test.m3",
                1,
                FactionId.Player,
                new Vec2(-8f, 0f),
                4,
                1.5f,
                400,
                "sandbox.rifleman",
                "sandbox.rifle",
                100,
                4f,
                12f,
                11f);

            if (includeCoverNodes)
            {
                battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(10, TacticalNodeType.Cover, new Vec2(-0.5f, -1f), 0.8f));
                battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(11, TacticalNodeType.Cover, new Vec2(1.2f, -1.1f), 0.8f));
                battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(12, TacticalNodeType.DefensivePosition, new Vec2(2.5f, 0.5f), 0.8f));
            }

            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(20, TacticalNodeType.SearchPoint, new Vec2(6f, 0f), 1.2f, true, 3f));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(21, TacticalNodeType.ExtractionPoint, new Vec2(12f, -4f), 1.5f, true, 0f, 1));
            battleState.AddTacticalNode(battleStateFactory.CreateTacticalNode(22, TacticalNodeType.EnemyIngress, new Vec2(10f, 4f), 1f));

            battleState.AddEnemy(battleStateFactory.CreateEnemy(9301, "sandbox.raider", FactionId.Enemy, new Vec2(8f, 0f), 55, 2.5f, 14f, 7f));
            battleState.AddEnemy(battleStateFactory.CreateEnemy(9302, "sandbox.raider", FactionId.Enemy, new Vec2(10f, 2f), 55, 2.5f, 14f, 7f));
            return battleState;
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

        private static TacticalNodeSnapshot FindNode(BattleSnapshot snapshot, int nodeId)
        {
            for (int i = 0; i < snapshot.TacticalNodes.Count; i++)
            {
                if (snapshot.TacticalNodes[i].NodeId == nodeId)
                {
                    return snapshot.TacticalNodes[i];
                }
            }

            return null;
        }

        private static BattleMemberSnapshot FindMember(BattleSnapshot snapshot, BattleEntityId memberId)
        {
            for (int i = 0; i < snapshot.Members.Count; i++)
            {
                if (snapshot.Members[i].MemberId == memberId)
                {
                    return snapshot.Members[i];
                }
            }

            return null;
        }

        private static BattleEnemySnapshot FindEnemy(BattleSnapshot snapshot, BattleEntityId enemyId)
        {
            for (int i = 0; i < snapshot.Enemies.Count; i++)
            {
                if (snapshot.Enemies[i].EnemyId == enemyId)
                {
                    return snapshot.Enemies[i];
                }
            }

            return null;
        }
    }
}
