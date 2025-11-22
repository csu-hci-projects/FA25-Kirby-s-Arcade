using UnityEngine;

public class DestroyOnTouch : MonoBehaviour
{
    // Shared by all pellets
    private static int redCount = 44;    // <-- SET THIS TO YOUR NUMBER OF RED PELLETS
    private static bool redPhase = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

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
}
