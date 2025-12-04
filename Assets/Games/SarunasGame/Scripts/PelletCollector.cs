using UnityEngine;

public class DestroyOnTouch2 : MonoBehaviour
{
    [Header("Collect Feedback")]
    public AudioClip collectSound;           // Drag your pellet/coin SFX here
    [Range(0f, 1f)]
    public float volume = 0.8f;

    public ParticleSystem collectParticles;  // Optional: drag a particle prefab here

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Play sound at the pellet's position
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);
            }

            // Spawn particles (if assigned)
            if (collectParticles != null)
            {
                Instantiate(collectParticles, transform.position, Quaternion.identity);
            }

            // Remove the pellet
            Destroy(gameObject);
        }
    }
}