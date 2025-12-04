using UnityEngine;
using UnityEngine.InputSystem;

public class StartPromptController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject startPrompt;
    
    private bool gameStarted = false;
    
    private void Start()
    {
        // Pause the game at start
        Time.timeScale = 0f;
        
        // Make sure the prompt is visible
        if (startPrompt != null)
        {
            startPrompt.SetActive(true);
        }
        
        Debug.Log("Game paused - Press K to start");
    }
    
    private void Update()
    {
        // Check if K is pressed and game hasn't started yet
        if (!gameStarted && Keyboard.current != null && Keyboard.current.kKey.wasPressedThisFrame)
        {
            StartGame();
        }
    }
    
    private void StartGame()
    {
        Debug.Log("K pressed - Starting game!");
        
        gameStarted = true;
        
        // Hide the start prompt
        if (startPrompt != null)
        {
            startPrompt.SetActive(false);
        }
        
        // Unpause the game
        Time.timeScale = 1f;
    }
}
