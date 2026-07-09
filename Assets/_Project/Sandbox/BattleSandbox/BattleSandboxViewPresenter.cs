using System.Collections.Generic;
using UnityEngine;
using Warzone.Combat;
using Warzone.Core.Math;

namespace Warzone.Sandbox.BattleSandbox
{
    public sealed class BattleSandboxViewPresenter : MonoBehaviour
    {
        private readonly Dictionary<BattleEntityId, SandboxMemberView> _memberViews = new Dictionary<BattleEntityId, SandboxMemberView>();
        private readonly Dictionary<int, SandboxSelectionMarkerView> _selectionMarkerViews = new Dictionary<int, SandboxSelectionMarkerView>();
        private readonly Dictionary<BattleEntityId, SandboxEnemyView> _enemyViews = new Dictionary<BattleEntityId, SandboxEnemyView>();
        private readonly Dictionary<int, SandboxTacticalNodeView> _nodeViews = new Dictionary<int, SandboxTacticalNodeView>();
        private readonly Dictionary<int, SandboxObstacleView> _obstacleViews = new Dictionary<int, SandboxObstacleView>();
        private readonly Dictionary<int, SandboxBuildingView> _buildingViews = new Dictionary<int, SandboxBuildingView>();
        private readonly Dictionary<int, SandboxEnvironmentalZoneView> _zoneViews = new Dictionary<int, SandboxEnvironmentalZoneView>();

        private SandboxFireLineView _fireLineView;

        private void Awake()
        {
            EnsureSupportComponents();
        }

        public void RebuildFromSnapshot(BattleSnapshot snapshot)
        {
            EnsureSupportComponents();
            ClearViews();

            if (snapshot == null)
            {
                return;
            }

            for (int i = 0; i < snapshot.Buildings.Count; i++)
            {
                CreateBuildingView(snapshot.Buildings[i]);
            }

            for (int i = 0; i < snapshot.Obstacles.Count; i++)
            {
                CreateObstacleView(snapshot.Obstacles[i]);
            }

            for (int i = 0; i < snapshot.Squads.Count; i++)
            {
                CreateSelectionMarkerView(snapshot.Squads[i]);
            }

            for (int i = 0; i < snapshot.Members.Count; i++)
            {
                CreateMemberView(snapshot.Members[i]);
            }

            for (int i = 0; i < snapshot.Enemies.Count; i++)
            {
                CreateEnemyView(snapshot.Enemies[i]);
            }

            for (int i = 0; i < snapshot.TacticalNodes.Count; i++)
            {
                CreateNodeView(snapshot.TacticalNodes[i]);
            }

            if (snapshot.Environment != null)
            {
                for (int i = 0; i < snapshot.Environment.Zones.Count; i++)
                {
                    CreateZoneView(snapshot.Environment.Zones[i]);
                }
            }
        }

        [System.Obsolete("Use RebuildFromSnapshot for snapshot-only view rebuilding.")]
        internal void Rebuild(BattleState battleState)
        {
            RebuildFromSnapshot(battleState != null ? BattleSnapshotFactory.Create(battleState) : null);
        }

        public void Refresh(BattleSnapshot snapshot, int selectedSquadId, bool showFireLines)
        {
            if (snapshot == null)
            {
                return;
            }

            for (int i = 0; i < snapshot.Squads.Count; i++)
            {
                BattleSquadSnapshot squad = snapshot.Squads[i];
                SandboxSelectionMarkerView markerView;
                if (_selectionMarkerViews.TryGetValue(squad.SquadId, out markerView) && markerView != null)
                {
                    markerView.transform.position = new Vector3(squad.Position.X, 0.25f, squad.Position.Y);
                    markerView.ApplySelected(squad.SquadId == selectedSquadId);
                }
            }

            for (int i = 0; i < snapshot.Members.Count; i++)
            {
                BattleMemberSnapshot member = snapshot.Members[i];
                SandboxMemberView memberView;
                if (_memberViews.TryGetValue(member.MemberId, out memberView) && memberView != null)
                {
                    memberView.transform.position = new Vector3(member.Position.X, member.IsAlive ? 0.6f : 0.15f, member.Position.Y);
                    memberView.ApplyState(member.SquadId == selectedSquadId, !member.IsAlive, member.IsExtracted, member.IsSuppressed, member.IsBroken || member.IsRetreating);
                }
            }

            for (int i = 0; i < snapshot.Enemies.Count; i++)
            {
                BattleEnemySnapshot enemy = snapshot.Enemies[i];
                SandboxEnemyView enemyView;
                if (_enemyViews.TryGetValue(enemy.EnemyId, out enemyView) && enemyView != null)
                {
                    enemyView.transform.position = new Vector3(enemy.Position.X, enemy.IsAlive ? 0.55f : 0.15f, enemy.Position.Y);
                    enemyView.ApplyState(enemy.IsAlive);
                }
            }

            for (int i = 0; i < snapshot.TacticalNodes.Count; i++)
            {
                TacticalNodeSnapshot node = snapshot.TacticalNodes[i];
                SandboxTacticalNodeView nodeView;
                if (_nodeViews.TryGetValue(node.NodeId, out nodeView) && nodeView != null)
                {
                    nodeView.ApplySnapshot(node);
                }
            }

            if (snapshot.Environment != null)
            {
                for (int i = 0; i < snapshot.Environment.Zones.Count; i++)
                {
                    EnvironmentalZoneSnapshot zone = snapshot.Environment.Zones[i];
                    SandboxEnvironmentalZoneView zoneView;
                    if (!_zoneViews.TryGetValue(zone.ZoneId, out zoneView) || zoneView == null)
                    {
                        CreateZoneView(zone);
                        _zoneViews.TryGetValue(zone.ZoneId, out zoneView);
                    }

                    if (zoneView != null)
                    {
                        zoneView.ApplySnapshot(zone);
                    }
                }
            }

            _fireLineView.Render(snapshot, _memberViews, _enemyViews, showFireLines);
        }

        public void ClearViews()
        {
            DestroyAll(_memberViews.Values);
            DestroyAll(_selectionMarkerViews.Values);
            DestroyAll(_enemyViews.Values);
            DestroyAll(_nodeViews.Values);
            DestroyAll(_obstacleViews.Values);
            DestroyAll(_buildingViews.Values);
            DestroyAll(_zoneViews.Values);

            _memberViews.Clear();
            _selectionMarkerViews.Clear();
            _enemyViews.Clear();
            _nodeViews.Clear();
            _obstacleViews.Clear();
            _buildingViews.Clear();
            _zoneViews.Clear();
        }

        private void EnsureSupportComponents()
        {
            if (_fireLineView == null)
            {
                _fireLineView = GetComponent<SandboxFireLineView>();
                if (_fireLineView == null)
                {
                    _fireLineView = gameObject.AddComponent<SandboxFireLineView>();
                }
            }
        }

        private void CreateMemberView(BattleMemberState memberState)
        {
            CreateMemberView(new BattleMemberSnapshot(
                memberState.MemberId,
                memberState.SquadId,
                memberState.Position,
                memberState.Facing,
                memberState.Health,
                memberState.MaxHealth,
                memberState.IsAlive,
                memberState.IsExtracted,
                memberState.OccupiedTacticalNodeId,
                memberState.WeaponId,
                memberState.CurrentTargetEnemyId,
                memberState.AttackCooldownRemaining,
                memberState.CurrentIntent != null ? memberState.CurrentIntent.IntentType.ToString() : "None",
                memberState.CurrentIntent != null ? (Vec2?)memberState.CurrentIntent.TargetPosition : null,
                memberState.CurrentIntent != null && memberState.CurrentIntent.IsCompleted,
                memberState.Pressure,
                memberState.MaxPressure,
                memberState.IsSuppressed,
                memberState.IsBroken,
                memberState.IsRetreating,
                memberState.RetreatTargetPosition,
                memberState.NightVisionLevel,
                memberState.SmokeVisionLevel,
                memberState.HasLightSource,
                memberState.DetectionRange,
                memberState.EffectiveDetectionRange));
        }

        private void CreateMemberView(BattleMemberSnapshot memberState)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            root.name = "SandboxMember_" + memberState.MemberId.Value;
            root.transform.SetParent(transform, false);
            root.transform.position = new Vector3(memberState.Position.X, 0.6f, memberState.Position.Y);
            root.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            Renderer renderer = root.GetComponent<Renderer>();
            renderer.material.color = new Color(0.25f, 0.55f, 0.75f);

            SandboxMemberView memberView = root.AddComponent<SandboxMemberView>();
            memberView.Initialize(memberState.MemberId.Value, memberState.SquadId, renderer);
            _memberViews[memberState.MemberId] = memberView;
        }

        private void CreateSelectionMarkerView(BattleSquadState squadState)
        {
            CreateSelectionMarkerView(new BattleSquadSnapshot(
                squadState.SquadId,
                squadState.FactionId,
                squadState.Position,
                squadState.DesiredPosition,
                squadState.CurrentOrder != null ? squadState.CurrentOrder.Name : "None",
                squadState.Stance,
                squadState.MemberIds.Count,
                squadState.MemberIds.Count,
                squadState.FormationSpacing));
        }

        private void CreateSelectionMarkerView(BattleSquadSnapshot squadState)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            root.name = "SandboxSquad_" + squadState.SquadId;
            root.transform.SetParent(transform, false);
            root.transform.position = new Vector3(squadState.Position.X, 0.25f, squadState.Position.Y);
            root.transform.localScale = new Vector3(0.45f, 0.1f, 0.45f);
            Renderer renderer = root.GetComponent<Renderer>();
            renderer.material.color = new Color(0.65f, 0.65f, 0.2f);

            SandboxSelectionMarkerView view = root.AddComponent<SandboxSelectionMarkerView>();
            view.Initialize(squadState.SquadId, renderer);
            _selectionMarkerViews[squadState.SquadId] = view;
        }

        private void CreateEnemyView(BattleEnemyState enemyState)
        {
            CreateEnemyView(new BattleEnemySnapshot(
                enemyState.EnemyId,
                enemyState.DefinitionId,
                enemyState.FactionId,
                enemyState.Position,
                enemyState.Health,
                enemyState.MaxHealth,
                enemyState.IsAlive,
                enemyState.OccupiedTacticalNodeId,
                enemyState.CurrentTargetMemberId,
                enemyState.AttackCooldownRemaining,
                enemyState.NightVisionLevel,
                enemyState.HasLightSource,
                enemyState.DetectionRange,
                enemyState.EffectiveDetectionRange));
        }

        private void CreateEnemyView(BattleEnemySnapshot enemyState)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Cube);
            root.name = "SandboxEnemy_" + enemyState.EnemyId.Value;
            root.transform.SetParent(transform, false);
            root.transform.position = new Vector3(enemyState.Position.X, 0.55f, enemyState.Position.Y);
            root.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            Renderer renderer = root.GetComponent<Renderer>();
            renderer.material.color = new Color(0.76f, 0.28f, 0.24f);

            SandboxEnemyView view = root.AddComponent<SandboxEnemyView>();
            view.Initialize(enemyState.EnemyId.Value, renderer);
            _enemyViews[enemyState.EnemyId] = view;
        }

        private void CreateNodeView(TacticalNodeState nodeState)
        {
            CreateNodeView(new TacticalNodeSnapshot(
                nodeState.NodeId,
                nodeState.NodeType,
                nodeState.Position,
                nodeState.Radius,
                nodeState.IsEnabled,
                nodeState.IsSearched,
                nodeState.SearchProgress,
                nodeState.RequiredSearchSeconds,
                nodeState.OccupyingMemberId));
        }

        private void CreateNodeView(TacticalNodeSnapshot nodeState)
        {
            PrimitiveType primitiveType = PrimitiveType.Cube;
            Vector3 scale = new Vector3(0.8f, 0.4f, 0.8f);
            float y = 0.2f;

            if (nodeState.NodeType == TacticalNodeType.ExtractionPoint)
            {
                primitiveType = PrimitiveType.Cylinder;
                scale = new Vector3(nodeState.Radius * 0.8f, 0.05f, nodeState.Radius * 0.8f);
                y = 0.05f;
            }
            else if (nodeState.NodeType == TacticalNodeType.SearchPoint)
            {
                scale = new Vector3(1f, 0.7f, 1f);
                y = 0.35f;
            }
            else if (nodeState.NodeType == TacticalNodeType.EnemyIngress)
            {
                primitiveType = PrimitiveType.Sphere;
                scale = new Vector3(0.7f, 0.7f, 0.7f);
                y = 0.35f;
            }

            GameObject root = GameObject.CreatePrimitive(primitiveType);
            root.name = "SandboxNode_" + nodeState.NodeId + "_" + nodeState.NodeType;
            root.transform.SetParent(transform, false);
            root.transform.position = new Vector3(nodeState.Position.X, y, nodeState.Position.Y);
            root.transform.localScale = scale;
            Renderer renderer = root.GetComponent<Renderer>();

            SandboxTacticalNodeView view = root.AddComponent<SandboxTacticalNodeView>();
            view.Initialize(nodeState.NodeId, nodeState.NodeType, renderer);
            _nodeViews[nodeState.NodeId] = view;
        }

        private void CreateObstacleView(TacticalObstacleState obstacleState)
        {
            CreateObstacleView(new TacticalObstacleSnapshot(
                obstacleState.ObstacleId,
                obstacleState.ObstacleType,
                obstacleState.Position,
                obstacleState.Radius,
                obstacleState.BlocksLineOfSight,
                obstacleState.BlocksFire,
                obstacleState.ProvidesCover,
                obstacleState.DamageReductionFactor,
                obstacleState.IsDestroyed));
        }

        private void CreateObstacleView(TacticalObstacleSnapshot obstacleState)
        {
            PrimitiveType primitiveType = obstacleState.ObstacleType == TacticalObstacleType.Window ? PrimitiveType.Cylinder : PrimitiveType.Cube;
            GameObject root = GameObject.CreatePrimitive(primitiveType);
            root.name = "SandboxObstacle_" + obstacleState.ObstacleId;
            root.transform.SetParent(transform, false);
            root.transform.position = new Vector3(obstacleState.Position.X, obstacleState.ObstacleType == TacticalObstacleType.Window ? 0.8f : 0.5f, obstacleState.Position.Y);
            float height = obstacleState.ObstacleType == TacticalObstacleType.LowCover ? 0.7f : 1.4f;
            if (obstacleState.ObstacleType == TacticalObstacleType.Window)
            {
                height = 0.1f;
            }

            root.transform.localScale = new Vector3(obstacleState.Radius * 1.6f, height, obstacleState.Radius * 1.6f);
            Renderer renderer = root.GetComponent<Renderer>();
            SandboxObstacleView view = root.AddComponent<SandboxObstacleView>();
            view.Initialize(obstacleState.ObstacleId, obstacleState.ObstacleType, renderer);
            _obstacleViews[obstacleState.ObstacleId] = view;
        }

        private void CreateBuildingView(BuildingState buildingState)
        {
            CreateBuildingView(new BuildingSnapshot(
                buildingState.BuildingId,
                buildingState.Position,
                buildingState.Radius,
                buildingState.IsEnterable,
                buildingState.TacticalNodeIds));
        }

        private void CreateBuildingView(BuildingSnapshot buildingState)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Cube);
            root.name = "SandboxBuilding_" + buildingState.BuildingId;
            root.transform.SetParent(transform, false);
            root.transform.position = new Vector3(buildingState.Position.X, 0.75f, buildingState.Position.Y);
            root.transform.localScale = new Vector3(buildingState.Radius * 1.6f, 1.5f, buildingState.Radius * 1.2f);
            Renderer renderer = root.GetComponent<Renderer>();
            SandboxBuildingView view = root.AddComponent<SandboxBuildingView>();
            view.Initialize(buildingState.BuildingId, renderer);
            _buildingViews[buildingState.BuildingId] = view;
        }

        private void CreateZoneView(EnvironmentalZoneSnapshot zoneState)
        {
            GameObject root = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            root.name = "SandboxEnvironmentZone_" + zoneState.ZoneId + "_" + zoneState.ZoneType;
            root.transform.SetParent(transform, false);
            root.transform.position = new Vector3(zoneState.Position.X, 0.05f, zoneState.Position.Y);
            root.transform.localScale = new Vector3(zoneState.Radius * 2f, 0.1f, zoneState.Radius * 2f);
            Renderer renderer = root.GetComponent<Renderer>();

            SandboxEnvironmentalZoneView view = root.AddComponent<SandboxEnvironmentalZoneView>();
            view.Initialize(zoneState.ZoneId, zoneState.ZoneType, renderer);
            view.ApplySnapshot(zoneState);
            _zoneViews[zoneState.ZoneId] = view;
        }

        private static void DestroyAll<T>(ICollection<T> views) where T : Component
        {
            foreach (T view in views)
            {
                if (view != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(view.gameObject);
                    }
                    else
                    {
                        DestroyImmediate(view.gameObject);
                    }
                }
            }
        }
    }
}
