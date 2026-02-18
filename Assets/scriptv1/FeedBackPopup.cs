using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FeedBackPopup : MonoBehaviour
{
    [Header("auto if null")]
    public Image image;
    public CanvasGroup canvasGroup;
    RectTransform rt;
    void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (!image) image = GetComponent<Image>();
        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Play(Sprite sprite, float upScreenRatio = 0.2f, float duration = 1f)
    {
        image.sprite = sprite;
        canvasGroup.alpha = 1f;
        StopAllCoroutines();
        StartCoroutine(Anim(upScreenRatio, duration));
    }

    IEnumerator Anim(float upScreenRatio, float duration)
    {
        Vector2 start = rt.anchoredPosition;
        Canvas canvas = GetComponentInParent<Canvas>();
        RectTransform canvaRt = canvas.GetComponent<RectTransform>();
        float up = canvaRt.rect.height * upScreenRatio;
        Vector2 end = start + Vector2.up * up;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / duration);
            float e = Mathf.SmoothStep(0f, 1f, p);
            rt.anchoredPosition = Vector2.Lerp(start, end, e);
            canvasGroup.alpha = 1f - p;
            yield return null;
        }
        Destroy(gameObject);
    }
}