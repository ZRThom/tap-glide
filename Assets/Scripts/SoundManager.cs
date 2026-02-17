using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public Slider volumeSlider;
    public CustomSettings settings;

    void Start()
    {
        if (settings == null) settings = CustomSettings.Load();

        if (settings != null && settings.devs != null && settings.devs.Count > 2)
        {
            if (float.TryParse(settings.devs[2], out float savedVolume))
            {
                volumeSlider.value = savedVolume;
                ApplyVolume(savedVolume);
            }
        }

        volumeSlider.onValueChanged.AddListener(delegate { OnVolumeChanged(); });
    }

    public void OnVolumeChanged()
    {
        float value = volumeSlider.value;
        ApplyVolume(value);

        if (settings != null && settings.devs != null && settings.devs.Count > 2)
        {
            settings.devs[2] = value.ToString("F2");
        }
    }

    private void ApplyVolume(float value)
    {
        AudioListener.volume = value;
    }
}