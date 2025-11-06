using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    Resolution[] resolutions;

    void Start()
    {
        // Build list of unique resolutions
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        List<Resolution> uniqueResolutions = new List<Resolution>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            bool exists = uniqueResolutions.Exists(r => r.width == resolutions[i].width && r.height == resolutions[i].height);
            if (!exists)
            {
                uniqueResolutions.Add(resolutions[i]);
                options.Add(resolutions[i].width + " x " + resolutions[i].height);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = uniqueResolutions.Count - 1;
                }
            }
        }

        resolutions = uniqueResolutions.ToArray();
        resolutionDropdown.AddOptions(options);

        // Load saved settings
        int savedResolutionIndex = PlayerPrefs.GetInt("resolutionIndex", currentResolutionIndex);
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;

        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = isFullscreen;

        ApplySettings(savedResolutionIndex, isFullscreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
        SyncAllMenus();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("resolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
        SyncAllMenus();
    }

    void ApplySettings(int resolutionIndex, bool fullscreen)
    {
        Resolution resolution = resolutions[Mathf.Clamp(resolutionIndex, 0, resolutions.Length - 1)];
        Screen.SetResolution(resolution.width, resolution.height, fullscreen);
        Screen.fullScreen = fullscreen;
    }

    void SyncAllMenus()
    {
        SettingsMenu[] menus = FindObjectsOfType<SettingsMenu>(true);
        foreach (SettingsMenu menu in menus)
        {
            if (menu == this) continue;

            bool isFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
            int resolutionIndex = PlayerPrefs.GetInt("resolutionIndex", 0);

            if (menu.fullscreenToggle != null)
                menu.fullscreenToggle.isOn = isFullscreen;

            if (menu.resolutionDropdown != null)
            {
                menu.resolutionDropdown.value = resolutionIndex;
                menu.resolutionDropdown.RefreshShownValue();
            }
        }
    }
}
