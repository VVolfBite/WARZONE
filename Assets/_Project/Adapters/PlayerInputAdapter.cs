using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Warzone.Adapters
{
    public sealed class PlayerInputAdapter : MonoBehaviour
    {
        public event Action LeftClickPressed;
        public event Action RightClickPressed;

        private void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            if (mouse.leftButton.wasPressedThisFrame)
            {
                LeftClickPressed?.Invoke();
            }

            if (mouse.rightButton.wasPressedThisFrame)
            {
                RightClickPressed?.Invoke();
            }
        }
    }
}
