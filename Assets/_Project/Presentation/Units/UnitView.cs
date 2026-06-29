using UnityEngine;

namespace Warzone.Presentation.Units
{
    public sealed class UnitView : MonoBehaviour
    {
        [SerializeField] private Renderer[] targetRenderers;
        [SerializeField] private Color selectedColor = Color.green;
        [SerializeField] private Color movingColor = Color.yellow;
        [SerializeField] private Color attackingColor = new Color(1f, 0.4f, 0.4f);
        [SerializeField] private Color deadColor = Color.gray;

        private Color[] _baseColors;
        private bool _selected;
        private bool _isDead;
        private float _hitFlashTime;

        private void Awake()
        {
            CacheBaseColors();
        }

        public void Initialize(Renderer[] renderers)
        {
            targetRenderers = renderers;
            CacheBaseColors();
        }

        private void CacheBaseColors()
        {
            if (targetRenderers == null)
            {
                _baseColors = new Color[0];
                return;
            }

            _baseColors = new Color[targetRenderers.Length];
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                _baseColors[i] = targetRenderers[i].material.color;
            }
        }

        public void SetSelected(bool selected)
        {
            if (_isDead)
            {
                return;
            }

            _selected = selected;
            ApplyColorOverride(null);
        }

        public void SetCommandStateColor(Warzone.Combat.SquadCommandState commandState)
        {
            if (_isDead)
            {
                return;
            }

            switch (commandState)
            {
                case Warzone.Combat.SquadCommandState.Moving:
                    ApplyColorOverride(movingColor);
                    break;
                case Warzone.Combat.SquadCommandState.Attacking:
                    ApplyColorOverride(attackingColor);
                    break;
                default:
                    ApplyColorOverride(null);
                    break;
            }
        }

        public void SetDead()
        {
            _isDead = true;
            _selected = false;
            ApplyColorOverride(deadColor);
        }

        public void FlashHit()
        {
            if (_isDead)
            {
                return;
            }

            _hitFlashTime = 0.12f;
        }

        private void Update()
        {
            if (_isDead)
            {
                return;
            }

            if (_hitFlashTime > 0f)
            {
                _hitFlashTime -= Time.deltaTime;
                ApplyColorOverride(Color.white);
            }
            else
            {
                ApplyColorOverride(null);
            }
        }

        private void ApplyColorOverride(Color? overrideColor)
        {
            for (int i = 0; i < targetRenderers.Length; i++)
            {
                if (_selected)
                {
                    targetRenderers[i].material.color = selectedColor;
                }
                else if (overrideColor.HasValue)
                {
                    targetRenderers[i].material.color = overrideColor.Value;
                }
                else
                {
                    targetRenderers[i].material.color = _baseColors[i];
                }
            }
        }
    }
}
