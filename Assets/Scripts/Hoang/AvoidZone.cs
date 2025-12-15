using UnityEngine;

public class AvoidZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            AI_Movement ai = other.GetComponent<AI_Movement>();
            if (ai != null)
            {
                ai.TurnAroundRandom(); // Quay đầu lại khi vào vùng
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            AI_Movement ai = other.GetComponent<AI_Movement>();
            if (ai != null)
            {
                Vector3 center = GetComponent<Collider>().bounds.center;
                ai.ReturnTo(center); // Quay về giữa vùng
            }
        }
    }

}
