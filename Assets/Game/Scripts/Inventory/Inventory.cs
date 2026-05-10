using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public string inventoryName = "Inventory";
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
        // ШАГ 1: Если предмет стакаемый, пытаемся добавить в существующие кучки
        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                // Если в слоте тот же предмет И он еще не заполнен до максимума
                if (slot.item == item && slot.amount < item.maxStackSize)
                {
                    int roomLeft = item.maxStackSize - slot.amount; // Сколько места осталось в слоте
                    
                    if (amount <= roomLeft)
                    {
                        // Если всё помещается в этот слот
                        slot.amount += amount;
                        onInventoryChanged?.Invoke();
                        return true;
                    }
                    else
                    {
                        // Если часть помещается, заполняем слот до конца и продолжаем искать для остатка
                        slot.amount = item.maxStackSize;
                        amount -= roomLeft;
                        // Не выходим из цикла, ищем следующий подходящий слот для "остатка"
                    }
                }
            }
        }

        // ШАГ 2: Если остались предметы, которые не влезли в стаки (или предмет не стакается)
        // Ищем пустые слоты, пока не распределим всё количество
        while (amount > 0)
        {
            InventorySlotData emptySlot = slots.Find(s => s.item == null);

            if (emptySlot != null)
            {
                emptySlot.item = item;
                
                // Если количество больше макс. стака, кладем только макс. стак и идем на следующий круг
                int amountToAdd = Mathf.Min(amount, item.maxStackSize);
                emptySlot.amount = amountToAdd;
                amount -= amountToAdd;
            }
            else
            {
                // Если пустых слотов больше нет, а предметы остались
                onInventoryChanged?.Invoke();
                return false; // Инвентарь переполнен
            }
        }

        onInventoryChanged?.Invoke();
        return true;
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