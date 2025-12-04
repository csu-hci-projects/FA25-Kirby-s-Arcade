using UnityEngine;

public class DestroyOnTouch : MonoBehaviour
{
    [Header("Pellet SFX")]
    public AudioClip collectSound; // Drag your SFX here (pellet collect sound)
    [Range(0f, 1f)]
    public float volume = 0.8f;

    [Header("Pellet Particles (Optional)")]
    public ParticleSystem collectParticles;  // Drag a particle prefab here

    // Shared by all pellets
    private static int redCount = 44;    // <-- SET THIS TO YOUR NUMBER OF RED PELLETS
    private static bool redPhase = true;

    void Start()
    {
        // Reset counts on every level load/restart (player touched -> scene reload)
        redCount = 44;   // Reset to your total red pellets
        redPhase = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Play SFX when collected (both red & blue)
        PlayCollectSound();
        PlayCollectParticles();

        // --- RED PELLETS ---
        if (CompareTag("red") && redPhase)
        {
            redCount--;        // one less red pellet
            Destroy(gameObject);

            if (redCount <= 0)
                redPhase = false;   // switch to blue phase
        }

        // --- BLUE PELLETS ---
        else if (CompareTag("blue") && !redPhase)
        {
            Destroy(gameObject);
        }
    }

    void PlayCollectSound()
    {
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);
        }
    }

    void PlayCollectParticles()
    {
        if (collectParticles != null)
        {
            Instantiate(collectParticles, transform.position, Quaternion.identity);
        }
    }
}