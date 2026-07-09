using Warzone.Core.Math;
using Warzone.Combat;
using Warzone.Content.Definitions;

namespace Warzone.Sandbox.BattleSandbox
{
    public static class M1MemberSquadScenarioFactory
    {
        public static BattleState CreateScenario()
        {
            BattleStateFactory battleStateFactory = new BattleStateFactory();
            BattleState battleState = battleStateFactory.CreateMemberSquadBattle(
                "sandbox.m1.member-squad",
                squadId: 1,
                factionId: FactionId.Player,
                rallyPosition: new Vec2(-6f, -4f),
                memberCount: 4,
                formationSpacing: 1.4f,
                startingMemberId: 100);

            battleState.AddEnemy(battleStateFactory.CreateEnemy(9001, "sandbox.target", FactionId.Enemy, new Vec2(8f, 6f), 50, 0f, 0f));
            battleState.AddTacticalNode(new TacticalNodeState(2, TacticalNodeType.RallyPoint, new Vec2(2f, 2f), 2f));
            return battleState;
        }
    }
}

