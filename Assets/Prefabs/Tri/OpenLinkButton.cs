using UnityEngine;

public class OpenLinkButton : MonoBehaviour
{
    // Gán link trong Inspector
    public string url = "https://example.com";

    public void OpenLink()
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
        else
        {
            Debug.LogWarning("URL trống!");
        }
    }
}
