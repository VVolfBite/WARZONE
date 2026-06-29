using System.Collections.Generic;
using UnityEngine;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Presentation.Units;

namespace Warzone.Adapters
{
    public sealed class SandboxPresentationSync
    {
        private readonly Dictionary<int, SandboxSquadView> _squadViews = new Dictionary<int, SandboxSquadView>();
        private readonly Dictionary<BattleEntityId, SandboxSquadView> _entityViews = new Dictionary<BattleEntityId, SandboxSquadView>();
        private readonly Camera _mainCamera;
        private readonly SandboxSelectionInfoOverlay _selectionInfoOverlay;

        public SandboxPresentationSync(Camera mainCamera, SandboxSelectionInfoOverlay selectionInfoOverlay)
        {
            _mainCamera = mainCamera;
            _selectionInfoOverlay = selectionInfoOverlay;
        }

        public IEnumerable<KeyValuePair<int, SandboxSquadView>> SquadViews => _squadViews;

        public void RebuildViews(BattleSession battleSession, ContentCatalog contentCatalog)
        {
            foreach (KeyValuePair<int, SandboxSquadView> pair in _squadViews)
            {
                if (pair.Value != null)
                {
                    Object.Destroy(pair.Value.gameObject);
                }
            }

            _squadViews.Clear();
            _entityViews.Clear();
            AddViewsForSquads(battleSession.Squads, contentCatalog);
        }

        public void AddViewsForSquads(IReadOnlyList<BattleSquadState> squads, ContentCatalog contentCatalog)
        {
            for (int i = 0; i < squads.Count; i++)
            {
                BattleSquadState squad = squads[i];
                if (_squadViews.ContainsKey(squad.SquadId))
                {
                    continue;
                }

                GameObject root = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                root.name = "Squad_" + squad.SquadId;
                root.transform.position = new Vector3(squad.Position.X, 1f, squad.Position.Y);

                UnitDefinition definition = contentCatalog.Units[squad.Units[0].DefinitionId];
                float diameter = definition.CollisionRadius * 2f;
                root.transform.localScale = new Vector3(diameter, 1.6f, diameter);

                Renderer renderer = root.GetComponent<Renderer>();
                renderer.material.color = squad.FactionId == FactionId.Player ? Color.cyan : Color.red;
                ApplyPrototypeModel(root, squad.FactionId);

                UnitView unitView = root.AddComponent<UnitView>();
                unitView.Initialize(new[] { renderer });

                UnitWorldUiView worldUiView = root.AddComponent<UnitWorldUiView>();
                worldUiView.Initialize(_mainCamera);

                SandboxSquadView squadView = root.AddComponent<SandboxSquadView>();
                squadView.Initialize(squad.SquadId, squad.FactionId, unitView, worldUiView);
                _squadViews[squad.SquadId] = squadView;

                if (squad.Units.Count > 0)
                {
                    _entityViews[squad.Units[0].EntityId] = squadView;
                }
            }
        }

        public void Sync(BattleSession battleSession, ContentCatalog contentCatalog, SandboxSelectionService selectionService, ObstacleVolume[] obstacleVolumes)
        {
            for (int i = 0; i < battleSession.Squads.Count; i++)
            {
                BattleSquadState squad = battleSession.Squads[i];
                if (!_squadViews.TryGetValue(squad.SquadId, out SandboxSquadView view) || view == null)
                {
                    continue;
                }

                view.transform.position = new Vector3(squad.Position.X, 1f, squad.Position.Y);
                view.SetSelected(selectionService.Contains(squad.SquadId));
                view.SetCommandState(squad.CommandState);

                UnitDefinition definition = contentCatalog.Units[squad.Units[0].DefinitionId];
                view.SetHealth((float)squad.Units[0].CurrentHealth / definition.MaxHealth);
                view.SetRangeVisible(selectionService.Contains(squad.SquadId), definition.Weapon.Range);
                ResolveObstacleOverlap(view, definition.CollisionRadius, obstacleVolumes);

                if (!squad.HasLivingUnits)
                {
                    view.SetDead();
                }
            }

            SyncSelectionPanels(battleSession, contentCatalog, selectionService);
        }

        public void RenderDamageEvents(BattleSession battleSession)
        {
            IReadOnlyList<DamageEvent> damageEvents = battleSession.ConsumeDamageEvents();
            for (int i = 0; i < damageEvents.Count; i++)
            {
                DamageEvent damageEvent = damageEvents[i];
                if (!_entityViews.TryGetValue(damageEvent.SourceEntityId, out SandboxSquadView sourceView) ||
                    !_entityViews.TryGetValue(damageEvent.TargetEntityId, out SandboxSquadView targetView) ||
                    sourceView == null ||
                    targetView == null)
                {
                    continue;
                }

                SpawnProjectile(
                    sourceView.transform.position + (Vector3.up * 0.75f),
                    targetView.transform.position + (Vector3.up * 0.75f),
                    damageEvent.ProjectileSpeed,
                    sourceView.FactionId == FactionId.Player ? Color.cyan : new Color(1f, 0.45f, 0.2f));

                if (damageEvent.DidKillTarget)
                {
                    targetView.SetDead();
                }
                else
                {
                    targetView.FlashHit();
                }
            }
        }

        public void SpawnCommandMarker(Vector3 worldPoint, Color color)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = "CommandMarker";
            marker.transform.position = new Vector3(worldPoint.x, 0.1f, worldPoint.z);
            marker.transform.localScale = new Vector3(0.35f, 0.02f, 0.35f);
            Renderer renderer = marker.GetComponent<Renderer>();
            renderer.material.color = color;
            Object.Destroy(marker.GetComponent<Collider>());
            marker.AddComponent<CommandMarkerView>();
        }

        private void SyncSelectionPanels(BattleSession battleSession, ContentCatalog contentCatalog, SandboxSelectionService selectionService)
        {
            BattleSquadState selectedSquad = null;
            foreach (int squadId in selectionService.SelectedSquadIds)
            {
                selectedSquad = FindSquad(battleSession, squadId);
                if (selectedSquad != null)
                {
                    break;
                }
            }

            if (selectedSquad != null)
            {
                UnitDefinition definition = contentCatalog.Units[selectedSquad.Units[0].DefinitionId];
                _selectionInfoOverlay.BindSelection(
                    "Selected: Squad " + selectedSquad.SquadId,
                    definition,
                    selectedSquad.Units[0].CurrentHealth,
                    definition.MaxHealth);
            }
            else
            {
                _selectionInfoOverlay.BindSelection(null, null, 0, 0);
            }

            if (selectionService.HoveredEnemySquadId.HasValue)
            {
                BattleSquadState hoverSquad = FindSquad(battleSession, selectionService.HoveredEnemySquadId.Value);
                if (hoverSquad != null)
                {
                    UnitDefinition definition = contentCatalog.Units[hoverSquad.Units[0].DefinitionId];
                    _selectionInfoOverlay.BindHover(
                        "Enemy: Squad " + hoverSquad.SquadId,
                        definition,
                        hoverSquad.Units[0].CurrentHealth,
                        definition.MaxHealth);
                    return;
                }
            }

            _selectionInfoOverlay.BindHover(null, null, 0, 0);
        }

        private static BattleSquadState FindSquad(BattleSession battleSession, int squadId)
        {
            for (int i = 0; i < battleSession.Squads.Count; i++)
            {
                if (battleSession.Squads[i].SquadId == squadId)
                {
                    return battleSession.Squads[i];
                }
            }

            return null;
        }

        private static void SpawnProjectile(Vector3 start, Vector3 target, float speed, Color color)
        {
            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = "Projectile";
            projectile.transform.localScale = Vector3.one * 0.25f;
            Object.Destroy(projectile.GetComponent<Collider>());
            ProjectileView projectileView = projectile.AddComponent<ProjectileView>();
            projectileView.Launch(start, target, speed, color);
        }

        private static void ApplyPrototypeModel(GameObject root, FactionId factionId)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.15f, 0f);
            body.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
            Renderer bodyRenderer = body.GetComponent<Renderer>();
            bodyRenderer.material.color = factionId == FactionId.Player ? new Color(0.25f, 0.65f, 0.95f) : new Color(0.55f, 0.65f, 0.2f);
            Object.Destroy(body.GetComponent<Collider>());

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(root.transform, false);
            head.transform.localPosition = new Vector3(0f, 1.05f, 0f);
            head.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            Renderer headRenderer = head.GetComponent<Renderer>();
            headRenderer.material.color = factionId == FactionId.Player ? new Color(0.92f, 0.92f, 0.96f) : new Color(0.78f, 0.88f, 0.48f);
            Object.Destroy(head.GetComponent<Collider>());
        }

        private static void ResolveObstacleOverlap(SandboxSquadView view, float collisionRadius, ObstacleVolume[] obstacleVolumes)
        {
            if (obstacleVolumes == null || obstacleVolumes.Length == 0 || view == null || view.IsDead)
            {
                return;
            }

            Vector3 position = view.transform.position;
            for (int i = 0; i < obstacleVolumes.Length; i++)
            {
                ObstacleVolume obstacle = obstacleVolumes[i];
                if (obstacle == null || obstacle.ObstacleCollider == null)
                {
                    continue;
                }

                Vector3 closest = obstacle.ObstacleCollider.ClosestPoint(position);
                System.Numerics.Vector2 current2D = new System.Numerics.Vector2(position.x, position.z);
                System.Numerics.Vector2 closest2D = new System.Numerics.Vector2(closest.x, closest.z);
                System.Numerics.Vector2 offset = current2D - closest2D;
                float distance = offset.Length();

                if (distance >= collisionRadius)
                {
                    continue;
                }

                System.Numerics.Vector2 pushDirection;
                if (distance <= 0.0001f)
                {
                    Vector3 obstacleCenter = obstacle.ObstacleCollider.bounds.center;
                    pushDirection = System.Numerics.Vector2.Normalize(current2D - new System.Numerics.Vector2(obstacleCenter.x, obstacleCenter.z));
                    if (pushDirection.LengthSquared() <= 0.0001f)
                    {
                        pushDirection = new System.Numerics.Vector2(1f, 0f);
                    }
                }
                else
                {
                    pushDirection = System.Numerics.Vector2.Normalize(offset);
                }

                float penetration = collisionRadius - distance;
                System.Numerics.Vector2 corrected = current2D + (pushDirection * penetration);
                position = new Vector3(corrected.X, position.y, corrected.Y);
                view.transform.position = position;
            }
        }
    }
}
