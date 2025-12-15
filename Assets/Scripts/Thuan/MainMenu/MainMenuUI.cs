using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene Settings")]
    public string scenePlay ;

    [Header("Scene Setting")]
    public string sceneNew;

    public void OnClickPlay()
    {
        if (!string.IsNullOrEmpty(scenePlay))
        {
            SceneManager.LoadScene(scenePlay);
        }
        else
        {
            Debug.LogWarning("Scene chưa được đặt!");
        }
    }

    public void OnClickNew()
    {
            SceneManager.LoadScene(sceneNew);
    }

    public void OnClickExit()
    {
        Application.Quit();
        Debug.Log("Thoát Game");
    }
}
