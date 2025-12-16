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
    public void ChangeToScene()
    {
       StartCoroutine(ChangeToSceneDelayed());
    }
    public IEnumerator ChangeToSceneDelayed()
    {
        yield return new WaitForSeconds(1f);
        LoadingScene.Instance.StartLoading(SceneName);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeToScene();
        }
    }
}
