using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Text.RegularExpressions;
using System.Collections;
using System.Threading.Tasks;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool onTalking;
    private bool onSetupStory = false;
    private bool isAnimating = false; // ✨ chống spam tween

    private string currentStoryBlockID;
    private bool playNextDialogueAutomatically = false;

    public GameObject noityCanInteract;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public GameObject hudPanel;
    public TMP_Text dialogueText;
    public TMP_Text npcNameText;
    public Transform choicesContainer;
    public Button choiceButtonPrefab;

    private DialogueBlock currentBlock;
    private List<string> currentLines;
    private List<DialogueChoice> currentChoices;
    private string currentNPC;
    private int currentLineIndex = -1;
    private bool awaitingChoice;
    private HashSet<string> playedDialogues = new HashSet<string>();
    private bool inputLocked = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        dialoguePanel.SetActive(false);
    }

    public void OpenCanInteract(string npcName)
    {
        noityCanInteract.SetActive(true);
        noityCanInteract.GetComponentInChildren<TextMeshProUGUI>().text = "Press F to talking with " + npcName;
    }

    public void CloseCanInteract()
    {
        noityCanInteract.SetActive(false);
    }

    public void StartDialogue(string npcName, DialogueBlock block)
    {
        if (onTalking) return;

        onTalking = true;
        CloseCanInteract();

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

        currentNPC = npcName;
        currentBlock = block;
        currentLineIndex = 0;
        playedDialogues.Add(block.dialogueID);

        dialoguePanel.SetActive(true);
        hudPanel.SetActive(false);

        var cg = dialoguePanel.GetComponent<CanvasGroup>();
        if (cg != null) cg.alpha = 0;
        dialoguePanel.transform.localScale = Vector3.zero;

        // ✨ Tween mở Panel
        dialoguePanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack)
            .OnStart(() => isAnimating = true)
            .OnComplete(() => isAnimating = false);

        if (cg != null)
            cg.DOFade(1, 0.3f);

        ShowNextLine();
    }

    public void LockInput() => inputLocked = true;
    public void UnlockInput() => inputLocked = false;

    private void Update()
    {
        if (dialoguePanel.activeSelf && !onSetupStory && Input.GetMouseButtonDown(0) && !awaitingChoice && !isAnimating && !inputLocked)
        {
            ShowNextLine();
        }
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

    private async Task ShowNextLine()
    {
        if (inputLocked || isAnimating || currentLineIndex < 0 || currentLines == null || currentLines.Count == 0)
            return;

        npcNameText.text = currentNPC;

        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            if (currentLines.Count < currentLineIndex - 1) return;

            string prevLine = currentLines[currentLineIndex - 1];
            ExtractSpeakerAndLine(prevLine, out string speaker, out string content);
            npcNameText.text = speaker;
            dialogueText.text = content;
            isTyping = false;
            return;
        }

        if (currentLineIndex < currentLines.Count)
        {
            string fullLine = currentLines[currentLineIndex++];
            ExtractSpeakerAndLine(fullLine, out string speaker, out string content);
            npcNameText.text = speaker;

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeTextWithTags(content));
        }
        else
        {
            await ShowChoicesAsync();
        }
    }

    private IEnumerator TypeTextWithTags(string fullText)
    {
        isTyping = true;
        dialogueText.text = "";
        var visibleText = "";
        var tagStack = new Stack<string>();
        var tagRegex = new Regex(@"<\/?[^>]+>");
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

        isTyping = false;
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

    private async Task ShowChoicesAsync()
    {
        if (currentBlock == null || currentChoices == null || currentChoices.Count == 0)
        {
         
            if (!string.IsNullOrEmpty(currentStoryBlockID) && !onSetupStory)
            {
                onSetupStory = true;
                LockInput();
                await SceneTransitionManager.Instance.DoFadeTransition(async () =>
                {
                    try
                    {
                        CloseDialogue();
                         Debug.Log(currentStoryBlockID);
                        await GameFlowManager.Instance.SetupStory(currentStoryBlockID);
                        GameStateManager.Instance.SaveGame();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError("Error during SetupStory: " + ex.Message);
                    }
                    finally
                    {
                        onSetupStory = false;
                        ResetVariable();
                        UnlockInput();
                    }
                });

                return;
            }
                CloseDialogueWithReset();
            return;
        }

        awaitingChoice = true;

        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);


        // Tính số lượng lựa chọn
        int choiceCount = currentChoices.Count;
        float offsetY = 60f * choiceCount; 

        // Lưu vị trí ban đầu
        RectTransform panelRect = dialoguePanel.GetComponent<RectTransform>();
        Vector2 originalPos = panelRect.anchoredPosition;

        // Di chuyển panel lên cao theo số lượng lựa chọn
        panelRect.DOAnchorPosY(originalPos.y + offsetY, 0.3f)
            .SetEase(Ease.InOutSine)
            .OnStart(() => isAnimating = true)
            .OnComplete(() => isAnimating = false);

        // Tạo button lựa chọn (sau khi bắt đầu tween)
        foreach (var choice in currentChoices)
        {
            var btn = Instantiate(choiceButtonPrefab, choicesContainer);
            btn.GetComponentInChildren<TMP_Text>().text = choice.text;

            var rect = btn.GetComponent<RectTransform>();
            rect.localScale = Vector3.zero;
            rect.DOScale(1, 0.5f).SetEase(Ease.OutBack);

            btn.onClick.AddListener(() => OnChoiceSelected(choice));
        }
    }

    private bool hasSelectedChoice = false; // thêm biến này vào class

    private void OnChoiceSelected(DialogueChoice choice)
    {
        if (hasSelectedChoice) return; // Ngăn spam
        hasSelectedChoice = true;
        LockInput(); // Khóa input
        awaitingChoice = false;
        playNextDialogueAutomatically = choice.playNextDialogueAutomatically;

        foreach (Transform child in choicesContainer)
        {
            Button btn = child.GetComponent<Button>();
            btn.onClick.RemoveAllListeners(); // Gỡ toàn bộ click
            btn.interactable = false;         // Vô hiệu hoá nút

            child.DOScale(0f, 0.3f).SetEase(Ease.OutBounce)
                .OnStart(() => isAnimating = true)
                .OnComplete(() =>
                {
                    dialoguePanel.transform.DOLocalMoveY(-390, 0.6f).SetEase(Ease.OutFlash)
                    .OnComplete(() => isAnimating = false);
                    Destroy(child.gameObject);
                });
        }

        if (!string.IsNullOrEmpty(choice.storyBlockID))
            currentStoryBlockID = choice.storyBlockID;


        if (choice.missionRaw != null)
            MissionManager.Instance.AcceptNewMission(choice.missionRaw.missionType, choice.missionRaw.id, choice.missionRaw.tile,
                choice.missionRaw.description, choice.missionRaw.type, choice.missionRaw.count, choice.missionRaw.loacation,
                choice.missionRaw.rewardGold, choice.missionRaw.rewardExp, choice.missionRaw.storyID);

        RelationshipManager.Instance.ModifyRelationship(currentNPC, choice.relationShipEff);

        if (!string.IsNullOrEmpty(choice.nextDialogueId))
            DialogueBlockNpcHandler.Instance.SetNpcDialogueID(currentNPC, choice.nextDialogueId);

        if (choice.choiceLines != null && choice.choiceLines.Count > 0)
        {
            currentLines = choice.choiceLines;
            currentLineIndex = 0;
            currentBlock = null;
            UnlockInput(); // Mở input trở lại
            hasSelectedChoice = false;
            ShowNextLine();
            return;
        }
        if (playNextDialogueAutomatically && !string.IsNullOrEmpty(choice.nextDialogueId))
        {
            Debug.Log($"On Choice Automatically continuing to next dialogue: {choice.nextDialogueId}");
            ResetVariable();
            CloseDialogue();
            dialoguePanel.transform.DOLocalMoveY(-390, 0.6f).SetEase(Ease.OutFlash);
            ContinueAutoDialogue();
        }
        else
        {
            CloseDialogueWithReset();
        }
        UnlockInput();           // Mở input trở lại
        hasSelectedChoice = false; // Reset cho lần sau
    }

    public void CloseDialogueWithReset()
    {
        ReNpcState();
        ResetVariable();
        CloseDialogue();
        dialoguePanel.transform.DOLocalMoveY(-390, 0.6f).SetEase(Ease.OutFlash);
    }

    public void CloseDialogue()
    {
        if (dialoguePanel != null)
        {
            var cg = dialoguePanel.GetComponent<CanvasGroup>();
            dialoguePanel.transform.DOScale(0, 0.2f).SetEase(Ease.InBack)
                .OnStart(() => isAnimating = true)
                .OnComplete(() =>
                {
                    isAnimating = false;
                    CloseDialogueRaw();
                });

            if (cg != null)
                cg.DOFade(0, 0.2f);
        }
        else
        {
            CloseDialogueRaw();
        }
    }

    public void CloseDialogueRaw()
    {
        dialoguePanel.SetActive(false);
        hudPanel.SetActive(true);
        foreach (Transform child in choicesContainer)
            Destroy(child.gameObject);
    }

    public void ResetVariable()
    {
        currentStoryBlockID = string.Empty;
        onTalking = false;
        currentBlock = null;
        currentLines = null;
        currentChoices = null;
        currentLineIndex = -1;
        awaitingChoice = false;
        isAnimating = false;
        hasSelectedChoice = false;
    }

    public void ReNpcState()
    {
        try
        {
            StoryExecutor.Instance.GetTransfromByID(currentNPC).gameObject.GetComponent<NPCInteract>().isTalking = false;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"Error resetting NPCInteract state: {ex.Message}");
        }
    }

    private async void ContinueAutoDialogue()
    {
        await Task.Delay(200);
        var npcObj = StoryExecutor.Instance.GetTransfromByID(currentNPC)?.gameObject;
        if (npcObj != null)
        {
            var npc = npcObj.GetComponent<NPCInteract>();
            if (npc != null)
            {
                onTalking = false;
                npc.SetDialogue();
            }
        }
    }
}
