using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Warzone.Application.Services;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class M4SpatialCombatSandboxBootstrap : MonoBehaviour
    {
        private readonly Dictionary<BattleEntityId, M1MemberView> _memberViews = new Dictionary<BattleEntityId, M1MemberView>();
        private readonly Dictionary<int, M1SquadMarkerView> _squadViews = new Dictionary<int, M1SquadMarkerView>();
        private readonly Dictionary<BattleEntityId, M2EnemyView> _enemyViews = new Dictionary<BattleEntityId, M2EnemyView>();
        private readonly Dictionary<int, M3TacticalNodeView> _nodeViews = new Dictionary<int, M3TacticalNodeView>();
        private readonly Dictionary<int, M4ObstacleView> _obstacleViews = new Dictionary<int, M4ObstacleView>();
        private readonly Dictionary<int, M4BuildingView> _buildingViews = new Dictionary<int, M4BuildingView>();

        private BattleService _battleService;
        private TacticalCommandService _tacticalCommandService;
        private M4SpatialCombatDebugPanel _debugPanel;
        private M4FireLineView _fireLineView;
        private Camera _mainCamera;
        private bool _isPaused;
        private bool _showFireLines = true;
        private int _selectedSquadId = 1;

        private void Awake()
        {
            EnsureGround();
            EnsureCamera();
            _debugPanel = gameObject.AddComponent<M4SpatialCombatDebugPanel>();
            _fireLineView = gameObject.AddComponent<M4FireLineView>();
            RebuildScenario();
        }

        private void Update()
        {
            HandlePauseToggle();
            HandleReset();
            HandleDebugLineToggle();
            HandleSelection();
            HandleCommandInput();

            if (!_isPaused && _battleService != null && !_battleService.IsBattleComplete())
            {
                _battleService.Tick(Time.deltaTime);
            }

            SyncViews();
            PublishSnapshot();
            _fireLineView.Render(_battleService != null ? _battleService.GetSnapshot() : null, _memberViews, _enemyViews, _showFireLines);
        }

        private void RebuildScenario()
        {
            ClearViews();
            M4SpatialCombatScenario scenario = M4SpatialCombatScenarioFactory.CreateScenario();
            _battleService = new BattleService(scenario.ContentCatalog);
            _tacticalCommandService = new TacticalCommandService(_battleService);
            _battleService.Start(scenario.BattleState);
            BuildViews(scenario.BattleState);
            _selectedSquadId = 1;
            _isPaused = false;
            PublishSnapshot();
        }

        private void HandlePauseToggle()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && (keyboard.pKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame))
            {
                _isPaused = !_isPaused;
            }
        }

        private void HandleReset()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.rKey.wasPressedThisFrame)
            {
                RebuildScenario();
            }
        }

        private void HandleDebugLineToggle()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.cKey.wasPressedThisFrame)
            {
                _showFireLines = !_showFireLines;
            }
        }

        private void HandleSelection()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
            {
                return;
            }

            RaycastHit hit;
            if (!TryRaycast(out hit))
            {
                return;
            }

            M1SquadMarkerView squadView = hit.collider.GetComponent<M1SquadMarkerView>();
            if (squadView != null)
            {
                _selectedSquadId = squadView.SquadId;
                return;
            }

            M1MemberView memberView = hit.collider.GetComponent<M1MemberView>();
            if (memberView != null)
            {
                _selectedSquadId = memberView.SquadId;
            }
        }

        private void HandleCommandInput()
        {
            Mouse mouse = Mouse.current;
            Keyboard keyboard = Keyboard.current;

            if (mouse != null && mouse.rightButton.wasPressedThisFrame)
            {
                RaycastHit hit;
                if (TryRaycast(out hit))
                {
                    _tacticalCommandService.MoveSquad(_selectedSquadId, new Vec2(hit.point.x, hit.point.z));
                }
            }

            if (keyboard == null)
            {
                return;
            }

            if (keyboard.dKey.wasPressedThisFrame)
            {
                RaycastHit hit;
                if (TryRaycast(out hit))
                {
                    _tacticalCommandService.DefendArea(_selectedSquadId, new Vec2(hit.point.x, hit.point.z), 8f);
                }
            }

            if (keyboard.sKey.wasPressedThisFrame)
            {
                BattleSnapshot snapshot = _battleService.GetSnapshot();
                BattleSquadSnapshot squad = FindSelectedSquad(snapshot, _selectedSquadId);
                if (squad != null)
                {
                    TacticalNodeSnapshot node = FindNearestNode(snapshot, squad.Position, TacticalNodeType.SearchPoint, true);
                    if (node != null)
                    {
                        _tacticalCommandService.SearchPoint(_selectedSquadId, node.NodeId);
                    }
                }
            }

            if (keyboard.eKey.wasPressedThisFrame)
            {
                BattleSnapshot snapshot = _battleService.GetSnapshot();
                BattleSquadSnapshot squad = FindSelectedSquad(snapshot, _selectedSquadId);
                if (squad != null)
                {
                    TacticalNodeSnapshot node = FindNearestNode(snapshot, squad.Position, TacticalNodeType.ExtractionPoint, false);
                    if (node != null)
                    {
                        _tacticalCommandService.ExtractSquad(_selectedSquadId, node.NodeId);
                    }
                }
            }
        }

        private void BuildViews(BattleState battleState)
        {
            foreach (BuildingState buildingState in battleState.BuildingsById.Values)
            {
                CreateBuildingView(buildingState);
            }

            foreach (TacticalObstacleState obstacleState in battleState.ObstaclesById.Values)
            {
                CreateObstacleView(obstacleState);
            }

            foreach (BattleSquadState squadState in battleState.SquadsById.Values)
            {
                CreateSquadMarker(squadState);
            }

            foreach (BattleMemberState memberState in battleState.MembersById.Values)
            {
                CreateMemberView(memberState);
            }

            foreach (BattleEnemyState enemyState in battleState.EnemiesById.Values)
            {
                CreateEnemyView(enemyState);
            }

            foreach (TacticalNodeState nodeState in battleState.TacticalNodesById.Values)
            {
                CreateNodeView(nodeState);
            }
        }

        private void SyncViews()
        {
            BattleSnapshot snapshot = _battleService != null ? _battleService.GetSnapshot() : null;
            if (snapshot == null)
            {
                return;
            }

            for (int i = 0; i < snapshot.Squads.Count; i++)
            {
                BattleSquadSnapshot squad = snapshot.Squads[i];
                M1SquadMarkerView squadView;
                if (_squadViews.TryGetValue(squad.SquadId, out squadView) && squadView != null)
                {
                    squadView.transform.position = new Vector3(squad.Position.X, 0.25f, squad.Position.Y);
                    squadView.SetSelected(squad.SquadId == _selectedSquadId);
                }
            }

            for (int i = 0; i < snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = snapshot.Members[i];
                M1MemberView memberView;
                if (_memberViews.TryGetValue(member.MemberId, out memberView) && memberView != null)
                {
                    memberView.transform.position = new Vector3(member.Position.X, member.IsAlive ? 0.6f : 0.15f, member.Position.Y);
                    memberView.SetStateVisual(member.SquadId == _selectedSquadId, !member.IsAlive, member.IsExtracted);
                }
            }

            for (int i = 0; i < snapshot.Enemies.Count; i++)
            {
                BattleEnemySnapshot enemy = snapshot.Enemies[i];
                M2EnemyView enemyView;
                if (_enemyViews.TryGetValue(enemy.EnemyId, out enemyView) && enemyView != null)
                {
                    enemyView.transform.position = new Vector3(enemy.Position.X, enemy.IsAlive ? 0.55f : 0.15f, enemy.Position.Y);
                    if (enemy.IsAlive)
                    {
                        enemyView.SetAliveVisual();
                    }
                    else
                    {
                        enemyView.SetDeadVisual();
                    }
                }
            }

            for (int i = 0; i < snapshot.TacticalNodes.Count; i++)
            {
                TacticalNodeSnapshot node = snapshot.TacticalNodes[i];
                M3TacticalNodeView nodeView;
                if (_nodeViews.TryGetValue(node.NodeId, out nodeView) && nodeView != null)
                {
                    nodeView.ApplySnapshot(node);
                }
            }
        }

        private void PublishSnapshot()
        {
            if (_debugPanel != null)
            {
                _debugPanel.Bind(_battleService != null ? _battleService.GetSnapshot() : null, _isPaused, _selectedSquadId, _showFireLines);
            }
        }

        private void CreateMemberView(BattleMemberState memberState)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            root.name = "M4Member_" + memberState.MemberId.Value;
            root.transform.position = new Vector3(memberState.Position.X, 0.6f, memberState.Position.Y);
            root.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            Renderer renderer = root.GetComponent<Renderer>();
            renderer.material.color = new Color(0.25f, 0.55f, 0.75f);

            M1MemberView memberView = root.AddComponent<M1MemberView>();
            memberView.Initialize(memberState.MemberId.Value, memberState.SquadId, renderer);
            _memberViews[memberState.MemberId] = memberView;
        }

        private void CreateSquadMarker(BattleSquadState squadState)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            root.name = "M4Squad_" + squadState.SquadId;
            root.transform.position = new Vector3(squadState.Position.X, 0.25f, squadState.Position.Y);
            root.transform.localScale = new Vector3(0.45f, 0.1f, 0.45f);
            Renderer renderer = root.GetComponent<Renderer>();
            renderer.material.color = new Color(0.65f, 0.65f, 0.2f);

            M1SquadMarkerView squadView = root.AddComponent<M1SquadMarkerView>();
            squadView.Initialize(squadState.SquadId, renderer);
            _squadViews[squadState.SquadId] = squadView;
        }

        private void CreateEnemyView(BattleEnemyState enemyState)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Cube);
            root.name = "M4Enemy_" + enemyState.EnemyId.Value;
            root.transform.position = new Vector3(enemyState.Position.X, 0.55f, enemyState.Position.Y);
            root.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            Renderer renderer = root.GetComponent<Renderer>();
            renderer.material.color = new Color(0.76f, 0.28f, 0.24f);

            M2EnemyView enemyView = root.AddComponent<M2EnemyView>();
            enemyView.Initialize(enemyState.EnemyId.Value, renderer);
            _enemyViews[enemyState.EnemyId] = enemyView;
        }

        private void CreateNodeView(TacticalNodeState nodeState)
        {
            PrimitiveType primitiveType = PrimitiveType.Cube;
            Vector3 scale = new Vector3(0.8f, 0.4f, 0.8f);
            float y = 0.2f;

            if (nodeState.NodeType == TacticalNodeType.ExtractionPoint)
            {
                primitiveType = PrimitiveType.Cylinder;
                scale = new Vector3(nodeState.Radius * 0.8f, 0.05f, nodeState.Radius * 0.8f);
                y = 0.05f;
            }
            else if (nodeState.NodeType == TacticalNodeType.SearchPoint)
            {
                primitiveType = PrimitiveType.Cube;
                scale = new Vector3(1f, 0.7f, 1f);
                y = 0.35f;
            }
            else if (nodeState.NodeType == TacticalNodeType.EnemyIngress)
            {
                primitiveType = PrimitiveType.Sphere;
                scale = new Vector3(0.7f, 0.7f, 0.7f);
                y = 0.35f;
            }

            GameObject root = GameObject.CreatePrimitive(primitiveType);
            root.name = "M4Node_" + nodeState.NodeId + "_" + nodeState.NodeType;
            root.transform.position = new Vector3(nodeState.Position.X, y, nodeState.Position.Y);
            root.transform.localScale = scale;
            Renderer renderer = root.GetComponent<Renderer>();

            M3TacticalNodeView nodeView;
            if (nodeState.NodeType == TacticalNodeType.SearchPoint)
            {
                nodeView = root.AddComponent<M3SearchPointView>();
            }
            else if (nodeState.NodeType == TacticalNodeType.ExtractionPoint)
            {
                nodeView = root.AddComponent<M3ExtractionPointView>();
            }
            else
            {
                nodeView = root.AddComponent<M3TacticalNodeView>();
            }

            nodeView.Initialize(nodeState.NodeId, nodeState.NodeType, renderer);
            _nodeViews[nodeState.NodeId] = nodeView;
        }

        private void CreateObstacleView(TacticalObstacleState obstacleState)
        {
            PrimitiveType primitiveType = obstacleState.ObstacleType == TacticalObstacleType.Window ? PrimitiveType.Cylinder : PrimitiveType.Cube;
            GameObject root = GameObject.CreatePrimitive(primitiveType);
            root.name = "M4Obstacle_" + obstacleState.ObstacleId;
            root.transform.position = new Vector3(obstacleState.Position.X, obstacleState.ObstacleType == TacticalObstacleType.Window ? 0.8f : 0.5f, obstacleState.Position.Y);
            float height = obstacleState.ObstacleType == TacticalObstacleType.LowCover ? 0.7f : 1.4f;
            if (obstacleState.ObstacleType == TacticalObstacleType.Window)
            {
                height = 0.1f;
            }

            root.transform.localScale = new Vector3(obstacleState.Radius * 1.6f, height, obstacleState.Radius * 1.6f);
            Renderer renderer = root.GetComponent<Renderer>();
            M4ObstacleView obstacleView = root.AddComponent<M4ObstacleView>();
            obstacleView.Initialize(obstacleState.ObstacleId, obstacleState.ObstacleType, renderer);
            _obstacleViews[obstacleState.ObstacleId] = obstacleView;
        }

        private void CreateBuildingView(BuildingState buildingState)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Cube);
            root.name = "M4Building_" + buildingState.BuildingId;
            root.transform.position = new Vector3(buildingState.Position.X, 0.75f, buildingState.Position.Y);
            root.transform.localScale = new Vector3(buildingState.Radius * 1.6f, 1.5f, buildingState.Radius * 1.2f);
            Renderer renderer = root.GetComponent<Renderer>();
            M4BuildingView buildingView = root.AddComponent<M4BuildingView>();
            buildingView.Initialize(buildingState.BuildingId, renderer);
            _buildingViews[buildingState.BuildingId] = buildingView;
        }

        private void ClearViews()
        {
            foreach (M1MemberView memberView in _memberViews.Values)
            {
                if (memberView != null)
                {
                    Destroy(memberView.gameObject);
                }
            }

            foreach (M1SquadMarkerView squadView in _squadViews.Values)
            {
                if (squadView != null)
                {
                    Destroy(squadView.gameObject);
                }
            }

            foreach (M2EnemyView enemyView in _enemyViews.Values)
            {
                if (enemyView != null)
                {
                    Destroy(enemyView.gameObject);
                }
            }

            foreach (M3TacticalNodeView nodeView in _nodeViews.Values)
            {
                if (nodeView != null)
                {
                    Destroy(nodeView.gameObject);
                }
            }

            foreach (M4ObstacleView obstacleView in _obstacleViews.Values)
            {
                if (obstacleView != null)
                {
                    Destroy(obstacleView.gameObject);
                }
            }

            foreach (M4BuildingView buildingView in _buildingViews.Values)
            {
                if (buildingView != null)
                {
                    Destroy(buildingView.gameObject);
                }
            }

            _memberViews.Clear();
            _squadViews.Clear();
            _enemyViews.Clear();
            _nodeViews.Clear();
            _obstacleViews.Clear();
            _buildingViews.Clear();
        }

        private void EnsureGround()
        {
            if (GameObject.Find("M4SandboxGround") != null)
            {
                return;
            }

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "M4SandboxGround";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(7f, 1f, 7f);
            Renderer renderer = ground.GetComponent<Renderer>();
            renderer.material.color = new Color(0.28f, 0.36f, 0.24f);
        }

        private void EnsureCamera()
        {
            _mainCamera = Camera.main;
            if (_mainCamera != null)
            {
                return;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(1f, 24f, -16f);
            cameraObject.transform.rotation = Quaternion.Euler(58f, 0f, 0f);
            _mainCamera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        private bool TryRaycast(out RaycastHit hit)
        {
            Mouse mouse = Mouse.current;
            if (_mainCamera == null || mouse == null)
            {
                hit = new RaycastHit();
                return false;
            }

            Ray ray = _mainCamera.ScreenPointToRay(mouse.position.ReadValue());
            return Physics.Raycast(ray, out hit, 1000f);
        }

        private static BattleSquadSnapshot FindSelectedSquad(BattleSnapshot snapshot, int selectedSquadId)
        {
            if (snapshot == null)
            {
                return null;
            }

            for (int i = 0; i < snapshot.Squads.Count; i++)
            {
                if (snapshot.Squads[i].SquadId == selectedSquadId)
                {
                    return snapshot.Squads[i];
                }
            }

            return null;
        }

        private static TacticalNodeSnapshot FindNearestNode(BattleSnapshot snapshot, Vec2 position, TacticalNodeType nodeType, bool onlyIncompleteSearch)
        {
            if (snapshot == null)
            {
                return null;
            }

            TacticalNodeSnapshot nearest = null;
            float nearestDistance = float.MaxValue;
            for (int i = 0; i < snapshot.TacticalNodes.Count; i++)
            {
                TacticalNodeSnapshot node = snapshot.TacticalNodes[i];
                if (node.NodeType != nodeType || !node.IsEnabled)
                {
                    continue;
                }

                if (onlyIncompleteSearch && nodeType == TacticalNodeType.SearchPoint && node.IsSearched)
                {
                    continue;
                }

                float distance = Vec2.Distance(position, node.Position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = node;
                }
            }

            return nearest;
        }
    }
}
