using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class ArcadeInteract : MonoBehaviour
{
    [Header("Assign from Scene")]
    public GameObject interactText;         // TMP UI object: "Press E to Play"
    public string minigameSceneName = "Duel2D";

    private bool playerInside = false;

    void Start()
    {
        if (interactText) interactText.SetActive(false); // hidden by default
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (interactText) interactText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (interactText) interactText.SetActive(false);
        }
    }

    void Update()
    {
        if (!playerInside) return;

        // New Input System: listen for E
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // If Duel2D isn’t created yet, this will fail — we’ll hook it up after.
            // For now you can leave it or comment it until Part C is done.
            SceneManager.LoadScene(minigameSceneName, LoadSceneMode.Single);
        }
    }
}