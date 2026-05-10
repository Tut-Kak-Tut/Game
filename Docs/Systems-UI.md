# UI System

## Files

- `Assets/Game/Scripts/UI/UIController.cs`
- `Assets/Game/Scripts/UI/BuffDisplay.cs`
- `Assets/Game/Scripts/UI/BuffIconUI.cs`
- `Assets/Game/Scripts/UI/DamageTextManager.cs`
- `Assets/Game/Scripts/UI/FloatingText.cs`
- `Assets/Game/Scripts/UI/UI_InventoryDisplay.cs`
- `Assets/Game/Scripts/UI/UI_InventorySlot.cs`

## Responsibilities

- `UIController` updates health/stamina bars from `CharacterStats`.
- Inventory UI scripts render and interact with inventories.
- Buff UI scripts present active effect state.
- `DamageTextManager` spawns and orients floating combat text in world-space UI.

## Runtime Wiring

- `DamageTextManager` now guards duplicate singleton instances and supports explicit camera assignment.
- `SceneRuntimeBootstrap` can inject shared UI services to gameplay systems at scene startup.

## Notes for Future Work

- Replace per-frame bar update with event-driven smoothing targets to reduce redundant updates.
- Standardize panel visibility transitions and avoid repeated full container rebuilds where possible.
