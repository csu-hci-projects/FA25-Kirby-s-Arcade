using UnityEngine;

public class GoalZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.Instance != null)
        {
            Debug.Log("U at tha end");
            GameManager.Instance.PlayerReachedGoal();
        }
        else
        {
            Debug.LogWarning("GoalZone: No GameManager.Instance found");
        }
    }
}