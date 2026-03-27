using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI amountText;

    private InventorySlotData slotData;
    private Inventory myInventory;
    private Inventory targetInventory;
    private UI_InventoryDisplay display;

    // Этот метод вызывается из UI_InventoryDisplay при отрисовке
    public void Setup(InventorySlotData data, Inventory me, Inventory other, UI_InventoryDisplay disp)
    {
        slotData = data;
        myInventory = me;
        targetInventory = other; // Это инвентарь "напротив" (если открыт сундук)
        display = disp;

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (slotData != null && slotData.item != null)
        {
            icon.sprite = slotData.item.icon;
            icon.enabled = true;
            amountText.text = slotData.amount > 1 ? slotData.amount.ToString() : "";
        }
        else
        {
            icon.enabled = false;
            amountText.text = "";
        }
    }

    // ВАЖНО: Привяжи этот метод к событию OnClick компонента Button на префабе слота
    public void OnSlotClick()
    {
        // 1. Если ячейка пустая — ничего не делаем
        if (slotData == null || slotData.item == null) return;

        // 2. Если второй инвентарь (например, сундук) не открыт — ничего не делаем
        // (Или можно добавить логику "Использовать/Съесть", если открыт только рюкзак)
        if (targetInventory == null) 
        {
            Debug.Log("Второй инвентарь не открыт. Некуда перекладывать.");
            return;
        }

        // 3. Пытаемся переместить предмет
        MoveItem();
    }

    private void MoveItem()
    {
        // Пытаемся добавить предмет в целевой инвентарь
        bool success = targetInventory.AddItem(slotData.item, slotData.amount);

        if (success)
        {
            // Если предмет успешно ушел — очищаем текущую ячейку
            slotData.item = null;
            slotData.amount = 0;

            // Обновляем весь интерфейс, чтобы изменения были видны везде
            display.RefreshUI();
        }
        else
        {
            Debug.Log("Нет места в другом инвентаре!");
        }
    }
}