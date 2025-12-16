using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SimpleLoad : MonoBehaviour
{
    public void PlayGame()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("GameScene");

        // Trong khi chưa load xong → chờ
        while (!operation.isDone)
        {
            yield return null;
        }
    }
}
