using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Configurations")]
    public AudioMixer audioMixer;
    public AudioMixerGroup musicMixerGroup;

    [Header("Audio Sources")]
    public AudioSource musicSource;

    [Header("SFX Pooling System")]
    [SerializeField] private int sfxPoolSize = 20;
    private AudioSource[] sfxPool;

    [Header("Background Music")]
    public AudioClip mainMenuMusic;
    public AudioClip gameMusic;

    [Header("Dynamic Music Scaling")]
    public bool dynamicMusicSpeedScaling = false;
    [SerializeField] private float minMusicPitch = 1.0f;
    [SerializeField] private float maxMusicPitch = 1.25f;
    [SerializeField] private float pitchTransitionSpeed = 2.0f;
    private float targetMusicPitch = 1.0f;

    private static float savedMainMenuMusicTime = 0f;
    private static float savedGameMusicTime = 0f;
    private Coroutine musicFadeCoroutine;

    [Header("Game Sounds")]
    public AudioClip scorePointsSound;
    public AudioClip tetrominoesLandingSound;
    public AudioClip lineBreakingSound;
    public AudioClip gameOverSound;
    
    [Header("UI Sounds")]
    public AudioClip buttonPress;
    
    [Header("Global Settings")]
    [Range(0f, 1f)] public float globalSfxVolume = 0.8f;
    [Range(0f, 1f)] public float globalMusicVolume = 0.5f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
            InitializePool();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(transform.root.gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SaveCurrentTrackTime(SceneManager.GetActiveScene().name);
        }
    }

    private void SaveCurrentTrackTime(string currentSceneName)
    {
        if (musicSource == null || !musicSource.isPlaying) return;

        if ((currentSceneName == "MainMenu") && musicSource.clip == mainMenuMusic)
        {
            savedMainMenuMusicTime = musicSource.time;
        }
        else if (currentSceneName == "Game" && musicSource.clip == gameMusic)
        {
            savedGameMusicTime = musicSource.time;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string activeScene = scene.name;

        if (musicSource == null) return;

        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
            musicFadeCoroutine = null;
        }

        musicSource.loop = true;
        musicSource.playOnAwake = false;

        if (activeScene == "Game")
        {
            if (musicSource.clip == mainMenuMusic && musicSource.isPlaying)
            {
                savedMainMenuMusicTime = musicSource.time;
            }

            musicSource.Stop();
            musicSource.clip = gameMusic;
            musicSource.volume = globalMusicVolume;

            if (gameMusic != null)
            {
                if (savedGameMusicTime > 0f && savedGameMusicTime < gameMusic.length)
                {
                    musicSource.time = savedGameMusicTime;
                }
                musicSource.Play();
            }
        }
        else if (activeScene == "MainMenu")
        {
            if (musicSource.clip == mainMenuMusic && musicSource.isPlaying)
            {
                return;
            }

            if (musicSource.clip == gameMusic && musicSource.isPlaying)
            {
                savedGameMusicTime = musicSource.time;
            }

            musicSource.Stop();
            musicSource.clip = mainMenuMusic;
            musicSource.volume = globalMusicVolume;

            if (mainMenuMusic != null)
            {
                if (savedMainMenuMusicTime > 0f && savedMainMenuMusicTime < mainMenuMusic.length)
                {
                    musicSource.time = savedMainMenuMusicTime;
                }
                musicSource.Play();
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (musicSource != null)
        {
            if (musicFadeCoroutine == null)
            {
                musicSource.volume = globalMusicVolume;
            }

            if (dynamicMusicSpeedScaling && musicSource.isPlaying)
            {
                musicSource.pitch = Mathf.MoveTowards(musicSource.pitch, targetMusicPitch, pitchTransitionSpeed * Time.deltaTime);
            }
            else
            {
                musicSource.pitch = 1.0f;
            }
        }
    }

    public void SetMusicIntensity(float normalizedIntensity)
    {
        targetMusicPitch = Mathf.Lerp(minMusicPitch, maxMusicPitch, Mathf.Clamp01(normalizedIntensity));
    }

    public void ResetAllTimelineMemory()
    {
        savedGameMusicTime = 0f;
        savedMainMenuMusicTime = 0f;
    }

    private void InitializePool()
    {
        UnityEngine.Audio.AudioMixerGroup[] sfxGroups = audioMixer != null ? audioMixer.FindMatchingGroups("SFXVol") : null;
        UnityEngine.Audio.AudioMixerGroup targetGroup = (sfxGroups != null && sfxGroups.Length > 0) ? sfxGroups[0] : null;

        if (audioMixer != null && musicMixerGroup != null)
        {
            musicSource.outputAudioMixerGroup = musicMixerGroup;
        }
        else if (audioMixer != null)
        {
            AudioMixerGroup[] musicGroups = audioMixer.FindMatchingGroups("MusicVol");
            if (musicGroups.Length > 0) musicSource.outputAudioMixerGroup = musicGroups[0];
        }

        sfxPool = new AudioSource[sfxPoolSize];
        for (int i = 0; i < sfxPoolSize; i++)
        {
            GameObject poolObj = new GameObject($"SFX_Pool_Source_{i}");
            poolObj.transform.SetParent(transform);

            AudioSource source = poolObj.AddComponent<AudioSource>();
            source.ignoreListenerPause = true;
            if (targetGroup != null) source.outputAudioMixerGroup = targetGroup;

            source.volume = globalSfxVolume;
            sfxPool[i] = source;
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        AudioSource oldestSource = sfxPool[0];
        float maxTime = 0f;

        for (int i = 0; i < sfxPool.Length; i++)
        {
            if (sfxPool[i] == null) continue;

            if (!sfxPool[i].isPlaying) return sfxPool[i];

            if (sfxPool[i].time > maxTime)
            {
                maxTime = sfxPool[i].time;
                oldestSource = sfxPool[i];
            }
        }

        oldestSource.Stop();
        return oldestSource;
    }

    public void TransitionToMusic(AudioClip newTrack, float fadeDuration = 1.0f)
    {
        if (musicSource == null) return;
        if (musicSource.clip == newTrack && musicSource.isPlaying) return;

        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        musicFadeCoroutine = StartCoroutine(CrossfadeMusicRoutine(newTrack, fadeDuration));
    }

    private IEnumerator CrossfadeMusicRoutine(AudioClip newClip, float duration)
    {
        float startVolume = musicSource.volume;

        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }
        musicSource.volume = 0f;

        musicSource.clip = newClip;
        if (newClip != null)
        {
            musicSource.Play();

            for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(0f, globalMusicVolume, t / duration);
                yield return null;
            }
            musicSource.volume = globalMusicVolume;
        }
        else
        {
            musicSource.Stop();
        }

        musicFadeCoroutine = null;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableAudioSource();
        if (source != null)
        {
            source.pitch = 1.0f;
            source.clip = clip;
            source.volume = globalSfxVolume;
            source.Play();
        }
    }

    public void PlaySFXWithPitch(AudioClip clip, float minPitch = 0.85f, float maxPitch = 1.15f)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableAudioSource();
        if (source != null)
        {

            source.clip = clip;
            source.volume = globalSfxVolume;
            source.pitch = Random.Range(minPitch, maxPitch);
            source.Play();
        }
    }

    public void PlayTetrominoesLandingSound() => PlaySFXWithPitch(tetrominoesLandingSound, 0.8f, 1.2f);
    public void PlayLineBreakingSound() => PlaySFXWithPitch(lineBreakingSound, 0.95f, 1.05f);
    public void PlayScorePointsSound() => PlaySFXWithPitch(scorePointsSound, 0.95f, 1.05f);
    public void PlayGameOverSound() => PlaySFXWithPitch(gameOverSound, 1f, 1.4f);
}
