namespace Warzone.Combat
{
    public static class FindTargetForMemberQuery
    {
        public static BattleEnemyState Execute(BattleState battleState, BattleMemberState memberState)
        {
            return FindNearestEnemyQuery.Execute(memberState, FindVisibleEnemiesQuery.Execute(battleState, memberState));
        }
    }
}
