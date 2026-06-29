using UnityEngine;
using UnityEngine.InputSystem;

namespace Warzone.Adapters
{
    public sealed class RtsCameraController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float baseMoveSpeed = 18f;
        [SerializeField] private float moveSmoothTime = 0.08f;
        [SerializeField] private bool enableEdgePanning = true;
        [SerializeField] private float edgePanningThickness = 10f;

        [Header("Zoom")]
        [SerializeField] private float zoomSpeed = 0.08f;
        [SerializeField] private float minHeight = 8f;
        [SerializeField] private float maxHeight = 30f;

        [Header("Rotation")]
        [SerializeField] private float rotationSpeed = 0.18f;
        [SerializeField] private float minPitch = 35f;
        [SerializeField] private float maxPitch = 75f;

        [Header("Bounds")]
        [SerializeField] private Vector2 minBounds = new Vector2(-40f, -40f);
        [SerializeField] private Vector2 maxBounds = new Vector2(40f, 40f);

        private Transform _pivot;
        private Vector3 _targetPosition;
        private Vector3 _positionVelocity;
        private float _yaw;
        private float _pitch = 55f;

        private void Awake()
        {
            GameObject pivotObject = new GameObject("RtsCameraPivot");
            _pivot = pivotObject.transform;
            _pivot.position = transform.position;
            _targetPosition = _pivot.position;

            Vector3 initialEuler = transform.rotation.eulerAngles;
            _yaw = initialEuler.y;
            _pitch = NormalizePitch(initialEuler.x);
            ApplyTransformImmediate();
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return;
            }

            Mouse mouse = Mouse.current;
            Vector2 mousePosition = mouse != null ? mouse.position.ReadValue() : Vector2.zero;

            HandleMovement(keyboard, mousePosition);
            HandleRotation(mouse);
            HandleZoom(mouse);

            ClampTargetPosition();
            _pivot.position = Vector3.SmoothDamp(_pivot.position, _targetPosition, ref _positionVelocity, moveSmoothTime);
            ApplyTransformImmediate();
        }

        private void HandleMovement(Keyboard keyboard, Vector2 mousePosition)
        {
            float horizontal = 0f;
            float vertical = 0f;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                horizontal -= 1f;
            }
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                horizontal += 1f;
            }
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                vertical -= 1f;
            }
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                vertical += 1f;
            }

            if (enableEdgePanning && IsPointerInGameWindow(mousePosition))
            {
                if (mousePosition.x <= edgePanningThickness)
                {
                    horizontal -= 1f;
                }
                else if (mousePosition.x >= Screen.width - edgePanningThickness)
                {
                    horizontal += 1f;
                }

                if (mousePosition.y <= edgePanningThickness)
                {
                    vertical -= 1f;
                }
                else if (mousePosition.y >= Screen.height - edgePanningThickness)
                {
                    vertical += 1f;
                }
            }

            Vector3 input = new Vector3(horizontal, 0f, vertical);
            if (input.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            float heightMultiplier = Mathf.Clamp(_pivot.position.y / maxHeight, 0.35f, 1.5f);
            float moveSpeed = baseMoveSpeed * heightMultiplier * Time.deltaTime;
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
            Vector3 movement = ((right * input.x) + (forward * input.z)).normalized * moveSpeed;
            _targetPosition += movement;
        }

        private void HandleRotation(Mouse mouse)
        {
            if (mouse == null || !mouse.middleButton.isPressed)
            {
                return;
            }

            Vector2 delta = mouse.delta.ReadValue();
            _yaw += delta.x * rotationSpeed;
            _pitch = Mathf.Clamp(_pitch - (delta.y * rotationSpeed), minPitch, maxPitch);
        }

        private void HandleZoom(Mouse mouse)
        {
            if (mouse == null)
            {
                return;
            }

            float scroll = mouse.scroll.ReadValue().y;
            if (Mathf.Approximately(scroll, 0f))
            {
                return;
            }

            Vector3 target = _targetPosition;
            target.y = Mathf.Clamp(target.y - (scroll * zoomSpeed), minHeight, maxHeight);
            _targetPosition = target;
        }

        private void ClampTargetPosition()
        {
            Vector3 clamped = _targetPosition;
            clamped.x = Mathf.Clamp(clamped.x, minBounds.x, maxBounds.x);
            clamped.z = Mathf.Clamp(clamped.z, minBounds.y, maxBounds.y);
            clamped.y = Mathf.Clamp(clamped.y, minHeight, maxHeight);
            _targetPosition = clamped;
        }

        private void ApplyTransformImmediate()
        {
            _pivot.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            transform.position = _pivot.position;
            transform.rotation = _pivot.rotation;
        }

        private static bool IsPointerInGameWindow(Vector2 mousePosition)
        {
            return mousePosition.x >= 0f &&
                mousePosition.y >= 0f &&
                mousePosition.x <= Screen.width &&
                mousePosition.y <= Screen.height;
        }

        private static float NormalizePitch(float pitch)
        {
            if (pitch > 180f)
            {
                pitch -= 360f;
            }

            return pitch;
        }
    }
}
