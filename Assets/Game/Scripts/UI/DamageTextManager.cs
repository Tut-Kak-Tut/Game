using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance { get; private set; }

    [SerializeField] private FloatingText prefab;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private Camera worldCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate DamageTextManager detected. Destroying duplicate.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (worldCamera == null)
            worldCamera = Camera.main;
    }

    public void SpawnText(Vector3 worldPos, string text, Color color)
    {
        if (prefab == null || canvasTransform == null)
            return;

        // Создаем префаб прямо в позиции существа
        FloatingText instance = Instantiate(prefab, worldPos, Quaternion.identity);
        
        // Привязываем к Canvas, НО сохраняем позицию в мире (true)
        instance.transform.SetParent(canvasTransform, true);
        
        // Поворачиваем текст к камере, чтобы он не был задом наперед
        if (worldCamera != null)
            instance.transform.rotation = Quaternion.LookRotation(instance.transform.position - worldCamera.transform.position);
        // Если текст все еще зеркальный, используй это вместо строки выше:
        // instance.transform.rotation = Camera.main.transform.rotation;

        instance.Setup(text, color);
    }
}