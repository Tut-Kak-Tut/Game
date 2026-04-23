using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTests
{
    [Test]
    public void AddItem_StacksIntoExistingSlot_WhenPossible()
    {
        GameObject go = new GameObject("inventory");
        Inventory inventory = go.AddComponent<Inventory>();
        inventory.slots = new List<InventorySlotData>
        {
            new InventorySlotData(),
            new InventorySlotData()
        };

        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.isStackable = true;
        item.maxStackSize = 20;

        inventory.AddItem(item, 5);
        inventory.AddItem(item, 4);

        Assert.AreEqual(item, inventory.slots[0].item);
        Assert.AreEqual(9, inventory.slots[0].amount);

        Object.DestroyImmediate(item);
        Object.DestroyImmediate(go);
    }

    [Test]
    public void AddItem_ReturnsFalse_WhenNoEmptySlotsRemain()
    {
        GameObject go = new GameObject("inventory");
        Inventory inventory = go.AddComponent<Inventory>();
        inventory.slots = new List<InventorySlotData>
        {
            new InventorySlotData(),
            new InventorySlotData()
        };

        ItemData nonStackable = ScriptableObject.CreateInstance<ItemData>();
        nonStackable.isStackable = false;
        nonStackable.maxStackSize = 1;

        Assert.IsTrue(inventory.AddItem(nonStackable, 1));
        Assert.IsTrue(inventory.AddItem(nonStackable, 1));
        Assert.IsFalse(inventory.AddItem(nonStackable, 1));

        Object.DestroyImmediate(nonStackable);
        Object.DestroyImmediate(go);
    }
}
