using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class ButtonScript : MonoBehaviour
{
    [Header("Game's Audio Settings")]
    public AudioSource audioSource;
    public AudioClip clickSound;
    public bool isButtonSelected = false;
    private Selectable cacheSelectable;

    void Awake()
    {
        cacheSelectable = GetComponent<Selectable>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayClickSound()
    {
        if (clickSound != null)
        {
            if (audioSource != null) audioSource.PlayOneShot(clickSound);
            else if (AudioManagerScript.Instance != null) AudioManagerScript.Instance.PlaySFX(clickSound);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
