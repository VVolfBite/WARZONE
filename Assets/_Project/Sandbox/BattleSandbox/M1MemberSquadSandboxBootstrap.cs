using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Warzone.Combat;

namespace Warzone.Sandbox.BattleSandbox
{
    // Sandbox-only bootstrap. Formal flow ownership can move into Application once M1 combat entry stabilizes.
    public sealed class M1MemberSquadSandboxBootstrap : MonoBehaviour
    {
        private readonly Dictionary<BattleEntityId, M1MemberView> _memberViews = new Dictionary<BattleEntityId, M1MemberView>();
        private readonly Dictionary<int, M1SquadMarkerView> _squadMarkers = new Dictionary<int, M1SquadMarkerView>();

        private BattleState _battleState;
        private BattleSimulation _battleSimulation;
        private M1MemberSquadDebugPanel _debugPanel;
        private Camera _mainCamera;
        private bool _isPaused;
        private int _selectedSquadId = 1;

        private void Awake()
        {
            EnsureGround();
            EnsureCamera();

            _battleState = M1MemberSquadScenarioFactory.CreateScenario();
            _battleSimulation = new BattleSimulation();
            _debugPanel = gameObject.AddComponent<M1MemberSquadDebugPanel>();

            BuildViews();
            PublishSnapshot();
        }

        private void Update()
        {
            HandleSelection();
            HandleCommandInput();
            HandlePauseToggle();

            if (!_isPaused)
            {
                _battleSimulation.Tick(_battleState, Time.deltaTime);
            }

            SyncViews();
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

            M1SquadMarkerView squadMarker = hit.collider.GetComponent<M1SquadMarkerView>();
            if (squadMarker != null)
            {
                _selectedSquadId = squadMarker.SquadId;
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

            _battleSimulation.Enqueue(_battleState, new MoveSquadCommand(_selectedSquadId, new System.Numerics.Vector2(hit.point.x, hit.point.z)));
        }

        private void BuildViews()
        {
            foreach (BattleSquadState squadState in _battleState.SquadsById.Values)
            {
                CreateSquadMarker(squadState);
            }

            foreach (BattleMemberState memberState in _battleState.MembersById.Values)
            {
                CreateMemberView(memberState);
            }
        }

        private void SyncViews()
        {
            foreach (BattleSquadState squadState in _battleState.SquadsById.Values)
            {
                M1SquadMarkerView markerView;
                if (_squadMarkers.TryGetValue(squadState.SquadId, out markerView) && markerView != null)
                {
                    markerView.transform.position = new Vector3(squadState.Position.X, 0.25f, squadState.Position.Y);
                    markerView.SetSelected(squadState.SquadId == _selectedSquadId);
                }
            }

            foreach (BattleMemberState memberState in _battleState.MembersById.Values)
            {
                M1MemberView memberView;
                if (_memberViews.TryGetValue(memberState.MemberId, out memberView) && memberView != null)
                {
                    memberView.transform.position = new Vector3(memberState.Position.X, 0.6f, memberState.Position.Y);
                    memberView.SetSelected(memberState.SquadId == _selectedSquadId);
                }
            }
        }

        private void PublishSnapshot()
        {
            BattleSnapshot snapshot = _battleSimulation.LatestSnapshot ?? BattleSnapshotFactory.Create(_battleState);
            _debugPanel.Bind(snapshot, _isPaused, true, _selectedSquadId);
        }

        private void CreateMemberView(BattleMemberState memberState)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            root.name = "M1Member_" + memberState.MemberId.Value;
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
            root.name = "M1SquadMarker_" + squadState.SquadId;
            root.transform.position = new Vector3(squadState.Position.X, 0.25f, squadState.Position.Y);
            root.transform.localScale = new Vector3(0.45f, 0.1f, 0.45f);
            Renderer renderer = root.GetComponent<Renderer>();
            renderer.material.color = new Color(0.65f, 0.65f, 0.2f);

            M1SquadMarkerView squadMarkerView = root.AddComponent<M1SquadMarkerView>();
            squadMarkerView.Initialize(squadState.SquadId, renderer);
            _squadMarkers[squadState.SquadId] = squadMarkerView;
        }

        private void EnsureGround()
        {
            if (GameObject.Find("M1SandboxGround") != null)
            {
                return;
            }

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "M1SandboxGround";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(5f, 1f, 5f);
            Renderer renderer = ground.GetComponent<Renderer>();
            renderer.material.color = new Color(0.28f, 0.38f, 0.24f);
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
            cameraObject.transform.position = new Vector3(0f, 18f, -12f);
            cameraObject.transform.rotation = Quaternion.Euler(55f, 0f, 0f);
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
    }
}
