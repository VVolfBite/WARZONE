using UnityEngine;
using Warzone.Combat;
using Warzone.Content.Definitions;

namespace Warzone.Sandbox.UI
{
    public sealed class SandboxSelectionInfoOverlay : MonoBehaviour
    {
        private UnitDefinition _playerDefinition;
        private int _playerCurrentHealth;
        private int _playerMaxHealth;
        private string _playerLabel;
        private string _playerStatusText;
        private string _playerAbilityText;

        private UnitDefinition _hoverDefinition;
        private int _hoverCurrentHealth;
        private int _hoverMaxHealth;
        private string _hoverLabel;
        private string _hoverStatusText;
        private string _hoverAbilityText;

        public void BindSelection(string label, UnitDefinition definition, int currentHealth, int maxHealth, BattleUnitState unit)
        {
            _playerLabel = label;
            _playerDefinition = definition;
            _playerCurrentHealth = currentHealth;
            _playerMaxHealth = maxHealth;
            _playerStatusText = BuildStatusText(unit);
            _playerAbilityText = definition != null && !string.IsNullOrEmpty(definition.ActiveAbilityId) ? definition.ActiveAbilityId : "None";
        }

        public void BindHover(string label, UnitDefinition definition, int currentHealth, int maxHealth, BattleUnitState unit)
        {
            _hoverLabel = label;
            _hoverDefinition = definition;
            _hoverCurrentHealth = currentHealth;
            _hoverMaxHealth = maxHealth;
            _hoverStatusText = BuildStatusText(unit);
            _hoverAbilityText = definition != null && !string.IsNullOrEmpty(definition.ActiveAbilityId) ? definition.ActiveAbilityId : "None";
        }

        private void OnGUI()
        {
            if (_playerDefinition != null)
            {
                GUILayout.BeginArea(new Rect(20f, Screen.height - 300f, 300f, 180f), GUI.skin.box);
                GUILayout.Label(_playerLabel);
                GUILayout.Label($"HP: {_playerCurrentHealth}/{_playerMaxHealth}");
                GUILayout.Label($"Weapon: {_playerDefinition.Weapon.Id}");
                GUILayout.Label($"Damage: {_playerDefinition.Weapon.DamagePerHit}");
                GUILayout.Label($"Range: {_playerDefinition.Weapon.Range:F1}");
                GUILayout.Label($"Armor: {_playerDefinition.ArmorType}");
                GUILayout.Label($"Aggro: {_playerDefinition.AggroRange:F1}");
                GUILayout.Label($"Speed: {_playerDefinition.MoveSpeed:F1}");
                GUILayout.Label($"Ability: {_playerAbilityText}");
                GUILayout.Label($"Status: {_playerStatusText}");
                GUILayout.EndArea();
            }

            if (_hoverDefinition != null)
            {
                GUILayout.BeginArea(new Rect(Screen.width - 320f, Screen.height - 200f, 300f, 180f), GUI.skin.box);
                GUILayout.Label(_hoverLabel);
                GUILayout.Label($"HP: {_hoverCurrentHealth}/{_hoverMaxHealth}");
                GUILayout.Label($"Weapon: {_hoverDefinition.Weapon.Id}");
                GUILayout.Label($"Damage: {_hoverDefinition.Weapon.DamagePerHit}");
                GUILayout.Label($"Range: {_hoverDefinition.Weapon.Range:F1}");
                GUILayout.Label($"Armor: {_hoverDefinition.ArmorType}");
                GUILayout.Label($"Aggro: {_hoverDefinition.AggroRange:F1}");
                GUILayout.Label($"Speed: {_hoverDefinition.MoveSpeed:F1}");
                GUILayout.Label($"Ability: {_hoverAbilityText}");
                GUILayout.Label($"Status: {_hoverStatusText}");
                GUILayout.EndArea();
            }
        }

        private static string BuildStatusText(BattleUnitState unit)
        {
            if (unit == null || unit.StatusEffects.Count == 0)
            {
                return "None";
            }

            string value = string.Empty;
            for (int i = 0; i < unit.StatusEffects.Count; i++)
            {
                if (i > 0)
                {
                    value += ", ";
                }

                ActiveStatusEffect effect = unit.StatusEffects[i];
                value += effect.Definition.DisplayName + " " + effect.RemainingDuration.ToString("F1") + "s";
            }

            return value;
        }
    }
}



