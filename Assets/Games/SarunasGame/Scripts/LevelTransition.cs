using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Optional UI

public class LevelTransition : MonoBehaviour
{
    [Header("Next Level (By Name)")]
    public string nextSceneName = ""; // e.g., "Level2" — leave empty for buildIndex +1 fallback

    [Header("Win/Loop (If No Next)")]
    public string finalSceneName = "0"; // e.g., "WinScreen" or "Level1" to loop

    [Header("UI Counter (Optional)")]
    public TextMeshProUGUI pelletCounterText;

    private bool levelComplete = false;
    private int lastRemainingCount = -1;

    void Update()
    {
        if (levelComplete) return;

        // Check remaining pellets
        GameObject[] remainingPellets = GameObject.FindGameObjectsWithTag("Pellet");
        int remaining = remainingPellets.Length;

        // Update UI
        if (pelletCounterText != null && remaining != lastRemainingCount)
        {
            lastRemainingCount = remaining;
            pelletCounterText.text = $"{remaining}";
        }

        // Transition!
        if (remaining == 0)
        {
            levelComplete = true;
            Debug.Log("🎉 All pellets collected! Loading next level...");

            string sceneToLoad;
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                sceneToLoad = nextSceneName; // Direct by NAME (flexible!)
            }
            else
            {
                // Fallback: Next by buildIndex
                int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
                if (nextIndex < SceneManager.sceneCountInBuildSettings)
                {
                    sceneToLoad = SceneManager.GetSceneByBuildIndex(nextIndex).name;
                }
                else
                {
                    sceneToLoad = finalSceneName; // Custom win/loop
                }
            }

            SceneManager.LoadScene(sceneToLoad);
        }
    }
}