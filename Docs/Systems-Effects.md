# Effects System

## Files

- `Assets/Game/Scripts/Effects/Core/EffectData.cs`
- `Assets/Game/Scripts/Effects/Core/ActiveEffect.cs`
- `Assets/Game/Scripts/Effects/Core/EffectHandler.cs`
- `Assets/Game/Scripts/Effects/EffectTrigger.cs`
- `Assets/Game/Scripts/GlobalEnums.cs`

## Responsibilities

- `EffectData` defines effect configuration (`duration`, `tickInterval`, stacking, modifiers, DOT/HOT settings).
- `EffectHandler` manages active effects and tick processing on an entity.
- `EffectTrigger` applies configured effects when a collider enters trigger volume.
- `GlobalEnums` defines effect/stat/damage categories.

## Runtime Wiring

- `EffectHandler` supports explicit `DamageTextManager` assignment through `SetDamageTextManager()`.
- Fallback to singleton exists for backward compatibility, but scene-level injection is preferred.

## Notes for Future Work

- Extend percent-based `StatModifier` handling in `ModifyStat()`.
- Consider splitting visuals from stat logic if effect complexity grows.
