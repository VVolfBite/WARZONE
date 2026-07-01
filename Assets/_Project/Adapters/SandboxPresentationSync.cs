using System.Collections.Generic;
using UnityEngine;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Foundation;
using Warzone.Presentation.Units;

namespace Warzone.Adapters
{
    public sealed class SandboxPresentationSync
    {
        private readonly Dictionary<int, SandboxSquadView> _squadViews = new Dictionary<int, SandboxSquadView>();
        private readonly Dictionary<BattleEntityId, SandboxSquadView> _entityViews = new Dictionary<BattleEntityId, SandboxSquadView>();
        private readonly Camera _mainCamera;
        private readonly SandboxSelectionInfoOverlay _selectionInfoOverlay;
        private readonly AudioService _audioService;
        private readonly SimpleObjectPool<DamageNumberView> _damageNumberPool;
        private readonly SimpleObjectPool<MuzzleFlashView> _muzzleFlashPool;
        private readonly SimpleObjectPool<CommandMarkerView> _commandMarkerPool;

        public SandboxPresentationSync(Camera mainCamera, SandboxSelectionInfoOverlay selectionInfoOverlay, AudioService audioService = null)
        {
            _mainCamera = mainCamera;
            _selectionInfoOverlay = selectionInfoOverlay;
            _audioService = audioService;
            _damageNumberPool = new SimpleObjectPool<DamageNumberView>(CreateDamageNumberView, preloadCount: 8);
            _muzzleFlashPool = new SimpleObjectPool<MuzzleFlashView>(CreateMuzzleFlashView, preloadCount: 6);
            _commandMarkerPool = new SimpleObjectPool<CommandMarkerView>(CreateCommandMarkerView, preloadCount: 4);
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

                GameObject root = new GameObject("Squad_" + squad.SquadId);
                root.name = "Squad_" + squad.SquadId;
                root.transform.position = new Vector3(squad.Position.X, 1f, squad.Position.Y);

                UnitDefinition definition = contentCatalog.Units[squad.Units[0].DefinitionId];
                float diameter = definition.CollisionRadius * 2f;
                root.transform.localScale = new Vector3(diameter, 1f, diameter);

                Renderer[] renderers = ApplyPrototypeModel(root, definition);

                UnitView unitView = root.AddComponent<UnitView>();
                unitView.Initialize(renderers);

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
                view.SetStatusTint(GetStatusTint(squad.Units[0]));
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
                SpawnMuzzleFlash(sourceView.transform.position + (Vector3.up * 1.1f), sourceView.FactionId == FactionId.Player ? Color.cyan : new Color(1f, 0.55f, 0.25f));
                SpawnDamageNumber(targetView.transform.position + (Vector3.up * 1.9f), damageEvent.DamageAmount, damageEvent.DidKillTarget ? new Color(1f, 0.35f, 0.35f) : Color.white);

                if (damageEvent.DidKillTarget)
                {
                    _audioService?.PlayUnitDeath();
                    targetView.SetDead();
                }
                else
                {
                    _audioService?.PlayUnitHit();
                    targetView.FlashHit();
                }
            }
        }

        public void SpawnCommandMarker(Vector3 worldPoint, Color color)
        {
            CommandMarkerView marker = _commandMarkerPool.Get();
            marker.transform.position = new Vector3(worldPoint.x, 0.1f, worldPoint.z);
            marker.transform.localScale = new Vector3(0.35f, 0.02f, 0.35f);
            Renderer renderer = marker.GetComponent<Renderer>();
            renderer.material.color = color;
            marker.Initialize(ReleaseCommandMarkerView);
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
                    definition.MaxHealth,
                    selectedSquad.Units[0]);
            }
            else
            {
                _selectionInfoOverlay.BindSelection(null, null, 0, 0, null);
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
                        definition.MaxHealth,
                        hoverSquad.Units[0]);
                    return;
                }
            }

            _selectionInfoOverlay.BindHover(null, null, 0, 0, null);
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

        private void SpawnDamageNumber(Vector3 position, int amount, Color color)
        {
            DamageNumberView damageNumberView = _damageNumberPool.Get();
            damageNumberView.transform.position = position;
            damageNumberView.Initialize(_mainCamera, amount.ToString(), color, ReleaseDamageNumberView);
        }

        private void SpawnMuzzleFlash(Vector3 position, Color color)
        {
            MuzzleFlashView flash = _muzzleFlashPool.Get();
            flash.transform.position = position;
            flash.transform.localScale = new Vector3(0.24f, 0.24f, 0.24f);
            Renderer renderer = flash.GetComponent<Renderer>();
            renderer.material.color = color;
            flash.Initialize(ReleaseMuzzleFlashView);
        }

        private DamageNumberView CreateDamageNumberView()
        {
            GameObject root = new GameObject("DamageNumber");
            DamageNumberView damageNumberView = root.AddComponent<DamageNumberView>();
            damageNumberView.Deactivate();
            return damageNumberView;
        }

        private MuzzleFlashView CreateMuzzleFlashView()
        {
            GameObject flash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            flash.name = "MuzzleFlash";
            Object.Destroy(flash.GetComponent<Collider>());
            MuzzleFlashView view = flash.AddComponent<MuzzleFlashView>();
            view.Deactivate();
            return view;
        }

        private CommandMarkerView CreateCommandMarkerView()
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = "CommandMarker";
            Object.Destroy(marker.GetComponent<Collider>());
            CommandMarkerView view = marker.AddComponent<CommandMarkerView>();
            view.Deactivate();
            return view;
        }

        private void ReleaseDamageNumberView(DamageNumberView view)
        {
            view.Deactivate();
            _damageNumberPool.Release(view);
        }

        private void ReleaseMuzzleFlashView(MuzzleFlashView view)
        {
            view.Deactivate();
            _muzzleFlashPool.Release(view);
        }

        private void ReleaseCommandMarkerView(CommandMarkerView view)
        {
            view.Deactivate();
            _commandMarkerPool.Release(view);
        }

        private static Color? GetStatusTint(BattleUnitState unit)
        {
            if (unit == null || unit.StatusEffects.Count == 0)
            {
                return null;
            }

            for (int i = 0; i < unit.StatusEffects.Count; i++)
            {
                string effectId = unit.StatusEffects[i].Definition.Id;
                if (effectId == "effect.zombie.toxic")
                {
                    return new Color(0.55f, 0.9f, 0.25f);
                }

                if (effectId == "effect.support.heal")
                {
                    return new Color(0.25f, 0.95f, 0.75f);
                }
            }

            return new Color(0.95f, 0.85f, 0.3f);
        }

        private static Renderer[] ApplyPrototypeModel(GameObject root, UnitDefinition definition)
        {
            FactionId factionId = definition.FactionId;
            List<Renderer> renderers = new List<Renderer>();

            GameObject basePlate = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            basePlate.name = "BasePlate";
            basePlate.transform.SetParent(root.transform, false);
            basePlate.transform.localPosition = new Vector3(0f, -0.45f, 0f);
            basePlate.transform.localScale = new Vector3(0.55f, 0.08f, 0.55f);
            Renderer baseRenderer = basePlate.GetComponent<Renderer>();
            baseRenderer.material.color = factionId == FactionId.Player ? new Color(0.18f, 0.24f, 0.3f) : new Color(0.24f, 0.18f, 0.18f);
            renderers.Add(baseRenderer);
            Object.Destroy(basePlate.GetComponent<Collider>());

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new Vector3(0f, 0.05f, 0f);
            body.transform.localScale = new Vector3(0.42f, 0.62f, 0.42f);
            Renderer bodyRenderer = body.GetComponent<Renderer>();
            bodyRenderer.material.color = GetUniformColor(definition);
            renderers.Add(bodyRenderer);
            Object.Destroy(body.GetComponent<Collider>());

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(root.transform, false);
            head.transform.localPosition = new Vector3(0f, 0.98f, 0f);
            head.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            Renderer headRenderer = head.GetComponent<Renderer>();
            headRenderer.material.color = factionId == FactionId.Player ? new Color(0.94f, 0.9f, 0.82f) : new Color(0.84f, 0.78f, 0.68f);
            renderers.Add(headRenderer);
            Object.Destroy(head.GetComponent<Collider>());

            GameObject shoulders = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shoulders.name = "Shoulders";
            shoulders.transform.SetParent(root.transform, false);
            shoulders.transform.localPosition = new Vector3(0f, 0.48f, 0f);
            shoulders.transform.localScale = new Vector3(0.78f, 0.18f, 0.24f);
            Renderer shouldersRenderer = shoulders.GetComponent<Renderer>();
            shouldersRenderer.material.color = bodyRenderer.material.color * 0.92f;
            renderers.Add(shouldersRenderer);
            Object.Destroy(shoulders.GetComponent<Collider>());

            GameObject weapon = GameObject.CreatePrimitive(PrimitiveType.Cube);
            weapon.name = "Weapon";
            weapon.transform.SetParent(root.transform, false);
            weapon.transform.localPosition = new Vector3(0.34f, 0.42f, 0.12f);
            weapon.transform.localScale = GetWeaponScale(definition);
            weapon.transform.localRotation = Quaternion.Euler(0f, -18f, 20f);
            Renderer weaponRenderer = weapon.GetComponent<Renderer>();
            weaponRenderer.material.color = new Color(0.16f, 0.16f, 0.18f);
            renderers.Add(weaponRenderer);
            Object.Destroy(weapon.GetComponent<Collider>());

            if (definition.Id == "unit.technical")
            {
                GameObject chassis = GameObject.CreatePrimitive(PrimitiveType.Cube);
                chassis.name = "Chassis";
                chassis.transform.SetParent(root.transform, false);
                chassis.transform.localPosition = new Vector3(0f, -0.02f, 0f);
                chassis.transform.localScale = new Vector3(1.2f, 0.45f, 1.9f);
                Renderer chassisRenderer = chassis.GetComponent<Renderer>();
                chassisRenderer.material.color = new Color(0.34f, 0.28f, 0.22f);
                renderers.Add(chassisRenderer);
                Object.Destroy(chassis.GetComponent<Collider>());
            }

            return renderers.ToArray();
        }

        private static Color GetUniformColor(UnitDefinition definition)
        {
            if (definition.FactionId == FactionId.Player)
            {
                if (definition.Id == "unit.sniper")
                {
                    return new Color(0.34f, 0.56f, 0.72f);
                }

                if (definition.Id == "unit.grenadier")
                {
                    return new Color(0.58f, 0.56f, 0.24f);
                }

                if (definition.Id == "unit.medic")
                {
                    return new Color(0.28f, 0.62f, 0.42f);
                }

                return new Color(0.24f, 0.46f, 0.68f);
            }

            if (definition.Id == "unit.warlord")
            {
                return new Color(0.42f, 0.12f, 0.12f);
            }

            if (definition.Id == "unit.technical")
            {
                return new Color(0.36f, 0.3f, 0.2f);
            }

            if (definition.Id == "unit.rpg")
            {
                return new Color(0.46f, 0.28f, 0.2f);
            }

            return new Color(0.36f, 0.22f, 0.22f);
        }

        private static Vector3 GetWeaponScale(UnitDefinition definition)
        {
            if (definition.Id == "unit.sniper")
            {
                return new Vector3(0.16f, 0.14f, 0.82f);
            }

            if (definition.Id == "unit.grenadier" || definition.Id == "unit.rpg")
            {
                return new Vector3(0.22f, 0.2f, 0.7f);
            }

            if (definition.Id == "unit.warlord")
            {
                return new Vector3(0.28f, 0.24f, 0.9f);
            }

            return new Vector3(0.14f, 0.14f, 0.6f);
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
