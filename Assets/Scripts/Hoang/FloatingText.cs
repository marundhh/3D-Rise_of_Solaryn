using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;        // Gán trong Inspector
    public float floatSpeed = 1.5f;          // Tốc độ bay lên
    public float fadeDuration = 1.0f;        // Thời gian mờ dần
    public Vector3 floatDirection = Vector3.up;  // Hướng bay

    private Color originalColor;
    private float timer = 0f;

    private void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponentInChildren<TextMeshProUGUI>();

        originalColor = textMesh.color;
    }

    public void Setup(string displayText)
    {
        textMesh.text = displayText;
        timer = 0f;
        originalColor = textMesh.color;
    }

    private void Update()
    {
        // Di chuyển bay lên
        transform.position += floatDirection * floatSpeed * Time.deltaTime;

        // Mờ dần
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(originalColor.a, 0, timer / fadeDuration);
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        // Xoá object
        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}
