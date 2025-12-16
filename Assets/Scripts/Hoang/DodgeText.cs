using UnityEngine;
using TMPro;

public class DodgeText : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public float floatSpeed = 1.5f;
    public float fadeDuration = 1.0f;
    public Vector3 floatDirection = Vector3.up;

    private Color originalColor;
    private float timer = 0f;

    private void Awake()
    {
        if (textMesh == null)
            textMesh = GetComponentInChildren<TextMeshProUGUI>();

        originalColor = textMesh.color;
    }

    public void Setup()
    {
        textMesh.text = "Avoid";
        textMesh.color = Color.cyan; // Bạn có thể thay màu ở đây
        timer = 0f;
    }

    private void Update()
    {
        transform.position += floatDirection * floatSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(originalColor.a, 0, timer / fadeDuration);
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}
