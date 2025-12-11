using UnityEngine;

public class PlayerHitHandler : MonoBehaviour
{
    private bool isProcessingHit;

    private void OnCollisionEnter(Collision collision)
    {
        if (isProcessingHit) return;

        if (collision.gameObject.CompareTag("Barrel"))
        {
            isProcessingHit = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayerHitAndRestart();
            }
            else
            {
                Debug.LogWarning("No GameManager.Instance found");
            }
        }
    }
}
