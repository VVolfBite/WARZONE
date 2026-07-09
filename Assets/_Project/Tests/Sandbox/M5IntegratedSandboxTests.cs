using NUnit.Framework;
using Warzone.Combat;
using Warzone.Sandbox.BattleSandbox;

namespace Warzone.Tests.Sandbox
{
    public sealed class M5IntegratedSandboxTests
    {
        [Test]
        public void M5ScenarioFactory_CreatesSquadMembersEnemiesNodesObstaclesAndObjectives()
        {
            M5IntegratedSandboxScenario scenario = M5IntegratedSandboxScenarioFactory.CreateScenario();

            Assert.That(scenario.BattleState.SquadsById.Count, Is.EqualTo(1));
            Assert.That(scenario.BattleState.MembersById.Count, Is.EqualTo(4));
            Assert.That(scenario.BattleState.EnemiesById.Count, Is.GreaterThanOrEqualTo(5));
            Assert.That(scenario.BattleState.TacticalNodesById.Count, Is.GreaterThan(0));
            Assert.That(scenario.BattleState.ObstaclesById.Count, Is.GreaterThan(0));
            Assert.That(scenario.BattleState.BuildingsById.Count, Is.GreaterThan(0));
            Assert.That(scenario.BattleState.CurrentBattleResult, Is.Null);
        }

        [Test]
        public void M5ScenarioFactory_ResultStartsIncomplete()
        {
            M5IntegratedSandboxScenario scenario = M5IntegratedSandboxScenarioFactory.CreateScenario();
            BattleMissionStatusSnapshot snapshot = BattleSnapshotFactory.Create(scenario.BattleState).MissionStatus;

            Assert.That(snapshot, Is.Not.Null);
            Assert.That(snapshot.IsBattleComplete, Is.False);
            Assert.That(snapshot.IsSearchObjectiveComplete, Is.False);
            Assert.That(snapshot.IsEliminateObjectiveComplete, Is.False);
            Assert.That(snapshot.IsExtractObjectiveComplete, Is.False);
        }

        [Test]
        public void BattleSandboxScenarioRegistry_ExposesM5IntegratedMode()
        {
            Assert.That(BattleSandboxScenarioRegistry.GetDisplayName(BattleSandboxMode.M5IntegratedSandbox), Is.EqualTo("M5 Integrated Sandbox"));
            Assert.That(BattleSandboxScenarioRegistry.IsCompatibilityEntry(BattleSandboxMode.M4SpatialCombat), Is.True);
            Assert.That(BattleSandboxScenarioRegistry.IsCompatibilityEntry(BattleSandboxMode.M5IntegratedSandbox), Is.False);
        }

        [Test]
        public void BattleSandboxCommandQueries_FindNearestSearchAndExtractionNode()
        {
            M5IntegratedSandboxScenario scenario = M5IntegratedSandboxScenarioFactory.CreateScenario();
            BattleSnapshot snapshot = BattleSnapshotFactory.Create(scenario.BattleState);
            BattleSquadSnapshot squad = BattleSandboxCommandQueries.FindSelectedSquad(snapshot, 1);

            TacticalNodeSnapshot searchNode = BattleSandboxCommandQueries.FindNearestNode(snapshot, squad.Position, TacticalNodeType.SearchPoint, true);
            TacticalNodeSnapshot extractionNode = BattleSandboxCommandQueries.FindNearestNode(snapshot, squad.Position, TacticalNodeType.ExtractionPoint, false);

            Assert.That(searchNode, Is.Not.Null);
            Assert.That(searchNode.NodeType, Is.EqualTo(TacticalNodeType.SearchPoint));
            Assert.That(extractionNode, Is.Not.Null);
            Assert.That(extractionNode.NodeType, Is.EqualTo(TacticalNodeType.ExtractionPoint));
        }
    }
}
