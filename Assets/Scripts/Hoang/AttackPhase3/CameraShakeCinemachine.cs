using UnityEngine;
using Cinemachine;

public class CameraShakeCinemachine : MonoBehaviour
{
    public static CameraShakeCinemachine Instance;

    private CinemachineVirtualCamera vCam;
    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;
    private float shakeIntensity;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        vCam = GetComponent<CinemachineVirtualCamera>();
        noise = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if(noise != null) 
            noise.m_AmplitudeGain = 0f; // mặc định không rung
    }

    public void Shake(float intensity, float time)
    {
        shakeIntensity = intensity;
        noise.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            // giảm dần amplitude về 0
            noise.m_AmplitudeGain = Mathf.Lerp(0f, shakeIntensity, shakeTimer / 0.5f);
        }
        else
        {
            noise.m_AmplitudeGain = 0f; // chắc chắn tắt rung
        }
    }
}
