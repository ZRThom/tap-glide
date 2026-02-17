using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResolutionManager : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public CustomSettings settings;
    private Resolution[] resolutions;

    void Start()
    {
        if (settings == null) settings = CustomSettings.Load();

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRateRatio.value.ToString("0") + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        if (settings != null && settings.devs != null && settings.devs.Count > 0)
        {
            settings.devs[0] = resolution.width + "x" + resolution.height;
        }
    }
}