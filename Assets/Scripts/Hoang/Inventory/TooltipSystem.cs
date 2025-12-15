using UnityEngine;
using TMPro;

public class TooltipSystem : MonoBehaviour
{
    public Canvas parentCanvas;
    public RectTransform ToolTipTransform;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    public static TooltipSystem Instance;

    private void Start()
    {
        Instance = this;
    }

    void Update()
    {
        if (parentCanvas == null || ToolTipTransform == null)
        {
            Debug.LogWarning("TooltipManager missing references!");
            return;
        }

        Vector2 movePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            Input.mousePosition,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
            out movePos
        );

        ToolTipTransform.localPosition = movePos;
    }

    public void Show(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
        ToolTipTransform.gameObject.SetActive(true);
    }


    public void Hide()
    {
        ToolTipTransform.gameObject.SetActive(false);
    }
}
