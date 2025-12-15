using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinSwordLogic : MonoBehaviour
{

    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyTest01>().TakeDamage(5);
           
        }
    }
}
