using NUnit.Framework;
using Warzone.Combat;
using Warzone.Sandbox.BattleSandbox;

namespace Warzone.Tests.Sandbox
{
    public sealed class M7EnvironmentCombatSandboxTests
    {
        [Test]
        public void M7ScenarioFactory_CreatesZonesAndNightSettings()
        {
            M7EnvironmentCombatScenario scenario = M7EnvironmentCombatScenarioFactory.CreateScenario();

            Assert.That(scenario.BattleState.EnvironmentState.IsNight, Is.True);
            Assert.That(scenario.BattleState.EnvironmentState.ZonesById.Count, Is.GreaterThanOrEqualTo(5));
            Assert.That(scenario.BattleState.MembersById.Count, Is.EqualTo(4));
            Assert.That(scenario.BattleState.EnemiesById.Count, Is.GreaterThanOrEqualTo(6));
        }

        [Test]
        public void ScenarioRegistry_ExposesM7Mode()
        {
            Assert.That(BattleSandboxScenarioRegistry.GetDisplayName(BattleSandboxMode.M7EnvironmentCombat), Is.EqualTo("M7 Environment Combat"));
            Assert.That(BattleSandboxScenarioRegistry.IsCompatibilityEntry(BattleSandboxMode.M7EnvironmentCombat), Is.False);
        }

        [Test]
        public void M7Scenario_StartsWithValidBattleState()
        {
            M7EnvironmentCombatScenario scenario = M7EnvironmentCombatScenarioFactory.CreateScenario();
            BattleSnapshot snapshot = BattleSnapshotFactory.Create(scenario.BattleState);

            Assert.That(snapshot.Environment, Is.Not.Null);
            Assert.That(snapshot.Environment.Zones.Count, Is.GreaterThan(0));
            Assert.That(snapshot.MissionStatus, Is.Not.Null);
        }
    }
}
