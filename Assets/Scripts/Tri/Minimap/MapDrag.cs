using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MapDragZoom : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public RectTransform mapContent;
    public RectTransform maskArea;
    public float zoomSpeed = 0.1f;
    public float minScale = 1f;
    public float maxScale = 2f;
    public float dragSmoothness = 10f;

    public List<Transform> mapIcons;

    // Biên giới hạn bản đồ thật (tự tính)
    private Vector2 mapBoundsMin;
    private Vector2 mapBoundsMax;

    private Vector2 lastPointerLocalPos;
    private Vector2 targetAnchoredPos;

    private void Start()
    {
        targetAnchoredPos = mapContent.anchoredPosition;

        // Tự động tính biên bản đồ thật
        CalculateMapBounds();

        // Đảm bảo icon và label hiển thị đúng khi start
        UpdateIconsAndLabels(mapContent.localScale.x);
    }

    private void Update()
    {
        HandleZoom();

        // Mượt hóa chuyển động
        mapContent.anchoredPosition = Vector2.Lerp(
            mapContent.anchoredPosition,
            targetAnchoredPos,
            Time.deltaTime * dragSmoothness
        );
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (mapContent.localScale.x <= minScale + 0.001f)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            maskArea, eventData.position, eventData.pressEventCamera, out lastPointerLocalPos
        );
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mapContent.localScale.x <= minScale + 0.001f)
            return;

        Vector2 currentLocalPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            maskArea, eventData.position, eventData.pressEventCamera, out currentLocalPos))
        {
            Vector2 delta = currentLocalPos - lastPointerLocalPos;
            lastPointerLocalPos = currentLocalPos;

            targetAnchoredPos += delta;
            ClampMapToCenter();
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float currentScale = mapContent.localScale.x;
            float newScale = Mathf.Clamp(currentScale + scroll * zoomSpeed, minScale, maxScale);
            mapContent.localScale = new Vector3(newScale, newScale, 1f);

            UpdateIconsAndLabels(newScale);

            if (Mathf.Approximately(newScale, minScale))
                targetAnchoredPos = Vector2.zero;

            ClampMapToCenter();
        }
    }

    private void UpdateIconsAndLabels(float scale)
    {
        bool showIcons = scale >= 1.1f;
        bool showLabels = scale >= 1.5f;

        foreach (Transform icon in mapIcons)
        {
            icon.gameObject.SetActive(showIcons);
            Transform label = icon.Find("Label");
            if (label != null)
                label.gameObject.SetActive(showLabels);
        }
    }

    private void ClampMapToCenter()
    {
        float scale = mapContent.localScale.x;
        Vector2 maskSize = maskArea.sizeDelta;

        float mapWidth = (mapBoundsMax.x - mapBoundsMin.x) * scale;
        float mapHeight = (mapBoundsMax.y - mapBoundsMin.y) * scale;

        float maxOffsetX = Mathf.Max(0, (mapWidth - maskSize.x) / 2f);
        float maxOffsetY = Mathf.Max(0, (mapHeight - maskSize.y) / 2f);

        float clampedX = Mathf.Clamp(targetAnchoredPos.x, -maxOffsetX, maxOffsetX);
        float clampedY = Mathf.Clamp(targetAnchoredPos.y, -maxOffsetY, maxOffsetY);

        targetAnchoredPos = new Vector2(clampedX, clampedY);
    }

    private void CalculateMapBounds()
    {
        // Lấy kích thước RectTransform gốc của mapContent
        Vector2 size = mapContent.sizeDelta;

        // Ở đây giả định pivot ở giữa => min là -size/2, max là size/2
        mapBoundsMin = -size / 2f;
        mapBoundsMax = size / 2f;
    }
}
