# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A 2D action-RPG / dungeon-crawler engine built in Unity. The architecture is modular and event-driven, with gameplay content (spells, skills, items, effects) defined as ScriptableObject assets rather than hardcoded.

A comprehensive design-doc set already exists under `Docs/`. **Read those for system internals — do not duplicate that content here.** Start with `Docs/INDEX.md` and `Docs/README.md`.

## Unity Setup

- **Editor version: `6000.4.0f1`** (Unity 6 LTS). Pinned in `ProjectSettings/ProjectVersion.txt`. Open the project with this exact version — opening with a different one will trigger an upgrade.
- **Main / only build scene:** `Assets/Game/Scenes/SampleScene.unity`.
- **Key non-built-in packages** (see `Packages/manifest.json` for full list and versions):
  - Input System, Universal Render Pipeline (URP 17), AI Navigation
  - 2D Animation, 2D Aseprite Importer, 2D PSD Importer, 2D SpriteShape, 2D Tilemap Extras
  - Timeline, Visual Scripting
- **Assemblies:** there are **no project-level `.asmdef` files** — all game code compiles into the default `Assembly-CSharp`. The only project-side `.asmdef`s belong to the third-party `NavMeshPlus` bundled under `Assets/Game/Models/NavMeshComponents/`. If you add an asmdef, expect to wire up references for the entire `Game.*` namespace tree.

## Build / Run / Test

- **Run:** open the project in Unity 6000.4.0f1 and press Play on `SampleScene`. There is no CLI build script.
- **Tests: there are none.** No `Tests/` folder, no test asmdefs, no EditMode/PlayMode test directories. Verify changes by Play-mode smoke test. Do not invent a `Test Runner` workflow that doesn't exist.

## Architectural Quick Map

The non-obvious wiring that's hard to re-derive from the file tree:

- **Entry point: `Assets/Game/Scripts/Core/SceneRuntimeBootstrap.cs`.** Its `Awake()` is the single source of scene setup:
  1. Ensures a `GameSession` singleton exists (instantiates `gameSessionPrefab` or creates a fallback `GameObject`).
  2. Resolves `DamageTextManager` (serialized ref → `FindAnyObjectByType` fallback).
  3. Calls `EnemyAI.SetTarget(playerTarget)` on every assigned enemy.
  4. Calls `CharacterStats.SetDamageTextManager(...)` on all stat receivers.
  5. Calls `EffectHandler.SetDamageTextManager(...)` on all effect receivers.

  When adding a new system that needs scene-wide wiring, hook it here rather than introducing a parallel bootstrapper.

- **Service root: `GameSession`** (`Assets/Game/Scripts/CoreRuntime/`) is the singleton. It exposes:
  - `IEventBus` — implemented by `RuntimeEventBus`
  - `SaveService` — with pluggable `ISaveDataProvider` registration

- **Two event channels — pick the right one:**
  - **`RuntimeEventBus`** — pub/sub for *struct* events implementing `IGameEvent` (e.g., `XpGrantedEvent`, `LevelUpEvent`, `SpellCastStartEvent`, `SpellResolvedEvent`), with a 256-slot replay queue. Use this for cross-system coordination.
  - **Direct C# `event Action`** on MonoBehaviours like `CharacterStats.OnResourceChanged` and `OnStatsRecalculated`. Use these for tight UI bindings (HP bars, buff icons).

- **Combat contract:** interfaces `ICombatant : ITargetable, IDamageable` in `Game.Combat`. **There is no shared base class for player and enemy** — composition over inheritance. Player and enemies both have a `CharacterStats` component plus role-specific behaviours (`PlayerController`, `EnemyAI`, `CombatantBehaviour`).

- **Damage flow:** `EnemyAI.TryAttack()` (or a spell effect) → `CharacterStats.TakeDamage(float, DamageType)` → applies armor/resistance → mutates `currentHealth` → fires `OnResourceChanged` → calls `DamageTextManager.SpawnText(worldPos, text, color)`.

- **Spell flow:** `SpellCasterRuntime.TryCast(...)` runs a coroutine through priming → casting → recover → cooldown, and publishes `SpellCastStartEvent` / `SpellResolvedEvent` on the bus.

- **Content as data:** new spells, skills, items, and effects are ScriptableObject `.asset` files — `SpellDefinition`, `SkillNodeDefinition`, `SkillTreeDefinition`, `ItemData`, `EffectData`. Author content as assets; do not bake values into scripts. See `Docs/EXTENDING.md` for how to add each type.

## Code Conventions

- **Namespaces:** `Game.<Subsystem>` — e.g. `Game.CoreRuntime`, `Game.Combat`, `Game.Spells`, `Game.Skills`. A few foundational types (`CharacterStats`, `DamageType`, `SceneRuntimeBootstrap`) intentionally sit in the global namespace; check references before relocating them.
- **Field exposure:** `[SerializeField] private` for inspector-edited state; public properties for cross-class access. `CharacterStats` exposes the six core attributes as public fields by design.
- **Events:** prefer the existing channels (RuntimeEventBus for systems, C# events for UI) over introducing new ones. The bus already supports replay for late subscribers.

## Where to Look First (under `Docs/`)

- `Docs/INDEX.md` — table of contents for everything below.
- `Docs/ARCHITECTURE.md` — high-level architecture and dependency graph.
- `Docs/SYSTEMS.md` plus `Docs/Systems-Character.md`, `Docs/Systems-Effects.md`, `Docs/Systems-Inventory.md`, `Docs/Systems-UI.md` — per-system deep dives.
- `Docs/EXTENDING.md` — how to add a new spell, skill, item, or effect.
- `Docs/DEVELOPMENT_STATUS.md` — what's done vs. roadmap.
- `Docs/SceneCatalog.md`, `Docs/AssetScope.md` — scene and asset inventory.

## What Not to Do

- Don't duplicate per-system documentation in this file — read the matching `Docs/Systems-*.md` instead.
- Don't hardcode gameplay content in C# — extend the relevant ScriptableObject type and author an `.asset`.
- Don't introduce a parallel singleton or service-locator alongside `GameSession`.
- Don't add tests assuming a test framework is configured — none is. If you intend to add tests, set up the `Tests/` folder and asmdefs explicitly and call that out.
- Don't bypass `SceneRuntimeBootstrap` for scene wiring; extend it.
