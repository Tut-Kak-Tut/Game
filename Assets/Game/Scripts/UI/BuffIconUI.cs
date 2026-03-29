using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffIconUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI timeText;

    public void UpdateIcon(Sprite icon, float remainingTime)
    {
        iconImage.sprite = icon;
        // Показываем время, если оно больше 0.1 сек
        timeText.text = remainingTime > 0.1f ? Mathf.CeilToInt(remainingTime).ToString() : "";
    }
}