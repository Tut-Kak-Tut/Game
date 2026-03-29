using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;

    [SerializeField] private FloatingText prefab;
    [SerializeField] private Transform canvasTransform;

    void Awake() => Instance = this;

    public void SpawnText(Vector3 worldPos, string text, Color color)
    {
        // Создаем префаб прямо в позиции существа
        FloatingText instance = Instantiate(prefab, worldPos, Quaternion.identity);
        
        // Привязываем к Canvas, НО сохраняем позицию в мире (true)
        instance.transform.SetParent(canvasTransform, true);
        
        // Поворачиваем текст к камере, чтобы он не был задом наперед
        instance.transform.rotation = Quaternion.LookRotation(instance.transform.position - Camera.main.transform.position);
        // Если текст все еще зеркальный, используй это вместо строки выше:
        // instance.transform.rotation = Camera.main.transform.rotation;

        instance.Setup(text, color);
    }
}