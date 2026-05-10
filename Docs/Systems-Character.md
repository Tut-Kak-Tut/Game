# Character System

## Files

- `Assets/Game/Scripts/Character/CharacterStats.cs`
- `Assets/Game/Scripts/Character/Player/PlayerController.cs`
- `Assets/Game/Scripts/Character/Enemy/EnemyAI.cs`
- `Assets/Game/Scripts/Character/Player/Interactor.cs`

## Responsibilities

- `CharacterStats` owns attributes, derived combat values, resources, regeneration, and damage processing.
- `PlayerController` reads input and applies movement/sprint/dash using `CharacterStats`.
- `EnemyAI` handles patrol/chase/attack behavior with `NavMeshAgent`.
- `Interactor` opens/closes inventory UI based on range and selected interactable inventory source.

## Runtime Dependencies

- `CharacterStats` emits `OnResourceChanged` and `OnStatsRecalculated` for UI and other systems.
- `EnemyAI` now expects an explicit `target` assignment (via inspector or bootstrap).
- `CharacterStats` can receive `DamageTextManager` through `SetDamageTextManager()`.

## Notes for Future Work

- Keep combat math centralized in `CharacterStats`.
- Prefer explicit scene wiring over object-tag lookups for AI targets.
