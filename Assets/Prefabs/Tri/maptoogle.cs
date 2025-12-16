using UnityEngine;

public class BigMapToggle : MonoBehaviour
{
    public GameObject bigMapPanel; // Gán BigMapPanel
    private bool isVisible = false;

    void Start()
    {
        bigMapPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isVisible = !isVisible;
            bigMapPanel.SetActive(isVisible);
        }
    }
}
