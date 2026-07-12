using NUnit.Framework;
using Warzone.Core.Math;
using Warzone.Sandbox.BattleSandbox;

namespace Warzone.Tests.Sandbox
{
    public sealed class U1PlaytestFeedbackSandboxTests
    {
        [Test]
        public void RuntimeContext_HoldPauseControlsEffectivePause()
        {
            BattleSandboxRuntimeContext context = CreateContext();

            context.SetHoldPause(true);
            Assert.That(context.IsHoldPaused, Is.True);
            Assert.That(context.IsPaused, Is.True);

            context.SetHoldPause(false);
            Assert.That(context.IsHoldPaused, Is.False);
            Assert.That(context.IsPaused, Is.False);
        }

        [Test]
        public void RuntimeContext_HoldPauseRestoresToggleStateOnRelease()
        {
            BattleSandboxRuntimeContext context = CreateContext();
            context.TogglePause();
            context.SetHoldPause(true);

            context.SetHoldPause(false);

            Assert.That(context.IsTogglePaused, Is.True);
            Assert.That(context.IsPaused, Is.True);
        }

        [Test]
        public void RuntimeContext_SpaceToggleAndHoldPauseCanCoexist()
        {
            BattleSandboxRuntimeContext context = CreateContext();

            context.TogglePause();
            context.SetHoldPause(true);

            Assert.That(context.IsTogglePaused, Is.True);
            Assert.That(context.IsHoldPaused, Is.True);
            Assert.That(context.IsPaused, Is.True);
        }

        [Test]
        public void RuntimeContext_CommandPlanVisibilityToggles()
        {
            BattleSandboxRuntimeContext context = CreateContext();

            context.SetCommandPlanVisible(true);
            Assert.That(context.ShowCommandPlan, Is.True);

            context.SetCommandPlanVisible(false);
            Assert.That(context.ShowCommandPlan, Is.False);
        }

        [Test]
        public void RuntimeContext_ShowingCommandPlanDoesNotRecordCommand()
        {
            BattleSandboxRuntimeContext context = CreateContext();

            context.SetCommandPlanVisible(true);

            Assert.That(context.RecentCommandRecords.Count, Is.EqualTo(0));
            Assert.That(context.LastCommandIssued, Is.EqualTo("None"));
        }

        [Test]
        public void RuntimeContext_RecordsRecentSquadCommands()
        {
            BattleSandboxRuntimeContext context = CreateContext();

            context.RecordSquadCommand("Move", new Vec2(3f, 4f), 12, 1.5f);

            Assert.That(context.RecentCommandRecords.Count, Is.EqualTo(1));
            Assert.That(context.RecentCommandRecords[0].CommandName, Is.EqualTo("Move"));
            Assert.That(context.RecentCommandRecords[0].DesiredPosition, Is.EqualTo((Vec2?)new Vec2(3f, 4f)));
        }

        private static BattleSandboxRuntimeContext CreateContext()
        {
            BattleSandboxRuntimeContext context = new BattleSandboxRuntimeContext();
            context.Bind(BattleSandboxMode.M8BuildingTactics, "M8 Building Tactics", null, null, 1);
            return context;
        }
    }
}
