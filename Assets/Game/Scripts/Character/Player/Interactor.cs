using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [Header("Настройки дистанции")]
    [Tooltip("Расстояние, на котором можно ОТКРЫТЬ сундук")]
    public float interactRange = 2.5f;
    
    [Tooltip("Расстояние, при котором сундук АВТОМАТИЧЕСКИ ЗАКРОЕТСЯ")]
    public float closeRange = 4.0f;

    [Header("Ссылки")]
    public LayerMask interactableLayer;
    public UI_InventoryDisplay uiDisplay;

    private Inventory currentOpenedChest;

    void Update()
    {
        // Проверяем авто-закрытие только если сундук открыт
        if (currentOpenedChest != null)
        {
            float distance = Vector2.Distance(transform.position, currentOpenedChest.transform.position);
            
            // Используем closeRange для закрытия
            if (distance > closeRange)
            {
                CloseInventory();
            }
        }
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        // Если инвентарь уже открыт, нажатие "E" всегда его закрывает
        if (uiDisplay.soloPanel.activeSelf || uiDisplay.dualPanel.activeSelf)
        {
            CloseInventory();
            return;
        }

        // Для открытия используем interactRange
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, interactableLayer);
        Inventory pInv = GetComponent<Inventory>();

        if (hit != null && hit.TryGetComponent<Inventory>(out Inventory chestInv))
        {
            currentOpenedChest = chestInv;
            uiDisplay.OpenDualInventory(pInv, chestInv);
        }
        else
        {
            currentOpenedChest = null;
            uiDisplay.OpenSoloInventory(pInv);
        }
    }

    private void CloseInventory()
    {
        uiDisplay.CloseAll();
        currentOpenedChest = null;
    }

    // Визуализация в редакторе для удобной настройки
    private void OnDrawGizmosSelected()
    {
        // Желтый круг - зона взаимодействия (открытие)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);

        // Красный круг - зона разрыва (закрытие)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, closeRange);
    }
}