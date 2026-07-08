using NUnit.Framework;
using System.Numerics;
using Warzone.Combat;
using Warzone.Content.Definitions;

namespace Warzone.Tests.Combat
{
    public sealed class BattleSimulationTests
    {
        [Test]
        public void BattleStateFactory_CreatesSquadWithMembers()
        {
            BattleStateFactory factory = new BattleStateFactory();
            BattleState battleState = factory.CreateMemberSquadBattle("test", 1, FactionId.Player, Vector2.Zero, 4, 1.5f);

            Assert.That(battleState.SquadsById.Count, Is.EqualTo(1));
            Assert.That(battleState.MembersById.Count, Is.EqualTo(4));
            Assert.That(battleState.SquadsById[1].MemberIds.Count, Is.EqualTo(4));
        }

        [Test]
        public void MoveSquadCommand_IsAcceptedForExistingSquad()
        {
            BattleState battleState = CreateBattleState();
            CommandSystem commandSystem = new CommandSystem();
            commandSystem.Enqueue(battleState, new MoveSquadCommand(1, new Vector2(10f, 2f)));

            BattleCommandProcessResult result = commandSystem.Process(battleState);

            Assert.That(result.AcceptedCount, Is.EqualTo(1));
            Assert.That(result.RejectedCount, Is.EqualTo(0));
            Assert.That(battleState.SquadsById[1].CurrentOrder, Is.TypeOf<MoveSquadCommand>());
        }

        [Test]
        public void MoveSquadCommand_IsRejectedForMissingSquad()
        {
            BattleState battleState = CreateBattleState();
            CommandSystem commandSystem = new CommandSystem();
            commandSystem.Enqueue(battleState, new MoveSquadCommand(99, new Vector2(10f, 2f)));

            BattleCommandProcessResult result = commandSystem.Process(battleState);

            Assert.That(result.AcceptedCount, Is.EqualTo(0));
            Assert.That(result.RejectedCount, Is.EqualTo(1));
        }

        [Test]
        public void SquadPlanningSystem_AssignsOneIntentPerAliveMember()
        {
            BattleState battleState = CreateBattleState();
            BattleSimulation simulation = new BattleSimulation();
            simulation.Enqueue(battleState, new MoveSquadCommand(1, new Vector2(6f, 0f)));

            simulation.Tick(battleState, 0f);

            foreach (BattleEntityId memberId in battleState.SquadsById[1].MemberIds)
            {
                Assert.That(battleState.MembersById[memberId].CurrentIntent, Is.Not.Null);
            }
        }

        [Test]
        public void FormationSystem_CreatesDistinctTargetPositionsForMembers()
        {
            BattleState battleState = CreateBattleState();
            BattleSimulation simulation = new BattleSimulation();
            simulation.Enqueue(battleState, new MoveSquadCommand(1, new Vector2(8f, 0f)));

            simulation.Tick(battleState, 0f);

            Vector2 first = battleState.MembersById[battleState.SquadsById[1].MemberIds[0]].CurrentIntent.TargetPosition;
            Vector2 second = battleState.MembersById[battleState.SquadsById[1].MemberIds[1]].CurrentIntent.TargetPosition;
            Assert.That(first, Is.Not.EqualTo(second));
        }

        [Test]
        public void MovementSystem_MovesMembersTowardAssignedTargets()
        {
            BattleState battleState = CreateBattleState();
            BattleSimulation simulation = new BattleSimulation();
            BattleMemberState firstMember = battleState.MembersById[battleState.SquadsById[1].MemberIds[0]];
            Vector2 start = firstMember.Position;
            simulation.Enqueue(battleState, new MoveSquadCommand(1, new Vector2(8f, 0f)));

            simulation.Tick(battleState, 0.5f);

            Assert.That(firstMember.Position, Is.Not.EqualTo(start));
        }

        [Test]
        public void BattleSnapshot_ContainsSquadAndMemberPositions()
        {
            BattleState battleState = CreateBattleState();
            BattleSimulation simulation = new BattleSimulation();
            simulation.Enqueue(battleState, new MoveSquadCommand(1, new Vector2(4f, 0f)));

            simulation.Tick(battleState, 0.25f);
            BattleSnapshot snapshot = simulation.LatestSnapshot;

            Assert.That(snapshot, Is.Not.Null);
            Assert.That(snapshot.Squads.Count, Is.EqualTo(1));
            Assert.That(snapshot.Members.Count, Is.EqualTo(4));
            Assert.That(snapshot.Members[0].Position, Is.Not.EqualTo(Vector2.Zero));
        }

        private static BattleState CreateBattleState()
        {
            BattleStateFactory factory = new BattleStateFactory();
            return factory.CreateMemberSquadBattle("test", 1, FactionId.Player, Vector2.Zero, 4, 1.5f);
        }
    }
}
