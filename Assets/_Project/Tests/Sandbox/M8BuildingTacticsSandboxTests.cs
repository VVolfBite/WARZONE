using System.IO;
using NUnit.Framework;
using Warzone.Combat;
using Warzone.Sandbox.BattleSandbox;

namespace Warzone.Tests.Sandbox
{
    public sealed class M8BuildingTacticsSandboxTests
    {
        [Test]
        public void M8ScenarioFactory_CreatesBuildingAndObjectives()
        {
            M8BuildingTacticsScenario scenario = M8BuildingTacticsScenarioFactory.CreateScenario();

            Assert.That(scenario.BattleState.BuildingsById.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(scenario.BattleState.BuildingsById[100].EntranceNodeIds.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(scenario.BattleState.BuildingsById[100].WindowNodeIds.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(scenario.BattleState.BuildingsById[100].InteriorNodeIds.Count, Is.GreaterThanOrEqualTo(4));
            Assert.That(scenario.BattleState.BuildingsById[100].SearchNodeIds.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(scenario.BattleState.MissionDefinitionId, Is.EqualTo("sandbox.m8.building-tactics"));
        }

        [Test]
        public void ScenarioRegistry_ExposesM8Mode()
        {
            Assert.That(BattleSandboxScenarioRegistry.GetDisplayName(BattleSandboxMode.M8BuildingTactics), Is.EqualTo("M8 Building Tactics"));
            Assert.That(BattleSandboxScenarioRegistry.IsCompatibilityEntry(BattleSandboxMode.M8BuildingTactics), Is.False);
        }

        [Test]
        public void SandboxMenu_IncludesM8CreateScenePath()
        {
            string source = File.ReadAllText(Path.Combine("Assets", "_Project", "Editor", "SandboxTools", "SandboxSceneCreateMenu.cs"));
            StringAssert.Contains("Create M8 Building Tactics Sandbox Scene", source);
            StringAssert.Contains("Sandbox_M8_BuildingTactics.unity", source);
        }
    }
}
