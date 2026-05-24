# Refactoring Plan — Space and Time

**Goal:** evolve a 1-day game-jam codebase into one that can sustain a
multi-year project: more levels, more tile/enemy/projectile types, more
team members, and a steady release cadence — without rewriting it all at
once.

**The product target is explicitly a moving target.** New tile variants
(basic, immovable, time-reversible — and more), new enemy archetypes,
new projectiles, new mechanics, and new visual styles will keep arriving.
Every system this plan touches must therefore be:

- **Modular** — one responsibility per file, no god classes.
- **Independent** — talks to peers through interfaces, events, and
  ScriptableObject data, not by reaching into their internals.
- **Replaceable** — a concrete behaviour can be swapped for another
  behind the same interface without ripple changes.
- **Expandable** — adding a new variant requires a new asset (and
  optionally a new component) and zero edits to existing systems.

If a phase below appears to violate these principles for short-term
convenience, raise it.

**Non-goals:**
- A clean-room rewrite. Every phase below preserves the current gameplay
  and is independently shippable.
- "Best-practice for the sake of it." We only adopt new patterns where
  there is a clear, ongoing payoff (new content, fewer regressions, less
  rework).
- Engine migration. Stay on Unity 6 + URP + the 2D Feature.

---

## 1. Diagnosis — what the jam code optimised for, and what it costs us now

The current code optimised for **two things**: writing the next feature in
the fewest minutes possible, and not having to coordinate between team
members during the jam. Those are good optimisations for a 24-hour sprint;
they are expensive after week two.

The concrete consequences we see in the repo today:

| Jam shortcut                                  | Why it was fast            | What it costs going forward                 |
|-----------------------------------------------|----------------------------|---------------------------------------------|
| Static singletons (`GameManager.pausedGame`, `GameTimeManager.gameTime`) | No DI to set up, accessible from anywhere | Cannot test, cannot run two instances, cross-scene leaks, every new system adds a new static flag |
| `FindFirstObjectByType` / `FindGameObjectWithTag` in `Start` | No inspector wiring needed | Silent nulls when load order changes; impossible to grep "what depends on the player?" |
| Polling `GameManager` flags every `Update` (`Dies.cs`, `Wins.cs`, `Enemy.cs`) | No event plumbing | Per-frame CPU, race conditions between systems that all wake up the same frame |
| Magic numbers (`75f`, `5f`, `2f`, layers `3/7/8`, `buildIndex < 2`) | Type the number once and move on | Designers cannot tune values without a programmer; renaming a layer breaks runtime |
| Flat `Assets/Scripts/` with no namespaces, no asmdefs | One folder, zero setup | Every script recompiles on every edit; no module boundaries; no way to mark internal APIs |
| Two parallel pause systems (`UIManager`, `pauseMenu`) | Both authors needed pause at the same time | Designer can't tell which one is "real"; future bugs land in the wrong file |
| Filename / class mismatch (`PauseMenu.cs` → `class pauseMenu`) | Unity tolerates it | Refactors and IDE go-to-definition behave oddly |
| Empty `Update`/`Start` left in stubs | "I might use them later" | Every empty `Update` is a real per-frame cost; obscures whether a behaviour is intentional |
| Inconsistent naming (`playGame` vs `Pause`, `is_dragging` vs `isAttacking`) | Two authors, two styles | Readers can't predict an API; new contributors guess wrong |
| Hardcoded scene navigation (`buildIndex + 1`, `< 2`) | One line, done | Every new level requires editing `GameManager.cs`; no way to do non-linear progression |
| `TileType` enum with 22 entries, only `YELLOW` used | "We'll add the others later" | Adding a real new tile colour requires more than the enum; the enum lies to readers |
| Debug logs left in (`"CRAZYYY"`, `"abc"`, `"Craztasda"`) | Faster than removing | Spam the console; mask real warnings |
| Mixed-language assets / Serbian comments | Native language of the authors | Future contributors can't search by intent |
| `Web/` and `Web.zip` checked in | Shareable URL for the jam | Repo bloated by ~30 MB, every rebuild churns the diff |
| 85 commits, messages like `yes`, `a`, `fix`, `final caouansfjbas` | Nobody was going to read them | Bisecting a regression is hopeless |
| No tests, no CI, no `.editorconfig` | Time pressure | Every change is hand-verified; regressions ship |

We don't fix all of these — we **stage** them, so each phase delivers a
visible improvement and the team can keep shipping content in parallel.

---

## 2. Guiding principles for the refactor

1. **Always shippable.** Every phase ends with the game still playable.
   No "big bang" merges.
2. **Behaviour-preserving by default.** Refactors don't change gameplay
   feel unless we explicitly opt in to a balance change.
3. **One thing at a time.** Don't bundle a rename, a re-architecture, and
   a new feature in the same PR.
4. **Data over code.** Once a value is touched by a designer, move it to
   a ScriptableObject. Code is for behaviour, data is for tuning.
5. **Editor-first verification.** A green compiler doesn't mean the
   scenes still work. Every refactor PR includes an editor-checked
   verification list.
6. **Leave a paved road.** Each new pattern we adopt gets a one-line
   "use this from now on" entry in `CLAUDE.md`, so the next contributor
   doesn't have to guess.

---

## 3. Phased plan

Phases are ordered to **maximise early payoff and minimise rollback
risk**. Each phase ends with a clear, testable acceptance check.

### Phase 0 — Repo & workflow hygiene (≈ ½ day, zero gameplay risk)

Done before any gameplay code is touched. Pure plumbing.

- [x] **Add `README.md`** with: one-paragraph pitch, Unity version,
      "open and press Play" instructions, link to `CLAUDE.md` and this
      plan.
- [x] **Add `.editorconfig`** with: 4-space indent, Allman braces,
      `dotnet_style_*` rules matching the **target** style (PascalCase
      types/methods, `_camelCase` private fields).
- [x] **Add a `LICENSE` file** (proprietary or MIT — user's call).
- [x] **Add Unity build artefacts to `.gitignore`**: `Web/`, `Web.zip`,
      `UpgradeLog.htm`. Remove them from tracking
      (`git rm -r --cached Web Web.zip UpgradeLog.htm`).
- [x] **Adopt a commit-message convention** (Conventional Commits is
      enough: `feat:`, `fix:`, `refactor:`, `chore:`, `docs:`). Add a
      one-paragraph "How we write commits" section to the README.
- [x] **Add a `.github/PULL_REQUEST_TEMPLATE.md`** with: summary, screenshots
      / GIF for visual changes, manual verification checklist.
- [x] **Branch protection on `main`**: require PR + one review. (Done in
      GitHub settings, not in the repo, but recorded here.)

**Acceptance:** repo size drops by ~30 MB; `git log` going forward
shows conventional messages; opening the project in Rider/VS picks up
the editorconfig style.

---

### Phase 1 — Naming, files, and dead code (≈ 1 day, low risk)

Make the code easier to read before changing what it does.

- [x] Rename `PauseMenu.cs::class pauseMenu` → delete the file entirely
      (it's the dead pause system; `UIManager` is the live one).
      Verify nothing in any scene references it
      (search the YAML for the script GUID, not the class name).
- [x] Standardise method casing to **PascalCase** for public API:
      `playGame` → `PlayGame`, `quitGame` → `QuitGame`,
      `pickUpKey` → `PickUpKey`, `useKey` → `UseKey`,
      `changePicture` → `ChangePicture`, `teleport` → `Teleport`.
      Update UI Button OnClick wiring in scenes (each renamed method
      needs its scene reference fixed — this is editor work).
- [x] Standardise private fields to `camelCase`; rewrite the few
      `is_dragging` / `is_rotating` to `isDragging` / `isRotating`.
- [ ] Remove empty `Update()` / `Start()` bodies in `Door`, `Key`,
      `Portal`, `PortalCollider`, `ShootSound`, `MainMenu`,
      `HandController` (if Start is truly empty).
- [x] Remove or downgrade dev-only `Debug.Log` strings:
      `"CRAZYYY"`, `"abc"`, `"Craztasda"`, `"pinestse"`, `"Quit"`,
      commented `//Debug.Log(stage)`. Keep the `LogWarning` in
      `CameraController` (it's actually useful).
- [x] Decide on the future of `ShootSound.cs` (presses K to test).
      Either delete it or move it to a clearly-marked `Dev/` folder.
- [x] Remove the typo `dirrection` in `Projectile.cs` (local var only —
      safe).
- [x] Translate the Serbian inline comments in `DragDropSystem.cs` to
      English (or, equivalently, delete the ones that just restate the
      code — most do).
- [x] Rename `EnemyCandleSecond Variant.prefab` (and friends) to remove
      the space — **only** via the Unity editor so references update.
- [x] Add a stub `ARCHITECTURE.md` that points at `CLAUDE.md` and this
      plan, and lists the modules-to-be (see Phase 4).

**Acceptance:** scenes still play; the Unity Console is quiet on Play;
no compile warnings; `pauseMenu` is gone.

---

### Phase 2 — Configuration via ScriptableObjects (≈ 2 days, low risk)

Pull tunables out of code so designers can iterate without a programmer
and without recompiling.

Introduce **`Assets/Data/`** and three ScriptableObject types:

- `GameSettings` (singleton SO, loaded once):
  - `startingTime` (was `75f`)
  - `maxTime` (was `75f`)
  - `attackTimeCost` (was `5f`)
  - `enemyAttackTimeDrain` (was `2f`)
  - `musicMediumThreshold` (was `50`)
  - `musicFastThreshold` (was `25`)
- `EnemyDefinition` (one per enemy archetype):
  - `moveSpeed`, `detectionRange`, `attackRange`, `attackCooldown`,
    `timeRefundOnKill`, `obstacleMask`, `sprite`, `animatorController`.
- `TileDefinition` (replaces the misleading `TileType` enum):
  - `id`, `displayName`, `prefab`, `colour`, `isMovable`, optional
    interaction component.

Migration steps:
1. Create the three SO types; populate one instance per current value.
2. Replace the literals in `GameTimeManager`, `PlayerAttack`, `Enemy`,
   `SongSpeedUp` with reads from `GameSettings`. Keep the SO referenced
   by a single bootstrap component in each scene (or via
   `Resources.Load` if we accept the cost — prefer the explicit
   reference).
3. Convert `Enemy.cs` to read its tunables from an
   `EnemyDefinition` SerializeField, defaulting to the existing values.
4. Delete the `TileType` enum once `TileDefinition` is wired through
   `Draggable`. Keep a compatibility shim for one phase if needed.

**Acceptance:** changing the starting time in the SO and pressing Play
changes the game without a script edit; the SO assets live under
`Assets/Data/`.

---

### Phase 3 — Untangle global state behind interfaces (≈ 3 days, medium risk)

The static-field singletons are the single biggest correctness risk and
testability blocker. Replace them with **explicit services** that other
code requests.

- [x] Introduce an `IGameTime` service (interface) with `Get`, `Add`,
      `Reduce` methods and a `TimeChanged` event.
- [x] Introduce an `IGameState` service exposing `IsPaused`,
      `IsGameOver`, events `Paused`, `Resumed`, `LevelCleared`,
      `PlayerDied`.
- [x] Provide both as `MonoBehaviour` implementations on a single
      `GameSystems` root object (one per scene), with a static
      service-locator `GameSystems.Time` / `GameSystems.State` —
      simpler than full DI, gives us testability and one obvious owner.
- [x] Migrate consumers one at a time:
      `WatchManager`, `SongSpeedUp` → `IGameTime`;
      `PlayerController`, `Enemy`, `Projectile`, `DragDropSystem`,
      `Dies`, `Wins`, `UIManager` → `IGameState`.
- [x] Replace per-frame polling (`if (GameManager.passed) …`) with
      **event subscriptions** (`state.LevelCleared += ShowWinPanel`).
- [x] Remove the old static API and replace with services (no thin
      adapters during the migration to avoid a flag-day rewrite; delete
      once no callers remain.

**Acceptance:** searching the codebase for `GameManager.` / `static
bool pausedGame` returns zero hits; `Dies.cs` / `Wins.cs` no longer
have an `Update`.

---

### Phase 4 — Folder structure, namespaces, assembly definitions (≈ 1 day, low risk)

Carve the flat `Scripts/` folder into modules so compile times stop
ballooning and module boundaries become real.

Proposed layout under `Assets/Scripts/`:

```
SpaceAndTime/
├── Core/          # IGameTime, IGameState, GameSettings, service locator
├── Player/        # PlayerController, PlayerAttack, HandController
├── Enemies/       # Enemy + future archetypes
├── Tiles/         # DragDropSystem, Draggable, TileDefinition
├── Interactables/ # Door, Key, Portal*, Green*
├── Projectiles/   # Projectile, PlayerAttack pool helpers
├── Audio/         # SongSpeedUp, ShootSound
├── Camera/        # CameraController, CameraZoom
├── UI/            # UIManager, MainMenu, PauseMenuButton, WatchManager, Dies, Wins
└── Dev/           # Anything that should NOT ship (cheat keys, debug overlays)
```

- [ ] Wrap each folder in a namespace (`SpaceAndTime.Core`,
      `SpaceAndTime.Player`, …).
- [ ] Add one `.asmdef` per folder, declaring only the references it
      actually needs. Most things depend on `Core`; UI also depends on
      `UnityEngine.UI` / TextMeshPro.
- [ ] Add a `SpaceAndTime.Tests.EditMode.asmdef` under
      `Assets/Tests/EditMode/` so the existing test framework package
      finally has somewhere to run tests.
- [ ] Move scripts to their new homes **through the Unity editor** so
      the `.meta` files (and every scene reference) follow them.

**Acceptance:** changing a UI script does not recompile the Tiles
module; `Test Runner` lists at least one (trivial) test that passes.

---

### Phase 5 — Input System migration (≈ 1 day, medium risk)

The `.inputactions` asset is already in the repo; nothing uses it.

- [ ] Confirm `Active Input Handling` is `Both` in Player Settings.
- [ ] Generate the C# class from `InputSystem_Actions.inputactions`.
- [ ] Migrate `PlayerController.Update` to read from the generated
      actions (`Move`, `Attack`). Wire the WASD/Arrow toggle as a
      control scheme.
- [ ] Migrate `UIManager.Update`'s Escape key to a `UI/Pause` action.
- [ ] Migrate `PlayerAttack.Update`'s Space-to-shoot.
- [ ] Migrate `DragDropSystem` (mouse) and `CameraZoom` (scroll) last,
      since they're the noisiest.

**Acceptance:** `Input.GetKey` / `Input.GetMouseButton*` returns no
hits under `Assets/Scripts/`; rebinding a key in the actions asset
changes the running game.

---

### Phase 6 — Level/Scene flow (≈ 1 day, low risk once Phase 3 is done)

Replace `SceneManager.GetActiveScene().buildIndex + 1` and the magic
`< 2` check with data.

- [ ] Add a `LevelSequence` ScriptableObject: ordered list of
      `LevelEntry { sceneAsset, displayName, isFinalLevel }`.
- [ ] `IGameState.LevelCleared` → `LevelLoader` looks up the next
      entry in `LevelSequence` and loads it (or shows credits if
      `isFinalLevel`).
- [ ] Add a `MainMenu` reference to the same `LevelSequence` so "Play"
      starts at index 0 of gameplay levels.
- [ ] Add level metadata that can later drive a level-select screen.

**Acceptance:** adding a `Level3` requires only:
(1) creating the scene, (2) dragging it into `LevelSequence`. No code
edit.

---

### Phase 7 — Content extensibility (≈ 2 days, medium risk)

Now that we have services, definitions, and modules, make the bits
designers will keep adding (enemies, tiles, projectiles, pickups)
extensible without code changes.

- [ ] **Enemies:** one `EnemyDefinition` SO per archetype, with optional
      behaviour overrides (move-only, ranged, etc.). `Enemy.cs` becomes
      a generic driver; specialised behaviour lives in pluggable
      components (`IEnemyBehaviour`).
- [ ] **Tiles:** `TileDefinition` SOs drive `Draggable`. The
      `isMovable` flag replaces the duplicated `*Imovable.prefab`
      tree. Wall-brim logic continues to live in `DragDropSystem`.
- [ ] **Projectiles:** generic projectile with a `ProjectileDefinition`
      SO (`speed`, `lifetime`, `timeCost`, `vfx`, `hitMask`).
- [ ] **Pickups:** unify `Key` and any future pickup behind a small
      `IPickup` interface, owned by the player's inventory.
- [ ] **Object pooling:** keep the hand-rolled pool in `PlayerAttack`,
      but factor it into a `Pool<T>` helper in `Core/` so the next
      author doesn't reimplement it.

**Acceptance:** adding a new enemy type requires (1) a prefab,
(2) a new `EnemyDefinition`, (3) optional component for unique
behaviour — no edits to `Enemy.cs`.

---

### Phase 8 — Persistence, settings, save/quit (≈ 2 days, medium risk)

Required before "multi-year" — players will expect to keep progress.

- [ ] `SaveData` SO with: highest-cleared level, audio volumes, input
      bindings (Phase 5 enables this).
- [ ] Persist via `JsonUtility` to `Application.persistentDataPath`.
      Single-slot is fine; structure the JSON so we can grow to multi-slot.
- [ ] Settings menu (volume sliders, fullscreen, key rebinds).
- [ ] Continue/Restart distinction in the main menu.

**Acceptance:** clearing Level 1 and quitting/relaunching brings the
player back to a menu that lets them continue from Level 2.

---

### Phase 9 — Tests, CI, and definition-of-done (≈ 1–2 days)

Cheap, mechanical wins now that asmdefs exist:

- [ ] EditMode tests for `IGameTime` (reduce/add/clamp boundaries),
      `LevelSequence` lookup, `Pool<T>`.
- [ ] PlayMode smoke test: load `Level1`, fire one bullet, check the
      `Projectile` deactivates.
- [ ] GitHub Action that runs `unityci/game-ci` to:
      (1) license-activate, (2) `Tests` step, (3) build WebGL on
      tagged releases.
- [ ] Add a "definition of done" to the PR template:
      *builds locally • tests pass • opened the affected scene • no
      new console warnings.*

**Acceptance:** PRs show a green check before merging; broken
`[SerializeField]` references are caught by the PlayMode smoke test.

---

## 4. What we explicitly defer

Listed so future contributors can stop pattern-matching for them:

- **Full DI framework** (Zenject / VContainer). The service locator is
  enough for this scale; revisit when we have > 50 systems.
- **ECS / DOTS.** No demonstrated CPU need; the entity count is small.
- **Custom editor tooling** beyond what ScriptableObjects give us for
  free. Build it when a designer asks for the third time.
- **Localisation.** Lock the in-game text to English first; localise
  once content has stabilised.
- **Networking / multiplayer.** Out of scope; the `multiplayer-center`
  package is installed but unused.

---

## 5. Risk register

| Risk                                                       | Likelihood | Impact | Mitigation                                                                 |
|------------------------------------------------------------|:----------:|:------:|----------------------------------------------------------------------------|
| Scene/prefab references break during folder moves          | High       | High   | Move files only through the Unity editor; verify with PlayMode smoke test  |
| Static → service migration leaves a half-migrated state    | Medium     | High   | Keep adapters during transition; gate each migration with a PR check       |
| Method renames break scene-bound `OnClick` handlers        | Medium     | Medium | Use Unity's "Find references in scene" before renaming; spot-check each UI |
| Designers fork their own SOs and lose changes              | Medium     | Medium | One repo for SO assets; lint that each SO has exactly one author per PR    |
| Input System migration regresses on WebGL                  | Low        | Medium | Build a WebGL build at the end of Phase 5 and smoke-test in a browser      |
| Large prefabs / scenes generate noisy YAML diffs           | High       | Low    | Adopt "Force Text Asset Serialization" (already on); review YAML in pairs  |

---

## 6. Sequencing summary (Gantt-flavoured)

```
Phase 0  ▓                                                     hygiene
Phase 1   ▓▓                                                   rename / dead code
Phase 2     ▓▓▓                                                ScriptableObject configs
Phase 3        ▓▓▓▓▓                                           services replace statics
Phase 4             ▓▓                                         folders / namespaces / asmdefs
Phase 5               ▓▓                                       Input System
Phase 6                 ▓▓                                     LevelSequence
Phase 7                   ▓▓▓                                  content extensibility
Phase 8                       ▓▓▓                              persistence + settings
Phase 9                          ▓▓                            tests + CI
```

Phases 0–2 can run in parallel with new content work — they touch only
code and tooling. Phases 3 and 4 need a coordinated "code-freeze
afternoon" because they touch many files. Phases 5–9 are again parallel
with content.

---

## 7. Acceptance: "the refactor is done when…"

- `Assets/Scripts/` has at most one assembly recompile per logical
  module change.
- Adding a level requires no code edits.
- Adding an enemy archetype requires no edits to `Enemy.cs`.
- No `static` mutable game state remains.
- No `FindFirstObjectByType` / `FindGameObjectWithTag` calls remain in
  gameplay scripts.
- A new contributor can clone the repo, open it in Unity, press Play,
  and reach Level 1 within five minutes — without reading any source.
- `CLAUDE.md` matches the actual conventions (i.e. its "as-is" notes
  have been promoted to "this is how we do it").
