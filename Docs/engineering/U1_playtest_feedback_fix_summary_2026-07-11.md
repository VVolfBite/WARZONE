# U1 Playtest Feedback Fix Summary

## 1. Goal

U1 fixes the first Unity playtest feedback pass for the Battle Sandbox. The scope is squad command feel, pause and command-plan diagnostics, input stability diagnostics, and minimal movement blocking.

Baseline when work started:

- Branch: `main`
- Start HEAD: `20529118da1aeeca43cfaa28c1ec707932ae3b84`
- M16 reference commit: `fa1c9eb32281a5a940db502d93ad203741e33acc`
- U1 implementation commit SHA: `92722849e1c6131c32b99dd7b26ef380b78e35e0`
- U1 handoff package commit SHA: recorded in `package_manifest.txt` inside the generated handoff zip.

## 2. Playtest Feedback

- The sandbox felt like direct single-member control instead of squad command.
- Right-click targeting could land on member, enemy, obstacle, node, or building colliders.
- Pause controls were ambiguous because `P` and `Space` both toggled pause.
- Obstacles affected sight/fire/cover but did not block movement.

## 3. Squad Command Feel

- Member click still selects the owning squad, but members no longer show a selected-member visual state.
- Selected squad marker is larger and stronger, making the squad center the primary operation object.
- Debug panels now lead with selected squad, current order, desired position, active/dead/extracted/retreating counts, and derived member targets.
- Right-click move and tactical commands record squad command debug records.

## 4. Shift Command Plan Overlay

- Hold `Shift` to show the current command plan.
- The overlay draws the squad center to desired-position command line.
- It draws the squad target marker.
- It draws each selected-squad member to its current intent target and a ghost marker at that target.
- The overlay labels the selected squad order and member assigned targets.
- This is not a multi-step command queue. It shows the current order and derived member intents only.

## 5. Pause Controls

- Hold `P`: sets hold pause while the key is pressed.
- Release `P`: clears hold pause and restores the Space toggle state.
- `Space`: toggles persistent pause.
- Effective pause is `IsTogglePaused || IsHoldPaused`.
- Commands remain allowed while paused under the existing sandbox policy and execute after resume.

## 6. Input Diagnostics

- Command target acquisition now uses `TryGetCommandGroundPosition`.
- Command targeting prefers a Ground layer/object hit.
- If a collider hit is not ground, the command target falls back to ray intersection with the `y=0` ground plane.
- Debug panels show last input action, last command, last command position, frame/time, raycast hit name, and whether ground fallback was used.
- Recent squad command debug records keep the last five commands.

## 7. Movement Blocking

- Added `MovementBlockingRule`.
- `MovementSystem` now checks movement segments against `TacticalObstacleState.BlocksMovement && !IsDestroyed`.
- A blocked member stops at a safe point before the obstacle and emits `MovementBlocked`.
- Blocked movement does not complete the intent.
- Low cover with `BlocksMovement=false` does not block movement.
- This is a minimal blocker, not pathfinding. Players must currently command around obstacles.

## 8. Still Not Implemented

- No complete pathfinding.
- No multi-step command queue.
- No formal UI.
- No Unity scene polish pass.
- No campaign, economy, or enemy expansion.

## 9. Verification Results

- `scripts/check_text_boundaries.ps1`: OK.
- `scripts/check_asmdef_references.ps1`: OK.
- `scripts/check_unity_project_static.ps1`: OK.
- `scripts/check_warzone_all.ps1`: OK. Dotnet/msbuild skipped because not found. Package path check skipped until package generation.
- Manual narrow csc compile for Core/Content/Combat: OK.
- Manual narrow csc compile for Combat tests: OK.
- `scripts/check_domain_compile.ps1`: FAILED due existing framework csc inability to resolve `System.Text.Json` in `Assets/_Project/Application/Save/JsonSaveGameSerializer.cs`.
- `scripts/check_unity_editmode.ps1`: SKIPPED because `Unity.exe` was not found.
- Unity Editor compile and actual EditMode execution: not verified in this shell.

## 10. Manual Unity Validation Checklist

- Open M8 Building Tactics sandbox.
- Click a member and confirm the squad, not a single member, is visually selected.
- Right-click move and confirm the debug panel records a squad command.
- Hold `Shift` and confirm the command plan overlay appears.
- Hold `P` and confirm pause is active only while held.
- Press `Space` and confirm toggle pause persists.
- Right-click over members/obstacles/buildings and confirm target position lands on ground.
- Command movement through a wall/building blocker and confirm members stop before it.
- Confirm debug panel shows `MovementBlocked` events.

## 11. Next Steps

- Add simple path steering or waypointing around blockers.
- Add a real command queue only after the current-order overlay is stable.
- Replace debug GUI with formal tactical UI later.
- Validate command-plan readability in Unity across M7 and M8 scenes.
