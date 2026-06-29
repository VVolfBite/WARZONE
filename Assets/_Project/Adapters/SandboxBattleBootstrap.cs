using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Warzone.Application;
using Warzone.Combat;
using Warzone.Content;
using Warzone.Content.Definitions;
using Warzone.Presentation.Units;

namespace Warzone.Adapters
{
    using NumericVector2 = System.Numerics.Vector2;
    using UnityVector2 = UnityEngine.Vector2;
    using UnityVector3 = UnityEngine.Vector3;

    public sealed class SandboxBattleBootstrap : MonoBehaviour
    {
        private readonly Dictionary<int, SandboxSquadView> _squadViews = new Dictionary<int, SandboxSquadView>();
        private readonly Dictionary<BattleEntityId, SandboxSquadView> _entityViews = new Dictionary<BattleEntityId, SandboxSquadView>();
        private readonly HashSet<int> _selectedSquadIds = new HashSet<int>();
        private readonly Queue<string> _notifications = new Queue<string>();
        private readonly List<WaveSpawnPlan> _wavePlans = new List<WaveSpawnPlan>();

        private ContentCatalog _contentCatalog;
        private BattleRuntimeHost _battleRuntimeHost;
        private Camera _mainCamera;
        private SelectionBoxOverlay _selectionBoxOverlay;
        private SandboxHudOverlay _sandboxHudOverlay;
        private SandboxSelectionInfoOverlay _selectionInfoOverlay;
        private ObstacleVolume[] _obstacleVolumes = new ObstacleVolume[0];
        private BattleSession _battleSession;
        private UnityVector2 _selectionStart;
        private bool _isDraggingSelection;
        private bool _isPaused;
        private bool _hasPublishedBattleResult;
        private int _activeWaveIndex;
        private int _totalWaveCount = 3;
        private float _nextWaveCountdown;
        private bool _waitingForNextWave;
        private int? _hoveredEnemySquadId;

        public void Configure(BattleRuntimeHost battleRuntimeHost, Camera mainCamera)
        {
            _battleRuntimeHost = battleRuntimeHost;
            _mainCamera = mainCamera;
            _battleRuntimeHost.BattleStarted += HandleBattleStarted;
            _selectionBoxOverlay = gameObject.AddComponent<SelectionBoxOverlay>();
            _sandboxHudOverlay = gameObject.AddComponent<SandboxHudOverlay>();
            _selectionInfoOverlay = gameObject.AddComponent<SandboxSelectionInfoOverlay>();
        }

        private void OnDestroy()
        {
            if (_battleRuntimeHost != null)
            {
                _battleRuntimeHost.BattleStarted -= HandleBattleStarted;
            }
        }

        private void Update()
        {
            HandlePauseInput();
            HandleInput();

            if (_battleSession == null)
            {
                _sandboxHudOverlay.Bind(_isPaused, 0, _totalWaveCount, "Waiting for mission...", string.Empty);
                return;
            }

            if (!_isPaused && _battleSession.CurrentOutcome == MissionOutcome.InProgress)
            {
                _battleSession.Tick(Time.deltaTime);
                UpdateWaveFlow(Time.deltaTime);
            }

            RenderDamageEvents();
            SyncViews();
            SyncSelectionPanels();
            UpdateNotifications();
            _sandboxHudOverlay.Bind(_isPaused, _activeWaveIndex, _totalWaveCount, GetObjectiveText(), BuildNotificationText());

            if (!_hasPublishedBattleResult && _battleSession.TryBuildResultOnce(out BattleResult result))
            {
                _hasPublishedBattleResult = true;
                _battleRuntimeHost.FinishBattle(result);
            }
        }

        private void HandleBattleStarted(MissionStartRequest request)
        {
            _contentCatalog = BuildSandboxContent();
            _battleSession = CreateSandboxBattle(_contentCatalog, request.Seed);
            _wavePlans.Clear();
            _wavePlans.AddRange(BuildWaves());
            _battleSession.ConfigurePendingWaveCount(_wavePlans.Count);
            _hasPublishedBattleResult = false;
            _isPaused = false;
            _selectedSquadIds.Clear();
            _hoveredEnemySquadId = null;
            _activeWaveIndex = 0;
            _nextWaveCountdown = 0f;
            _waitingForNextWave = false;
            _notifications.Clear();
            _notifications.Enqueue("Demo started");
            _obstacleVolumes = FindObjectsByType<ObstacleVolume>(FindObjectsSortMode.None);
            CreateOrRefreshViews();
            SpawnNextWave(forceImmediate: true);
        }

        private static ContentCatalog BuildSandboxContent()
        {
            WeaponDefinition playerWeapon = new WeaponDefinition("weapon.player.rifle", 12f, 0.75f, 4, 26f);
            WeaponDefinition enemyWeapon = new WeaponDefinition("weapon.enemy.claws", 2.0f, 1.2f, 1, 14f);

            Dictionary<string, UnitDefinition> units = new Dictionary<string, UnitDefinition>
            {
                ["unit.player.infantry"] = new UnitDefinition("unit.player.infantry", "Infantry", FactionId.Player, 22, 5.75f, playerWeapon, 15f, 0.72f),
                ["unit.enemy.zombie"] = new UnitDefinition("unit.enemy.zombie", "Zombie", FactionId.Enemy, 10, 2.8f, enemyWeapon, 8f, 0.78f)
            };

            Dictionary<string, MissionDefinition> missions = new Dictionary<string, MissionDefinition>
            {
                ["mission.sandbox"] = new MissionDefinition("mission.sandbox", "Sandbox", 1, 3)
            };

            return new ContentCatalog(units, missions);
        }

        private static BattleSession CreateSandboxBattle(ContentCatalog catalog, int seed)
        {
            List<BattleSquadState> squads = new List<BattleSquadState>
            {
                CreateSingleUnitSquad(1, FactionId.Player, "unit.player.infantry", -14f, -3.5f, 22),
                CreateSingleUnitSquad(2, FactionId.Player, "unit.player.infantry", -12f, 0f, 22),
                CreateSingleUnitSquad(3, FactionId.Player, "unit.player.infantry", -14f, 3.5f, 22),
                CreateSingleUnitSquad(4, FactionId.Player, "unit.player.infantry", -16f, 0f, 22)
            };

            return new BattleSession(
                squads,
                new CommandProcessor(),
                new CombatResolver(catalog),
                seed);
        }

        private List<WaveSpawnPlan> BuildWaves()
        {
            return new List<WaveSpawnPlan>
            {
                new WaveSpawnPlan(1, 1.5f, new[]
                {
                    CreateSingleUnitSquad(101, FactionId.Enemy, "unit.enemy.zombie", 16f, -4f, 10),
                    CreateSingleUnitSquad(102, FactionId.Enemy, "unit.enemy.zombie", 18f, 1f, 10)
                }),
                new WaveSpawnPlan(2, 2.5f, new[]
                {
                    CreateSingleUnitSquad(103, FactionId.Enemy, "unit.enemy.zombie", 17f, -2f, 10),
                    CreateSingleUnitSquad(104, FactionId.Enemy, "unit.enemy.zombie", 19f, 2f, 10)
                }),
                new WaveSpawnPlan(3, 3.0f, new[]
                {
                    CreateSingleUnitSquad(105, FactionId.Enemy, "unit.enemy.zombie", 20f, 0f, 12),
                    CreateSingleUnitSquad(106, FactionId.Enemy, "unit.enemy.zombie", 21f, 3.5f, 12)
                })
            };
        }

        private void UpdateWaveFlow(float deltaTimeSeconds)
        {
            if (_battleSession == null || _activeWaveIndex >= _wavePlans.Count)
            {
                return;
            }

            bool enemyAlive = _battleSession.Squads.Any(squad => squad.FactionId == FactionId.Enemy && squad.HasLivingUnits);
            if (enemyAlive)
            {
                _waitingForNextWave = false;
                _nextWaveCountdown = 0f;
                return;
            }

            if (!_waitingForNextWave)
            {
                _waitingForNextWave = true;
                _nextWaveCountdown = _wavePlans[_activeWaveIndex].DelaySeconds;
                _notifications.Enqueue($"Wave {_wavePlans[_activeWaveIndex].WaveIndex} incoming");
            }

            _nextWaveCountdown -= deltaTimeSeconds;
            if (_nextWaveCountdown > 0f)
            {
                return;
            }

            SpawnNextWave();
        }

        private void SpawnNextWave(bool forceImmediate = false)
        {
            if (_activeWaveIndex >= _wavePlans.Count)
            {
                return;
            }

            WaveSpawnPlan plan = _wavePlans[_activeWaveIndex];
            _waitingForNextWave = false;
            _nextWaveCountdown = 0f;

            _battleSession.ConsumePendingWave();
            _battleSession.AddSquads(plan.Squads);
            CreateViewsForSquads(plan.Squads);
            _notifications.Enqueue($"Wave {plan.WaveIndex} deployed");
            _activeWaveIndex++;
        }

        private void CreateOrRefreshViews()
        {
            foreach (SandboxSquadView view in _squadViews.Values.ToList())
            {
                if (view != null)
                {
                    Destroy(view.gameObject);
                }
            }

            _squadViews.Clear();
            _entityViews.Clear();
            CreateViewsForSquads(_battleSession.Squads);
        }

        private void CreateViewsForSquads(IEnumerable<BattleSquadState> squads)
        {
            foreach (BattleSquadState squad in squads)
            {
                if (_squadViews.ContainsKey(squad.SquadId))
                {
                    continue;
                }

                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                go.name = $"Squad_{squad.SquadId}";
                go.transform.position = new UnityVector3(squad.Position.X, 1f, squad.Position.Y);
                UnitDefinition definition = _contentCatalog.Units[squad.Units[0].DefinitionId];
                float diameter = definition.CollisionRadius * 2f;
                go.transform.localScale = new UnityVector3(diameter, 1.6f, diameter);

                Renderer renderer = go.GetComponent<Renderer>();
                renderer.material.color = squad.FactionId == FactionId.Player ? Color.cyan : Color.red;
                ApplyPrototypeModel(go, squad.FactionId);

                UnitView unitView = go.AddComponent<UnitView>();
                unitView.Initialize(new[] { renderer });
                UnitWorldUiView worldUiView = go.AddComponent<UnitWorldUiView>();
                worldUiView.Initialize(_mainCamera);

                SandboxSquadView squadView = go.AddComponent<SandboxSquadView>();
                squadView.Initialize(squad.SquadId, squad.FactionId, unitView, worldUiView);
                _squadViews[squad.SquadId] = squadView;
                if (squad.Units.Count > 0)
                {
                    _entityViews[squad.Units[0].EntityId] = squadView;
                }
            }
        }

        private void SyncViews()
        {
            foreach (BattleSquadState squad in _battleSession.Squads)
            {
                if (!_squadViews.TryGetValue(squad.SquadId, out SandboxSquadView view) || view == null)
                {
                    continue;
                }

                view.transform.position = new UnityVector3(squad.Position.X, 1f, squad.Position.Y);
                view.SetSelected(_selectedSquadIds.Contains(squad.SquadId));
                view.SetCommandState(squad.CommandState);

                UnitDefinition definition = _contentCatalog.Units[squad.Units[0].DefinitionId];
                view.SetHealth((float)squad.Units[0].CurrentHealth / definition.MaxHealth);
                view.SetRangeVisible(_selectedSquadIds.Contains(squad.SquadId), definition.Weapon.Range);
                ResolveObstacleOverlap(view, definition.CollisionRadius);

                if (!squad.HasLivingUnits)
                {
                    view.SetDead();
                }
            }
        }

        private void RenderDamageEvents()
        {
            if (_battleSession == null)
            {
                return;
            }

            IReadOnlyList<DamageEvent> damageEvents = _battleSession.ConsumeDamageEvents();
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
                    sourceView.transform.position + (UnityVector3.up * 0.75f),
                    targetView.transform.position + (UnityVector3.up * 0.75f),
                    damageEvent.ProjectileSpeed,
                    sourceView.FactionId == FactionId.Player ? Color.cyan : new Color(1f, 0.45f, 0.2f));

                if (damageEvent.DidKillTarget)
                {
                    targetView.SetDead();
                }
            }
        }

        private void HandleInput()
        {
            if (_battleSession == null || _mainCamera == null)
            {
                return;
            }

            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                return;
            }

            if (mouse.leftButton.wasPressedThisFrame)
            {
                _selectionStart = mouse.position.ReadValue();
                _isDraggingSelection = true;
            }

            if (mouse.rightButton.wasPressedThisFrame)
            {
                TryIssueCommand();
            }

            if (_isDraggingSelection)
            {
                UnityVector2 current = mouse.position.ReadValue();
                Rect selectionRect = BuildScreenRect(_selectionStart, current);
                _selectionBoxOverlay.SetSelection(selectionRect, true);

                if (mouse.leftButton.wasReleasedThisFrame)
                {
                    _isDraggingSelection = false;
                    _selectionBoxOverlay.SetSelection(Rect.zero, false);

                    if (selectionRect.width < 8f && selectionRect.height < 8f)
                    {
                        TrySelectSingleSquad();
                    }
                    else
                    {
                        TrySelectSquadsInRect(selectionRect);
                    }
                }
            }

            UpdateHoverTarget();
        }

        private void HandlePauseInput()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.pKey.wasPressedThisFrame)
            {
                _isPaused = !_isPaused;
            }
        }

        private void TrySelectSingleSquad()
        {
            if (!TryRaycast(out RaycastHit hit))
            {
                if (!IsShiftHeld() && !IsCtrlHeld())
                {
                    _selectedSquadIds.Clear();
                }

                return;
            }

            SandboxSquadView squadView = hit.collider.GetComponent<SandboxSquadView>();
            if (squadView != null && squadView.FactionId == FactionId.Player)
            {
                bool shift = IsShiftHeld();
                bool ctrl = IsCtrlHeld();
                if (!shift && !ctrl)
                {
                    _selectedSquadIds.Clear();
                    _selectedSquadIds.Add(squadView.SquadId);
                }
                else if (ctrl)
                {
                    if (_selectedSquadIds.Contains(squadView.SquadId))
                    {
                        _selectedSquadIds.Remove(squadView.SquadId);
                    }
                    else
                    {
                        _selectedSquadIds.Add(squadView.SquadId);
                    }
                }
                else
                {
                    _selectedSquadIds.Add(squadView.SquadId);
                }
            }
            else if (squadView != null && squadView.FactionId == FactionId.Enemy)
            {
                _hoveredEnemySquadId = squadView.SquadId;
                if (!IsShiftHeld() && !IsCtrlHeld())
                {
                    _selectedSquadIds.Clear();
                }
            }
            else if (!IsShiftHeld() && !IsCtrlHeld())
            {
                _selectedSquadIds.Clear();
            }
        }

        private void TryIssueCommand()
        {
            if (_selectedSquadIds.Count == 0 || !TryRaycast(out RaycastHit hit))
            {
                return;
            }

            SandboxSquadView squadView = hit.collider.GetComponent<SandboxSquadView>();
            if (squadView != null && squadView.FactionId == FactionId.Enemy)
            {
                foreach (int squadId in _selectedSquadIds)
                {
                    _battleSession.ExecuteCommand(new Command(CommandType.Attack, squadId, squadView.SquadId, queue: IsShiftHeld()));
                }

                SpawnCommandMarker(hit.point, Color.red);
                return;
            }

            UnityEngine.Vector3 point = hit.point;
            List<int> orderedSquadIds = _selectedSquadIds.OrderBy(id => id).ToList();
            List<NumericVector2> destinations = BuildFormationDestinations(new NumericVector2(point.x, point.z), orderedSquadIds.Count);
            for (int i = 0; i < orderedSquadIds.Count; i++)
            {
                _battleSession.ExecuteCommand(new Command(
                    CommandType.Move,
                    orderedSquadIds[i],
                    destination: destinations[i],
                    queue: IsShiftHeld()));
            }

            SpawnCommandMarker(point, Color.cyan);
        }

        private bool TryRaycast(out RaycastHit hit)
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                hit = default;
                return false;
            }

            Ray ray = _mainCamera.ScreenPointToRay(mouse.position.ReadValue());
            return Physics.Raycast(ray, out hit, 1000f);
        }

        private void TrySelectSquadsInRect(Rect selectionRect)
        {
            bool shift = IsShiftHeld();
            bool ctrl = IsCtrlHeld();
            HashSet<int> boxedSelection = new HashSet<int>();
            foreach (KeyValuePair<int, SandboxSquadView> pair in _squadViews)
            {
                SandboxSquadView squadView = pair.Value;
                if (squadView == null || squadView.FactionId != FactionId.Player)
                {
                    continue;
                }

                UnityVector3 screenPosition = _mainCamera.WorldToScreenPoint(squadView.transform.position);
                UnityVector2 guiPoint = new UnityVector2(screenPosition.x, Screen.height - screenPosition.y);
                if (selectionRect.Contains(guiPoint))
                {
                    boxedSelection.Add(pair.Key);
                }
            }

            if (!shift && !ctrl)
            {
                _selectedSquadIds.Clear();
                foreach (int squadId in boxedSelection)
                {
                    _selectedSquadIds.Add(squadId);
                }
                return;
            }

            if (shift)
            {
                foreach (int squadId in boxedSelection)
                {
                    _selectedSquadIds.Add(squadId);
                }
            }

            if (ctrl)
            {
                foreach (int squadId in boxedSelection)
                {
                    if (_selectedSquadIds.Contains(squadId))
                    {
                        _selectedSquadIds.Remove(squadId);
                    }
                    else
                    {
                        _selectedSquadIds.Add(squadId);
                    }
                }
            }
        }

        private void UpdateHoverTarget()
        {
            _hoveredEnemySquadId = null;
            if (!TryRaycast(out RaycastHit hit))
            {
                return;
            }

            SandboxSquadView squadView = hit.collider.GetComponent<SandboxSquadView>();
            if (squadView != null && squadView.FactionId == FactionId.Enemy)
            {
                _hoveredEnemySquadId = squadView.SquadId;
            }
        }

        private void SyncSelectionPanels()
        {
            BattleSquadState selectedSquad = null;
            foreach (int squadId in _selectedSquadIds)
            {
                selectedSquad = FindSquad(squadId);
                if (selectedSquad != null)
                {
                    break;
                }
            }

            if (selectedSquad != null)
            {
                UnitDefinition definition = _contentCatalog.Units[selectedSquad.Units[0].DefinitionId];
                _selectionInfoOverlay.BindSelection(
                    $"Selected: Squad {selectedSquad.SquadId}",
                    definition,
                    selectedSquad.Units[0].CurrentHealth,
                    definition.MaxHealth);
            }
            else
            {
                _selectionInfoOverlay.BindSelection(null, null, 0, 0);
            }

            if (_hoveredEnemySquadId.HasValue)
            {
                BattleSquadState hoverSquad = FindSquad(_hoveredEnemySquadId.Value);
                if (hoverSquad != null)
                {
                    UnitDefinition definition = _contentCatalog.Units[hoverSquad.Units[0].DefinitionId];
                    _selectionInfoOverlay.BindHover(
                        $"Enemy: Squad {hoverSquad.SquadId}",
                        definition,
                        hoverSquad.Units[0].CurrentHealth,
                        definition.MaxHealth);
                    return;
                }
            }

            _selectionInfoOverlay.BindHover(null, null, 0, 0);
        }

        private string GetObjectiveText()
        {
            if (_battleSession == null)
            {
                return "Initialize";
            }

            if (_battleSession.CurrentOutcome == MissionOutcome.Victory)
            {
                return "Objective complete";
            }

            if (_battleSession.CurrentOutcome == MissionOutcome.Defeat)
            {
                return "Mission failed";
            }

            return $"Hold the line. Waves cleared: {_activeWaveIndex}/{_totalWaveCount}";
        }

        private void UpdateNotifications()
        {
            while (_notifications.Count > 4)
            {
                _notifications.Dequeue();
            }
        }

        private string BuildNotificationText()
        {
            if (_notifications.Count == 0)
            {
                return "No alerts";
            }

            return string.Join("\n", _notifications.ToArray());
        }

        private BattleSquadState FindSquad(int squadId)
        {
            for (int i = 0; i < _battleSession.Squads.Count; i++)
            {
                if (_battleSession.Squads[i].SquadId == squadId)
                {
                    return _battleSession.Squads[i];
                }
            }

            return null;
        }

        private static BattleSquadState CreateSingleUnitSquad(int squadId, FactionId factionId, string definitionId, float x, float y, int health)
        {
            return new BattleSquadState(
                squadId,
                factionId,
                new NumericVector2(x, y),
                new List<BattleUnitState>
                {
                    new BattleUnitState(new BattleEntityId(squadId), definitionId, factionId, health)
                });
        }

        private static void SpawnProjectile(UnityVector3 start, UnityVector3 target, float speed, Color color)
        {
            GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            projectile.name = "Projectile";
            projectile.transform.localScale = UnityVector3.one * 0.25f;
            Object.Destroy(projectile.GetComponent<Collider>());
            ProjectileView projectileView = projectile.AddComponent<ProjectileView>();
            projectileView.Launch(start, target, speed, color);
        }

        private static void SpawnCommandMarker(UnityVector3 worldPoint, Color color)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.name = "CommandMarker";
            marker.transform.position = new UnityVector3(worldPoint.x, 0.1f, worldPoint.z);
            marker.transform.localScale = new UnityVector3(0.35f, 0.02f, 0.35f);
            Renderer renderer = marker.GetComponent<Renderer>();
            renderer.material.color = color;
            Object.Destroy(marker.GetComponent<Collider>());
            marker.AddComponent<CommandMarkerView>();
        }

        private static void ApplyPrototypeModel(GameObject root, FactionId factionId)
        {
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new UnityVector3(0f, 0.15f, 0f);
            body.transform.localScale = new UnityVector3(0.55f, 0.55f, 0.55f);
            Renderer bodyRenderer = body.GetComponent<Renderer>();
            bodyRenderer.material.color = factionId == FactionId.Player ? new Color(0.25f, 0.65f, 0.95f) : new Color(0.55f, 0.65f, 0.2f);
            Object.Destroy(body.GetComponent<Collider>());

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.transform.SetParent(root.transform, false);
            head.transform.localPosition = new UnityVector3(0f, 1.05f, 0f);
            head.transform.localScale = new UnityVector3(0.45f, 0.45f, 0.45f);
            Renderer headRenderer = head.GetComponent<Renderer>();
            headRenderer.material.color = factionId == FactionId.Player ? new Color(0.92f, 0.92f, 0.96f) : new Color(0.78f, 0.88f, 0.48f);
            Object.Destroy(head.GetComponent<Collider>());
        }

        private static Rect BuildScreenRect(UnityVector2 start, UnityVector2 end)
        {
            float xMin = Mathf.Min(start.x, end.x);
            float xMax = Mathf.Max(start.x, end.x);
            float yMin = Mathf.Min(start.y, end.y);
            float yMax = Mathf.Max(start.y, end.y);
            return Rect.MinMaxRect(xMin, Screen.height - yMax, xMax, Screen.height - yMin);
        }

        private static bool IsShiftHeld()
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && (keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed);
        }

        private static bool IsCtrlHeld()
        {
            Keyboard keyboard = Keyboard.current;
            return keyboard != null && (keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed);
        }

        private static List<NumericVector2> BuildFormationDestinations(NumericVector2 center, int count)
        {
            List<NumericVector2> destinations = new List<NumericVector2>();
            if (count <= 0)
            {
                return destinations;
            }

            if (count == 1)
            {
                destinations.Add(center);
                return destinations;
            }

            int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
            int rows = Mathf.CeilToInt((float)count / columns);
            float spacing = 1.75f;
            float width = (columns - 1) * spacing;
            float height = (rows - 1) * spacing;

            for (int i = 0; i < count; i++)
            {
                int row = i / columns;
                int column = i % columns;
                float offsetX = (column * spacing) - (width * 0.5f);
                float offsetY = (row * spacing) - (height * 0.5f);
                destinations.Add(new NumericVector2(center.X + offsetX, center.Y + offsetY));
            }

            return destinations;
        }

        private void ResolveObstacleOverlap(SandboxSquadView view, float collisionRadius)
        {
            if (_obstacleVolumes == null || _obstacleVolumes.Length == 0 || view == null || view.IsDead)
            {
                return;
            }

            UnityVector3 position = view.transform.position;
            for (int i = 0; i < _obstacleVolumes.Length; i++)
            {
                ObstacleVolume obstacle = _obstacleVolumes[i];
                if (obstacle == null || obstacle.ObstacleCollider == null)
                {
                    continue;
                }

                UnityVector3 closest = obstacle.ObstacleCollider.ClosestPoint(position);
                NumericVector2 current2D = new NumericVector2(position.x, position.z);
                NumericVector2 closest2D = new NumericVector2(closest.x, closest.z);
                NumericVector2 offset = current2D - closest2D;
                float distance = offset.Length();

                if (distance >= collisionRadius)
                {
                    continue;
                }

                NumericVector2 pushDirection;
                if (distance <= 0.0001f)
                {
                    UnityVector3 obstacleCenter = obstacle.ObstacleCollider.bounds.center;
                    pushDirection = NumericVector2.Normalize(current2D - new NumericVector2(obstacleCenter.x, obstacleCenter.z));
                    if (pushDirection.LengthSquared() <= 0.0001f)
                    {
                        pushDirection = new NumericVector2(1f, 0f);
                    }
                }
                else
                {
                    pushDirection = NumericVector2.Normalize(offset);
                }

                float penetration = collisionRadius - distance;
                NumericVector2 corrected = current2D + (pushDirection * penetration);
                position = new UnityVector3(corrected.X, position.y, corrected.Y);
                view.transform.position = position;
            }
        }

        private sealed class WaveSpawnPlan
        {
            public WaveSpawnPlan(int waveIndex, float delaySeconds, IReadOnlyList<BattleSquadState> squads)
            {
                WaveIndex = waveIndex;
                DelaySeconds = delaySeconds;
                Squads = squads;
            }

            public int WaveIndex { get; }
            public float DelaySeconds { get; }
            public IReadOnlyList<BattleSquadState> Squads { get; }
        }
    }
}
