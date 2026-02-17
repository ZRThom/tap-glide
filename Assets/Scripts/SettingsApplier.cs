using UnityEngine;

public class SettingsApplier : MonoBehaviour
{
    public CustomSettings settings;

    void Start()
    {
        if (settings == null) settings = CustomSettings.Load();
        ApplyAllSettings();
    }

    public void ApplyAllSettings()
    {
        if (settings == null || settings.devs == null || settings.devs.Count < 3) return;

        string[] res = settings.devs[0].Split('x');
        if (res.Length == 2)
        {
            if (int.TryParse(res[0], out int width) && int.TryParse(res[1], out int height))
            {
                Screen.SetResolution(width, height, Screen.fullScreen);
            }
        }
        if (float.TryParse(settings.devs[1], out float brightness))
        {
            RenderSettings.ambientLight = new Color(brightness, brightness, brightness);
        }
        if (float.TryParse(settings.devs[2], out float volume))
        {
            AudioListener.volume = volume;
        }
    }
}