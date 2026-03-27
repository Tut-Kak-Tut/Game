using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amountText;

    private InventorySlotData data;
    private Inventory myInventory;
    private Inventory targetInventory;
    private UI_InventoryDisplay display;

    public void Setup(InventorySlotData slotData, Inventory me, Inventory other, UI_InventoryDisplay disp)
    {
        data = slotData;
        myInventory = me;
        targetInventory = other;
        display = disp;

        if (data.item != null)
        {
            icon.sprite = data.item.icon;
            icon.enabled = true;
            amountText.text = data.amount > 1 ? data.amount.ToString() : "";
        }
        else
        {
            icon.enabled = false;
            amountText.text = "";
        }
    }

    // Вызывается при клике на кнопку ячейки
    public void TransferItem()
    {
        if (data.item == null || targetInventory == null) return;

        // Пытаемся добавить предмет в "соседний" инвентарь
        if (targetInventory.AddItem(data.item, data.amount))
        {
            // Если получилось — удаляем из текущего
            myInventory.RemoveItem(data);
            display.RefreshUI();
        }
        else
        {
            Debug.Log("Нет места в целевом инвентаре!");
        }
    }
}