using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class BattleSandboxInputController : MonoBehaviour
    {
        private BattleSandboxRuntimeContext _context;
        private UnityEngine.Camera _mainCamera;
        private Action _resetAction;
        private Action _applyPressureAction;
        private Action _clearPressureAction;
        private Action _emitIncomingFireAction;
        private Action _toggleNightAction;
        private Action _toggleVisionAction;
        private Action<Vec2> _spawnSmokeAction;
        private Action<Vec2> _spawnFireAction;
        private Action<Vec2> _spawnLightAction;
        private string _lastCommandRaycastHitName;
        private bool _lastCommandUsedGroundFallback;

        public void Initialize(BattleSandboxRuntimeContext context, UnityEngine.Camera mainCamera, Action resetAction)
        {
            Initialize(context, mainCamera, resetAction, null, null, null);
        }

        public void Initialize(
            BattleSandboxRuntimeContext context,
            UnityEngine.Camera mainCamera,
            Action resetAction,
            Action applyPressureAction,
            Action clearPressureAction,
            Action emitIncomingFireAction,
            Action toggleNightAction = null,
            Action toggleVisionAction = null,
            Action<Vec2> spawnSmokeAction = null,
            Action<Vec2> spawnFireAction = null,
            Action<Vec2> spawnLightAction = null)
        {
            _context = context;
            _mainCamera = mainCamera;
            _resetAction = resetAction;
            _applyPressureAction = applyPressureAction;
            _clearPressureAction = clearPressureAction;
            _emitIncomingFireAction = emitIncomingFireAction;
            _toggleNightAction = toggleNightAction;
            _toggleVisionAction = toggleVisionAction;
            _spawnSmokeAction = spawnSmokeAction;
            _spawnFireAction = spawnFireAction;
            _spawnLightAction = spawnLightAction;
        }

        private void Update()
        {
            if (_context == null)
            {
                return;
            }

            HandlePauseToggle();
            HandlePauseHold();
            HandleCommandPlanVisibility();
            HandleReset();
            HandleDebugLineToggle();
            HandleDebugPressureKeys();
            HandleEnvironmentDebugKeys();
            HandleSelection();
            HandleCommandInput();
        }

        private void HandlePauseToggle()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
            {
                _context.TogglePause();
            }
        }

        private void HandlePauseHold()
        {
            Keyboard keyboard = Keyboard.current;
            _context.SetHoldPause(keyboard != null && keyboard.pKey.isPressed);
        }

        private void HandleCommandPlanVisibility()
        {
            Keyboard keyboard = Keyboard.current;
            bool showPlan = keyboard != null && (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed);
            _context.SetCommandPlanVisible(showPlan);
        }

        private void HandleReset()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.rKey.wasPressedThisFrame && _resetAction != null)
            {
                _resetAction();
            }
        }

        private void HandleDebugLineToggle()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.cKey.wasPressedThisFrame)
            {
                _context.ToggleFireLines();
            }
        }

        private void HandleDebugPressureKeys()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.tKey.wasPressedThisFrame && _applyPressureAction != null)
            {
                _applyPressureAction();
            }

            if (keyboard.yKey.wasPressedThisFrame && _clearPressureAction != null)
            {
                _clearPressureAction();
            }

            if (keyboard.gKey.wasPressedThisFrame && _emitIncomingFireAction != null)
            {
                _emitIncomingFireAction();
            }
        }

        private void HandleEnvironmentDebugKeys()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            if (keyboard.nKey.wasPressedThisFrame && _toggleNightAction != null)
            {
                _toggleNightAction();
            }

            if (keyboard.vKey.wasPressedThisFrame && _toggleVisionAction != null)
            {
                _toggleVisionAction();
            }

            if (keyboard.bKey.wasPressedThisFrame && _spawnSmokeAction != null)
            {
                Vec2 position;
                if (TryGetCommandGroundPosition(out position))
                {
                    RecordInput("B", "SpawnSmoke", position);
                    _spawnSmokeAction(position);
                }
            }

            if (keyboard.fKey.wasPressedThisFrame && _spawnFireAction != null)
            {
                Vec2 position;
                if (TryGetCommandGroundPosition(out position))
                {
                    RecordInput("F", "SpawnFire", position);
                    _spawnFireAction(position);
                }
            }

            if (keyboard.lKey.wasPressedThisFrame && _spawnLightAction != null)
            {
                Vec2 position;
                if (TryGetCommandGroundPosition(out position))
                {
                    RecordInput("L", "SpawnLight", position);
                    _spawnLightAction(position);
                }
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

            SandboxSelectionMarkerView markerView = hit.collider.GetComponent<SandboxSelectionMarkerView>();
            if (markerView != null)
            {
                _context.SetSelectedSquad(markerView.SquadId);
                return;
            }

            SandboxMemberView memberView = hit.collider.GetComponent<SandboxMemberView>();
            if (memberView != null)
            {
                _context.SetSelectedSquad(memberView.SquadId);
            }
        }

        private void HandleCommandInput()
        {
            Mouse mouse = Mouse.current;
            Keyboard keyboard = Keyboard.current;

            if (mouse != null && mouse.rightButton.wasPressedThisFrame)
            {
                Vec2 position;
                if (TryGetCommandGroundPosition(out position))
                {
                    _context.TacticalCommandService.MoveSquad(_context.SelectedSquadId, position);
                    RecordSquadCommand("Move", position, "RMB");
                }
            }

            if (keyboard == null)
            {
                return;
            }

            if (keyboard.dKey.wasPressedThisFrame)
            {
                Vec2 position;
                if (TryGetCommandGroundPosition(out position))
                {
                    _context.TacticalCommandService.DefendArea(_context.SelectedSquadId, position, 8f);
                    RecordSquadCommand("Defend", position, "D");
                }
            }

            BattleSnapshot snapshot = _context.GetSnapshot();
            BattleSquadSnapshot squad = BattleSandboxCommandQueries.FindSelectedSquad(snapshot, _context.SelectedSquadId);

            if (keyboard.sKey.wasPressedThisFrame && squad != null)
            {
                TacticalNodeSnapshot node = BattleSandboxCommandQueries.FindNearestNode(snapshot, squad.Position, TacticalNodeType.SearchPoint, true);
                if (node != null)
                {
                    _context.TacticalCommandService.SearchPoint(_context.SelectedSquadId, node.NodeId);
                    RecordSquadCommand("Search", node.Position, "S");
                }
            }

            if (keyboard.eKey.wasPressedThisFrame && squad != null)
            {
                TacticalNodeSnapshot node = BattleSandboxCommandQueries.FindNearestNode(snapshot, squad.Position, TacticalNodeType.ExtractionPoint, false);
                if (node != null)
                {
                    _context.TacticalCommandService.ExtractSquad(_context.SelectedSquadId, node.NodeId);
                    RecordSquadCommand("Extract", node.Position, "E");
                }
            }

            if (_context.Mode == BattleSandboxMode.M8BuildingTactics && squad != null)
            {
                BuildingSnapshot building = BattleSandboxCommandQueries.FindNearestBuilding(snapshot, squad.Position, true);
                if (building != null)
                {
                    if (keyboard.gKey.wasPressedThisFrame)
                    {
                        _context.TacticalCommandService.EnterBuilding(_context.SelectedSquadId, building.BuildingId);
                        RecordSquadCommand("EnterBuilding", building.Position, "G");
                    }

                    if (keyboard.hKey.wasPressedThisFrame)
                    {
                        _context.TacticalCommandService.DefendBuilding(_context.SelectedSquadId, building.BuildingId);
                        RecordSquadCommand("DefendBuilding", building.Position, "H");
                    }

                    if (keyboard.jKey.wasPressedThisFrame)
                    {
                        _context.TacticalCommandService.SearchBuilding(_context.SelectedSquadId, building.BuildingId);
                        RecordSquadCommand("SearchBuilding", building.Position, "J");
                    }
                }
            }
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

        private bool TryGetCommandGroundPosition(out Vec2 position)
        {
            Mouse mouse = Mouse.current;
            if (_mainCamera == null || mouse == null)
            {
                position = Vec2.Zero;
                _lastCommandRaycastHitName = "-";
                _lastCommandUsedGroundFallback = false;
                return false;
            }

            Ray ray = _mainCamera.ScreenPointToRay(mouse.position.ReadValue());
            RaycastHit[] hits = Physics.RaycastAll(ray, 1000f);
            Array.Sort(hits, CompareRaycastHitDistance);
            _lastCommandRaycastHitName = hits.Length > 0 && hits[0].collider != null ? hits[0].collider.name : "-";
            for (int i = 0; i < hits.Length; i++)
            {
                if (IsGroundHit(hits[i]))
                {
                    position = new Vec2(hits[i].point.x, hits[i].point.z);
                    _lastCommandUsedGroundFallback = false;
                    return true;
                }
            }

            float enter;
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            if (groundPlane.Raycast(ray, out enter))
            {
                Vector3 point = ray.GetPoint(enter);
                position = new Vec2(point.x, point.z);
                _lastCommandUsedGroundFallback = true;
                return true;
            }

            position = Vec2.Zero;
            _lastCommandUsedGroundFallback = false;
            return false;
        }

        private static int CompareRaycastHitDistance(RaycastHit left, RaycastHit right)
        {
            return left.distance.CompareTo(right.distance);
        }

        private static bool IsGroundHit(RaycastHit hit)
        {
            if (hit.collider == null)
            {
                return false;
            }

            GameObject gameObject = hit.collider.gameObject;
            if (gameObject.name.IndexOf("Ground", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }

            string layerName = LayerMask.LayerToName(gameObject.layer);
            return layerName != null && layerName.IndexOf("Ground", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void RecordSquadCommand(string commandName, Vec2 desiredPosition, string inputAction)
        {
            _context.RecordSquadCommand(commandName, desiredPosition, Time.frameCount, Time.time);
            RecordInput(inputAction, commandName, desiredPosition);
        }

        private void RecordInput(string inputAction, string commandName, Vec2 position)
        {
            _context.RecordInput(inputAction, commandName, position, Time.frameCount, Time.time, _lastCommandRaycastHitName, _lastCommandUsedGroundFallback);
        }
    }
}
