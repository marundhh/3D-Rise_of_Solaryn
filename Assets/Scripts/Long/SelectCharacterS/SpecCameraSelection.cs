using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;
public class SpecCameraSelection : MonoBehaviour
{
    public GameObject characterSelection;
    public Button Select;
    public Transform baseTranform;
    public CinemachineVirtualCamera virtualCamera;

    public float zoomSpeed = 5f;
    public float lookAtLerpSpeed = 3f;

    private float targetFOV;
    private Transform targetLookAt;

    private Transform smoothLookAtTarget; // đối tượng tạm dùng để LookAt mượt

    private void Start()
    {
        Select.interactable = false;
        targetFOV = 55f;
        targetLookAt = baseTranform;

        // Tạo GameObject tạm một lần duy nhất
        GameObject tempGO = new GameObject("SmoothLookAt");
        smoothLookAtTarget = tempGO.transform;
        smoothLookAtTarget.position = baseTranform.position;

        virtualCamera.LookAt = smoothLookAtTarget;
    }

    public void ZomeInCharacter(GameObject character)
    {
        Select.interactable = true;
        characterSelection = character;
        targetLookAt = character.transform;
        targetFOV = 30;
    }

    public void ZomeOutCharacter()
    {
        Select.interactable = false;
        targetLookAt = baseTranform;
        targetFOV = 55f;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ZomeOutCharacter();
        }

        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(
            virtualCamera.m_Lens.FieldOfView,
            targetFOV,
            Time.deltaTime * zoomSpeed
        );

        if (targetLookAt != null)
        {
            smoothLookAtTarget.position = Vector3.Lerp(
                smoothLookAtTarget.position,
                targetLookAt.position,
                Time.deltaTime * lookAtLerpSpeed
            );
        }
    }
}
