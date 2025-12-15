using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTest01 : MonoBehaviour
{
    public float Health;
  public void TakeDamage(float damage)
  {
        Health -= damage;
        Debug.Log($"Enmey -{damage}HP");
        Debug.Log($"Enmey {Health}HP");
    }
}
