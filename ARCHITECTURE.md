# Architecture

> **Status: living document.** The codebase is mid-refactor; what is
> *implemented* today is described in §3, what is *target* is in §4, and
> [`REFACTORING_PLAN.md`](REFACTORING_PLAN.md) lists how we get from one
> to the other.

## 1. Where to start

- [`CLAUDE.md`](CLAUDE.md) — the codebase as it stands today: structure,
  conventions, design philosophy, and known sharp edges. **Read this
  first.**
- [`REFACTORING_PLAN.md`](REFACTORING_PLAN.md) — the phased plan for
  evolving the game-jam codebase into a maintainable project.

## 2. Design principles (apply to everything)

The product is a moving target. New tile types (basic, immovable,
time-reversible — and more to come), new enemy archetypes, new
projectiles, new mechanics, new visual styles are all expected.

Every system is therefore built to be:

- **Modular** — one responsibility per file.
- **Independent** — communicates through interfaces, events, or
  ScriptableObject data; never reaches into another system's internals.
- **Replaceable** — concrete behaviour swaps behind the same interface.
- **Expandable** — adding a new variant means adding one asset (and
  optionally one component), not editing an existing system.

If you find yourself about to add a `switch`/`case` or a new enum branch
inside a long-lived system, stop and add a new ScriptableObject or
component instead.

## 3. Current state (post Phase 2)

- **Tunables live in data, not code.** Three ScriptableObjects:
  - `GameSettings` — global tunables (timer, attack costs, music
    thresholds). One singleton at `Assets/Resources/Data/GameSettings.asset`,
    accessed via `GameSettings.Current`.
  - `EnemyDefinition` — per-archetype enemy stats. Wired into each
    enemy prefab's `_definition` slot; `Enemy.cs` reads from it in
    `Start()` and falls back to its inspector fields.
  - `TileDefinition` — per-tile-type metadata
    (`displayName`, `isMovable`, `colour`). Wired into each `Draggable`
    prefab; `Draggable.IsMovable` falls back to `tileType == YELLOW`.
- **Global runtime state is still static** (`GameManager.pausedGame`,
  `GameTimeManager.gameTime`). Systems still poll those statics every
  frame. **Phase 3 replaces this with services and events.**
- All gameplay code lives flat in `Assets/Scripts/` with no namespaces
  and no assembly definitions. **Phase 4 splits this into modules.**

## 4. Target module layout

Phase 4 of the refactoring plan splits `Assets/Scripts/` into modules,
each with its own namespace and assembly definition:

| Module          | Responsibility                                              |
|-----------------|-------------------------------------------------------------|
| `Core`          | Game services (time, state), settings, service locator      |
| `Player`        | Player movement, attack, custom cursor                      |
| `Enemies`       | Enemy AI and archetypes                                     |
| `Tiles`         | Drag-drop puzzle grid and tile definitions                  |
| `Interactables` | Doors, keys, portals, destructibles                         |
| `Projectiles`   | Projectile behaviour and pooling                            |
| `Audio`         | Music and sound effects                                     |
| `Camera`        | Camera follow and zoom                                      |
| `UI`            | Menus, HUD, win/lose screens                                |
| `Dev`           | Debug-only tooling that must not ship                       |

## 5. Target runtime model

- **Services over statics.** `IGameTime` and `IGameState` replace the
  static `GameManager`/`GameTimeManager` fields, owned by a single
  `GameSystems` root per scene.
- **Events over polling.** Systems subscribe to service events
  (`LevelCleared`, `PlayerDied`, `Paused`) instead of checking flags in
  `Update`. Per-frame display-style polling (`WatchManager` reading the
  clock) is fine; per-frame change-detection polling (`Dies.cs` checking
  whether the player just died) is not.
- **Data over code.** Tunable values live in ScriptableObjects
  (`GameSettings`, `EnemyDefinition`, `TileDefinition`,
  `ProjectileDefinition`), so designers iterate without recompiling.
- **Scene flow via data.** A `LevelSequence` ScriptableObject defines
  level order, replacing hardcoded build-index arithmetic.
- **Pluggable content.** Each tile / enemy / projectile / pickup family
  is built so a new variant slots in via a new SO (data) plus an
  optional component (behaviour), with zero edits to existing system
  files.

## 6. Tile types — current and planned

| Type            | Movable | Behaviour script | Status          |
|-----------------|:-------:|------------------|-----------------|
| Basic / yellow  | yes     | (drag/drop only) | shipped         |
| Immovable / red | no      | (drag/drop only) | shipped         |
| Special / green | no      | `Green.cs`       | shipped         |
| Portal          | no      | `Portal.cs`      | shipped         |
| Time-reversible | yes     | TBD              | planned         |
| (future)        | ?       | ?                | open            |

Every type gets its own `TileDefinition` SO. Any new behaviour goes in
its own component on the prefab — not as a branch inside `DragDropSystem`
or `Draggable`.
