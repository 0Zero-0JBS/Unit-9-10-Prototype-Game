using UnityEngine;
using UnityEngine.Audio;

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

    private static float savedMainMenuMusicTime = 0f;
    private static float savedBattleMusicTime = 0f;
    private float thrustVolumeModifier = 1.0f;

    [Header("Game Sounds")]
    public AudioClip scorePointsSound;
    public AudioClip tetrominoesLandingSound;
    public AudioClip lineBreakingSound;
    
    [Header("UI Sounds")]
    public AudioClip buttonClick;
    
    [Header("Global Settings")]
    [Range(0f, 1f)] public float globalSfxVolume = 0.8f;
    [Range(0f, 1f)] public float globalMusicVolume = 0.5f;
    public bool dynamicMusicSpeedScaling = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
