using UnityEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private Transform cameraRig;  // GameObject chứa virtualCam, dùng để di chuyển

    private void Awake()
    {
        Instance = this;
    }

    public async UniTask MoveAndLook(Vector3 targetPosition, Vector3 lookOffset, float moveDuration, float lookDuration)
    {
        cameraRig.gameObject.SetActive(true); // Đảm bảo cameraRig đang hoạt động
        // Tạm ẩn fade nếu đang bật
        bool wasFadeActive = SceneTransitionManager.Instance.fadeCanvas.gameObject.activeSelf;
        if (wasFadeActive)
            SceneTransitionManager.Instance.fadeCanvas.gameObject.SetActive(false);

        if(targetPosition != Vector3.zero)
        {
            await cameraRig.DOMove(targetPosition, moveDuration)
                .SetEase(Ease.InOutSine)
                .AsyncWaitForCompletion();
        }

        // Tính toán các góc quay
        Quaternion originalRot = virtualCam.transform.rotation;
        Quaternion targerRot = Quaternion.Euler(originalRot.eulerAngles + lookOffset);
     
        await virtualCam.transform.DORotateQuaternion(targerRot, lookDuration)
                                  .SetEase(Ease.InOutSine)
                                  .AsyncWaitForCompletion();

  /*      // Quay phải
        await virtualCam.transform.DORotateQuaternion(rightRot, lookDuration * 2f)
                                  .SetEase(Ease.InOutSine)
                                  .AsyncWaitForCompletion();*/

        // Trở về
        await virtualCam.transform.DORotateQuaternion(originalRot, lookDuration)
                                  .SetEase(Ease.InOutSine)
                                  .AsyncWaitForCompletion();

        cameraRig.gameObject.SetActive(false); 
        // Bật lại fade nếu cần
        if (wasFadeActive)
            SceneTransitionManager.Instance.fadeCanvas.gameObject.SetActive(true);
    }

}
