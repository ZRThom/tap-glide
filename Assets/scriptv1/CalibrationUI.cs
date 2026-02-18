using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalibrationUI : MonoBehaviour
{
    private const string CALIB_KEY = "CALIBRATION_MS";
    [Header("UI")]
    public Slider slider;
    public TMP_Text valueText;
    public TMP_Text minText;
    public TMP_Text maxText;
    [Header("Range (ms)")]
    public float minMs = -200f;
    public float maxMs = 200f;

    public Metronome metronome;
    
    void Awake()
    {
        slider.minValue = minMs;
        slider.maxValue = maxMs;
        if (minText) minText.text = $"{minMs:0} ms";
        if (maxText) maxText.text = $"{maxMs:0} ms";

        float saved = PlayerPrefs.GetFloat(CALIB_KEY, 0f);
        slider.SetValueWithoutNotify(saved);
        RefreshText(saved);
        // hook
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float ms)
    {
        RefreshText(ms);
        PlayerPrefs.SetFloat(CALIB_KEY, ms);
        PlayerPrefs.Save();
        if (metronome != null) metronome.SetCalibration(ms, save: false);
    }

    void RefreshText(float ms)
    {
        if (valueText) valueText.text = $"{ms:+0;-0;0} ms";
    }
}
