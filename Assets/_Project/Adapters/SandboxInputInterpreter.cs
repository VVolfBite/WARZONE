using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Warzone.Combat;
using Warzone.Content.Definitions;

namespace Warzone.Adapters
{
    public sealed class SandboxInputInterpreter
    {
        private readonly Camera _mainCamera;
        private readonly SelectionBoxOverlay _selectionBoxOverlay;
        private readonly SandboxSelectionService _selectionService;
        private readonly SandboxCommandDispatcher _commandDispatcher;
        private readonly SandboxPresentationSync _presentationSync;
        private Vector2 _selectionStart;
        private bool _isDraggingSelection;

        public SandboxInputInterpreter(
            Camera mainCamera,
            SelectionBoxOverlay selectionBoxOverlay,
            SandboxSelectionService selectionService,
            SandboxCommandDispatcher commandDispatcher,
            SandboxPresentationSync presentationSync)
        {
            _mainCamera = mainCamera;
            _selectionBoxOverlay = selectionBoxOverlay;
            _selectionService = selectionService;
            _commandDispatcher = commandDispatcher;
            _presentationSync = presentationSync;
        }

        public bool ConsumePauseToggle()
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard.pKey.wasPressedThisFrame;
        }

        public bool ConsumeCameraFocus()
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard.spaceKey.wasPressedThisFrame;
        }

        public bool ConsumePrimaryAbility()
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && keyboard.qKey.wasPressedThisFrame;
        }

        public bool TryConsumeTeamCommand(out int slotIndex, out bool bindTeam)
        {
            slotIndex = -1;
            bindTeam = false;

            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return false;
            }

            bindTeam = keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed;
            if (keyboard.digit1Key.wasPressedThisFrame) { slotIndex = 0; return true; }
            if (keyboard.digit2Key.wasPressedThisFrame) { slotIndex = 1; return true; }
            if (keyboard.digit3Key.wasPressedThisFrame) { slotIndex = 2; return true; }
            if (keyboard.digit4Key.wasPressedThisFrame) { slotIndex = 3; return true; }
            if (keyboard.digit5Key.wasPressedThisFrame) { slotIndex = 4; return true; }
            if (keyboard.digit6Key.wasPressedThisFrame) { slotIndex = 5; return true; }
            if (keyboard.digit7Key.wasPressedThisFrame) { slotIndex = 6; return true; }
            if (keyboard.digit8Key.wasPressedThisFrame) { slotIndex = 7; return true; }
            if (keyboard.digit9Key.wasPressedThisFrame) { slotIndex = 8; return true; }

            return false;
        }

        public void Tick(BattleSession battleSession)
        {
            if (battleSession == null || _mainCamera == null)
            {
                return;
            }

            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            if (mouse.leftButton.wasPressedThisFrame)
            {
                _selectionStart = mouse.position.ReadValue();
                _isDraggingSelection = true;
            }

            if (mouse.rightButton.wasPressedThisFrame)
            {
                TryIssueCommand(battleSession);
            }

            if (_isDraggingSelection)
            {
                Vector2 current = mouse.position.ReadValue();
                Rect selectionRect = BuildScreenRect(_selectionStart, current);
                _selectionBoxOverlay.SetSelection(selectionRect, true);

                if (mouse.leftButton.wasReleasedThisFrame)
                {
                    _isDraggingSelection = false;
                    _selectionBoxOverlay.SetSelection(Rect.zero, false);

                    if (selectionRect.width < 8f && selectionRect.height < 8f)
                    {
                        TrySelectSingleSquad();
                    }
                    else
                    {
                        TrySelectSquadsInRect(selectionRect);
                    }
                }
            }

            UpdateHoverTarget();
        }

        private void TrySelectSingleSquad()
        {
            if (!TryRaycast(out RaycastHit hit))
            {
                if (!IsShiftHeld() && !IsCtrlHeld())
                {
                    _selectionService.Clear();
                }

                return;
            }

            SandboxSquadView squadView = hit.collider.GetComponent<SandboxSquadView>();
            if (squadView != null && squadView.FactionId == FactionId.Player)
            {
                bool shift = IsShiftHeld();
                bool ctrl = IsCtrlHeld();

                if (!shift && !ctrl)
                {
                    _selectionService.SelectExclusive(squadView.SquadId);
                }
                else if (ctrl)
                {
                    _selectionService.Toggle(squadView.SquadId);
                }
                else
                {
                    _selectionService.Add(squadView.SquadId);
                }
            }
            else if (squadView != null && squadView.FactionId == FactionId.Enemy)
            {
                _selectionService.SetHoverEnemy(squadView.SquadId);
                if (!IsShiftHeld() && !IsCtrlHeld())
                {
                    _selectionService.Clear();
                }
            }
            else if (!IsShiftHeld() && !IsCtrlHeld())
            {
                _selectionService.Clear();
            }
        }

        private void TryIssueCommand(BattleSession battleSession)
        {
            if (_selectionService.SelectedSquadIds.Count == 0 || !TryRaycast(out RaycastHit hit))
            {
                return;
            }

            SandboxSquadView squadView = hit.collider.GetComponent<SandboxSquadView>();
            if (squadView != null && squadView.FactionId == FactionId.Enemy)
            {
                _commandDispatcher.IssueAttack(battleSession, _selectionService.SelectedSquadIds, squadView.SquadId, IsShiftHeld());
                _presentationSync.SpawnCommandMarker(hit.point, Color.red);
                return;
            }

            List<int> orderedSquadIds = _selectionService.BuildOrderedSelection();
            _commandDispatcher.IssueMove(battleSession, orderedSquadIds, hit.point, IsShiftHeld());
            _presentationSync.SpawnCommandMarker(hit.point, Color.cyan);
        }

        private void TrySelectSquadsInRect(Rect selectionRect)
        {
            bool shift = IsShiftHeld();
            bool ctrl = IsCtrlHeld();
            List<int> boxedSelection = new List<int>();

            foreach (KeyValuePair<int, SandboxSquadView> pair in _presentationSync.SquadViews)
            {
                SandboxSquadView squadView = pair.Value;
                if (squadView == null || squadView.FactionId != FactionId.Player)
                {
                    continue;
                }

                Vector3 screenPosition = _mainCamera.WorldToScreenPoint(squadView.transform.position);
                Vector2 guiPoint = new Vector2(screenPosition.x, Screen.height - screenPosition.y);
                if (selectionRect.Contains(guiPoint))
                {
                    boxedSelection.Add(pair.Key);
                }
            }

            if (!shift && !ctrl)
            {
                _selectionService.Clear();
                for (int i = 0; i < boxedSelection.Count; i++)
                {
                    _selectionService.Add(boxedSelection[i]);
                }

                return;
            }

            if (shift)
            {
                for (int i = 0; i < boxedSelection.Count; i++)
                {
                    _selectionService.Add(boxedSelection[i]);
                }
            }

            if (ctrl)
            {
                for (int i = 0; i < boxedSelection.Count; i++)
                {
                    _selectionService.Toggle(boxedSelection[i]);
                }
            }
        }

        private void UpdateHoverTarget()
        {
            _selectionService.SetHoverEnemy(null);
            if (!TryRaycast(out RaycastHit hit))
            {
                return;
            }

            SandboxSquadView squadView = hit.collider.GetComponent<SandboxSquadView>();
            if (squadView != null && squadView.FactionId == FactionId.Enemy)
            {
                _selectionService.SetHoverEnemy(squadView.SquadId);
            }
        }

        private bool TryRaycast(out RaycastHit hit)
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                hit = default;
                return false;
            }

            Ray ray = _mainCamera.ScreenPointToRay(mouse.position.ReadValue());
            return Physics.Raycast(ray, out hit, 1000f);
        }

        private static Rect BuildScreenRect(Vector2 start, Vector2 end)
        {
            float xMin = Mathf.Min(start.x, end.x);
            float xMax = Mathf.Max(start.x, end.x);
            float yMin = Mathf.Min(start.y, end.y);
            float yMax = Mathf.Max(start.y, end.y);
            return Rect.MinMaxRect(xMin, Screen.height - yMax, xMax, Screen.height - yMin);
        }

        private static bool IsShiftHeld()
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed);
        }

        private static bool IsCtrlHeld()
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && (keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed);
        }
    }
}
