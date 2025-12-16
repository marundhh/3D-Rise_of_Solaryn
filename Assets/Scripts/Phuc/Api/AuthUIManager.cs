using TMPro;
using UnityEngine;

public class AuthUIManager : MonoBehaviour
{
    [Header("Register Fields")]
    public TMP_InputField registerEmailInput;
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerPasswordInput;

    [Header("OTP Verification")]
    public TMP_InputField otpEmailInput;
    public TMP_InputField otpCodeInput;

    [Header("Login Fields")]
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;

    [Header("Status")]
    public TMP_Text statusText;

    public void OnRegisterClicked()
    {
        statusText.text = "";

        string email = registerEmailInput.text;
        string username = registerUsernameInput.text;
        string password = registerPasswordInput.text;

        ApiManager.Instance.Register(email, username, password,
            onSuccess: (res) => statusText.text = $"{res.message}",
            onError: (err) => statusText.text = $"Registration failed: {err}");
    }

    public void OnVerifyOTPClicked()
    {
        statusText.text = "";

        string email = otpEmailInput.text;
        string otp = otpCodeInput.text;

        ApiManager.Instance.VerifyEmail(email, otp,
            onSuccess: (res) => statusText.text = $"{res.message}",
            onError: (err) => statusText.text = $"OTP is wrong: {err}");
    }

    public void OnLoginClicked()
    {
        statusText.text = "";

        string username = loginUsernameInput.text;
        string password = loginPasswordInput.text;

        ApiManager.Instance.Login(username, password,
            onSuccess: (loginRes) =>
            {
                ApiManager.Instance.GetPlayerState(
                    onSuccess: (playerRes) =>
                    {
                        
                        Debug.Log("Player data loaded");
                        this.gameObject.SetActive(false);
                    },
                    onError: (err) =>
                    {
                        
                        if (err.Contains("No saved game state found"))
                        {
                            this.gameObject.SetActive(false);
                            Debug.Log("Nguoi choi chua co du lieu PlayerState");
                            // TODO: Xử lý tạo dữ liệu mặc định
                        }
                        else
                        {
                            Debug.Log("Load player state error: " + err);
                            statusText.text = "Khong the tai du lieu nguoi choi";
                        }
                    });
            },
            onError: (err) =>
            {
                Debug.Log("Login failed: " + err);
                statusText.text = "Login failed: " + err;
            });
    }

}