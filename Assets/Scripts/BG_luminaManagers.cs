using UnityEngine;
using UnityEngine.UI;

public class BG_LuminaManager : MonoBehaviour
{
    public Slider slider;
    private Image targetImage;

    void Awake()
    {
        targetImage = GetComponent<Image>();
    }

    public void UpdateBrightness(float value)
    {
        if (targetImage != null)

        {

            targetImage.color = new Color(value, value, value, 1f);
        }
        slider.SetValueWithoutNotify(value);
    }

    void OnSliderChanged(float ms)
    {
        PlayerPrefs.Save();
    }
 }