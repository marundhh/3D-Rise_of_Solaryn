using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReachLocationMissionChecker : MonoBehaviour
{
   public string m_ReachedLocation;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameEventSystem.Dispatch( new ReachedLocationEvent(m_ReachedLocation));
        }
    }
}
