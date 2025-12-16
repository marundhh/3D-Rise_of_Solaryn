using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json; 
public class ApiManager : MonoBehaviour
{
    public static ApiManager Instance;
    public string authToken;

    private const string BASE_URL_AUTH = "https://riseofsolaryn-api.onrender.com/api/auth";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    #region Auth Api
    public void Register(string email, string username, string password, Action<MessageResponse> onSuccess, Action<string> onError)
    {
        var payload = new RegisterPayload(email, username, password);
        string json = JsonUtility.ToJson(payload);
        StartCoroutine(SendPostRequest($"{BASE_URL_AUTH}/register", json, onSuccess, onError));
    }

    public void VerifyEmail(string email, string otp, Action<MessageResponse> onSuccess, Action<string> onError)
    {
        var payload = new VerifyPayload(email, otp);
        string json = JsonUtility.ToJson(payload);
        StartCoroutine(SendPostRequest($"{BASE_URL_AUTH}/verify-email", json, onSuccess, onError));
    }

    public void Login(string username, string password, Action<LoginResponse> onSuccess, Action<string> onError)
    {
        var payload = new LoginPayload(username, password);
        string json = JsonUtility.ToJson(payload);
        StartCoroutine(SendLoginRequest($"{BASE_URL_AUTH}/login", json, onSuccess, onError));
    }

    
    private IEnumerator SendPostRequest(string url, string jsonBody, Action<MessageResponse> onSuccess, Action<string> onError)
    {
        var request = CreateRequest(url, jsonBody);
        yield return request.SendWebRequest();

        if (IsSuccess(request))
        {
            MessageResponse response = JsonUtility.FromJson<MessageResponse>(request.downloadHandler.text);
            onSuccess?.Invoke(response);
        }
        else onError?.Invoke(request.downloadHandler.text);
    }

    // ===== Request Đăng Nhập =====
    private IEnumerator SendLoginRequest(string url, string jsonBody, Action<LoginResponse> onSuccess, Action<string> onError)
    {
        var request = CreateRequest(url, jsonBody);
        yield return request.SendWebRequest();

        if (IsSuccess(request))
        {
            LoginResponse response = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);

            authToken = response.token;

            Debug.Log("Login successful token saved");
            onSuccess?.Invoke(response);
        }
        else
        {
            Debug.Log("Login failed response: " + request.downloadHandler.text);
            onError?.Invoke(request.downloadHandler.text);
        }
    }

    private UnityWebRequest CreateRequest(string url, string jsonBody)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("accept", "*/*");
        return request;
    }
    #endregion Api

    #region Player Save Api
    public void GetPlayerState(Action<PlayerStateResponse> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendGetPlayerStateRequest("https://riseofsolaryn-api.onrender.com/api/PlayerState", onSuccess, onError));
    }

    private IEnumerator SendGetPlayerStateRequest(string url, Action<PlayerStateResponse> onSuccess, Action<string> onError)
    {
        var request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", $"Bearer {authToken}");
        request.SetRequestHeader("accept", "application/json");

        yield return request.SendWebRequest();

        if (IsSuccess(request))
        {
            PlayerStateResponse response = JsonUtility.FromJson<PlayerStateResponse>(request.downloadHandler.text);
            onSuccess?.Invoke(response);
        }
        else
        {
            onError?.Invoke(request.downloadHandler.text);
        }
    }

    public void SavePlayerState(PlayerStateResponse data, Action<SavePlayerStateResponse> onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(data);
        StartCoroutine(SendSavePlayerStateRequest("https://riseofsolaryn-api.onrender.com/api/PlayerState/save", json, onSuccess, onError));
    }

    private IEnumerator SendSavePlayerStateRequest(string url, string jsonBody, Action<SavePlayerStateResponse> onSuccess, Action<string> onError)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("accept", "*/*");
        request.SetRequestHeader("Authorization", $"Bearer {authToken}");

        yield return request.SendWebRequest();

        if (IsSuccess(request))
        {
            SavePlayerStateResponse response = JsonUtility.FromJson<SavePlayerStateResponse>(request.downloadHandler.text);
            onSuccess?.Invoke(response);
        }
        else
        {
            onError?.Invoke(request.downloadHandler.text);
        }
    }
    #endregion

    #region NPC api

    // ==== NPC DIALOGUES ====
    private const string BASE_URL_NPC_DIALOGUE = "https://riseofsolaryn-api.onrender.com/api/save-data/npc-dialogues";

    public void SaveNpcDialogues(List<NpcDialogueEntry> entries, Action<MessageResponse> onSuccess, Action<string> onError)
    {
        var payload = new SaveNpcDialoguePayload(entries);
        string json = JsonUtility.ToJson(payload);
        StartCoroutine(SendAuthorizedPost(BASE_URL_NPC_DIALOGUE, json, onSuccess, onError));
    }

    public void GetNpcDialogues(Action<NpcDialogueListResponse> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendAuthorizedGet<NpcDialogueListResponse>(BASE_URL_NPC_DIALOGUE, onSuccess, onError));
    }

    // ==== RELATIONSHIPS ====
    private const string BASE_URL_RELATIONSHIPS = "https://riseofsolaryn-api.onrender.com/api/save-data/relationships";

    public void SaveRelationships(List<RelationEntry> entries, Action<MessageResponse> onSuccess, Action<string> onError)
    {
        var payload = new SaveRelationshipsPayload(entries);
        string json = JsonUtility.ToJson(payload);
        StartCoroutine(SendAuthorizedPost(BASE_URL_RELATIONSHIPS, json, onSuccess, onError));
    }

    public void GetRelationships(Action<RelationshipListResponse> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendAuthorizedGet<RelationshipListResponse>(BASE_URL_RELATIONSHIPS, onSuccess, onError));
    }

    // ==== MISSIONS ====
    private const string BASE_URL_MISSIONS = "https://riseofsolaryn-api.onrender.com/api/save-data/missions";

    public void SaveMissions(List<MissionSaveData> missions, Action<MessageResponse> onSuccess, Action<string> onError)
    {
        var payload = new SaveMissionsPayload(missions);
        string json = JsonConvert.SerializeObject(payload);
        StartCoroutine(SendAuthorizedPost(BASE_URL_MISSIONS, json, onSuccess, onError));
    }

    public void GetMissions(Action<MissionListResponse> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendAuthorizedGet<MissionListResponse>(BASE_URL_MISSIONS, onSuccess, onError));
    }

    // ==== STORY STATE ====
    private const string BASE_URL_STORY_STATE = "https://riseofsolaryn-api.onrender.com/api/save-data/story-state";

    public void SaveStoryState(StoryStateResponse data, Action<MessageResponse> onSuccess, Action<string> onError)
    {
        string json = JsonUtility.ToJson(data);
        StartCoroutine(SendAuthorizedPost(BASE_URL_STORY_STATE, json, onSuccess, onError));
    }

    public void GetStoryState(Action<StoryStateResponse> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendAuthorizedGet<StoryStateResponse>(BASE_URL_STORY_STATE, onSuccess, onError));
    }

    // ==== CURRENT STORY BLOCK ====
    private const string BASE_URL_CURRENT_STORY_BLOCK = "https://riseofsolaryn-api.onrender.com/api/save-data/current-story-block";

    public void SaveCurrentStoryBlock(string blockId, Action<MessageResponse> onSuccess, Action<string> onError)
    {
        // Server nhận raw string -> bọc thủ công
        string json = "\"" + blockId + "\"";
        StartCoroutine(SendAuthorizedPost(BASE_URL_CURRENT_STORY_BLOCK, json, onSuccess, onError));
    }

    public void GetCurrentStoryBlock(Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(SendAuthorizedGetRawString(BASE_URL_CURRENT_STORY_BLOCK, onSuccess, onError));
    }


    // ==== Generic Helpers ====
    private IEnumerator SendAuthorizedPost(string url, string jsonBody, Action<MessageResponse> onSuccess, Action<string> onError)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {authToken}");
        request.SetRequestHeader("accept", "*/*");

        yield return request.SendWebRequest();

        if (IsSuccess(request))
        {
            MessageResponse response = JsonUtility.FromJson<MessageResponse>(request.downloadHandler.text);
            onSuccess?.Invoke(response);
        }
        else onError?.Invoke(request.downloadHandler.text);
    }

    private IEnumerator SendAuthorizedGet<T>(string url, Action<T> onSuccess, Action<string> onError)
    {
        var request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", $"Bearer {authToken}");
        request.SetRequestHeader("accept", "*/*");
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (IsSuccess(request))
        {
            T response = JsonUtility.FromJson<T>(request.downloadHandler.text);
            onSuccess?.Invoke(response);
        }
        else onError?.Invoke(request.downloadHandler.text);
    }

    private IEnumerator SendAuthorizedGetRawString(string url, Action<string> onSuccess, Action<string> onError)
    {
        var request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", $"Bearer {authToken}");
        request.SetRequestHeader("accept", "*/*");
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();

        if (IsSuccess(request))
        {
            string raw = request.downloadHandler.text.Trim('"');
            onSuccess?.Invoke(raw);
        }
        else onError?.Invoke(request.downloadHandler.text);
    }
    #endregion

    private bool IsSuccess(UnityWebRequest request)
    {
#if UNITY_2020_1_OR_NEWER
        return request.result == UnityWebRequest.Result.Success;
#else
        return !request.isHttpError && !request.isNetworkError;
#endif
    }
}
