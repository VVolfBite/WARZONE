using System.Collections.Generic;

namespace Warzone.Combat
{
    public sealed class BattleState
    {
        private readonly Dictionary<int, BattleSquadState> _squadsById = new Dictionary<int, BattleSquadState>();
        private readonly Dictionary<BattleEntityId, BattleMemberState> _membersById = new Dictionary<BattleEntityId, BattleMemberState>();
        private readonly Dictionary<BattleEntityId, BattleEnemyState> _enemiesById = new Dictionary<BattleEntityId, BattleEnemyState>();
        private readonly Dictionary<int, TacticalNodeState> _tacticalNodesById = new Dictionary<int, TacticalNodeState>();
        private readonly Dictionary<int, TacticalObstacleState> _obstaclesById = new Dictionary<int, TacticalObstacleState>();
        private readonly Dictionary<int, BuildingState> _buildingsById = new Dictionary<int, BuildingState>();
        private readonly List<PendingDamageRequest> _pendingDamageRequests = new List<PendingDamageRequest>();
        private readonly List<BattleEventRecord> _recentEvents = new List<BattleEventRecord>();

        public BattleState(string battleId)
        {
            BattleId = battleId;
            CommandQueue = new BattleCommandQueue();
            EventBuffer = new BattleEventBuffer();
        }

        public string BattleId { get; private set; }
        public float ElapsedTimeSeconds { get; private set; }
        public string MissionDefinitionId { get; private set; }
        public BattleCommandQueue CommandQueue { get; private set; }
        public BattleEventBuffer EventBuffer { get; private set; }
        public BattleMissionStatusSnapshot CurrentMissionStatus { get; private set; }
        public BattleResult CurrentBattleResult { get; private set; }

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

        public IReadOnlyDictionary<int, TacticalObstacleState> ObstaclesById
        {
            get { return _obstaclesById; }
        }

        public IReadOnlyDictionary<int, BuildingState> BuildingsById
        {
            get { return _buildingsById; }
        }

        public IReadOnlyList<PendingDamageRequest> PendingDamageRequests
        {
            get { return _pendingDamageRequests; }
        }

        public IReadOnlyList<BattleEventRecord> RecentEvents
        {
            get { return _recentEvents; }
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

        public void AddObstacle(TacticalObstacleState obstacleState)
        {
            if (obstacleState == null)
            {
                return;
            }

            _obstaclesById[obstacleState.ObstacleId] = obstacleState;
        }

        public void AddBuilding(BuildingState buildingState)
        {
            if (buildingState == null)
            {
                return;
            }

            _buildingsById[buildingState.BuildingId] = buildingState;
        }

        public bool TryGetSquad(int squadId, out BattleSquadState squadState)
        {
            return _squadsById.TryGetValue(squadId, out squadState);
        }

        public bool TryGetMember(BattleEntityId memberId, out BattleMemberState memberState)
        {
            return _membersById.TryGetValue(memberId, out memberState);
        }

        public bool TryGetEnemy(BattleEntityId enemyId, out BattleEnemyState enemyState)
        {
            return _enemiesById.TryGetValue(enemyId, out enemyState);
        }

        public bool TryGetTacticalNode(int nodeId, out TacticalNodeState nodeState)
        {
            return _tacticalNodesById.TryGetValue(nodeId, out nodeState);
        }

        public bool TryGetObstacle(int obstacleId, out TacticalObstacleState obstacleState)
        {
            return _obstaclesById.TryGetValue(obstacleId, out obstacleState);
        }

        public bool TryGetBuilding(int buildingId, out BuildingState buildingState)
        {
            return _buildingsById.TryGetValue(buildingId, out buildingState);
        }

        public void EnqueueDamage(PendingDamageRequest damageRequest)
        {
            if (damageRequest == null)
            {
                return;
            }

            _pendingDamageRequests.Add(damageRequest);
        }

        public void ClearPendingDamageRequests()
        {
            _pendingDamageRequests.Clear();
        }

        public void AddEvent(BattleEventRecord battleEvent)
        {
            if (battleEvent == null)
            {
                return;
            }

            EventBuffer.Add(battleEvent);
            _recentEvents.Add(battleEvent);
            if (_recentEvents.Count > 24)
            {
                _recentEvents.RemoveAt(0);
            }
        }

        public void SetMissionDefinitionId(string missionDefinitionId)
        {
            MissionDefinitionId = missionDefinitionId;
        }

        public void SetMissionStatus(BattleMissionStatusSnapshot missionStatus)
        {
            CurrentMissionStatus = missionStatus;
        }

        public void SetBattleResult(BattleResult battleResult)
        {
            CurrentBattleResult = battleResult;
        }
    }
}

