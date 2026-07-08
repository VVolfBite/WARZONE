using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class BattleState
    {
        private readonly Dictionary<int, BattleSquadState> _squadsById = new Dictionary<int, BattleSquadState>();
        private readonly Dictionary<BattleEntityId, BattleMemberState> _membersById = new Dictionary<BattleEntityId, BattleMemberState>();
        private readonly Dictionary<BattleEntityId, BattleEnemyState> _enemiesById = new Dictionary<BattleEntityId, BattleEnemyState>();
        private readonly Dictionary<int, TacticalNodeState> _tacticalNodesById = new Dictionary<int, TacticalNodeState>();

        public BattleState(string battleId)
        {
            BattleId = battleId;
            CommandQueue = new BattleCommandQueue();
            EventBuffer = new BattleEventBuffer();
        }

        public string BattleId { get; private set; }
        public float ElapsedTimeSeconds { get; private set; }
        public BattleCommandQueue CommandQueue { get; private set; }
        public BattleEventBuffer EventBuffer { get; private set; }

        public IReadOnlyDictionary<int, BattleSquadState> SquadsById
        {
            get { return _squadsById; }
        }

        public IReadOnlyDictionary<BattleEntityId, BattleMemberState> MembersById
        {
            get { return _membersById; }
        }

        public IReadOnlyDictionary<BattleEntityId, BattleEnemyState> EnemiesById
        {
            get { return _enemiesById; }
        }

        public IReadOnlyDictionary<int, TacticalNodeState> TacticalNodesById
        {
            get { return _tacticalNodesById; }
        }

        public void AdvanceTime(float deltaTimeSeconds)
        {
            ElapsedTimeSeconds += deltaTimeSeconds;
        }

        public void AddSquad(BattleSquadState squadState)
        {
            if (squadState == null)
            {
                return;
            }

            _squadsById[squadState.SquadId] = squadState;
        }

        public void AddMember(BattleMemberState memberState)
        {
            if (memberState == null)
            {
                return;
            }

            _membersById[memberState.MemberId] = memberState;
        }

        public void AddEnemy(BattleEnemyState enemyState)
        {
            if (enemyState == null)
            {
                return;
            }

            _enemiesById[enemyState.EnemyId] = enemyState;
        }

        public void AddTacticalNode(TacticalNodeState nodeState)
        {
            if (nodeState == null)
            {
                return;
            }

            _tacticalNodesById[nodeState.NodeId] = nodeState;
        }

        public bool TryGetSquad(int squadId, out BattleSquadState squadState)
        {
            return _squadsById.TryGetValue(squadId, out squadState);
        }

        public bool TryGetMember(BattleEntityId memberId, out BattleMemberState memberState)
        {
            return _membersById.TryGetValue(memberId, out memberState);
        }
    }
}
