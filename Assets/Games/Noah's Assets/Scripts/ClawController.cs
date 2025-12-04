using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class ClawController : MonoBehaviour
{
    [Header("References")]
    public ClawGrabber clawGrabber;
    public ClawGrabZone grabZone;
    public MoveClaw moveClaw;
    public Transform chainTop;
    public GameObject[] charactersToDisable;
    
    [Header("Drop Settings")]
    public float dropDistance = 5f;
    public float dropSpeed = 3f;
    public float raiseSpeed = 3f;
    public Vector3 dropOffLocation = new Vector3(-25, 5, -7);
    
    [Header("Timing")]
    public float grabDelay = 0.5f;
    public float holdDelay = 0.3f;
    
    [Header("Attempts System")]
    public TextMeshProUGUI attemptsText;
    public int maxAttempts = 3;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip clawDropSound;
    
    private bool isGrabbing = false;
    private int attemptsRemaining;
    private bool gameOver = false;
    
    // Public property so WinBox can check attempts
    public int AttemptsRemaining => attemptsRemaining;
    
    void Start()
    {
        dropOffLocation = new Vector3(-20, 32, -4);
        Debug.Log("Drop-off set to: " + dropOffLocation);
        
        // Set attempts
        attemptsRemaining = maxAttempts;
        UpdateAttemptsDisplay();
    }
    
    void Update()
    {
        // Only allow space if not already grabbing and game isn't over
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && !isGrabbing && !gameOver)
        {
            StartCoroutine(GrabSequence());
        }
    }
    
    IEnumerator GrabSequence()
    {
        Debug.Log("=== GRAB SEQUENCE START ===");
        isGrabbing = true;
        
        // Decrement attempts when claw goes down
        attemptsRemaining--;
        UpdateAttemptsDisplay();
        
        Vector3 startPosition = chainTop.position;
        Debug.Log("Start Position: " + startPosition);
        
        // 1. Disable player control and characters
        Debug.Log("1. Disabling controls and characters");
        moveClaw.enabled = false;
        DisableCharacters();
        
        // 2. Open claw
        Debug.Log("2. Opening claw");
        clawGrabber.SetClawOpen(true);
        yield return new WaitForSeconds(0.5f);
        
        // 3. Drop down
        Vector3 dropTarget = startPosition + Vector3.down * dropDistance;
        Debug.Log("3. Dropping to: " + dropTarget + " (distance: " + dropDistance + ")");
        
        // Play suck sound effect
        if (audioSource != null && clawDropSound != null)
        {
            audioSource.PlayOneShot(clawDropSound);
        }
        
        yield return StartCoroutine(MoveTo(dropTarget, dropSpeed));
        Debug.Log("3. Reached drop position");
        
        // 4. Wait at bottom
        Debug.Log("4. Waiting at bottom");
        yield return new WaitForSeconds(grabDelay);
        
        // 5. Close claw
        Debug.Log("5. Closing claw");
        clawGrabber.SetClawOpen(false);
        yield return new WaitForSeconds(holdDelay);
        
        // 6 Grab objects
        Debug.Log("6. Grabbing objects in zone");
        grabZone.GrabObjects();

        // 7. Raise back up
        Debug.Log("7. Raising back to start: " + startPosition);
        yield return StartCoroutine(MoveTo(startPosition, raiseSpeed));
        Debug.Log("7. Reached start position");
        
        // 8. Move to drop-off location
        if (grabZone.grabbed) {
            Debug.Log("8. Moving to drop-off: " + dropOffLocation);
            yield return StartCoroutine(MoveTo(dropOffLocation, dropSpeed));
            Debug.Log("8. Reached drop-off");            
        }
        
        // 9. Open claw
        Debug.Log("9. Opening claw to release");
        grabZone.ReleaseObjects();
        clawGrabber.SetClawOpen(true);
        yield return new WaitForSeconds(1f);

        // 10. Close claw again
        Debug.Log("10. Closing claw");
        clawGrabber.SetClawOpen(false);
        yield return new WaitForSeconds(holdDelay);
        
        // 11. Return to start
        Debug.Log("11. Returning to start: " + startPosition);
        yield return StartCoroutine(MoveTo(startPosition, raiseSpeed));
        Debug.Log("11. Back at start");
        
        // 12. Re-enable player control and characters (unless game over)
        if (attemptsRemaining > 0)
        {
            Debug.Log("12. Re-enabling controls and characters");
            moveClaw.enabled = true;
            EnableCharacters();
        }
        else
        {
            Debug.Log("12. Game Over - no attempts remaining");
            gameOver = true;
            
            // Trigger the lose screen
            WinBox winBox = FindObjectOfType<WinBox>();
            if (winBox != null)
            {
                winBox.ShowLoseScreen();
            }
        }
        
        isGrabbing = false;
        Debug.Log("Grab Cycle Over");
    }
    
    void UpdateAttemptsDisplay()
    {
        if (attemptsText != null)
        {
            attemptsText.text = "Number of Attempts: " + attemptsRemaining;
        }
    }
    
    IEnumerator MoveTo(Vector3 targetPosition, float speed)
    {
        while (Vector3.Distance(chainTop.position, targetPosition) > 0.1f)
        {
            chainTop.position = Vector3.MoveTowards(
                chainTop.position, 
                targetPosition, 
                speed * Time.deltaTime
            );
            yield return null;
        }
        chainTop.position = targetPosition;
    }
    
    void DisableCharacters()
    {
        foreach (GameObject character in charactersToDisable)
        {
            if (character != null)
            {
                Animator anim = character.GetComponent<Animator>();
                if (anim != null) anim.enabled = false;
                
                MonoBehaviour[] scripts = character.GetComponents<MonoBehaviour>();
                foreach (var script in scripts)
                {
                    if (script.GetType().Name.Contains("Move") || 
                        script.GetType().Name.Contains("Roamer"))
                    {
                        script.enabled = false;
                    }
                }
                
                Rigidbody rb = character.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = true;
                    rb.constraints = RigidbodyConstraints.None;
                }
            }
        }
    }

    void EnableCharacters()
    {
        foreach (GameObject character in charactersToDisable)
        {
            if (character != null)
            {
                Animator anim = character.GetComponent<Animator>();
                if (anim != null) anim.enabled = true;
                
                MonoBehaviour[] scripts = character.GetComponents<MonoBehaviour>();
                foreach (var script in scripts)
                {
                    script.enabled = true;
                }
                
                // Reset rotation to true zero rotation
                character.transform.rotation = Quaternion.identity;
                
                // Reset position height to 1.65 while keeping current X and Z
                Vector3 currentPosition = character.transform.position;
                character.transform.position = new Vector3(currentPosition.x, 1.65f, currentPosition.z);
                
                Rigidbody rb = character.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                }
            }
        }
    }
}