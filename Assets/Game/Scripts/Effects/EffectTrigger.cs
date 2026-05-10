using UnityEngine;

public class EffectTrigger : MonoBehaviour
{
    public EffectData effectToApply; // Сюда перетащим наш PoisonEffect

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, есть ли у того, кто вошел в зону, обработчик эффектов
        EffectHandler handler = other.GetComponent<EffectHandler>();

        if (handler != null)
        {
            handler.ApplyEffect(effectToApply);
            Debug.Log("Эффект " + effectToApply.effectName + " наложен на " + other.name);
        }
    }
}