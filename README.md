# Space and Time

A 2D top-down puzzle-action game. The player explores a rearrangeable grid
of "tiles" (rooms), shoots projectiles, picks up keys, fights enemies,
charges portals, and reaches an exit door — all on a countdown timer that
doubles as the player's health.

The project began as a ~1-day game jam build and is being evolved into a
longer-lived project. See [`REFACTORING_PLAN.md`](REFACTORING_PLAN.md) for
the roadmap.

## Tech stack

- **Engine:** Unity 6 (`6000.0.40f1`)
- **Render pipeline:** Universal Render Pipeline (URP) 17.0.4
- **Primary build target:** WebGL

## Getting started

1. Install [Unity Hub](https://unity.com/download).
2. Install Unity **`6000.0.40f1`** (the exact version is recorded in
   `ProjectSettings/ProjectVersion.txt`).
3. Open the `SpaceAndTime/` folder in Unity Hub.
4. Open `Assets/Scenes/mainMenu.unity` and press **Play** — or open
   `Assets/Scenes/Level1.unity` to skip the menu.

## Building (WebGL)

**File → Build Settings → Web → Build.** The build output is not committed
to the repository (see `.gitignore`).

## Repository conventions

### Commit messages

Use [Conventional Commits](https://www.conventionalcommits.org/): a
type prefix followed by an imperative summary.

```
feat:     a new gameplay feature
fix:      a bug fix
refactor: a code change that neither fixes a bug nor adds a feature
chore:    tooling, build, or repo maintenance
docs:     documentation only
```

Example: `feat: add Level3 and portal puzzle`.

### Pull requests

Every change goes through a pull request with at least one review.
Fill in the PR template, include a screenshot or GIF for visual changes,
and complete the manual verification checklist.

> **Branch protection:** `main` is protected — it requires a pull request
> and one approving review. Configure this in the GitHub repository
> settings (Settings → Branches); it cannot be set from the repo files.

## Documentation

- [`CLAUDE.md`](CLAUDE.md) — codebase structure, conventions, and known
  sharp edges. Read this before making changes.
- [`REFACTORING_PLAN.md`](REFACTORING_PLAN.md) — the phased plan for
  evolving this codebase into a maintainable long-term project.

## License

Proprietary — all rights reserved. See [`LICENSE`](LICENSE).
