# Architecture

> **Status: stub.** This document describes the *intended* architecture.
> The codebase is mid-refactor; see [`REFACTORING_PLAN.md`](REFACTORING_PLAN.md)
> for how we get from the current state to the one described here.

## Where to start

- [`CLAUDE.md`](CLAUDE.md) — the codebase as it stands today: structure,
  conventions, and known sharp edges. **Read this first.**
- [`REFACTORING_PLAN.md`](REFACTORING_PLAN.md) — the phased plan for
  evolving the game-jam codebase into a maintainable project.

## Current state

All gameplay code lives flat in `Assets/Scripts/` with no namespaces and
no assembly definitions. Global state is held in static fields
(`GameManager`, `GameTimeManager`). Systems communicate by polling those
statics every frame. This works but does not scale — see the diagnosis
section of the refactoring plan.

## Target module layout

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

## Target runtime model

- **Services over statics.** `IGameTime` and `IGameState` replace the
  static `GameManager`/`GameTimeManager` fields, owned by a single
  `GameSystems` root object per scene.
- **Events over polling.** Systems subscribe to service events
  (`LevelCleared`, `PlayerDied`, `Paused`) instead of checking flags in
  `Update`.
- **Data over code.** Tunable values live in ScriptableObjects
  (`GameSettings`, `EnemyDefinition`, `TileDefinition`,
  `ProjectileDefinition`), so designers iterate without recompiling.
- **Scene flow via data.** A `LevelSequence` ScriptableObject defines
  level order, replacing hardcoded build-index arithmetic.

This document will be expanded as each refactoring phase lands.
