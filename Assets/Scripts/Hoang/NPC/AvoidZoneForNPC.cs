using UnityEngine;

public class AvoidZoneForNPC : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPCWalk"))
        {
            NPCWalk npc = other.GetComponent<NPCWalk>();
            if (npc != null)
            {
                npc.TurnAroundRandom(); // Quay đầu lại khi vào vùng
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPCWalk"))
        {
            NPCWalk npc = other.GetComponent<NPCWalk>();
            if (npc != null)
            {
                Vector3 center = GetComponent<Collider>().bounds.center;
                npc.ReturnTo(center); // Quay về giữa vùng
            }
        }
    }
}
