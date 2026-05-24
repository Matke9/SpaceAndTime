# CLAUDE.md

Guidance for AI assistants working in the **Space and Time** repository. Read
this before making changes; it captures the structure, conventions, and the
known sharp edges of this codebase.

---

## 1. Project at a glance

- **Genre / pitch:** 2D top-down puzzle-action game. The player explores a
  rearrangeable grid of "tiles" (rooms), shoots projectiles, picks up keys,
  fights enemies, charges portals, and reaches an exit door — all on a
  countdown timer that doubles as the player's health.
- **Origin:** Built during a game jam over ~1 day (the bulk of commits are
  dated **2025-06-08**), with a handful of polish commits in the days
  following. The codebase reflects that: it works, but it's optimised for
  "ship by the deadline" rather than long-term maintainability.
- **Engine:** **Unity 6 (`6000.3.6f1`)** with the **Universal Render Pipeline
  (URP) 17.0.4** and the **2D Feature** package.
- **Primary build target:** **WebGL** (a built `Web/` folder and `Web.zip`
  are checked into the repo — see §7 "Known sharp edges").

---

## 1a. Design philosophy — read this before adding anything new

**The game is being actively developed and its content will keep growing.**
New tile types, new enemy archetypes, new projectiles, new interactables,
new mechanics, new levels, and new visual styles are all expected.

Treat every system as if it will gain a sibling within a month:

- **Modular.** A system does one thing. Movement, attacking, pausing, time
  tracking, tile placement — each lives in one file with one
  responsibility. New behaviour goes in a new file, not bolted onto an
  existing one.
- **Independent.** No system reaches into another system's internals.
  Communication goes through interfaces, ScriptableObject data, or events.
  Avoid `FindFirstObjectByType` once a service layer exists for the thing
  you need (Phase 3 onward).
- **Replaceable.** Any concrete implementation should be swappable for
  another behind the same interface. This is why we have `IGameTime` /
  `IGameState`, `EnemyDefinition`, `TileDefinition`. A new behaviour =
  a new `ScriptableObject` (data) and/or a new component (logic), not an
  edit to a god class.
- **Expandable.** Adding a new tile colour, a new enemy archetype, or a
  new projectile must require only:
  1. one new ScriptableObject asset (data), and
  2. optionally one new MonoBehaviour component (behaviour).
  No edits to `DragDropSystem`, `Enemy.cs`, etc. If you find yourself
  about to add a switch/case or an enum branch in a long-lived system,
  stop and ask whether the new variant should live in its own component
  or definition instead.

**Tile types known to be in scope:**

- *Basic / movable* — yellow tiles, the puzzle's main mover. Wired via
  `YellowTileDefinition` (or per-instance variants going forward).
- *Immovable* — red tiles, locked in place. Wired via
  `RedTileDefinition`.
- *Time-reversible* — a planned movable variant; details to be designed.
  When added, it will get its own `TileDefinition` and (likely) its own
  behaviour component.
- *Special / interactive* — the current green / portal tiles, which
  carry their own behaviour script (`Green.cs`, `Portal.cs`). New
  variants should follow the same pattern: a `TileDefinition` plus an
  optional behaviour component, never an edit to `DragDropSystem`.

Expect more types beyond these. If you write code that hardcodes the
current set, you are creating tomorrow's bug.

---

## 2. Repository layout

```
SpaceAndTime/
├── Assets/                       # All Unity-managed content
│   ├── Animations/               # Animator controllers + .anim clips (flat)
│   ├── Images/MainMenu/          # Main-menu raster art
│   ├── Material/                 # Physics materials
│   ├── Prefabs/                  # Gameplay prefabs (flat folder)
│   ├── Resources.meta            # Resources/ folder is missing (only .meta)
│   ├── Scenes/                   # See §3 for scene roles
│   ├── Scripts/                  # ALL gameplay code, flat, no subfolders
│   ├── Settings/                 # URP renderer + scene template
│   ├── Sounds/                   # .wav music + sfx (large, uncompressed)
│   ├── Sprites/                  # Mixed-language subfolders (see §6)
│   ├── Texture/                  # (empty / placeholder)
│   ├── TextMesh Pro/             # Stock TMP install
│   ├── InputSystem_Actions.inputactions   # New Input System asset (UNUSED — see §5)
│   ├── DefaultVolumeProfile.asset
│   └── UniversalRenderPipelineGlobalSettings.asset
├── Packages/                     # manifest.json + packages-lock.json
├── ProjectSettings/              # Unity project settings (commit these!)
├── Web/                          # CHECKED-IN WebGL build output (see §7)
├── UpgradeLog.htm                # Stale Unity upgrade report (safe to delete)
├── .vsconfig                     # Visual Studio workload hint
├── .idea/                        # Rider settings (committed)
└── .gitignore                    # Standard Unity gitignore
```

There is currently **no `README.md`**, **no `LICENSE`**, **no CI config**, and
**no test code** (despite `com.unity.test-framework` being installed).

---

## 3. Scenes

`ProjectSettings/EditorBuildSettings.asset` lists three scenes in the build:

| Index | Scene             | Role                                  |
|------:|-------------------|---------------------------------------|
| 0     | `mainMenu.unity`  | Title screen, "Play" / "Quit"         |
| 1     | `Level1.unity`    | First playable level                  |
| 2     | `Level2.unity`    | Second playable level (currently last)|

`GameManager.Update` advances to the next scene via
`SceneManager.GetActiveScene().buildIndex + 1` and gates it with
`buildIndex < 2`. Adding a new level requires:
1. Adding the scene to **Build Settings** (the file above), and
2. Bumping the `< 2` check in `GameManager.cs` — see §7.

The following scenes exist in `Assets/Scenes/` but are **NOT** in the build —
treat them as scratch/dev scenes and do not delete without asking:

- `AAAAAAAA.unity`, `Bandrejnane.unity`, `Dusan.unity`, `SampleScene.unity`,
  `SampleSceneAgain.unity`, `escapeMenu.unity`.

---

## 4. Script inventory (`Assets/Scripts/`)

All ~27 scripts live in the root of `Scripts/` with no namespaces and no
assembly definitions. Grouped by responsibility:

**Global state / flow**
- `GameManager.cs` — static flags (`pausedGame`, `gameOver`, `passed`) +
  scene advancement. Acts as a global event bus by polling.
- `GameTimeManager.cs` — static countdown timer (starts at 75s, also serves
  as player HP). `AddTime`, `ReduceTime`, `GetTime` are static.
- `UIManager.cs` — pause/resume animations, restart, quit, main-menu nav.
- `PauseMenu.cs` — **second, parallel** pause implementation (class named
  `pauseMenu`, lowercase — file/class mismatch). Likely vestigial; prefer
  `UIManager`.

**Player**
- `PlayerController.cs` — WASD/arrow movement, sprite waddle, key inventory,
  death.
- `PlayerAttack.cs` — space-to-shoot, projectile pooling via
  `fireballHolder` children.
- `HandController.cs` — custom cursor (idle/pinch hand sprites).

**Enemies**
- `Enemy.cs` — LoS check + chase + attack (drains time on hit).
- `DiodeParticleSystem.cs` — particle FX driven by `Enemy.isAttacking`.

**Tile / drag-drop puzzle**
- `DragDropSystem.cs` — owns the `Dictionary<Vector2Int, GameObject>` of
  placed tiles, mouse-driven move + rotate, neighbour wall updates.
- `Draggable.cs` — per-tile component; declares `TileType` enum (22 colour
  variants, only `YELLOW` is actually used today).

**Interactables**
- `Door.cs` — consumes a key to trigger `GameManager.NextLevel()`.
- `Key.cs` — picked up by the player, increments `PlayerController.key`.
- `Portal.cs` + `PortalCollider.cs` — multi-stage portal that "charges" when
  hit by projectiles, then teleports the player.
- `Green.cs` + `GreenCollider.cs` — multi-stage destructible (vine/road).
- `Projectile.cs` — pooled projectile object.

**Camera / audio / misc**
- `CameraController.cs` — smooth-follow with optional bounds.
- `CameraZoom.cs` — scroll-wheel zoom on the orthographic size.
- `SongSpeedUp.cs` — switches between slow/medium/fast music tracks based
  on remaining game time.
- `ShootSound.cs` — orphan: plays a clip on the `K` key (dev/debug).
- `WatchManager.cs` — drives the on-screen clock fill amount.
- `Dies.cs`, `Wins.cs` — toggle "you died" / "you won" UI panels by polling
  `GameManager` flags every frame.
- `MainMenu.cs`, `PauseMenuButton.cs` — UI glue.

---

## 5. Conventions & invariants (as-is)

Following these will keep you consistent with the current codebase; the
refactor plan (`REFACTORING_PLAN.md`) describes what to change going forward.

### Coding style (current)
- **C# version:** Unity 6 default (C# 9). No `nullable` enabled.
- **Namespaces:** None — every script is in the global namespace.
- **Naming:** Inconsistent. The codebase mixes:
  - PascalCase methods (`Die`, `Attack`, `Pause`) and
  - camelCase methods (`playGame`, `quitGame`, `pickUpKey`, `useKey`,
    `changePicture`, `teleport`).
  - PascalCase classes — **except `pauseMenu`** in `PauseMenu.cs`.
  - Static fields are sometimes lowercase (`pausedGame`, `gameOver`).
  - Private fields are camelCase, sometimes prefixed (`is_dragging`,
    `is_rotating`) — Python-style snake_case sneaks in.
  - **When in doubt:** prefer PascalCase for types/public methods and
    camelCase for fields, even if neighbouring code disagrees.
- **Indentation:** 4 spaces.
- **Braces:** Allman style (new line).
- **Comments:** Some are in **Serbian** (e.g. "Proveravamo kolizije",
  "Update suseda", "Pomoćna funkcija"). Don't translate them in passing
  unless you're rewriting the surrounding code anyway.

### Architecture invariants the code relies on today
- **Tunable values live in ScriptableObjects.** As of Phase 2:
  - `GameSettings` (singleton, loaded from `Assets/Resources/Data/GameSettings.asset`
    via `GameSettings.Current`) holds the global magic numbers: `startingTime`,
    `maxTime`, `attackTimeCost`, `enemyAttackTimeDrain`, `musicMediumThreshold`,
    `musicFastThreshold`. **Do not** reintroduce hardcoded 75/5/2/50/25 values
    — read them from `GameSettings.Current`.
  - `EnemyDefinition` (per-archetype, under `Assets/Data/`) holds an
    enemy's stats. `Enemy.cs` reads from `_definition` in `Start()` and
    falls back to its own inspector fields when none is assigned.
    A new enemy = a new prefab + a new `EnemyDefinition`, no `Enemy.cs` edit.
  - `TileDefinition` (per-tile-type, under `Assets/Data/`) holds tile
    metadata. `Draggable.IsMovable` reads from `_definition.isMovable`
    and falls back to `tileType == YELLOW`. A new tile type = a new
    `TileDefinition` (and a behaviour component if it has unique logic).
    The `TileType` enum is a serialization shim and **must not** be
    extended for new content; add a `TileDefinition` instead.
- **Session state lives behind services, not statics.** As of Phase 3:
  - `IGameState` (implemented by `GameManager`) owns `IsPaused`,
    `IsGameOver`, `IsLevelCleared` plus the events `Paused`, `Resumed`,
    `PlayerDied`, `LevelCleared`. Reach it via `GameSystems.State`.
  - `IGameTime` (implemented by `GameTimeManager`) owns the countdown
    plus the `TimeChanged` event. Reach it via `GameSystems.Time`.
  - The previous `GameManager.pausedGame` / `GameTimeManager.GetTime()`
    static API has been removed; **do not reintroduce static
    mutable game state.** When a system reacts to a transition
    (player died, level cleared, paused/resumed), subscribe to the
    matching event instead of polling a flag every frame.
- **Pause is a "current-state" check, not a polled change.** Per-frame
  `Update` guards that skip work when paused stay as
  `if (GameSystems.State?.IsPaused ?? false) return;`. Use events when
  you care about the transition; use the property when you care about
  the value right now.
- **Health and the timer share one value.** `IGameTime.ReduceTime`
  returns `false` and triggers `PlayerDied` (via `IGameState.TriggerGameOver`)
  when the time would go non-positive.
- `IGameState.TriggerLevelCleared()` sets `IsLevelCleared = true` and
  pauses the game; pressing `Space` afterwards (handled inside
  `GameManager.Update`) loads the next scene in the build.
- The drag/drop tile registry (`DragDropSystem.draggableObjects`) is
  keyed by `Vector2Int` cell coordinates on `DragDropSystem.grid`. Tiles
  register themselves in `Draggable.Start()`.
- **Layers used (see `ProjectSettings/TagManager.asset`):**
  `3 Player`, `6 Enemy`, `7 Wall`, `8 Dragged`, `9 Item`, `10 Projectile`.
  Several scripts hard-code these layer numbers (`Projectile.cs` checks
  `collision.gameObject.layer == 3`, `DragDropSystem.SetLayerRecursively`
  toggles between `7` and `8`). If you need to introduce a new layer,
  prefer `LayerMask.NameToLayer` — but expect existing code to keep using
  the literals.
- **Tags relied on:** `"Player"` (used by several `FindGameObjectWithTag`
  callers). Don't rename it.
- **Magic numbers worth knowing:**
  - `75f` — max game time / starting time. Appears in `GameTimeManager`
    and (as `.75f - x/100`) in `WatchManager`.
  - `5f` — time cost of one shot (`PlayerAttack.Attack`).
  - `2f` — time drained per enemy hit (`Enemy.MoveEnemy`).
  - `25/50` — thresholds where `SongSpeedUp` swaps music tracks.

### Input
- The project has **the new Input System package installed** (1.13.0) and
  a `.inputactions` asset (`Assets/InputSystem_Actions.inputactions`), but
  **none of the gameplay scripts use it**. All input goes through the
  legacy `UnityEngine.Input` (e.g. `Input.GetKey(KeyCode.W)`,
  `Input.GetMouseButtonDown(0)`, `Input.GetAxis("Mouse ScrollWheel")`).
- Both input backends are likely enabled in Player Settings ("Both"). If
  you add new input handling, **stay on `UnityEngine.Input` for now** to
  match the existing code — switching is a planned refactor, not a
  drive-by change.

### Prefabs / scenes
- Prefabs live flat in `Assets/Prefabs/`. Note the **space** in
  `EnemyCandleSecond Variant.prefab` and `EnemyDioda Variant.prefab` /
  `EnemyHourglass Variant.prefab` — these are Unity-auto-generated variant
  names. Don't rename them without updating every scene that references them.
- Several `*Imovable.prefab` variants exist (immovable versions of tiles).
  Note: `Imovable` is a typo of "Immovable" but it's baked into asset GUIDs
  via filenames — leave it alone unless doing a coordinated rename.

---

## 6. Languages in the codebase

This is a multilingual codebase. You will encounter:

- **Serbian** identifiers and folder names — primarily for art assets.
  Examples: `CrveniTajlovi/` ("red tiles"), `PokretniTajlovi(Zuti)/`
  ("movable tiles (yellow)"), `Ruka/` ("hand"), `VrataKljuc/`
  ("door key"). Scene name `Bandrejnane`. Comments like
  *"Pomoćna funkcija za postavljanje novog targeta"*.
- **English** code identifiers (class, method, and variable names).
- **Occasional swearing/banter** in commit messages and the odd debug
  string. Treat these as informal — do not echo them in commits or PRs
  you author.

When adding new content, **prefer English** for new identifiers, file
names, and comments. Do not mass-rename existing Serbian assets without
explicit user direction — asset GUIDs follow the `.meta` files, not the
filenames, so renames are safe in theory, but the scenes/prefabs reference
sprites by serialized GUID and will silently miss any renames that aren't
done through the Unity editor.

---

## 7. Known sharp edges (read before editing)

These are issues that will surprise you. The refactor plan addresses most of
them; for now, **work around** rather than "fix in passing", because each
has cross-cutting implications.

1. ~~**Two parallel pause systems.**~~ *Resolved in Phase 1.*
   `UIManager.Pause/Resume` is the only pause path; `PauseMenu.cs` was deleted.
2. ~~**Filename / class-name mismatch.**~~ *Resolved in Phase 1.*
3. ~~**Static singletons everywhere.**~~ *Resolved in Phase 3.*
   State now lives behind `IGameState` / `IGameTime`, accessed via
   `GameSystems.State` / `GameSystems.Time`. Scene reloads still need to
   reinitialise — the implementations do this in `Awake`.
4. **`FindFirstObjectByType` / `FindGameObjectWithTag` in `Start`.** Many
   scripts grab references this way. If the script's `Start` runs before
   the target exists (script execution order, additive scenes), it silently
   fails. New code should prefer `[SerializeField]` references wired in
   the inspector.
5. **Hardcoded scene-index check.** `GameManager.Update` advances levels
   only when `buildIndex < 2`. If you add `Level3`, **also bump that
   constant**, otherwise the win-screen `Space` press does nothing.
6. **Built artefact checked into the repo.** `Web/` and the 30 MB `Web.zip`
   are committed. Don't blow up the repo by adding to them; if you need to
   reproduce a build, run **Build → WebGL** from Unity and overwrite the
   folder. **Better long-term:** add `Web/` to `.gitignore` (see refactor
   plan).
7. **Debug logs left in.** `"CRAZYYY"`, `"abc"`, `"Craztasda"`,
   `"pinestse"`, `"Quit"`. Don't add to them. Removing them is fine in a
   focused cleanup pass.
8. **Empty `Update` / `Start` methods** — several scripts (`Door`, `Key`,
   `Portal`, `PortalCollider`, `ShootSound`) have empty Unity messages.
   Unity still calls them every frame — they have measurable cost in
   aggregate. Safe to remove the empty bodies; don't add new ones.
9. **The `TileType` enum** (22 entries) is mostly aspirational — only
   `YELLOW` (the default, value 0) is checked in code
   (`Draggable.tileType == 0`). Adding new tile colours via the enum
   alone won't change behaviour.
10. **`Resources.meta` exists but `Resources/` does not.** No code uses
    `Resources.Load`, so this is benign — but don't add anything to a
    `Resources/` folder unless you also create it.
11. **`Web.zip`** and **`UpgradeLog.htm`** are repo cruft. Safe to delete
    in a cleanup pass (with user approval).
12. **Build artefact `Web.zip`** is 30 MB — most of the repo's size.

---

## 8. Build, run, and test workflow

There is no CI, no Makefile, no `package.json`-style scripts. Everything
goes through Unity.

### Opening the project
1. Install Unity Hub.
2. Install **Unity `6000.0.40f1`** (the version recorded in
   `ProjectSettings/ProjectVersion.txt`). Newer 6.x patch releases will
   prompt an upgrade — let it happen if approved by the user, then
   re-commit `ProjectVersion.txt`.
3. Open the folder `SpaceAndTime/` in Unity Hub.

### Running in the editor
- Open `Assets/Scenes/mainMenu.unity` and press **Play**.
- Or open `Assets/Scenes/Level1.unity` directly to skip the menu.

### Building (WebGL)
- **File → Build Settings → Web**, then **Build**. Output goes to a
  folder of your choosing (historically `Web/` at the repo root).
- A pre-built copy is checked in at `Web/` + `Web.zip` for convenience.

### Tests
- `com.unity.test-framework` is installed but **no `Tests` folder exists
  and no `.asmdef` is configured for tests**. Running
  **Window → General → Test Runner → Run All** will report 0 tests.

### Linting / formatting
- **None configured.** No `.editorconfig`, no Roslyn analyzers, no
  `omnisharp.json`. Rely on the IDE defaults.

---

## 9. Working in this repo as an assistant

### General rules
- **Be conservative.** This codebase relies on a lot of implicit
  conventions (layer numbers, scene indices, tag strings, prefab names).
  Changing any of those without a full sweep will silently break a scene.
- **Editor-only changes need verification.** Most non-trivial edits in
  Unity affect `.unity` and `.prefab` YAML files — you cannot fully
  validate those from the command line. State plainly when a change
  needs the Unity editor open to verify, and ask the user to confirm.
- **Don't claim "ran tests" or "verified in browser" unless you did.**
  There is no headless harness in this repo.

### Editing scripts
- Stick to one script per file, classname matching filename. When you
  encounter the existing `pauseMenu` mismatch, leave it alone unless
  you're explicitly cleaning up.
- Default to **not adding `Update`** to a new MonoBehaviour unless you
  need per-frame work. Prefer events, coroutines, or `FixedUpdate` for
  physics.
- New MonoBehaviours that need references should expose them as
  `[SerializeField] private` fields, not look them up with
  `FindFirstObjectByType`.
- Always gate gameplay code on `GameManager.pausedGame == false` (until
  the refactor introduces a proper pause channel — see plan).

### Editing scenes / prefabs
- Prefer to make changes through the Unity editor where possible. If you
  must hand-edit YAML, only do so for clearly mechanical changes (e.g.
  bulk-renaming a serialized field) and always show a diff to the user
  first.
- Be alert: **GUIDs in `.meta` files matter**. Never delete `.meta`
  files for assets that are referenced; this will detach every reference
  in every scene/prefab.

### Git workflow
- The local working branch for the current Claude session is
  `claude/codebase-docs-refactor-plan-dYsmT` — push there, not `main`.
- Commit messages historically have been low-effort ("yes", "fix", "a",
  random phrases in Serbian). **Do not match that style.** Write
  conventional, imperative commit subjects (e.g. `docs: add CLAUDE.md
  with project conventions`).
- **Never** `git push --force` to `main`. Never amend or rewrite history
  on a shared branch.
- The remote `Web.zip` is large; avoid committing additional large
  binaries unless asked.

### When asked to add a feature
1. Find the closest existing analogue (e.g. "another tile type" → look
   at the green tile pair `Green.cs` + `GreenCollider.cs` and the portal
   pair `Portal.cs` + `PortalCollider.cs`).
2. Mirror that file/prefab pair rather than inventing a new architecture.
3. If the closest analogue is one of the refactor targets in
   `REFACTORING_PLAN.md`, ask whether the user wants the new feature in
   the **old style** (consistent, ships fast) or the **new style**
   (lifts a slice of the refactor with it).

### When asked to refactor
- Refer to `REFACTORING_PLAN.md`. Each phase there is scoped to be
  shippable on its own. Do not attempt the whole refactor in one PR.
- After any structural refactor, re-open every scene in the editor and
  press Play — broken `[SerializeField]` references show up there, not
  in the compiler.

---

## 10. Quick reference: where is X?

| I want to…                                | Look at…                                  |
|-------------------------------------------|-------------------------------------------|
| Change starting time / max HP             | `GameTimeManager.cs` (`75f`)              |
| Change shot cost / cooldown               | `PlayerAttack.cs`                         |
| Tweak enemy chase / attack                | `Enemy.cs`                                |
| Add a new level                           | New scene → Build Settings → bump `< 2`   |
|                                           | check in `GameManager.cs`                 |
| Change pause behaviour                    | `UIManager.cs` (Pause/Resume)             |
| Change camera follow / bounds             | `CameraController.cs`                     |
| Music speed-up thresholds                 | `SongSpeedUp.cs` (`< 25`, `< 50`)         |
| Tile drag/drop / rotate / placement       | `DragDropSystem.cs` + `Draggable.cs`      |
| Win / lose UI                             | `Wins.cs` / `Dies.cs` + the panels they   |
|                                           | toggle on the Canvas prefab               |
| Layer numbers                             | `ProjectSettings/TagManager.asset`        |
| Tag list (only `Player` is used)          | `ProjectSettings/TagManager.asset`        |

---

## 11. Companion documents

- **`REFACTORING_PLAN.md`** — phased plan for evolving this game-jam
  codebase into something that can sustain a multi-year project. Read it
  before making structural changes.
