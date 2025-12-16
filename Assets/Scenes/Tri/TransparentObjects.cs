using UnityEngine;
using System.Collections.Generic;

public class TransparentObstacle : MonoBehaviour
{
    public Transform Camera;
    public Transform player;
    public LayerMask obstacleLayer; // Layer của các vật cản

    private List<Renderer> currentObstacles = new List<Renderer>();
    public Material transparentMaterial; // Gắn material URP Transparent vào đây
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

    private void Start()
    {
       
    }
    void Update()
    {
        if(Camera == null || player == null)
        {
            Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
            return;
        }
        Vector3 direction = player.position - Camera.position;
        float distance = Vector3.Distance(player.position, Camera.position);
        RaycastHit hit;
        
        if(Physics.Raycast(Camera.position, direction, out hit, distance, obstacleLayer))
        {
           // Debug.DrawRay(Camera.position, direction, Color.red, 3f);
          //  Debug.Log($"🔍 Phát hiện vật cản: {hit.transform.name} tại khoảng cách {distance}");
            Renderer[] rends = hit.transform.parent.GetComponentsInChildren<Renderer>();
            foreach(var rend in rends)
            {
                    if (!originalMaterials.ContainsKey(rend))
                    {
                        originalMaterials[rend] = rend.materials;
                    }

                    // Gán material trong suốt
                    Material[] transparentMats = new Material[rend.materials.Length];
                    for (int i = 0; i < transparentMats.Length; i++)
                    {
                        transparentMats[i] = transparentMaterial;
                    }
                    rend.materials = transparentMats;

                    currentObstacles.Add(rend);
            }
            
        }
        else
        {
            MakeAllVisible();
        }

    }

    void MakeAllVisible()
    {
        foreach (Renderer rend in currentObstacles)
        {
            if (rend != null && originalMaterials.ContainsKey(rend))
            {
                rend.materials = originalMaterials[rend];
            }
        }

        currentObstacles.Clear();
    }
}
