using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Collections;
using System.Threading.Tasks;
using Cinemachine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    #region Camera Settings
    [Header("Camera Settings")]
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera focusCamera;
    public float focusFOV;
    public float transitionTime;
    public float cameraDistance;
    public float cameraHeight;
    public LayerMask obstacleMask;

    private float originalFOV;
    private Transform npcTarget;
    #endregion

    #region UI References
    [Header("UI References")]
    public GameObject dialoguePanel;
    public GameObject hudPanel;
    public TMP_Text dialogueText;
    public TMP_Text npcNameText;
    public Transform choicesContainer;
    public Button choiceButtonPrefab;
    public GameObject noityCanInteract;
    #endregion

    #region Dialogue State
    private Coroutine typingCoroutine;
    private DialogueBlock currentBlock;
    private List<string> currentLines;
    private List<DialogueChoice> currentChoices;
    private string currentNPC;
    private string currentStoryBlockID;
    private int currentLineIndex = -1;

    private bool isTyping = false;
    private bool isAnimating = false;
    private bool awaitingChoice = false;
    private bool inputLocked = false;
    private bool onTalking = false;
    private bool onSetupStory = false;
    private bool hasSelectedChoice = false;
    private bool playNextDialogueAutomatically = false;
    private string openMenuID = string.Empty;

    private bool readyForNextLine = false; // NEW: đồng bộ hiển thị từng dòng
    private HashSet<string> playedDialogues = new();
    #endregion

    #region Singleton
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }
    #endregion

    #region Update Loop
    private void Update()
    {
        // Nếu đang nói mà panel bị tắt (do animator/logic khác), đóng an toàn
        if (onTalking && dialoguePanel != null && !dialoguePanel.activeSelf)
        {
            CloseDialogueWithReset();
            return;
        }

        if (dialoguePanel == null || !dialoguePanel.activeSelf) return;
        if (onSetupStory || isAnimating || inputLocked || awaitingChoice) return;

        // Click để lật câu
        if (Input.GetMouseButtonDown(0))
        {
            ShowNextLine();
        }
    }
    #endregion

    #region Interact Notify
    public void OpenCanInteract(string npcName)
    {
        if (noityCanInteract == null) return;
        noityCanInteract.SetActive(true);
        var tmp = noityCanInteract.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = $"Press F to Interact {npcName}";
    }

    public void OpenCanCollect(string item)
    {
        if (noityCanInteract == null) return;
        noityCanInteract.SetActive(true);
        var tmp = noityCanInteract.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = $"Press F to Collect {item}";
    }

    public void CloseCanInteract()
    {
        if (noityCanInteract != null) noityCanInteract.SetActive(false);
    }
    #endregion

    #region Public API
    public void StartDialogue(string npcName, DialogueBlock block)
    {
        if (onTalking || block == null) return;

        PlayerMovement.isInputLocked = true;
        onTalking = true;
        CloseCanInteract();

        npcTarget = StoryExecutor.Instance.GetTransfromByID(npcName);
        FocusOnNPC();

        SetupDialogueBlock(block, npcName);
        SetupDialogueUI();
        ShowNextLine();
    }
    #endregion

    #region Setup
    private void SetupDialogueBlock(DialogueBlock block, string npcName)
    {
        currentNPC = npcName;
        currentBlock = block;
        playedDialogues.Add(block.dialogueID);

        // Điều kiện block 1/2
        if (block.condition.IsMet(npcName))
        {
            currentLines = block.lines;
            currentChoices = block.choices;
        }
        else
        {
            currentLines = block.lines2;
            currentChoices = block.choices2;
        }
        currentLineIndex = 0;
        readyForNextLine = false;
    }

    private void SetupDialogueUI()
    {
        if (hudPanel == null)
            hudPanel = HUDManager.instance != null ? HUDManager.instance.hudPanel : null;
        if (hudPanel != null) hudPanel.SetActive(false);

        if (dialoguePanel == null) return;
        dialoguePanel.SetActive(true);

        var cg = dialoguePanel.GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = 0f;
        dialoguePanel.transform.localScale = Vector3.zero;

        dialoguePanel.transform.DOScale(1f, 0.3f)
            .SetEase(Ease.OutBack)
            .OnStart(() => isAnimating = true)
            .OnComplete(() => isAnimating = false);

        if (cg != null) cg.DOFade(1f, 0.3f);
    }
    #endregion

    #region Show Lines
    private async void ShowNextLine()
    {
        if (inputLocked || isAnimating) return;
        if (currentLineIndex < 0 || currentLines == null || currentLines.Count == 0)
        {
            await ShowChoicesAsync();
            return;
        }

        // Nếu đang gõ → skip to end
        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            string prevLine = currentLines[Mathf.Max(0, currentLineIndex - 1)];
            ExtractSpeakerAndLine(prevLine, out string speakerPrev, out string contentPrev);
            npcNameText.text = speakerPrev;
            dialogueText.text = contentPrev;
            isTyping = false;
            readyForNextLine = true;
            return;
        }

        // Nếu dòng trước chưa "ready", không nhảy tiếp
        if (!readyForNextLine && currentLineIndex > 0) return;

        npcNameText.text = currentNPC;

        if (currentLineIndex < currentLines.Count)
        {
            string fullLine = currentLines[currentLineIndex++];
            ExtractSpeakerAndLine(fullLine, out string speaker, out string content);
            npcNameText.text = speaker;

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            readyForNextLine = false;
            typingCoroutine = StartCoroutine(TypeTextWithTags(content, () =>
            {
                isTyping = false;
                readyForNextLine = true;
            }));
        }
        else
        {
            await ShowChoicesAsync();
        }
    }

    private IEnumerator TypeTextWithTags(string fullText, System.Action onFinished)
    {
        isTyping = true;
        dialogueText.text = "";
        string visibleText = "";
        var tagStack = new Stack<string>();
        var tagRegex = new Regex("</?[^>]+>");
        var matches = tagRegex.Matches(fullText);

        int charIndex = 0;
        int nextTagStart = matches.Count > 0 ? matches[0].Index : -1;
        int tagMatchIndex = 0;

        while (charIndex < fullText.Length)
        {
            if (charIndex == nextTagStart)
            {
                string tag = matches[tagMatchIndex].Value;
                visibleText += tag;

                if (!tag.StartsWith("</")) tagStack.Push(tag);
                else if (tagStack.Count > 0) tagStack.Pop();

                charIndex += tag.Length;
                tagMatchIndex++;
                nextTagStart = tagMatchIndex < matches.Count ? matches[tagMatchIndex].Index : -1;
            }
            else
            {
                visibleText += fullText[charIndex];
                dialogueText.text = visibleText + GetClosingTags(tagStack);
                charIndex++;
                yield return new WaitForSeconds(0.02f);
            }
        }
        onFinished?.Invoke();
    }

    private string GetClosingTags(Stack<string> tags)
    {
        if (tags.Count == 0) return "";
        var array = tags.ToArray();
        System.Array.Reverse(array);
        string closing = "";
        foreach (var openTag in array)
        {
            string tagName = openTag.Substring(1, openTag.Length - 2).Split(' ')[0];
            closing += $"</{tagName}>";
        }
        return closing;
    }

    private void ExtractSpeakerAndLine(string rawLine, out string speaker, out string content)
    {
        speaker = currentNPC;
        content = rawLine;

        if (!string.IsNullOrEmpty(rawLine) && rawLine.StartsWith("("))
        {
            int endIndex = rawLine.IndexOf(')');
            if (endIndex > 0)
            {
                speaker = rawLine.Substring(1, endIndex - 1).Trim();
                content = rawLine.Substring(endIndex + 1).TrimStart();
            }
        }
    }
    #endregion

    #region Choices
    private async Task ShowChoicesAsync()
    {
        // Hết lines → nếu không có choices thì qua story/menu/close
        if (currentBlock == null || currentChoices == null || currentChoices.Count == 0)
        {
            await TryTriggerStoryOrOpenMenuOrClose();
            return;
        }

        awaitingChoice = true;

        // Clear choices cũ
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        // Nhấc panel lên chút để có khoảng trống cho lựa chọn
        float offsetY = 60f * currentChoices.Count;
        RectTransform panelRect = dialoguePanel.GetComponent<RectTransform>();
        Vector2 originalPos = panelRect.anchoredPosition;

        panelRect.DOAnchorPosY(originalPos.y + offsetY, 0.3f)
            .SetEase(Ease.InOutSine)
            .OnStart(() => isAnimating = true)
            .OnComplete(() => isAnimating = false);

        foreach (var choice in currentChoices)
        {
            var btn = Instantiate(choiceButtonPrefab, choicesContainer);
            var txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt != null) txt.text = choice.text;

            var rt = btn.GetComponent<RectTransform>();
            if (rt != null) rt.localScale = Vector3.zero;

            btn.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
            btn.onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    private async Task TryTriggerStoryOrOpenMenuOrClose()
    {
        // Mở Menu nếu có
        if (!string.IsNullOrEmpty(openMenuID))
        {
            if (MenuManager.Instance != null)
            {
                MenuManager.Instance.OpenMenu(openMenuID);
            }
        }

        // Ưu tiên Story
        if (!string.IsNullOrEmpty(currentStoryBlockID) && !onSetupStory)
        {
            onSetupStory = true;
            inputLocked = true;

            await SceneTransitionManager.Instance.DoFadeTransition(async () =>
            {
                try
                {
                    // CloseDialogue(); // Ẩn panel trước khi chạy cutscene/story
                    // CloseDialogueRaw();
                    if (dialoguePanel != null) dialoguePanel.SetActive(false);
                    await GameFlowManager.Instance.CallSetupStoryNoneTransiton(currentStoryBlockID);
                    GameStateManager.Instance.SaveGame();
                }
                catch
                {
                    Debug.LogError($"Failed to setup story block: {currentStoryBlockID}");
                }
            });
        }


        // Mặc định: đóng đối thoại
        CloseDialogueWithReset();
    }
    #endregion

    #region Choice Selected
    private void OnChoiceSelected(DialogueChoice choice)
    {
        if (hasSelectedChoice) return;
        hasSelectedChoice = true;
        inputLocked = true;
        awaitingChoice = false;

        playNextDialogueAutomatically = choice.playNextDialogueAutomatically;

        // Khoá và ẩn các button choice
        foreach (Transform child in choicesContainer)
        {
            Button btn = child.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.interactable = false;
            }

            child.DOScale(0f, 0.3f).SetEase(Ease.OutBounce)
                .OnStart(() => isAnimating = true)
                .OnComplete(() =>
                {
                    dialoguePanel.transform.DOLocalMoveY(-390, 0.6f)
                        .SetEase(Ease.OutFlash)
                        .OnComplete(() => isAnimating = false);
                    Destroy(child.gameObject);
                });
        }

        // Áp hiệu ứng side-effect
        if (!string.IsNullOrEmpty(choice.storyBlockID))
            currentStoryBlockID = choice.storyBlockID;

        if (!string.IsNullOrEmpty(choice.openMenuID))
            openMenuID = choice.openMenuID;

        if (choice.missionRaw != null)
        {
            MissionManager.Instance.AcceptNewMission(
                choice.missionRaw.missionType,
                choice.missionRaw.id,
                choice.missionRaw.tile,
                choice.missionRaw.description,
                choice.missionRaw.type,
                choice.missionRaw.count,
                choice.missionRaw.loacation,
                choice.missionRaw.rewardGold,
                choice.missionRaw.rewardExp,
                choice.missionRaw.storyID
            );
        }

        RelationshipManager.Instance.ModifyRelationship(currentNPC, choice.relationShipEff);

        if (!string.IsNullOrEmpty(choice.nextDialogueId))
            DialogueBlockNpcHandler.Instance.SetNpcDialogueID(currentNPC, choice.nextDialogueId);

        // Có choiceLines → hiển thị xong rồi mới gọi AutomaticNextDialogue()
        if (choice.choiceLines != null && choice.choiceLines.Count > 0)
        {
            currentLines = choice.choiceLines;
            currentChoices = null;
            currentBlock = null;

            currentLineIndex = 0;
            readyForNextLine = false;

            inputLocked = false;
            hasSelectedChoice = false;

            AutomaticNextDialogue();
        }
        else
        {
            // Không có choiceLines → xử lý chuyển tiếp ngay
            inputLocked = false;
            hasSelectedChoice = false;
            AutomaticNextDialogue();
        }
    }

    #endregion

    #region Flow Control
    private async void AutomaticNextDialogue()
    {
        Debug.Log($"DialogueManager: AutomaticNextDialogue() called with playNextDialogueAutomatically = {playNextDialogueAutomatically}");
        Debug.LogFormat("DialogueManager: currentNPC = {0}, currentStoryBlockID = {1}, openMenuID = {2}",
            currentNPC, currentStoryBlockID, openMenuID);
        if (playNextDialogueAutomatically)
        {
            var npcObj = StoryExecutor.Instance.GetTransfromByID(currentNPC)?.gameObject;
            if (npcObj != null)
            {
                var npc = npcObj.GetComponent<NPCInteract>();
                if (npc != null)
                {
                    //CloseDialogue();
                    onTalking = false;
                    npc.SetDialogue();
                    return;
                }
            }
            await TryTriggerStoryOrOpenMenuOrClose();
        }
        else
        {
            await TryTriggerStoryOrOpenMenuOrClose();
        }
    }
    #endregion

    #region Close & Reset
    public void CloseDialogueWithReset()
    {
        ReNpcState();
        ResetVariable();
        CloseDialogue();
        if (dialoguePanel != null)
            dialoguePanel.transform.DOLocalMoveY(-390, 0.6f).SetEase(Ease.OutFlash);
    }

    public void CloseDialogue()
    {
        if (dialoguePanel == null)
        {
            CloseDialogueRaw();
            return;
        }

        var cg = dialoguePanel.GetComponent<CanvasGroup>();
        dialoguePanel.transform.DOScale(0f, 0.2f)
            .SetEase(Ease.InBack)
            .OnStart(() => isAnimating = true)
            .OnComplete(() =>
            {
                isAnimating = false;
                CloseDialogueRaw();
            });

        if (cg != null) cg.DOFade(0f, 0.2f);
    }

    public void CloseDialogueRaw()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);

        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);

        ResetCamera();
    }

    public void ResetVariable()
    {
        PlayerMovement.isInputLocked = false;

        currentStoryBlockID = string.Empty;
        currentBlock = null;
        currentLines = null;
        currentChoices = null;
        currentLineIndex = -1;

        awaitingChoice = false;
        isAnimating = false;
        hasSelectedChoice = false;
        onTalking = false;
        onSetupStory = false;
        isTyping = false;
        inputLocked = false;
        openMenuID = string.Empty;

        readyForNextLine = false;
    }

    public void ReNpcState()
    {
        try
        {
            var t = StoryExecutor.Instance.GetTransfromByID(currentNPC);
            if (t != null)
            {
                var ni = t.gameObject.GetComponent<NPCInteract>();
                if (ni != null) ni.isTalking = false;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Error resetting NPCInteract state: {ex.Message}");
        }
    }
    #endregion

    #region Camera Helpers
    private void FocusOnNPC()
    {
        if (playerCamera == null)
        {
            var go = GameObject.FindWithTag("CameraTaget");
            if (go != null) playerCamera = go.GetComponent<CinemachineVirtualCamera>();
        }
        if (focusCamera == null || npcTarget == null) return;

        originalFOV = focusCamera.m_Lens.FieldOfView;

        Vector3 cameraPos = GetBestCameraPosition(npcTarget.position);
        focusCamera.transform.position = cameraPos;

        Vector3 dir = npcTarget.position - focusCamera.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);

        focusCamera.transform.rotation = lookRotation;
        focusCamera.Priority = 100;
    }

    private Vector3 GetBestCameraPosition(Vector3 targetPos)
    {
        Vector3[] directions =
        {
            new Vector3(1,0,0),
            new Vector3(-1,0,0),
            new Vector3(0,0,1),
            new Vector3(0,0,-1),
            new Vector3(0.7f,0,0.7f),
            new Vector3(-0.7f,0,0.7f),
            new Vector3(0.7f,0,-0.7f),
            new Vector3(-0.7f,0,-0.7f),
        };

        foreach (var dir in directions)
        {
            Vector3 checkPos = targetPos + dir.normalized * cameraDistance + Vector3.up * cameraHeight;
            if (!Physics.Linecast(checkPos, targetPos + Vector3.up * 1.5f, obstacleMask))
            {
                return checkPos;
            }
        }
        return targetPos + Vector3.back * cameraDistance + Vector3.up * cameraHeight;
    }

    private void ResetCamera()
    {
        focusCamera.LookAt = null;
        focusCamera.Priority = -1;
    }
    #endregion
}
