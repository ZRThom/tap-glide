using UnityEngine;

public class FeedBackSpawner : MonoBehaviour
{
    public FeedBackPopup popupPrefab;
    public RectTransform parentArea;
    public Sprite okSprite;
    public Sprite niceSprite;
    public Sprite perfectSprite;
    public Sprite badSprite;

    [Header("Settings")]
    public float upScreenRatio = 0.2f;
    public float duration = 1f;

    // Ces méthodes sont appelées par tes scripts de collision/timing
    public void ShowOK() => Spawn(okSprite);
    public void ShowNice() => Spawn(niceSprite);
    public void ShowPerfect() => Spawn(perfectSprite);
    public void ShowBad() => Spawn(badSprite);

    void Spawn (Sprite sprite)
    {
        if (!popupPrefab || !parentArea || !sprite) return;
        FeedBackPopup popup = Instantiate(popupPrefab, parentArea);
        RectTransform rt = popup.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        popup.Play(sprite, upScreenRatio, duration);
    }
}