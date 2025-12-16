using UnityEngine;

public class MapToggle : MonoBehaviour
{
    public GameObject miniMapPanel;
    public GameObject bigMapPanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            bool isBigMap = !bigMapPanel.activeSelf;
            miniMapPanel.SetActive(!isBigMap);
            bigMapPanel.SetActive(isBigMap);
        }
    }
}
