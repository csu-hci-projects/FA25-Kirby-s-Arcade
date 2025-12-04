using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // For TextMeshPro (Window > TextMeshPro > Import if needed)

public class ArcadeMachine : MonoBehaviour
{
    [Header("Scene to Load")]
    public string pakMangSceneName = "PakMang"; // Exact scene name (case-sensitive!)

    [Header("Interaction")]
    public float interactionDistance = 3f; // How close player must be

    [Header("UI Prompt")]
    public GameObject promptObject; // Drag your Prompt Canvas/Text GO here (set active=false initially)
    public string promptMessage = "Press E to Play Pak Mang";

    private bool playerInRange = false;

    void Start()
    {
        // Hide prompt initially
        if (promptObject != null) promptObject.SetActive(false);

        // Setup trigger collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            SphereCollider sphereCol = gameObject.AddComponent<SphereCollider>();
            sphereCol.isTrigger = true;
            sphereCol.radius = interactionDistance;
        }
        else
        {
            col.isTrigger = true;
        }
    }

    void Update()
    {
        // Check for E press when in range
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"🎮 Loading {pakMangSceneName}...");
            SceneManager.LoadScene(pakMangSceneName);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            ShowPrompt(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            ShowPrompt(false);
        }
    }

    void ShowPrompt(bool show)
    {
        if (promptObject != null)
        {
            promptObject.SetActive(show);
        }
    }
}