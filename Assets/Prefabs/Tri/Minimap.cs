using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    /*public Transform Player;
    float y_camera;
    void Start()
    {
        y_camera = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Player.position.x,y_camera,Player.position.z);
        //transform.rotation = Quaternion.Euler(90, Player.eulerAngles.y, 0);
    }*/

    [Header("Player Root")]
    public Transform player;                
    public string playerRootName = "PlayerRoot"; 

    [Header("Camera Options")]
    public bool useOffset = false;        
    public Vector3 offset = new Vector3(0, 67, 0);
    private float y_camera;  

    void Start()
    {
        if (!useOffset)
            y_camera = transform.position.y;

        if (player == null)
        {
            var root = GameObject.Find(playerRootName);
            if (root != null) player = root.transform;
        }
    }

    void LateUpdate()
    {
        if (player == null)
        {
            var root = GameObject.Find(playerRootName);
            if (root != null) player = root.transform;
            return;
        }

        if (useOffset)
        {
            transform.position = player.position + offset;
        }
        else
        {
            transform.position = new Vector3(player.position.x, y_camera, player.position.z);
        }

        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
