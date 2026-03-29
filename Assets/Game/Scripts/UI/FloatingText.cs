using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textElement;
    private float _duration = 1f;
    private float _timer;
    private Color _startColor;

    public void Setup(string value, Color color)
    {
        textElement.text = value;
        textElement.color = color;
        _startColor = color;
        _timer = _duration;

        // Небольшой разброс, чтобы цифры не накладывались
        transform.localPosition += new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
    }

    void Update()
    {
        // Летим вверх в мировых координатах
        transform.position += Vector3.up * 1.5f * Time.deltaTime;

        _timer -= Time.deltaTime;
        float alpha = _timer / _duration;
        textElement.color = new Color(_startColor.r, _startColor.g, _startColor.b, alpha);

        if (_timer <= 0) Destroy(gameObject);
    }
}