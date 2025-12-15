using UnityEngine;


public class UIBillboarding : MonoBehaviour
{
    private Camera cam;
    private void Awake()
    {
        cam = Camera.main;
    }
    void Update()
    {
        transform.forward = cam.transform.forward;
    }
}
