using UnityEngine;
using UnityEngine.InputSystem; // Добавляем пространство имен

public class Interactor : MonoBehaviour
{
    public float interactRange = 2.5f;
    public LayerMask interactableLayer;
    public UI_InventoryDisplay uiDisplay;

    // Этот метод будет вызываться автоматически через Player Input компонент
    // Если в твоем Input Action Asset кнопка называется "Interact"
    public void OnInteract(InputValue value)
    {
        // Проверяем только момент нажатия (isPressed)
        if (value.isPressed)
        {
            Debug.Log("Кнопка E нажата! Пытаюсь открыть инвентарь...");
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        if (uiDisplay.gameObject.activeSelf)
        {
            uiDisplay.gameObject.SetActive(false);
        }
        else
        {
            CheckInteraction();
        }
    }

    void CheckInteraction()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, interactRange, interactableLayer);
        Inventory pInv = GetComponent<Inventory>();

        uiDisplay.gameObject.SetActive(true);

        // Ищем сундук
        if (hit != null && hit.TryGetComponent<Inventory>(out Inventory chestInv))
        {
            uiDisplay.OpenInventory(pInv, chestInv);
        }
        else
        {
            // Открываем только себя
            uiDisplay.OpenInventory(pInv);
        }
    }
    
    // Визуализация радиуса взаимодействия в редакторе (удобно для настройки)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}