using Unity.VisualScripting;

//controls BGM and SFX for game, takes inputs for audio sources and various
//mp3 files in order to be utilized in the game
//in this case, using attack sound effects, jump sound effects, and background music
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip background;
    public AudioClip atkSound;

    public AudioClip airAttackSound;
    public AudioClip jumpSound;

    void Start()
    {
        //looping background music
        musicSource.clip = background;
        musicSource.loop = true;
        musicSource.Play();
    }

    //Method to play a SFX clip, being that of the atk and jump sound effects
    //SFX and BGM detatched from each other in order to allow for their volume to be controlled independently
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}