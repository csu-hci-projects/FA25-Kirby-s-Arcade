using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SimpleLevelMusic : MonoBehaviour
{
    [Header("This Level's Song")]
    public AudioClip thisLevelSong;   // Drag your song here in EACH level
    public float volume = 0.7f;
    public bool playOnAwake = true;
    public bool loop = true;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = thisLevelSong;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = playOnAwake;

        // Stop any other music that might be playing from previous level
        StopAllOtherMusic();
    }

    void Start()
    {
        if (thisLevelSong != null)
            audioSource.Play();
    }

    void StopAllOtherMusic()
    {
        // Find all music objects from previous levels and destroy them
        SimpleLevelMusic[] others = FindObjectsOfType<SimpleLevelMusic>();
        foreach (SimpleLevelMusic other in others)
        {
            if (other != this)
            {
                if (other.audioSource != null && other.audioSource.isPlaying)
                    other.audioSource.Stop();
                Destroy(other.gameObject);
            }
        }
    }
}