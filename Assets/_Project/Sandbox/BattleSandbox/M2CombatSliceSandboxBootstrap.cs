using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Warzone.Application.Services;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    // Compatibility entry kept for legacy M2 verification. M5 uses BattleSandboxLauncher as the unified sandbox entry.
    public sealed class M2CombatSliceSandboxBootstrap : MonoBehaviour
    {
        private readonly Dictionary<BattleEntityId, M1MemberView> _memberViews = new Dictionary<BattleEntityId, M1MemberView>();
        private readonly Dictionary<int, M1SquadMarkerView> _squadViews = new Dictionary<int, M1SquadMarkerView>();
        private readonly Dictionary<BattleEntityId, M2EnemyView> _enemyViews = new Dictionary<BattleEntityId, M2EnemyView>();

        private BattleService _battleService;
        private TacticalCommandService _tacticalCommandService;
        private M2CombatDebugPanel _debugPanel;
        private UnityEngine.Camera _mainCamera;
        private bool _isPaused;
        private int _selectedSquadId = 1;

        private void Awake()
        {
            EnsureGround();
            EnsureCamera();
            _debugPanel = gameObject.AddComponent<M2CombatDebugPanel>();
            RebuildScenario();
        }

        private void Update()
        {
            HandlePauseToggle();
            HandleReset();
            HandleSelection();
            HandleCommandInput();

            if (!_isPaused)
            {
                _battleService.Tick(Time.deltaTime);
            }

            SyncViews();
            PublishSnapshot();
        }

        private void RebuildScenario()
        {
            ClearViews();
            M2CombatScenario scenario = M2CombatSliceScenarioFactory.CreateScenario();
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
            if (mouse == null || !mouse.rightButton.wasPressedThisFrame)
            {
                return;
            }

            RaycastHit hit;
            if (!TryRaycast(out hit))
            {
                return;
            }

            _tacticalCommandService.MoveSquad(_selectedSquadId, new Vec2(hit.point.x, hit.point.z));
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
        }

        private void SyncViews()
        {
            BattleSnapshot snapshot = _battleService.GetSnapshot();
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
                    memberView.transform.position = new Vector3(member.Position.X, 0.6f, member.Position.Y);
                    memberView.SetSelected(member.SquadId == _selectedSquadId);
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
        }

        private void PublishSnapshot()
        {
            _debugPanel.Bind(_battleService != null ? _battleService.GetSnapshot() : null, _isPaused, _selectedSquadId);
        }

        private void CreateMemberView(BattleMemberState memberState)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            root.name = "M2Member_" + memberState.MemberId.Value;
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
            root.name = "M2Squad_" + squadState.SquadId;
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
            root.name = "M2Enemy_" + enemyState.EnemyId.Value;
            root.transform.position = new Vector3(enemyState.Position.X, 0.55f, enemyState.Position.Y);
            root.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            Renderer renderer = root.GetComponent<Renderer>();
            renderer.material.color = new Color(0.76f, 0.28f, 0.24f);

            M2EnemyView enemyView = root.AddComponent<M2EnemyView>();
            enemyView.Initialize(enemyState.EnemyId.Value, renderer);
            _enemyViews[enemyState.EnemyId] = enemyView;
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

            _memberViews.Clear();
            _squadViews.Clear();
            _enemyViews.Clear();
        }

        private void EnsureGround()
        {
            if (GameObject.Find("M2SandboxGround") != null)
            {
                return;
            }

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "M2SandboxGround";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(5f, 1f, 5f);
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
            cameraObject.transform.position = new Vector3(0f, 18f, -12f);
            cameraObject.transform.rotation = Quaternion.Euler(55f, 0f, 0f);
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
    }
}
