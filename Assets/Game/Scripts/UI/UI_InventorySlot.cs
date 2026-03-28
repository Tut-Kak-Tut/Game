using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Важно: нужно для распознавания ПКМ

public class UI_InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public TextMeshProUGUI amountText;

    private InventorySlotData slotData;
    private Inventory myInventory;
    private Inventory targetInventory;
    private UI_InventoryDisplay display;

    public void Setup(InventorySlotData data, Inventory me, Inventory other, UI_InventoryDisplay disp)
    {
        slotData = data;
        myInventory = me;
        targetInventory = other;
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

    // Этот метод ловит клики мышки автоматически (нужен компонент Button или Image с Raycast Target)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (slotData == null || slotData.item == null) return;

        // ЛЕВАЯ КНОПКА: Перекладываем в другой инвентарь
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (targetInventory != null)
            {
                MoveItem();
            }
        }
        // ПРАВАЯ КНОПКА: Используем предмет (едим/пьем)
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            UseItem();
        }
    }

    private void MoveItem()
    {
        if (targetInventory.AddItem(slotData.item, slotData.amount))
        {
            slotData.item = null;
            slotData.amount = 0;
            display.RefreshUI();
        }
        else
        {
            Debug.Log("Нет места в другом инвентаре!");
        }
    }

    private void UseItem()
    {
        ItemData item = slotData.item;

        // Проверяем, можно ли это вообще использовать
        if (item.itemType == ItemType.Consumable)
        {
            Debug.Log($"Использовано: {item.itemName}. Эффект здоровья: {item.healthEffect}");
            
            
            // Уменьшаем количество
            slotData.amount--;

            // Если предметы закончились - очищаем слот
            if (slotData.amount <= 0)
            {
                slotData.item = null;
            }

            display.RefreshUI();
        }
        else
        {
            Debug.Log("Этот предмет нельзя использовать!");
        }
    }

    // Оставляем этот метод для старой привязки через Button Component, если нужно
    public void OnSlotClick()
    {
        OnPointerClick(new PointerEventData(EventSystem.current) { button = PointerEventData.InputButton.Left });
    }
}