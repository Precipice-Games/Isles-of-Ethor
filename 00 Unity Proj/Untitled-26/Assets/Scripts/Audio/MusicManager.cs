using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;

    [Header("Mapping Music to Scenes")]
    [SerializeField] private AudioClip mainMenuTrack;
    [SerializeField] private AudioClip motherIslandTrack;
    [SerializeField] private AudioClip oasisIslandTrack;
    [SerializeField] private AudioClip iceIslandTrack;

    [Header("Puzzle Track Per Scene")]
    [SerializeField] private AudioClip motherIslandPuzzleTrack;
    [SerializeField] private AudioClip oasisIslandPuzzleTrack;
    [SerializeField] private AudioClip iceIslandPuzzleTrack;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.75f;
    [SerializeField] private float defaultVolume = 0.6f;

    private Coroutine fadeRoutine;
    private AudioClip currentSceneTrack;
    private AudioClip currentPuzzleTrack;
    private bool isInPuzzleMode = false;

    private void Awake()
    {
        //Ensures only one instance of MusicManager exists and persists across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
    }

    // Subscribe to events
    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        RuneCircle.puzzleTriggered += OnPuzzleTriggered;
        PlayerFixedMovement.updatePuzzleStatus += OnPuzzleCompleted;
    }
    
    // Unsubscribe from events
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
        RuneCircle.puzzleTriggered -= OnPuzzleTriggered;
        PlayerFixedMovement.updatePuzzleStatus -= OnPuzzleCompleted;
    }
    
    private void Start()
    {
        // Apply the appropriate music for the initial scene
        UpdateSceneTrackReferences(SceneManager.GetActiveScene().name);
        PlayCorrectTrackForCurrentState();
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        // Update the music based on the new active scene
        UpdateSceneTrackReferences(newScene.name);
        isInPuzzleMode = false;
        PlayCorrectTrackForCurrentState();
    }
    /// <summary>
    /// Determines which music track to play based on the current
    /// scene name and initiates playback with a fade transition.
    /// Also determines when to fade into the puzzle track for the 
    /// island that the player is currently on, if applicable.
    /// </summary>
    /// <param name="sceneName"></param>
    private void OnPuzzleTriggered(PuzzleInformation info)
    {
        isInPuzzleMode = true;
        PlayCorrectTrackForCurrentState();
    }

    private void OnPuzzleCompleted(PuzzleInformation info)
    {
        EndPuzzleModeMusic();
    }

    public void EndPuzzleModeMusic()
    {
        if (!isInPuzzleMode) return;

        isInPuzzleMode = false;
        PlayCorrectTrackForCurrentState();
    }

    private void UpdateSceneTrackReferences(string sceneName)
    {
        currentSceneTrack = null;
        currentPuzzleTrack = null;

        switch (sceneName)
        {
            case "MainMenu":
                currentSceneTrack = mainMenuTrack;
                break;

            case "Mother_Island":
                currentSceneTrack = motherIslandTrack;
                currentPuzzleTrack = motherIslandPuzzleTrack;
                break;

            case "Oasis_Island":
                currentSceneTrack = oasisIslandTrack;
                currentPuzzleTrack = oasisIslandPuzzleTrack;
                break;

            case "Ice_Island":
                currentSceneTrack = iceIslandTrack;
                currentPuzzleTrack = iceIslandPuzzleTrack;
                break;
        }
    }

    private void PlayCorrectTrackForCurrentState()
    {
        AudioClip trackToPlay = null;

        if (isInPuzzleMode)
        {
            trackToPlay = currentPuzzleTrack != null ? currentPuzzleTrack : currentSceneTrack;
        }
        else
        {
            trackToPlay = currentSceneTrack;
        }

        if (trackToPlay != null)
        {
            Play(trackToPlay, defaultVolume);
        }
        else
        {
            StopMusic();
        }
    }

    /// <summary>
    /// Called by ApplySceneMusic() to handle playback specifics, such
    /// as fading between tracks.
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="volume"></param>
    public void Play(AudioClip clip, float volume)
    {
        // Ensure the clip is not null and that the
        // requested track isn't already playing
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }
        fadeRoutine = StartCoroutine(FadeTo(clip, volume));
    }

    public void PauseMusic() => musicSource.Pause();
    public void ResumeMusic() => musicSource.UnPause();

    /// <summary>
    /// Called by ApplySceneMusic() to stop the current track.
    /// </summary>
    public void StopMusic()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }
        musicSource.Stop();
        musicSource.clip = null;
    }

    private IEnumerator FadeTo(AudioClip newClip, float targetVolume)
    {
        float startVolume = musicSource.volume;

        //Fades out current music
        if (musicSource.isPlaying)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }
        }

        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();

        //Fades in new music
        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, time / fadeDuration);
            yield return null;
        }

        musicSource.volume = targetVolume;
        fadeRoutine = null;
    }
}