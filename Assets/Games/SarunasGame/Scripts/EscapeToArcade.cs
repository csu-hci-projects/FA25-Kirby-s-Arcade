using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeToArcade : MonoBehaviour
{
    [Header("Return Scene")]
    public string arcadeLevelSceneName = "ArcadeLevel"; // Exact scene name (case-sensitive!)

    void Update()
    {
        // ESC pressed → Load ArcadeLevel
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC pressed! Returning to ArcadeLevel...");
            SceneManager.LoadScene(arcadeLevelSceneName);
        }
    }
}