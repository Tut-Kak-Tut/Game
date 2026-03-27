using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public string inventoryName = "Инвентарь";
    public int slotCount = 20;
    public List<InventorySlotData> slots = new List<InventorySlotData>();

    public delegate void OnInventoryChanged();
    public OnInventoryChanged onInventoryChanged;

    void Awake()
    {
        // Инициализируем пустые слоты, если список пуст
        if (slots.Count == 0)
        {
            for (int i = 0; i < slotCount; i++)
                slots.Add(new InventorySlotData());
        }
    }

    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (slot.item == item && slot.amount < item.maxStackSize)
                {
                    slot.amount += amount;
                    onInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        foreach (var slot in slots)
        {
            if (slot.item == null)
            {
                slot.item = item;
                slot.amount = amount;
                onInventoryChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    // Метод для удаления предмета (пригодится для крафта или выбрасывания)
    public void RemoveItem(InventorySlotData slot)
    {
        slot.item = null;
        slot.amount = 0;
        onInventoryChanged?.Invoke();
    }
}

[System.Serializable]
public class InventorySlotData
{
    public ItemData item;
    public int amount;
}