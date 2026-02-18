using UnityEngine;
using UnityEngine.UI;

public class BG_LuminaManager : MonoBehaviour
{
    public CustomSettings settings;
    public Slider slider;

    private Image targetImage;
    private SpriteRenderer targetSprite;

    void Awake()
    {
        targetImage = GetComponent<Image>();
        targetSprite = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (settings != null)
        {
            settings.LoadSettings();
            ApplyBrightness(settings.bgBrightness);
            
            if (slider != null)
            {
                slider.SetValueWithoutNotify(settings.bgBrightness);
            }
        }
    }

    public void UpdateBrightness(float value)
    {
        ApplyBrightness(value);

        if (settings != null)
        {
            settings.bgBrightness = value;
            settings.SaveSettings();
        }
    }

    private void ApplyBrightness(float value)
    {
        Color newColor = new Color(value, value, value, 1f);

        if (targetImage != null)
            targetImage.color = newColor;

        if (targetSprite != null)
            targetSprite.color = newColor;
    }
}