using UnityEngine;
using UnityEngine.InputSystem;

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
    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.iKey.wasPressedThisFrame) Spawn(okSprite);
        if (kb.oKey.wasPressedThisFrame) Spawn(niceSprite);
        if (kb.pKey.wasPressedThisFrame) Spawn(perfectSprite);
        if (kb.lKey.wasPressedThisFrame) Spawn(badSprite);
    }
    void Spawn (Sprite sprite)
    {
        if (!popupPrefab || !parentArea || !sprite) return;
        FeedBackPopup popup = Instantiate(popupPrefab, parentArea);
        RectTransform rt = popup.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        popup.Play(sprite, upScreenRatio, duration);
    }
}
