using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Warzone.Application.Services;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    // Compatibility entry kept for legacy M3 verification. M5 uses BattleSandboxLauncher as the unified sandbox entry.
    public sealed class M3TacticalMissionSandboxBootstrap : MonoBehaviour
    {
        private readonly Dictionary<BattleEntityId, M1MemberView> _memberViews = new Dictionary<BattleEntityId, M1MemberView>();
        private readonly Dictionary<int, M1SquadMarkerView> _squadViews = new Dictionary<int, M1SquadMarkerView>();
        private readonly Dictionary<BattleEntityId, M2EnemyView> _enemyViews = new Dictionary<BattleEntityId, M2EnemyView>();
        private readonly Dictionary<int, M3TacticalNodeView> _nodeViews = new Dictionary<int, M3TacticalNodeView>();

        private BattleService _battleService;
        private TacticalCommandService _tacticalCommandService;
        private M3TacticalMissionDebugPanel _debugPanel;
        private UnityEngine.Camera _mainCamera;
        private bool _isPaused;
        private int _selectedSquadId = 1;

        private void Awake()
        {
            EnsureGround();
            EnsureCamera();
            _debugPanel = gameObject.AddComponent<M3TacticalMissionDebugPanel>();
            RebuildScenario();
        }

        private void Update()
        {
            HandlePauseToggle();
            HandleReset();
            HandleSelection();
            HandleCommandInput();

            if (!_isPaused && _battleService != null)
            {
                _battleService.Tick(Time.deltaTime);
            }

            SyncViews();
            PublishSnapshot();
        }

        private void RebuildScenario()
        {
            ClearViews();
            M3TacticalMissionScenario scenario = M3TacticalMissionScenarioFactory.CreateScenario();
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
                    _tacticalCommandService.DefendArea(_selectedSquadId, new Vec2(hit.point.x, hit.point.z), 6f);
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
                _debugPanel.Bind(_battleService != null ? _battleService.GetSnapshot() : null, _isPaused, _selectedSquadId);
            }
        }

        private void CreateMemberView(BattleMemberState memberState)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            root.name = "M3Member_" + memberState.MemberId.Value;
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
            root.name = "M3Squad_" + squadState.SquadId;
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
            root.name = "M3Enemy_" + enemyState.EnemyId.Value;
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
            Vector3 scale = new Vector3(0.9f, 0.6f, 0.9f);
            float y = 0.3f;

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
            root.name = "M3Node_" + nodeState.NodeId + "_" + nodeState.NodeType;
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

            _memberViews.Clear();
            _squadViews.Clear();
            _enemyViews.Clear();
            _nodeViews.Clear();
        }

        private void EnsureGround()
        {
            if (GameObject.Find("M3SandboxGround") != null)
            {
                return;
            }

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "M3SandboxGround";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(6f, 1f, 6f);
            Renderer renderer = ground.GetComponent<Renderer>();
            renderer.material.color = new Color(0.28f, 0.36f, 0.24f);
        }

        private void EnsureCamera()
        {
            _mainCamera = UnityEngine.Camera.main;
            if (_mainCamera != null)
            {
                return;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(1f, 22f, -14f);
            cameraObject.transform.rotation = Quaternion.Euler(58f, 0f, 0f);
            _mainCamera = cameraObject.AddComponent<UnityEngine.Camera>();
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
