using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "DonkeyKong";    // DK level
    [SerializeField] private string arcadeSceneName;  // Arcade hub

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void ReturnToArcade()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(arcadeSceneName);
    }
}
