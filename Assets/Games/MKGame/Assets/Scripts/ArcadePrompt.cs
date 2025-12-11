using UnityEngine;
using UnityEngine.InputSystem;       // New Input System
using UnityEngine.SceneManagement;   // For scene loading

public class ArcadePrompt : MonoBehaviour
{
    [Header("Assign the TMP text (e.g., PromptText)")]
    public GameObject promptGO;

    [Header("Scene to load when E is pressed")]
    public string minigameSceneName = "Duel2D";

    // support rigs with multiple colliders
    private int playerOverlapCount = 0;
    private bool Inside => playerOverlapCount > 0;

    void Start()
    {
        if (promptGO) promptGO.SetActive(false);
    }

    void OnEnable()
    {
        if (promptGO) promptGO.SetActive(false);
        playerOverlapCount = 0;
    }

    void OnDisable()
    {
        if (promptGO) promptGO.SetActive(false);
        playerOverlapCount = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOverlapCount++;
            if (promptGO && Inside) promptGO.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOverlapCount = Mathf.Max(0, playerOverlapCount - 1);
            if (promptGO && !Inside) promptGO.SetActive(false);
        }
    }

    void Update()
    {
        if (!Inside) return;

        // New Input System keypress
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // optional: briefly hide prompt to avoid seeing it during load
            if (promptGO) promptGO.SetActive(false);

            // load your minigame scene
            SceneManager.LoadScene(minigameSceneName, LoadSceneMode.Single);
        }
    }
}