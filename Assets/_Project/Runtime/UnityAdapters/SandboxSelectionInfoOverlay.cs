using UnityEngine;
using Warzone.Content.Definitions;

namespace Warzone.UnityAdapters
{
    public sealed class SandboxSelectionInfoOverlay : MonoBehaviour
    {
        private UnitDefinition _playerDefinition;
        private int _playerCurrentHealth;
        private int _playerMaxHealth;
        private string _playerLabel;

        private UnitDefinition _hoverDefinition;
        private int _hoverCurrentHealth;
        private int _hoverMaxHealth;
        private string _hoverLabel;

        public void BindSelection(string label, UnitDefinition definition, int currentHealth, int maxHealth)
        {
            _playerLabel = label;
            _playerDefinition = definition;
            _playerCurrentHealth = currentHealth;
            _playerMaxHealth = maxHealth;
        }

        public void BindHover(string label, UnitDefinition definition, int currentHealth, int maxHealth)
        {
            _hoverLabel = label;
            _hoverDefinition = definition;
            _hoverCurrentHealth = currentHealth;
            _hoverMaxHealth = maxHealth;
        }

        private void OnGUI()
        {
            if (_playerDefinition != null)
            {
                GUILayout.BeginArea(new Rect(20f, Screen.height - 160f, 280f, 140f), GUI.skin.box);
                GUILayout.Label(_playerLabel);
                GUILayout.Label($"HP: {_playerCurrentHealth}/{_playerMaxHealth}");
                GUILayout.Label($"Weapon: {_playerDefinition.Weapon.Id}");
                GUILayout.Label($"Damage: {_playerDefinition.Weapon.DamagePerHit}");
                GUILayout.Label($"Range: {_playerDefinition.Weapon.Range:F1}");
                GUILayout.Label($"Speed: {_playerDefinition.MoveSpeed:F1}");
                GUILayout.EndArea();
            }

            if (_hoverDefinition != null)
            {
                GUILayout.BeginArea(new Rect(Screen.width - 300f, Screen.height - 160f, 280f, 140f), GUI.skin.box);
                GUILayout.Label(_hoverLabel);
                GUILayout.Label($"HP: {_hoverCurrentHealth}/{_hoverMaxHealth}");
                GUILayout.Label($"Weapon: {_hoverDefinition.Weapon.Id}");
                GUILayout.Label($"Damage: {_hoverDefinition.Weapon.DamagePerHit}");
                GUILayout.Label($"Range: {_hoverDefinition.Weapon.Range:F1}");
                GUILayout.Label($"Speed: {_hoverDefinition.MoveSpeed:F1}");
                GUILayout.EndArea();
            }
        }
    }
}
