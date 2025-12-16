using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIEffectController : MonoBehaviour
{
    public enum EffectMode
    {
        None,
        Scale,
        SlideFromLeft
    }

    [Header("Config")]
    public EffectMode mode = EffectMode.None;
    public float duration = 0.5f;

    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector3 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;
        originalPosition = rectTransform.anchoredPosition;
    }

    private void OnEnable()
    {
        StopAllCoroutines();

        switch (mode)
        {
            case EffectMode.Scale:
                StartCoroutine(PlayScaleIn());
                break;
            case EffectMode.SlideFromLeft:
                StartCoroutine(PlaySlideIn());
                break;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        switch (mode)
        {
            case EffectMode.Scale:
               // StartCoroutine(PlayScaleOut());
                break;
            case EffectMode.SlideFromLeft:
                StartCoroutine(PlaySlideOut());
                break;
        }
    }

    private IEnumerator PlayScaleIn()
    {
        rectTransform.localScale = Vector3.zero;
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / duration;
            rectTransform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            yield return null;
        }
        rectTransform.localScale = originalScale;
    }

    private IEnumerator PlayScaleOut()
    {
        Vector3 startScale = rectTransform.localScale;
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / duration;
            rectTransform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        rectTransform.localScale = Vector3.zero;
    }

    private IEnumerator PlaySlideIn()
    {
        Vector2 offscreen = new Vector2(1920, originalPosition.y);
        rectTransform.anchoredPosition = offscreen;

        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(offscreen, originalPosition, t);
            yield return null;
        }
        rectTransform.anchoredPosition = originalPosition;
    }

    private IEnumerator PlaySlideOut()
    {
        Vector2 target = new Vector2(1920, originalPosition.y);
        Vector2 start = rectTransform.anchoredPosition;

        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }
        rectTransform.anchoredPosition = target;
        transform.parent.gameObject.SetActive(false);
    }
}
