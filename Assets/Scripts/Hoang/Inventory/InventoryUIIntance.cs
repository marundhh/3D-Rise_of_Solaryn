using UnityEngine;

public class InventoryUIIntance : MonoBehaviour
{
    public static InventoryUIIntance instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
