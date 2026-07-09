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
            Action emitIncomingFireAction)
        {
            _context = context;
            _mainCamera = mainCamera;
            _resetAction = resetAction;
            _applyPressureAction = applyPressureAction;
            _clearPressureAction = clearPressureAction;
            _emitIncomingFireAction = emitIncomingFireAction;
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
