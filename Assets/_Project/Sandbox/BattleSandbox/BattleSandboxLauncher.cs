using UnityEngine;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class BattleSandboxLauncher : MonoBehaviour
    {
        [SerializeField] private BattleSandboxMode mode = BattleSandboxMode.M5IntegratedSandbox;

        public BattleSandboxMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        private void Awake()
        {
            Launch();
        }

        public void Launch()
        {
            DisableAllEntries();

            switch (mode)
            {
                case BattleSandboxMode.M1MemberMovement:
                    EnsureCompatibilityEntry<M1MemberSquadSandboxBootstrap>();
                    break;
                case BattleSandboxMode.M2CombatSlice:
                    EnsureCompatibilityEntry<M2CombatSliceSandboxBootstrap>();
                    break;
                case BattleSandboxMode.M3TacticalMission:
                    EnsureCompatibilityEntry<M3TacticalMissionSandboxBootstrap>();
                    break;
                case BattleSandboxMode.M4SpatialCombat:
                    EnsureCompatibilityEntry<M4SpatialCombatSandboxBootstrap>();
                    break;
                case BattleSandboxMode.M5IntegratedSandbox:
                    EnsureCompatibilityEntry<M5IntegratedSandboxBootstrap>();
                    break;
                case BattleSandboxMode.M6PressureRetreat:
                    EnsureCompatibilityEntry<M6PressureRetreatSandboxBootstrap>();
                    break;
                case BattleSandboxMode.M7EnvironmentCombat:
                    EnsureCompatibilityEntry<M7EnvironmentCombatSandboxBootstrap>();
                    break;
                default:
                    EnsureCompatibilityEntry<M5IntegratedSandboxBootstrap>();
                    break;
            }

            enabled = false;
        }

        private void DisableAllEntries()
        {
            DisableEntry<M1MemberSquadSandboxBootstrap>();
            DisableEntry<M2CombatSliceSandboxBootstrap>();
            DisableEntry<M3TacticalMissionSandboxBootstrap>();
            DisableEntry<M4SpatialCombatSandboxBootstrap>();
            DisableEntry<M5IntegratedSandboxBootstrap>();
            DisableEntry<M6PressureRetreatSandboxBootstrap>();
            DisableEntry<M7EnvironmentCombatSandboxBootstrap>();
        }

        private void DisableEntry<T>() where T : MonoBehaviour
        {
            T entry = GetComponent<T>();
            if (entry != null)
            {
                entry.enabled = false;
            }
        }

        private void EnsureCompatibilityEntry<T>() where T : MonoBehaviour
        {
            T entry = GetComponent<T>();
            if (entry == null)
            {
                entry = gameObject.AddComponent<T>();
            }

            entry.enabled = true;
        }
    }
}
