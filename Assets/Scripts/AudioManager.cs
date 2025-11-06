using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public List<AudioClip> gameMusicTracks; // multiple gameplay tracks

    public AudioClip gameOverMusic;
    [HideInInspector] public AudioClip currentGameTrack;

    [Header("SFX Clips")]
    public AudioClip atkSound;
    public AudioClip airAttackSound;
    public AudioClip jumpSound;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Load saved track for gameplay
        if (SceneManager.GetActiveScene().buildIndex == 1 && gameMusicTracks.Count > 0)
        {
            int savedIndex = PlayerPrefs.GetInt("SelectedTrack", 0);
            savedIndex = Mathf.Clamp(savedIndex, 0, gameMusicTracks.Count - 1);
            currentGameTrack = gameMusicTracks[savedIndex];
        }

        PlayMusicForScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If entering gameplay scene, ensure saved track plays
        if (scene.buildIndex == 1 && gameMusicTracks.Count > 0)
        {
            if (currentGameTrack == null)
            {
                int savedIndex = PlayerPrefs.GetInt("SelectedTrack", 0);
                savedIndex = Mathf.Clamp(savedIndex, 0, gameMusicTracks.Count - 1);
                currentGameTrack = gameMusicTracks[savedIndex];
            }
        }

        PlayMusicForScene(scene.buildIndex);
    }

    void PlayMusicForScene(int sceneIndex)
    {
        AudioClip newClip = null;

        if (sceneIndex == 0)
        {
            // Main Menu
            newClip = mainMenuMusic;
        }
        else if (sceneIndex == 1)
        {
            // Gameplay
            newClip = currentGameTrack ?? gameMusicTracks[0];
            currentGameTrack = newClip; // make sure currentGameTrack is set
        }
        else if (sceneIndex == 2)
        {
            // Game Over
            newClip = gameOverMusic;
        }

        if (newClip != null)
        {
            // Only switch if it's different
            if (musicSource.clip != newClip)
            {
                musicSource.clip = newClip;
                musicSource.loop = true;
                musicSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("No music clip assigned for this scene!");
        }
    }

    public void ChangeMusic(int index)
    {
        if (index >= 0 && index < gameMusicTracks.Count)
        {
            currentGameTrack = gameMusicTracks[index];
            musicSource.clip = currentGameTrack;
            musicSource.Play();

            PlayerPrefs.SetInt("SelectedTrack", index);
            PlayerPrefs.Save();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
