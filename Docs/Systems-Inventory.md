# Inventory System

## Files

- `Assets/Game/Scripts/Inventory/Inventory.cs`
- `Assets/Game/Scripts/Inventory/ItemData.cs`
- `Assets/Game/Scripts/UI/UI_InventoryDisplay.cs`
- `Assets/Game/Scripts/UI/UI_InventorySlot.cs`

## Responsibilities

- `Inventory` stores slots, performs stack-aware add/remove operations, and signals UI refresh via `onInventoryChanged`.
- `ItemData` defines item metadata and effects through `ScriptableObject`.
- `UI_InventoryDisplay` renders solo and dual-inventory views.
- `UI_InventorySlot` handles click interactions (move item, use consumable).

## Data Flow

1. Gameplay/UI requests item transfer or usage.
2. `Inventory` mutates slot data.
3. UI display refreshes slot visuals from updated `slots`.

## Notes for Future Work

- Keep `InventorySlotData` defined only in `Inventory.cs` to avoid duplicate model definitions.
- Add usage hooks for applying consumable effects directly to `CharacterStats`.
