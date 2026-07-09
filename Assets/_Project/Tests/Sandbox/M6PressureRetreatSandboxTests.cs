using NUnit.Framework;
using Warzone.Combat;
using Warzone.Sandbox.BattleSandbox;

namespace Warzone.Tests.Sandbox
{
    public sealed class M6PressureRetreatSandboxTests
    {
        [Test]
        public void M6ScenarioFactory_CreatesSquadEnemiesRallyAndExtraction()
        {
            M6PressureRetreatScenario scenario = M6PressureRetreatScenarioFactory.CreateScenario();

            Assert.That(scenario.BattleState.SquadsById.Count, Is.EqualTo(1));
            Assert.That(scenario.BattleState.MembersById.Count, Is.EqualTo(4));
            Assert.That(scenario.BattleState.EnemiesById.Count, Is.GreaterThanOrEqualTo(6));
            Assert.That(ContainsNodeType(scenario.BattleState, TacticalNodeType.RallyPoint), Is.True);
            Assert.That(ContainsNodeType(scenario.BattleState, TacticalNodeType.ExtractionPoint), Is.True);
        }

        [Test]
        public void M6Scenario_StartsWithIncompleteBattleResult()
        {
            M6PressureRetreatScenario scenario = M6PressureRetreatScenarioFactory.CreateScenario();
            BattleSnapshot snapshot = BattleSnapshotFactory.Create(scenario.BattleState);

            Assert.That(snapshot.MissionStatus, Is.Not.Null);
            Assert.That(snapshot.MissionStatus.IsBattleComplete, Is.False);
            Assert.That(snapshot.BattleResult, Is.Null);
        }

        [Test]
        public void BattleSandboxScenarioRegistry_ExposesM6Mode()
        {
            Assert.That(BattleSandboxScenarioRegistry.GetDisplayName(BattleSandboxMode.M6PressureRetreat), Is.EqualTo("M6 Pressure Retreat"));
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
