using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoPlayerController : MonoBehaviour
{
    public static VideoPlayerController Instance;
    public VideoPlayer videoPlayer;
    public GameObject Cutscenepanel;
    public string videoName ;        // Tên file video trong StreamingAssets
    public Button skipButton;                      // Gắn UI Button để Skip
    public bool forceReplay = false; 
    public bool hasPlayedThisRun = false; // Đánh dấu đã chơi video này trong lần chạy hiện tại    
    public GameObject Camera; // Giao diện video player, nếu có

    private void Awake()
    {
       /* if(Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Giữ VideoPlayerController khi chuyển cảnh
        }
        else
        {
            Destroy(gameObject); // Nếu đã có instance, hủy đối tượng này
        }*/
    }
    private void Start()
    {
        PlayCutScene(videoName);
    }
    public void PlayCutScene(string videoName)
    {
        this.videoName = videoName;

        if (!forceReplay && !PlayerData.instance.isNewGame && !hasPlayedThisRun)
        {
            gameObject.SetActive(false);
            Cutscenepanel.SetActive(false);
            GameStateManager.Instance.StartGame();
            Invoke(nameof(DisableStartCamme), 2f); // Tắt camera sau 0.1 giây
            return;
        }

        Cutscenepanel.SetActive(true); 
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, videoName);
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = path;

        videoPlayer.isLooping = false;
        videoPlayer.Play();
        AudioManager.Instance?.StopBGM(); // Dừng nhạc nền khi phát video

        videoPlayer.loopPointReached += OnVideoFinished; // Gọi khi video kết thúc

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipVideo);
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        EndVideo();
    }

    public void SkipVideo()
    {
        videoPlayer.Stop();
        EndVideo();
    }

    void EndVideo()
    { 
        PlayerPrefs.Save();
        gameObject.SetActive(false);               // Tắt toàn bộ video UI
        if (!AudioManager.Instance)
        {
            AudioManager.Instance?.StartNormalBGMLoop(); // Bắt đầu lại nhạc nền
        }
        Cutscenepanel.SetActive(false); // Tắt panel cutscene
        hasPlayedThisRun = true;        // Đánh dấu đã chơi video này trong lần chạy hiện tại
        GameStateManager.Instance.StartGame();
        Invoke(nameof(DisableStartCamme), 2f); // Tắt camera sau 0.1 giây

    }

    public void DisableStartCamme()
    {
        Camera.SetActive(false); // Tắt camera nếu không cần thiết
    }
}
