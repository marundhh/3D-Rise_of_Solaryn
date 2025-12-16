using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.SceneManagement;

public class CheatMenu : MonoBehaviour
{
    public List<Transform> positionCheckPoints = new List<Transform>();
    public Transform shoppe;

    public GameObject cheatPanel;

    public Transform playerRoot;
    private void Start()
    {
        playerRoot = GameObject.FindGameObjectWithTag("Player").transform;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            cheatPanel.SetActive(!cheatPanel.activeSelf);
        }
    }
    public void SetAllCurrentMissionIsComplete()
    {
        foreach (var mission in MissionManager.Instance.GetAllMissions())
        {
            if(mission.IsCompleted) continue; 
            mission.OnMissionComplete();
        }

    }
    public void AddEXP()
    {
        PlayerLevel.instance.GainExp(10000);
    }
    public void AddGold()
    {
        PlayerData.instance.AddCoin(10000);
    }
    public void ClearAllCurrentMission()
    {
        MissionManager.Instance.ClearAllMions();
    }

    public void ChangeToSceneLevel2()
    {
         LoadingScene.Instance.StartLoading("Tang3");
    }
    public void GotoShoppe()
    {
        StartCoroutine(TeleportText(shoppe));
    }
    public void CheckPoint0()
    {
        GameFlowManager.Instance.CallSetupStory("CP0");
    }
    public void CheckPoint1()
    {
        GameFlowManager.Instance.CallSetupStory("CP1");
        StartCoroutine(Teleport(0));
    }
    public void CheckPoint2()
    {
        StartCoroutine(Teleport(1));
    }
    public void CheckPoint3()
    {
        GameFlowManager.Instance.CallSetupStory("CP3");
        StartCoroutine(Teleport(2));
    }
    public void CheckPoint4()
    {
        StartCoroutine(Teleport(3));
    }


    void CacheRefs()
    {
        if (playerRoot == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            playerRoot = go ? go.transform : null;        // child
        }
    }
    IEnumerator Teleport(int index)
    {
        CacheRefs();

        // 1) TẮT điều khiển chuyển động/physics để tránh ghi đè
        var move = playerRoot ? playerRoot.GetComponent<PlayerMovement>() : null;
        var cc = playerRoot ? playerRoot.GetComponent<CharacterController>() : null;
         var rb = (playerRoot ? playerRoot.GetComponent<Rigidbody>() : null) ?? (playerRoot ? playerRoot.GetComponent<Rigidbody>() : null);
        var anim = playerRoot ? playerRoot.GetComponentInChildren<Animator>() : null;

        if (move) move.enabled = false;
        if (cc) cc.enabled = false;
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // tạm thời để teleport sạch
        }
        if (anim) anim.applyRootMotion = false;


        if (playerRoot) { playerRoot.position = positionCheckPoints[index].position; playerRoot.localRotation = Quaternion.identity; }

        Physics.SyncTransforms(); // đồng bộ ngay lập tức
        yield return null;        // đợi qua 1 frame, tránh script khác ghi đè trong cùng frame

        // 3) BẬT LẠI theo thứ tự
        if (rb) rb.isKinematic = false;
        if (cc) cc.enabled = true;
        if (move) move.enabled = true;
        if (anim) anim.applyRootMotion = true;
    }
    IEnumerator TeleportText(Transform transform)
    {
        CacheRefs();

        // 1) TẮT điều khiển chuyển động/physics để tránh ghi đè
        var move = playerRoot ? playerRoot.GetComponent<PlayerMovement>() : null;
        var cc = playerRoot ? playerRoot.GetComponent<CharacterController>() : null;
        var rb = (playerRoot ? playerRoot.GetComponent<Rigidbody>() : null) ?? (playerRoot ? playerRoot.GetComponent<Rigidbody>() : null);
        var anim = playerRoot ? playerRoot.GetComponentInChildren<Animator>() : null;

        if (move) move.enabled = false;
        if (cc) cc.enabled = false;
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true; // tạm thời để teleport sạch
        }
        if (anim) anim.applyRootMotion = false;


        if (playerRoot) { playerRoot.position = transform.position; playerRoot.localRotation = Quaternion.identity; }

        Physics.SyncTransforms(); // đồng bộ ngay lập tức
        yield return null;        // đợi qua 1 frame, tránh script khác ghi đè trong cùng frame

        // 3) BẬT LẠI theo thứ tự
        if (rb) rb.isKinematic = false;
        if (cc) cc.enabled = true;
        if (move) move.enabled = true;
        if (anim) anim.applyRootMotion = true;
    }

}
