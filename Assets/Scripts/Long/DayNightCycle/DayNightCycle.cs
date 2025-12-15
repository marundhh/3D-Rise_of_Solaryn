using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float oneDaytoSecond;
    public float oneMinuteto;
   public float seconds = 0f;
   public float temp;


   
    void Update()
    {
        temp += Time.deltaTime / oneDaytoSecond;
        temp = temp % 1;
        transform.localRotation = Quaternion.Euler(temp * 360, 0, 0);

    }
}
