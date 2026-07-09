using NUnit.Framework;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Core.Math;

namespace Warzone.Tests.Combat
{
    public sealed class M4SpatialCombatTests
    {
        [Test]
        public void CanCreateLowCoverHighCoverWallAndWindowObstacles()
        {
            BattleState battleState = CreateBattleState();

            Assert.That(battleState.ObstaclesById[200].ObstacleType, Is.EqualTo(TacticalObstacleType.LowCover));
            Assert.That(battleState.ObstaclesById[204].ObstacleType, Is.EqualTo(TacticalObstacleType.HighCover));
            Assert.That(battleState.ObstaclesById[206].ObstacleType, Is.EqualTo(TacticalObstacleType.BuildingBlocker));
            Assert.That(battleState.ObstaclesById[208].ObstacleType, Is.EqualTo(TacticalObstacleType.Window));
        }

        [Test]
        public void WallBlocksLineOfSight_AndLowCoverDoesNotFullyBlockFire()
        {
            BattleState battleState = CreateBattleState();

            LineOfSightResult blocked = LineOfSightRule.Evaluate(battleState, new Vec2(5f, 1.5f), new Vec2(10.5f, 1.5f));
            FireLineResult lowCoverFire = FireLineRule.Evaluate(battleState, new Vec2(-6f, -2.4f), new Vec2(-3.4f, -2.4f));

            Assert.That(blocked.HasLineOfSight, Is.False);
            Assert.That(lowCoverFire.CanFire, Is.True);
        }

        [Test]
        public void HighCoverOrWallBlocksFire_AndWindowAllowsSight()
        {
            BattleState battleState = CreateBattleState();

            FireLineResult blockedFire = FireLineRule.Evaluate(battleState, new Vec2(5f, 1.5f), new Vec2(10.5f, 1.5f));
            LineOfSightResult windowSight = LineOfSightRule.Evaluate(battleState, new Vec2(6.5f, 2.4f), new Vec2(11f, 2.4f));

            Assert.That(blockedFire.CanFire, Is.False);
            Assert.That(windowSight.HasLineOfSight, Is.True);
        }

        [Test]
        public void MemberAssignedToCover_GetsCoverNode_AndDeadOrExtractedMembersDoNotHoldCover()
        {
            BattleState battleState = CreateBattleState();
            BattleSimulation simulation = CreateSimulation();
            simulation.Enqueue(battleState, new DefendAreaCommand(1, new Vec2(2f, 2f), 8f));
            simulation.Tick(battleState, 2f);

            BattleMemberState coveredMember = battleState.MembersById[new BattleEntityId(500)];
            Assert.That(coveredMember.OccupiedTacticalNodeId.HasValue || coveredMember.CurrentIntent.IntentType == MemberIntentType.TakeCover || coveredMember.CurrentIntent.IntentType == MemberIntentType.HoldPosition, Is.True);

            coveredMember.ApplyDamage(200);
            new DeathCleanupSystem().Execute(battleState);
            Assert.That(coveredMember.OccupiedTacticalNodeId.HasValue, Is.False);
        }

        [Test]
        public void CoverReducesIncomingDamage_AndNoCoverMeansFullDamage()
        {
            BattleState battleState = CreateBattleState();
            BattleMemberState memberState = battleState.MembersById[new BattleEntityId(500)];
            BattleEnemyState enemyState = battleState.EnemiesById[new BattleEntityId(9401)];

            memberState.UpdatePosition(new Vec2(-4.2f, -2.4f));
            enemyState.UpdatePosition(new Vec2(-1.5f, -2.4f));
            FireLineResult coveredFire = FireLineRule.Evaluate(battleState, enemyState.Position, memberState.Position);
            battleState.EnqueueDamage(new PendingDamageRequest(enemyState.EnemyId, memberState.MemberId, 20, "sandbox.raider", true, coveredFire.DamageMultiplier, coveredFire.CoverObstacleId));
            new DamageSystem().Execute(battleState);
            int coveredHealth = memberState.Health;

            BattleState openBattleState = CreateBattleState();
            BattleMemberState openMember = openBattleState.MembersById[new BattleEntityId(500)];
            BattleEnemyState openEnemy = openBattleState.EnemiesById[new BattleEntityId(9401)];
            openMember.UpdatePosition(new Vec2(-10f, 0f));
            openEnemy.UpdatePosition(new Vec2(-7f, 0f));
            FireLineResult openFire = FireLineRule.Evaluate(openBattleState, openEnemy.Position, openMember.Position);
            openBattleState.EnqueueDamage(new PendingDamageRequest(openEnemy.EnemyId, openMember.MemberId, 20, "sandbox.raider", true, openFire.DamageMultiplier, openFire.CoverObstacleId));
            new DamageSystem().Execute(openBattleState);

            Assert.That(coveredHealth, Is.GreaterThan(openMember.Health));
        }

        [Test]
        public void MemberAndEnemyPerception_RespectWallLineOfSight()
        {
            BattleState battleState = CreateBattleState();
            BattleMemberState memberState = battleState.MembersById[new BattleEntityId(500)];
            BattleEnemyState enemyState = battleState.EnemiesById[new BattleEntityId(9401)];
            memberState.UpdatePosition(new Vec2(5f, 1.5f));
            enemyState.UpdatePosition(new Vec2(10.5f, 1.5f));

            new PerceptionSystem().Execute(battleState);
            new EnemyAwarenessSystem().Execute(battleState);

            Assert.That(memberState.VisibleEnemyIds.Count, Is.EqualTo(0));
            Assert.That(enemyState.CurrentTargetMemberId.HasValue, Is.False);
        }

        [Test]
        public void MemberAndEnemyCannotShootThroughBlockedFireLine()
        {
            BattleState battleState = CreateBattleState();
            ContentCatalog catalog = CreateCatalog();
            BattleMemberState memberState = battleState.MembersById[new BattleEntityId(500)];
            BattleEnemyState enemyState = battleState.EnemiesById[new BattleEntityId(9401)];
            memberState.UpdatePosition(new Vec2(5f, 1.5f));
            enemyState.UpdatePosition(new Vec2(10.5f, 1.5f));
            memberState.SetCurrentTargetEnemy(enemyState.EnemyId);
            enemyState.SetCurrentTargetMember(memberState.MemberId);

            new FireSystem(catalog).Execute(battleState, 1f);
            new EnemyFireSystem(catalog).Execute(battleState, 1f);

            Assert.That(battleState.PendingDamageRequests.Count, Is.EqualTo(0));
            Assert.That(ContainsEvent(battleState, BattleEventTypes.ShotBlocked), Is.True);
        }

        [Test]
        public void BuildingStateReferencesWindowAndDoorNodes_AndDefendCanAssignWindowNode()
        {
            BattleState battleState = CreateBattleState();
            BuildingState buildingState = battleState.BuildingsById[100];
            BattleSimulation simulation = CreateSimulation();

            simulation.Enqueue(battleState, new DefendAreaCommand(1, new Vec2(7.5f, 2.2f), 4f));
            simulation.Tick(battleState, 0f);

            Assert.That(buildingState.TacticalNodeIds.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(battleState.TacticalNodesById[16].NodeType, Is.EqualTo(TacticalNodeType.Window));
            Assert.That(battleState.TacticalNodesById[18].NodeType, Is.EqualTo(TacticalNodeType.Doorway));

            bool assignedToWindow = false;
            foreach (BattleEntityId memberId in battleState.SquadsById[1].MemberIds)
            {
                BattleMemberState memberState = battleState.MembersById[memberId];
                if (memberState.CurrentIntent != null && memberState.CurrentIntent.TacticalNodeId.HasValue && memberState.CurrentIntent.TacticalNodeId.Value == 16)
                {
                    assignedToWindow = true;
                    break;
                }
            }

            Assert.That(assignedToWindow, Is.True);
        }

        [Test]
        public void SearchEliminateExtractObjectives_ProduceBattleResult()
        {
            BattleState battleState = CreateBattleState();
            BattleResultSystem resultSystem = new BattleResultSystem(CreateCatalog());

            battleState.TacticalNodesById[20].MarkSearchStarted();
            battleState.TacticalNodesById[20].MarkSearched();
            battleState.TacticalNodesById[20].MarkLootDiscovered();

            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                enemyState.ApplyDamage(999);
            }

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                memberState.MarkExtracted();
            }

            BattleMissionStatusSnapshot missionStatus = resultSystem.UpdateMissionStatus(battleState);
            BattleResult battleResult = resultSystem.UpdateBattleResult(battleState);

            Assert.That(missionStatus.IsSearchObjectiveComplete, Is.True);
            Assert.That(missionStatus.IsEliminateObjectiveComplete, Is.True);
            Assert.That(missionStatus.IsExtractObjectiveComplete, Is.True);
            Assert.That(battleResult, Is.Not.Null);
            Assert.That(battleResult.CompletionType, Is.EqualTo(BattleCompletionType.Success));
        }

        [Test]
        public void BattleResult_IncludesCasualtiesExtractedMembersAndLoot()
        {
            BattleState battleState = CreateBattleState();
            BattleResultSystem resultSystem = new BattleResultSystem(CreateCatalog());
            battleState.AddEvent(new BattleEventRecord(BattleEventTypes.LootDiscovered, 1, new BattleEntityId(500), "supplies", null, 2));
            battleState.MembersById[new BattleEntityId(500)].ApplyDamage(999);
            battleState.MembersById[new BattleEntityId(501)].MarkExtracted();
            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                enemyState.ApplyDamage(999);
            }
            battleState.TacticalNodesById[20].MarkSearchStarted();
            battleState.TacticalNodesById[20].MarkSearched();
            battleState.MembersById[new BattleEntityId(502)].MarkExtracted();
            battleState.MembersById[new BattleEntityId(503)].MarkExtracted();

            resultSystem.UpdateMissionStatus(battleState);
            BattleResult battleResult = resultSystem.UpdateBattleResult(battleState);

            Assert.That(battleResult.CasualtyResult.DeadMemberIds.Count, Is.EqualTo(1));
            Assert.That(battleResult.ExtractionResult.ExtractedMemberIds.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(battleResult.LootResult.LootCount, Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void M4ScenarioFactory_CreatesSquadMembersEnemiesNodesObstaclesAndObjectives()
        {
            BattleState battleState = CreateBattleState();

            Assert.That(battleState.SquadsById.Count, Is.EqualTo(1));
            Assert.That(battleState.MembersById.Count, Is.EqualTo(4));
            Assert.That(battleState.EnemiesById.Count, Is.GreaterThanOrEqualTo(5));
            Assert.That(battleState.TacticalNodesById.Count, Is.GreaterThan(0));
            Assert.That(battleState.ObstaclesById.Count, Is.GreaterThan(0));
            Assert.That(battleState.BuildingsById.Count, Is.GreaterThan(0));

            Assert.That(ContainsNodeType(battleState, TacticalNodeType.SearchPoint), Is.True);
            Assert.That(ContainsNodeType(battleState, TacticalNodeType.ExtractionPoint), Is.True);
        }

        private static BattleSimulation CreateSimulation()
        {
            return new BattleSimulation(CreateCatalog());
        }

        private static ContentCatalog CreateCatalog()
        {
            return TestCombatContentFactory.CreateSpatialCombatCatalog();
        }

        private static BattleState CreateBattleState()
        {
            return TestCombatContentFactory.CreateSpatialBattleState();
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

        private static bool ContainsNodeType(BattleState battleState, TacticalNodeType nodeType)
        {
            foreach (TacticalNodeState nodeState in battleState.TacticalNodesById.Values)
            {
                if (nodeState.NodeType == nodeType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
