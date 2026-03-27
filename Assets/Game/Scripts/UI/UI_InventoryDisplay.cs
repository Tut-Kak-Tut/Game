using UnityEngine;
using TMPro;

public class UI_InventoryDisplay : MonoBehaviour
{
    [Header("Player Side")]
    public Transform playerContainer;
    public TextMeshProUGUI playerTitle;

    [Header("External Side (Chest)")]
    public GameObject externalPanel; 
    public Transform externalContainer;
    public TextMeshProUGUI externalTitle;

    [Header("Prefabs")]
    public GameObject slotPrefab;

    private Inventory playerInv;
    private Inventory chestInv;

    // Открытие рюкзака
    public void OpenInventory(Inventory pInv, Inventory cInv = null)
    {
        playerInv = pInv;
        chestInv = cInv;

        playerTitle.text = playerInv.inventoryName;

        if (chestInv != null)
        {
            externalPanel.SetActive(true);
            externalTitle.text = chestInv.inventoryName;
        }
        else
        {
            externalPanel.SetActive(false);
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        ClearContainer(playerContainer);
        ClearContainer(externalContainer);

        DrawSlots(playerInv, playerContainer, chestInv); // Передаем соседа
        if (externalPanel.activeSelf && chestInv != null)
        {
            DrawSlots(chestInv, externalContainer, playerInv); // Сосед - игрок
        }
    }

    private void DrawSlots(Inventory currentInv, Transform container, Inventory otherInv)
    {
        foreach (var slotData in currentInv.slots)
        {
            GameObject obj = Instantiate(slotPrefab, container);
            // Setup теперь принимает: данные слота, текущий инвентарь и "соседний"
            obj.GetComponent<UI_InventorySlot>().Setup(slotData, currentInv, otherInv, this);
        }
    }

    private void ClearContainer(Transform container)
    {
        foreach (Transform child in container) Destroy(child.gameObject);
    }
}