using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WinBox : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject winPanel; // The panel with buttons
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button returnToHubButton;
    
    [Header("Settings")]
    [SerializeField] private string winMessage = "Kirby Wins!";
    [SerializeField] private string loseMessage = "You Lose!";
    [SerializeField] private string hubSceneName = "HubScene";
    [SerializeField] private float winDelay = 2f; // Delay before showing win screen
    
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioSource backgroundMusicSource; // Reference to background music
    
    private string currentSceneName;
    private bool hasWon = false;
    
    private void Start()
    {
        // Get current scene name for restart
        currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"WinBox initialized in scene: {currentSceneName}");
        
        // Hide text at start
        if (displayText != null)
        {
            displayText.enabled = false;
        }
        
        // Hide win panel at start
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        
        // Add button listeners
        if (tryAgainButton != null)
        {
            tryAgainButton.onClick.AddListener(TryAgain);
            Debug.Log("Try Again button listener added");
        }
        else
        {
            Debug.LogError("Try Again button is NULL!");
        }
        
        if (returnToHubButton != null)
        {
            returnToHubButton.onClick.AddListener(ReturnToHub);
            Debug.Log("Return to Hub button listener added");
        }
        else
        {
            Debug.LogError("Return to Hub button is NULL!");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is named "kirby" and hasn't won yet
        if (other.gameObject.name == "kirby" && !hasWon)
        {
            Debug.Log("Kirby entered the trigger!");
            hasWon = true;
            StartCoroutine(ShowWinScreenAfterDelay());
        }
    }
    
    private IEnumerator ShowWinScreenAfterDelay()
    {
        Debug.Log("Waiting 2 seconds before showing win screen...");
        
        // Wait for the specified delay
        yield return new WaitForSeconds(winDelay);
        
        ShowWinScreen();
    }
    
    private void ShowWinScreen()
    {
        Debug.Log("Showing win screen now!");
        
        // Unlock cursor (arcade level hides it)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Mute background music
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = 0f;
        }
        
        // Play kirby dance
        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }
        
        // Show the win message
        if (displayText != null)
        {
            displayText.text = winMessage;
            displayText.enabled = true;
        }
        
        // Show the win panel with buttons
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        
        // Make sure buttons are interactable
        if (tryAgainButton != null)
        {
            tryAgainButton.interactable = true;
        }
        if (returnToHubButton != null)
        {
            returnToHubButton.interactable = true;
        }
        
        // Pause the game
        Time.timeScale = 0f;
        
        // Disable claw controls
        ClawController clawController = FindObjectOfType<ClawController>();
        if (clawController != null)
        {
            clawController.enabled = false;
        }
    }
    
    // Public method that ClawController can call when attempts reach 0
    public void ShowLoseScreen()
    {
        Debug.Log("Showing lose screen!");

        // Unlock cursor (arcade level hides it)        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Show the lose message
        if (displayText != null)
        {
            displayText.text = loseMessage;
            displayText.enabled = true;
        }
        
        // Show the panel with buttons
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        
        // Make sure buttons are interactable
        if (tryAgainButton != null)
        {
            tryAgainButton.interactable = true;
        }
        if (returnToHubButton != null)
        {
            returnToHubButton.interactable = true;
        }
        
        // Pause the game
        Time.timeScale = 0f;
        
        // Disable claw controls
        ClawController clawController = FindObjectOfType<ClawController>();
        if (clawController != null)
        {
            clawController.enabled = false;
        }
    }
    
    private void TryAgain()
    {
        Debug.Log("TRY AGAIN BUTTON CLICKED!");
        Debug.Log("Restarting level...");
        
        // Lock cursor back
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Bring back background music volume
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = 1f;
        }
        
        // Unpause game
        Time.timeScale = 1f;
        
        // Reload current scene
        SceneManager.LoadScene(currentSceneName);
    }
    
    private void ReturnToHub()
    {
        Debug.Log("RETURN TO HUB BUTTON CLICKED!");
        Debug.Log("Returning to hub...");
        
        // Unlock cursor for arcade
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Restore background music volume
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = 1f;
        }
        
        // Unpause game
        Time.timeScale = 1f;
        
        // Load hub scene
        SceneManager.LoadScene(hubSceneName);
    }
}

