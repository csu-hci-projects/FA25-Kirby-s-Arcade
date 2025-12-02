// using UnityEngine;
// using TMPro; // If using TextMeshPro
// // using UnityEngine.UI; // If using legacy Text

// public class WinBox : MonoBehaviour
// {
//     [SerializeField] private TextMeshProUGUI displayText; // Or Text if using legacy UI
//     [SerializeField] private string messageToDisplay = "Kirby entered!";
    
//     private void Start()
//     {
//         // Make sure the text is hidden at start
//         if (displayText != null)
//         {
//             displayText.enabled = false;
//         }
//     }
    
//     private void OnTriggerEnter(Collider other)
//     {
//         // Check if the entering object is named "kirby"
//         if (other.gameObject.name == "kirby")
//         {
//             Debug.Log("Kirby entered the trigger!"); // Also log to console
            
//             if (displayText != null)
//             {
//                 displayText.text = messageToDisplay;
//                 displayText.enabled = true;
//             }
//         }
//     }
    
//     private void OnTriggerExit(Collider other)
//     {
//         // Optional: Hide the text when Kirby leaves
//         if (other.gameObject.name == "kirby")
//         {
//             if (displayText != null)
//             {
//                 displayText.enabled = false;
//             }
//         }
//     }
// }
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections; // Needed for IEnumerator

public class WinBox : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private GameObject winPanel; // The panel with buttons
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button returnToHubButton;
    
    [Header("Settings")]
    [SerializeField] private string messageToDisplay = "Kirby Wins!";
    [SerializeField] private string hubSceneName = "ArcadeLevel";
    [SerializeField] private float winDelay = 2f; // Delay before showing win screen
    
    private string currentSceneName;
    private bool hasWon = false;
    
    private void Start()
    {
        // Get current scene name for restart
        currentSceneName = SceneManager.GetActiveScene().name;
        
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
        }
        
        if (returnToHubButton != null)
        {
            returnToHubButton.onClick.AddListener(ReturnToHub);
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
        
        // Show the message text
        if (displayText != null)
        {
            displayText.text = messageToDisplay;
            displayText.enabled = true;
        }
        
        // Show the win panel with buttons
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        
        // Optional: Pause the game
        Time.timeScale = 0f;
        
        // Optional: Disable claw controls
        ClawController clawController = FindObjectOfType<ClawController>();
        if (clawController != null)
        {
            clawController.enabled = false;
        }
    }
    
    private void TryAgain()
    {
        Debug.Log("Restarting level...");
        
        // Unpause game
        Time.timeScale = 1f;
        
        // Reload current scene
        SceneManager.LoadScene(currentSceneName);
    }
    
    private void ReturnToHub()
    {
        Debug.Log("Returning to hub...");
        
        // Unpause game
        Time.timeScale = 1f;
        
        // Load hub scene
        SceneManager.LoadScene(hubSceneName);
    }
    
    private void OnTriggerExit(Collider other)
    {
        // Optional: You might not need this anymore since win is permanent
        // But keeping it here in case you want different behavior
    }
}