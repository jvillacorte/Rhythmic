using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MusicDropdown : MonoBehaviour
{
    public Dropdown musicDropdown; // assign your Dropdown in the Inspector

    void Start()
    {
        if (AudioManager.instance == null || musicDropdown == null) return;

        // Clear any existing options
        musicDropdown.ClearOptions();

        // Populate dropdown with track names from AudioManager
        List<string> options = new List<string>();
        for (int i = 0; i < AudioManager.instance.gameMusicTracks.Count; i++)
        {
            if (AudioManager.instance.gameMusicTracks[i] != null)
                options.Add(AudioManager.instance.gameMusicTracks[i].name);
            else
                options.Add("Track " + i);
        }
        musicDropdown.AddOptions(options);

        // Set dropdown value to saved track or default 0
        int savedIndex = PlayerPrefs.GetInt("SelectedTrack", 0);
        savedIndex = Mathf.Clamp(savedIndex, 0, AudioManager.instance.gameMusicTracks.Count - 1);
        musicDropdown.value = savedIndex;

        // Listen for dropdown changes
        musicDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnDropdownValueChanged(int index)
    {
        if (AudioManager.instance != null)
        {
            // Update current track and save selection
            AudioManager.instance.ChangeMusic(index);
        }
    }
}
