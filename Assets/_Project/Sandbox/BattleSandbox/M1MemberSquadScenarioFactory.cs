using System.Numerics;
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
                rallyPosition: new Vector2(-6f, -4f),
                memberCount: 4,
                formationSpacing: 1.4f,
                startingMemberId: 100);

            battleState.AddEnemy(new BattleEnemyState(new BattleEntityId(9001), FactionId.Enemy, new Vector2(8f, 6f), 50));
            battleState.AddTacticalNode(new TacticalNodeState(2, new Vector2(2f, 2f), 2f));
            return battleState;
        }
    }
}
