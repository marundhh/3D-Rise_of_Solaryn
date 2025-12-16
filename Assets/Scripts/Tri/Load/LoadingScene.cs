using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public static LoadingScene Instance;
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;
    [SerializeField] private GameObject loadingPanel;
    public string sceneToLoad;

    private void Awake()
    {
       if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartLoading(string sceneName)
    {
        sceneToLoad = sceneName;
        loadingPanel.SetActive(true);
        StartCoroutine(LoadSceneAsync());
    }
    IEnumerator LoadSceneAsync()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("SceneLoader: sceneToLoad chưa được gán!");
            yield break;
        }

        yield return null; // Đợi 1 frame để chắc chắn UI đã sẵn sàng

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        float displayProgress = 0f;

        while (!operation.isDone)
        {
            // Giá trị thực từ AsyncOperation
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Tăng mượt tới targetProgress
            displayProgress = Mathf.MoveTowards(displayProgress, targetProgress, Time.deltaTime);

            // Gán lên UI
            loadingBar.value = displayProgress;
            loadingText.text = Mathf.RoundToInt(displayProgress * 100f) + "%";

            // Khi đã đầy 100%
            if (displayProgress >= 1f && targetProgress >= 1f)
            {
                operation.allowSceneActivation = true;
                yield return new WaitForSeconds(1f); // delay chút cho đẹp
                loadingPanel.SetActive(false);
                yield break; // Kết thúc coroutine
            }

            yield return null;
        }
    }
    public void OnDisableLoadingPanel()
    {
        loadingPanel.SetActive(false);
    }
}
