using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string SceneName;
    public bool startChangeScene = false;

    private void Update()
    {
        if (startChangeScene)
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}
