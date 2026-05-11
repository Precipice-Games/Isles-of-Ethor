using UnityEngine;
using UnityEngine.Audio;

///<summary>
///This script manages the sound effects for our game.
///It will handle playing the appropriate SFX for player
///interactions like card clicking and tile moving,
///SFX for impossible tile movement and other feedback sounds,
///as well as eventually handling looping ambiance
///</summary>
public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance {get; private set;}    

    public enum FootstepSFX
    {
        Mother,
        Ice,
        Oasis,
        Flower,
        Airship
    }

    //Unity inspector field for the audio source that will play the SFX
    [Header("Audio Source")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource footstepSource;

    //Unity inspector field for the audio mixer group to route the SFX through
    [Header("Mixer Routing")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    //Unity inspector fields for the common SFX used in the game (will be expanded as needed)
    [Header("SFX")]
    [SerializeField] private AudioClip cardClickSFX;
    [SerializeField] private AudioClip invalidMoveSFX;
    [SerializeField] private AudioClip puzzleResetSFX;
    [SerializeField] private AudioClip runeCircleSFX;
    [SerializeField] private AudioClip menuSFX;
    [SerializeField] private AudioClip exitButtonSFX;

    //Inspector Fields for the different footstep SFX for each island and airship
    [Header("Footstep SFX")]
    [SerializeField] private AudioClip motherFootstepSFX;
    [SerializeField] private AudioClip iceFootstepSFX;
    [SerializeField] private AudioClip oasisFootstepSFX;
    [SerializeField] private AudioClip flowerFootstepSFX;
    [SerializeField] private AudioClip airshipFootstepSFX;

    private bool suppressNextCardClick = false;

    //This will be used to determine which footstep SFX to play based on the current island the player is on
    private FootstepSFX currentFootstepSFX = FootstepSFX.Mother;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (sfxSource != null && sfxMixerGroup != null)
        {
            sfxSource.outputAudioMixerGroup = sfxMixerGroup;
        }

        if (footstepSource != null && sfxMixerGroup != null)
        {
            footstepSource.outputAudioMixerGroup = sfxMixerGroup;
        }
    }
    
    private void OnEnable()
    {
        ResetPuzzle.resetPuzzle += OnPuzzleReset;
    }

    private void OnDisable()
    {
        ResetPuzzle.resetPuzzle -= OnPuzzleReset;
    }
    public void PlayCardClick()
    {
        if (suppressNextCardClick)
        {
            suppressNextCardClick = false;
            return;
        }

        PlayClip(cardClickSFX);
    }

    public void PlayInvalidMove()
    {
        suppressNextCardClick = true;
        PlayClip(invalidMoveSFX);
    }

    public void PlayRuneCircle()
    {
        PlayClip(runeCircleSFX);
    }

    public void PlayMenu()
    {
        PlayClip(menuSFX);
    }
    
    public void PlayExitButton()
    {
        PlayClip(exitButtonSFX);
    }
    private void OnPuzzleReset()
    {
        PlayClip(puzzleResetSFX);
    }

    public void SetFootstepSFX(FootstepSFX newSFX)
    {
        Debug.Log("Footstep area changed to: " + newSFX);
        currentFootstepSFX = newSFX;
    }

    public void PlayFootstep()
    {
        AudioClip clipToPlay = null;

        if (currentFootstepSFX == FootstepSFX.Mother)
        {
            clipToPlay = motherFootstepSFX;
        }
        else if (currentFootstepSFX == FootstepSFX.Ice)
        {
            clipToPlay = iceFootstepSFX;
        }
        else if (currentFootstepSFX == FootstepSFX.Oasis)
        {
            clipToPlay = oasisFootstepSFX;
        }
        else if (currentFootstepSFX == FootstepSFX.Flower)
        {
            clipToPlay = flowerFootstepSFX;
        }
        else if (currentFootstepSFX == FootstepSFX.Airship)
        {
            clipToPlay = airshipFootstepSFX;
        }

        if (footstepSource == null || clipToPlay == null)
        {
            return;
        }

        if (footstepSource.isPlaying)
        {
            return;
        }

        footstepSource.clip = clipToPlay;
        footstepSource.Play();
    }

    public void PlayClip(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }
}
