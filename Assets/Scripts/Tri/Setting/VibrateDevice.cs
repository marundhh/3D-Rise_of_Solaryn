using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrateDevice : MonoBehaviour
{
    public void OnClickVibrate()
    {
        if (SettingMenuManager.isVibrate)
        {
           Handheld.Vibrate();
           Debug.Log("vibrate hoatdong");

        }
       
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
