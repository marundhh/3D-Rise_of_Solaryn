using UnityEngine;
using TMPro;

public class UpdateUICoin : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

    private void OnEnable()
    {
        PlayerData.instance.OnCoinChanged += UpdateCoinDisplay;
        UpdateCoinDisplay(PlayerData.instance.coin);
    }

    private void OnDisable()
    {
        if (PlayerData.instance != null)
        {
            PlayerData.instance.OnCoinChanged -= UpdateCoinDisplay;
        }
    }

    private void UpdateCoinDisplay(int newCoinValue)
    {
        coinText.text = newCoinValue.ToString();
    }

    public void ButtonWeb()
    {
        Debug.Log("Open Link");
       
        Application.OpenURL("https://riseofsolaryn.onrender.com/payment.html");
    }
}