using UnityEngine;
using TMPro;

public class UI_InventoryDisplay : MonoBehaviour
{
    [Header("Вариант 1: Одиночный (Только игрок)")]
    public GameObject soloPanel;
    public Transform soloPlayerContainer;

    [Header("Вариант 2: Двойной (Игрок + Сундук)")]
    public GameObject dualPanel;
    public Transform dualPlayerContainer;
    public Transform dualExternalContainer;
    public TextMeshProUGUI externalTitle;

    [Header("Общие настройки")]
    public GameObject slotPrefab;

    private Inventory playerInv;
    private Inventory currentExternalInv;

    // Метод 1: Открыть только рюкзак
    public void OpenSoloInventory(Inventory pInv)
    {
        playerInv = pInv;
        currentExternalInv = null; // Сундука нет

        soloPanel.SetActive(true);
        dualPanel.SetActive(false);
        
        RefreshUI();
    }

    // Метод 2: Открыть вместе с сундуком
    public void OpenDualInventory(Inventory pInv, Inventory eInv)
    {
        playerInv = pInv;
        currentExternalInv = eInv;

        soloPanel.SetActive(false);
        dualPanel.SetActive(true);

        if (externalTitle != null) externalTitle.text = eInv.inventoryName;

        RefreshUI();
    }

    public void CloseAll()
    {
        soloPanel.SetActive(false);
        dualPanel.SetActive(false);
    }

    public void RefreshUI()
    {
        // Очищаем всё
        ClearContainer(soloPlayerContainer);
        ClearContainer(dualPlayerContainer);
        ClearContainer(dualExternalContainer);

        // Если открыта одиночная панель
        if (soloPanel.activeSelf)
        {
            DrawSlots(playerInv, soloPlayerContainer, null);
        }
        // Если открыта двойная панель
        else if (dualPanel.activeSelf)
        {
            DrawSlots(playerInv, dualPlayerContainer, currentExternalInv);
            DrawSlots(currentExternalInv, dualExternalContainer, playerInv);
        }
    }

    private void DrawSlots(Inventory inv, Transform container, Inventory other)
    {
        if (inv == null || container == null) return;
        
        foreach (var slotData in inv.slots)
        {
            GameObject obj = Instantiate(slotPrefab, container);
            obj.GetComponent<UI_InventorySlot>().Setup(slotData, inv, other, this);
        }
    }

    private void ClearContainer(Transform container)
    {
        if (container == null) return;
        foreach (Transform child in container) Destroy(child.gameObject);
    }
}