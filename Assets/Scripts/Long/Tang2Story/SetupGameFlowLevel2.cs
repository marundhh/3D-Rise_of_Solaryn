using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

public class SetupGameFlowLevel2 : MonoBehaviour
{
    public Transform playerSpawnPoint;
    public Transform playerRoot;
    public Transform playerPoint;
    void Start()
    {
        Debug.Log("SetupGameFlowLevel2 Start");
        playerRoot = GameObject.FindGameObjectWithTag("Player").transform;
        Debug.Log("playerRootPoint " + playerRoot.name);
        playerPoint = playerRoot.transform.parent;

        playerRoot.position = playerSpawnPoint.position;
        StartCoroutine(TeleportText(playerSpawnPoint));
        

        GameFlowManager.Instance.CallSetupStory("Story2D100");
        PlayerStats.instance.SetupForRespawn();
    }

    IEnumerator TeleportText(Transform transform)
    {

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
