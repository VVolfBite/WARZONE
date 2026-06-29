using UnityEngine;
using Warzone.Combat;
using Warzone.Content.Definitions;
using Warzone.Presentation.Units;

namespace Warzone.Adapters
{
    public sealed class SandboxSquadView : MonoBehaviour
    {
        [SerializeField] private int squadId;
        [SerializeField] private FactionId factionId;
        [SerializeField] private UnitView unitView;
        [SerializeField] private UnitWorldUiView worldUiView;
        [SerializeField] private bool isDead;
        private Vector3 _aliveScale = Vector3.one;
        private float _deathSinkOffset;

        public int SquadId => squadId;
        public FactionId FactionId => factionId;
        public bool IsDead => isDead;

        public void Initialize(int newSquadId, FactionId newFactionId, UnitView newUnitView, UnitWorldUiView newWorldUiView)
        {
            squadId = newSquadId;
            factionId = newFactionId;
            unitView = newUnitView;
            worldUiView = newWorldUiView;
            _aliveScale = transform.localScale;
        }

        public void SetSelected(bool selected)
        {
            if (isDead)
            {
                return;
            }

            if (unitView != null)
            {
                unitView.SetSelected(selected);
            }
        }

        public void SetCommandState(SquadCommandState commandState)
        {
            if (isDead)
            {
                return;
            }

            if (unitView != null)
            {
                unitView.SetCommandStateColor(commandState);
            }
        }

        public void SetDead()
        {
            if (isDead)
            {
                return;
            }

            isDead = true;
            if (unitView != null)
            {
                unitView.SetDead();
            }

            if (worldUiView != null)
            {
                worldUiView.SetRangeVisible(false, 0f, Color.gray);
            }

            transform.localScale = new Vector3(_aliveScale.x, _aliveScale.y * 0.35f, _aliveScale.z);
            _deathSinkOffset = 0.65f;
        }

        public void SetHealth(float currentHealthNormalized)
        {
            if (worldUiView != null)
            {
                worldUiView.SetHealth(currentHealthNormalized);
            }
        }

        public void SetRangeVisible(bool visible, float radius)
        {
            if (worldUiView != null)
            {
                worldUiView.SetRangeVisible(visible, radius, factionId == FactionId.Player ? Color.cyan : Color.red);
            }
        }

        private void LateUpdate()
        {
            if (!isDead || _deathSinkOffset <= 0f)
            {
                return;
            }

            float sinkStep = Time.deltaTime * 1.2f;
            float applied = Mathf.Min(sinkStep, _deathSinkOffset);
            transform.position += Vector3.down * applied;
            _deathSinkOffset -= applied;
        }
    }
}
