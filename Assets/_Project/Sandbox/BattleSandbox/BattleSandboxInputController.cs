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
        private Camera _mainCamera;
        private Action _resetAction;
        private Action _applyPressureAction;
        private Action _clearPressureAction;
        private Action _emitIncomingFireAction;
        private Action _toggleNightAction;
        private Action _toggleVisionAction;
        private Action<Vec2> _spawnSmokeAction;
        private Action<Vec2> _spawnFireAction;
        private Action<Vec2> _spawnLightAction;

        public void Initialize(BattleSandboxRuntimeContext context, Camera mainCamera, Action resetAction)
        {
            Initialize(context, mainCamera, resetAction, null, null, null);
        }

        public void Initialize(
            BattleSandboxRuntimeContext context,
            Camera mainCamera,
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
            if (keyboard != null && (keyboard.pKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame))
            {
                _context.TogglePause();
            }
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
                if (TryGetGroundPosition(out position))
                {
                    _spawnSmokeAction(position);
                }
            }

            if (keyboard.fKey.wasPressedThisFrame && _spawnFireAction != null)
            {
                Vec2 position;
                if (TryGetGroundPosition(out position))
                {
                    _spawnFireAction(position);
                }
            }

            if (keyboard.lKey.wasPressedThisFrame && _spawnLightAction != null)
            {
                Vec2 position;
                if (TryGetGroundPosition(out position))
                {
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
                RaycastHit hit;
                if (TryRaycast(out hit))
                {
                    _context.TacticalCommandService.MoveSquad(_context.SelectedSquadId, new Vec2(hit.point.x, hit.point.z));
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
                    _context.TacticalCommandService.DefendArea(_context.SelectedSquadId, new Vec2(hit.point.x, hit.point.z), 8f);
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
                }
            }

            if (keyboard.eKey.wasPressedThisFrame && squad != null)
            {
                TacticalNodeSnapshot node = BattleSandboxCommandQueries.FindNearestNode(snapshot, squad.Position, TacticalNodeType.ExtractionPoint, false);
                if (node != null)
                {
                    _context.TacticalCommandService.ExtractSquad(_context.SelectedSquadId, node.NodeId);
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
                    }

                    if (keyboard.hKey.wasPressedThisFrame)
                    {
                        _context.TacticalCommandService.DefendBuilding(_context.SelectedSquadId, building.BuildingId);
                    }

                    if (keyboard.jKey.wasPressedThisFrame)
                    {
                        _context.TacticalCommandService.SearchBuilding(_context.SelectedSquadId, building.BuildingId);
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

        private bool TryGetGroundPosition(out Vec2 position)
        {
            RaycastHit hit;
            if (TryRaycast(out hit))
            {
                position = new Vec2(hit.point.x, hit.point.z);
                return true;
            }

            position = Vec2.Zero;
            return false;
        }
    }
}
