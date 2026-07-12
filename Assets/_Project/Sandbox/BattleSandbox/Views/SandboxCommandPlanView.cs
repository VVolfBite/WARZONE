using System.Collections.Generic;
using UnityEngine;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class SandboxCommandPlanView : MonoBehaviour
    {
        private readonly List<LineRenderer> _lines = new List<LineRenderer>();
        private readonly List<GameObject> _markers = new List<GameObject>();
        private BattleSnapshot _snapshot;
        private int _selectedSquadId;
        private bool _isVisible;
        private Material _lineMaterial;
        private Material _markerMaterial;

        public void Render(
            BattleSnapshot snapshot,
            IReadOnlyDictionary<BattleEntityId, SandboxMemberView> memberViews,
            IReadOnlyDictionary<int, SandboxTacticalNodeView> nodeViews,
            int selectedSquadId,
            bool isVisible)
        {
            _snapshot = snapshot;
            _selectedSquadId = selectedSquadId;
            _isVisible = isVisible;
            ClearDrawables();

            if (!isVisible || snapshot == null)
            {
                return;
            }

            EnsureMaterials();
            BattleSquadSnapshot squad = BattleSandboxCommandQueries.FindSelectedSquad(snapshot, selectedSquadId);
            if (squad == null)
            {
                return;
            }

            AddLine(ToWorld(squad.Position, 0.12f), ToWorld(squad.DesiredPosition, 0.12f), new Color(1f, 0.88f, 0.22f), 0.08f);
            AddMarker("SandboxSquadTarget_" + selectedSquadId, squad.DesiredPosition, new Color(1f, 0.88f, 0.22f), 0.55f);

            for (int i = 0; i < snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = snapshot.Members[i];
                if (member.SquadId != selectedSquadId || !member.IsAlive || member.IsExtracted || !member.MoveTarget.HasValue)
                {
                    continue;
                }

                Vec2 target = member.MoveTarget.Value;
                AddLine(ToWorld(member.Position, 0.18f), ToWorld(target, 0.18f), new Color(0.45f, 0.85f, 1f), 0.035f);
                AddMarker("SandboxMemberIntentTarget_" + member.MemberId.Value, target, new Color(0.45f, 0.85f, 1f), 0.28f);
            }
        }

        private void OnGUI()
        {
            if (!_isVisible || _snapshot == null)
            {
                return;
            }

            BattleSquadSnapshot squad = BattleSandboxCommandQueries.FindSelectedSquad(_snapshot, _selectedSquadId);
            if (squad == null)
            {
                return;
            }

            GUILayout.BeginArea(new Rect(Screen.width - 330f, 12f, 318f, 360f), GUI.skin.box);
            GUILayout.Label("Selected Squad " + squad.SquadId);
            GUILayout.Label("Current Order: " + squad.CurrentCommand);
            GUILayout.Label("Desired: " + FormatVec2(squad.DesiredPosition));
            GUILayout.Space(4f);
            GUILayout.Label("Member Assigned Targets");

            for (int i = 0; i < _snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = _snapshot.Members[i];
                if (member.SquadId != _selectedSquadId)
                {
                    continue;
                }

                string target = member.MoveTarget.HasValue ? FormatVec2(member.MoveTarget.Value) : "-";
                TacticalNodeSnapshot node = member.MoveTarget.HasValue ? FindNodeAt(_snapshot, member.MoveTarget.Value) : null;
                string nodeText = node != null ? " node " + node.NodeId + " " + node.NodeType : string.Empty;
                GUILayout.Label("#" + member.MemberId.Value + " " + member.CurrentIntent + " -> " + target + nodeText);
            }

            GUILayout.EndArea();
        }

        private LineRenderer AddLine(Vector3 start, Vector3 end, Color color, float width)
        {
            GameObject lineObject = new GameObject("SandboxCommandPlanLine");
            lineObject.transform.SetParent(transform, false);
            LineRenderer line = lineObject.AddComponent<LineRenderer>();
            line.material = _lineMaterial;
            line.positionCount = 2;
            line.SetPosition(0, start);
            line.SetPosition(1, end);
            line.startWidth = width;
            line.endWidth = width;
            line.startColor = color;
            line.endColor = color;
            _lines.Add(line);
            return line;
        }

        private void AddMarker(string name, Vec2 position, Color color, float scale)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = name;
            marker.transform.SetParent(transform, false);
            marker.transform.position = ToWorld(position, 0.1f);
            marker.transform.localScale = new Vector3(scale, 0.04f, scale);
            Renderer renderer = marker.GetComponent<Renderer>();
            renderer.material = _markerMaterial;
            renderer.material.color = color;
            _markers.Add(marker);
        }

        private void ClearDrawables()
        {
            for (int i = 0; i < _lines.Count; i++)
            {
                if (_lines[i] != null)
                {
                    Destroy(_lines[i].gameObject);
                }
            }

            for (int i = 0; i < _markers.Count; i++)
            {
                if (_markers[i] != null)
                {
                    Destroy(_markers[i]);
                }
            }

            _lines.Clear();
            _markers.Clear();
        }

        private void EnsureMaterials()
        {
            if (_lineMaterial == null)
            {
                _lineMaterial = new Material(Shader.Find("Sprites/Default"));
            }

            if (_markerMaterial == null)
            {
                _markerMaterial = new Material(Shader.Find("Standard"));
            }
        }

        private static TacticalNodeSnapshot FindNodeAt(BattleSnapshot snapshot, Vec2 position)
        {
            for (int i = 0; i < snapshot.TacticalNodes.Count; i++)
            {
                TacticalNodeSnapshot node = snapshot.TacticalNodes[i];
                if (Vec2.DistanceSquared(node.Position, position) <= 0.01f)
                {
                    return node;
                }
            }

            return null;
        }

        private static Vector3 ToWorld(Vec2 position, float y)
        {
            return new Vector3(position.X, y, position.Y);
        }

        private static string FormatVec2(Vec2 value)
        {
            return "(" + value.X.ToString("F1") + ", " + value.Y.ToString("F1") + ")";
        }
    }
}
