using UnityEngine;
using UnityEngine.UI;

// .. Store all sound effect names to easily refere to them from outside the class scope
public enum SoundEffectName
{
    HIT,
    BUTTON_CLICK,
    COLLECT_COINS,
    COLLECT_FRUIT,
    EAT_FRUIT,
    NEW_HIGH_SCORE,
    SPAWN_BOMB
};

public enum PlayingMusicType
{
    NONE = -1,
    MENU = 0,
    IN_GAME = 0,
    POWER_UP_ACTIVE = 1,
};

[System.Serializable]
public struct SoundEffect
{
    public SoundEffectName effectName;
    public AudioClip effectAudio;
}

//Required components of this class
[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static int isMusicMuted;
    public static int isSoundEffectsMuted;
    public static PlayingMusicType musicType;

    public AudioClip[] MusicAudios;

    [Header("Please set sfx names in SoundEffectName enum in SoundManager.cs.")]
    public SoundEffect[] soundEffectAudios;

    public static SoundManager Instance = null;

    public GameObject soundButton;
    public GameObject musicButton;

    private AudioSource audioSrc;      // the attached audio source
    private float startingVol;         // store the starting volume of the audio source

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)   // duplicate
        {
            Destroy(gameObject);

            throw new UnityException("Duplicate Game Manager!!");
        }

        DontDestroyOnLoad(gameObject);     // make it survive scene changes

        audioSrc = GetComponent<AudioSource>();
        musicType = PlayingMusicType.NONE;   

        // .. Get the previously saved music and sound effects states
        isMusicMuted = PlayerSettings.GetMusicState();
        isSoundEffectsMuted = PlayerSettings.GetSoundEffectsState();

        startingVol = audioSrc.volume;
    }

    void Start()
    {
        if (isMusicMuted == 1)
            musicButton.GetComponent<Toggle>().isOn = true;
        else
            musicButton.GetComponent<Toggle>().isOn = false;

        if (isSoundEffectsMuted == 1)
            soundButton.GetComponent<Toggle>().isOn = false;
        else
            soundButton.GetComponent<Toggle>().isOn = true;

        PlayMenuMusic();
    }

    /// <summary>
    /// Toggle sound effects state to On/Off
    /// </summary>
    public void ToggleMuteSoundEffects()
    {
        if (isSoundEffectsMuted == 0)
        {
            isSoundEffectsMuted = 1;
        }
        else
        {
            isSoundEffectsMuted = 0;
        }

        // .. Save the sound effects state
        PlayerSettings.SetSoundEffectsState(isSoundEffectsMuted);
    }

    public void PlaySoundEffect(SoundEffectName effectName, float vol = -1)
    {
        if (isSoundEffectsMuted == 0)   // if the sound effects aren't muted, play it
        {
            for (int i = 0; i < soundEffectAudios.Length; i++)
            {
                if (soundEffectAudios[i].effectName == effectName)
                {
                    // set the audio source volume if specified, if not set the default volume
                    vol = (vol != -1f) ? vol : startingVol;

                    audioSrc.PlayOneShot(soundEffectAudios[i].effectAudio, vol);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Toggle music state to On/Off
    /// </summary>
    public void ToggleMuteMusic()
    {
        if (isMusicMuted == 0)
        {
            musicType = PlayingMusicType.NONE;
            isMusicMuted = 1;
            audioSrc.Stop();
        }
        else
        {
            isMusicMuted = 0;
            PlayMenuMusic();
        }

        // save the new music state
        PlayerSettings.SetMusicState(isMusicMuted);
    }
    
    public void PlayMenuMusic()
    {
        if (musicType != PlayingMusicType.MENU)
        {
            if ((MusicAudios.Length > 0) && (isMusicMuted == 0))
            {
                audioSrc.clip = MusicAudios[(int)PlayingMusicType.MENU];
                audioSrc.loop = true;
                audioSrc.Play();
                musicType = PlayingMusicType.MENU;
            }
        }
    }

    public void PlayMusic(PlayingMusicType type)
    {
        if ((MusicAudios.Length > 0) && (isMusicMuted == 0))
        {
            audioSrc.clip = MusicAudios[(int)type];
            audioSrc.loop = true;
            audioSrc.Play();
            musicType = type;
        }
    }

    /// <summary>
    /// Mute the audio source
    /// </summary>
    public void StopMusic()
    {
        musicType = PlayingMusicType.NONE;
        audioSrc.Stop();
    }
}