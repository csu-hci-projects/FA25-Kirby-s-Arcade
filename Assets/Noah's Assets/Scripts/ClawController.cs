// using UnityEngine;
// using UnityEngine.InputSystem;
// using System.Collections;

// public class ClawController : MonoBehaviour
// {
//     [Header("References")]
//     public ClawGrabber clawGrabber;
//     public ClawGrabZone grabZone; // ADD THIS
//     public MoveClaw moveClaw;
//     public Transform chainTop;
//     public GameObject[] charactersToDisable;
    
//     [Header("Drop Settings")]
//     public float dropDistance = 5f;
//     public float dropSpeed = 3f;
//     public float raiseSpeed = 3f;
//     public Vector3 dropOffLocation = new Vector3(-25, 5, -7);
    
//     [Header("Timing")]
//     public float grabDelay = 0.5f;
//     public float holdDelay = 0.3f;
    
//     private bool isGrabbing = false;
    
//     void Start()
//     {
//         dropOffLocation = new Vector3(-20, 32, -4);
//         Debug.Log("Drop-off set to: " + dropOffLocation);
//     }
    
//     void Update()
//     {
//         if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && !isGrabbing)
//         {
//             StartCoroutine(GrabSequence());
//         }
//     }
    
//     IEnumerator GrabSequence()
//     {
//         Debug.Log("=== GRAB SEQUENCE START ===");
//         isGrabbing = true;
        
//         Vector3 startPosition = chainTop.position;
//         Debug.Log("Start Position: " + startPosition);
        
//         // 1. Disable player control and characters
//         Debug.Log("1. Disabling controls and characters");
//         moveClaw.enabled = false;
//         DisableCharacters();
        
//         // 2. Open claw
//         Debug.Log("2. Opening claw");
//         clawGrabber.SetClawOpen(true);
//         yield return new WaitForSeconds(0.5f);
        
//         // 3. Drop down
//         Vector3 dropTarget = startPosition + Vector3.down * dropDistance;
//         Debug.Log("3. Dropping to: " + dropTarget + " (distance: " + dropDistance + ")");
//         yield return StartCoroutine(MoveTo(dropTarget, dropSpeed));
//         Debug.Log("3. Reached drop position");
        
//         // 4. Wait at bottom
//         Debug.Log("4. Waiting at bottom");
//         yield return new WaitForSeconds(grabDelay);
        
//         // 5. Close claw (grab)
//         Debug.Log("5. Closing claw");
//         clawGrabber.SetClawOpen(false);
//         yield return new WaitForSeconds(holdDelay);
        
//         // 5b. ACTUALLY GRAB OBJECTS - ADD THIS
//         Debug.Log("5b. Grabbing objects in zone");
//         grabZone.GrabObjects();
        
//         // 6. Raise back up
//         Debug.Log("6. Raising back to start: " + startPosition);
//         yield return StartCoroutine(MoveTo(startPosition, raiseSpeed));
//         Debug.Log("6. Reached start position");
        
//         // 7. Move to drop-off location
//         Debug.Log("7. Moving to drop-off: " + dropOffLocation);
//         yield return StartCoroutine(MoveTo(dropOffLocation, dropSpeed));
//         Debug.Log("7. Reached drop-off");
        
//         // 8. Open claw (release)
//         Debug.Log("8. Opening claw to release");
//         grabZone.ReleaseObjects(); // ADD THIS - Release before opening
//         clawGrabber.SetClawOpen(true);
//         yield return new WaitForSeconds(0.5f);
        
//         // 9. Return to start
//         Debug.Log("9. Returning to start: " + startPosition);
//         yield return StartCoroutine(MoveTo(startPosition, raiseSpeed));
//         Debug.Log("9. Back at start");
        
//         // 10. Re-enable player control and characters
//         Debug.Log("10. Re-enabling controls and characters");
//         moveClaw.enabled = true;
//         EnableCharacters();
        
//         isGrabbing = false;
//         Debug.Log("=== GRAB SEQUENCE COMPLETE ===");
//     }
    
//     IEnumerator MoveTo(Vector3 targetPosition, float speed)
//     {
//         while (Vector3.Distance(chainTop.position, targetPosition) > 0.1f)
//         {
//             chainTop.position = Vector3.MoveTowards(
//                 chainTop.position, 
//                 targetPosition, 
//                 speed * Time.deltaTime
//             );
//             yield return null;
//         }
//         chainTop.position = targetPosition;
//     }
    
//     void DisableCharacters()
//     {
//         foreach (GameObject character in charactersToDisable)
//         {
//             if (character != null)
//             {
//                 Animator anim = character.GetComponent<Animator>();
//                 if (anim != null) anim.enabled = false;
                
//                 MonoBehaviour[] scripts = character.GetComponents<MonoBehaviour>();
//                 foreach (var script in scripts)
//                 {
//                     if (script.GetType().Name.Contains("Move") || 
//                         script.GetType().Name.Contains("Roamer"))
//                     {
//                         script.enabled = false;
//                     }
//                 }
                
//                 Rigidbody rb = character.GetComponent<Rigidbody>();
//                 if (rb != null)
//                 {
//                     rb.useGravity = true;
//                     rb.constraints = RigidbodyConstraints.None;
//                 }
//             }
//         }
//     }
    
//     void EnableCharacters()
//     {
//         foreach (GameObject character in charactersToDisable)
//         {
//             if (character != null)
//             {
//                 Animator anim = character.GetComponent<Animator>();
//                 if (anim != null) anim.enabled = true;
                
//                 MonoBehaviour[] scripts = character.GetComponents<MonoBehaviour>();
//                 foreach (var script in scripts)
//                 {
//                     script.enabled = true;
//                 }
                
//                 Rigidbody rb = character.GetComponent<Rigidbody>();
//                 if (rb != null)
//                 {
//                     rb.useGravity = false;
//                     rb.constraints = RigidbodyConstraints.FreezePositionY;
//                 }
//             }
//         }
//     }
// }

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class ClawController : MonoBehaviour
{
    [Header("References")]
    public ClawGrabber clawGrabber;
    public ClawGrabZone grabZone; // ADD THIS
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
    
    private bool isGrabbing = false;
    private int attemptsRemaining;
    
    void Start()
    {
        dropOffLocation = new Vector3(-20, 32, -4);
        Debug.Log("Drop-off set to: " + dropOffLocation);
        
        // Initialize attempts
        attemptsRemaining = maxAttempts;
        UpdateAttemptsDisplay();
    }
    
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && !isGrabbing)
        {
            StartCoroutine(GrabSequence());
        }
    }
    
    IEnumerator GrabSequence()
    {
        bool grab = false;
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
        yield return StartCoroutine(MoveTo(dropTarget, dropSpeed));
        Debug.Log("3. Reached drop position");
        
        // 4. Wait at bottom
        Debug.Log("4. Waiting at bottom");
        yield return new WaitForSeconds(grabDelay);
        
        // 5. Close claw (grab)
        Debug.Log("5. Closing claw");
        clawGrabber.SetClawOpen(false);
        yield return new WaitForSeconds(holdDelay);
        
        // 5b. ACTUALLY GRAB OBJECTS - ADD THIS
        Debug.Log("5b. Grabbing objects in zone");
        grabZone.GrabObjects();

        
        // 6. Raise back up
        Debug.Log("6. Raising back to start: " + startPosition);
        yield return StartCoroutine(MoveTo(startPosition, raiseSpeed));
        Debug.Log("6. Reached start position");
        
        // 7. Move to drop-off location
        if (grabZone.grabbed) {
        Debug.Log("7. Moving to drop-off: " + dropOffLocation);
        yield return StartCoroutine(MoveTo(dropOffLocation, dropSpeed));
        Debug.Log("7. Reached drop-off");            
        }
        
        // 8. Open claw (release)
        Debug.Log("8. Opening claw to release");
        grabZone.ReleaseObjects(); // ADD THIS - Release before opening
        clawGrabber.SetClawOpen(true);
        yield return new WaitForSeconds(1f);

        // 5. Close claw (grab) AGAIN
        Debug.Log("5. Closing claw");
        clawGrabber.SetClawOpen(false);
        yield return new WaitForSeconds(holdDelay);
        
        // 9. Return to start
        Debug.Log("9. Returning to start: " + startPosition);
        yield return StartCoroutine(MoveTo(startPosition, raiseSpeed));
        Debug.Log("9. Back at start");
        
        // 10. Re-enable player control and characters
        Debug.Log("10. Re-enabling controls and characters");
        moveClaw.enabled = true;
        EnableCharacters();
        
        isGrabbing = false;
        Debug.Log("=== GRAB SEQUENCE COMPLETE ===");
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
    
    // void EnableCharacters()
    // {
    //     foreach (GameObject character in charactersToDisable)
    //     {
    //         if (character != null)
    //         {
    //             Animator anim = character.GetComponent<Animator>();
    //             if (anim != null) anim.enabled = true;
                
    //             MonoBehaviour[] scripts = character.GetComponents<MonoBehaviour>();
    //             foreach (var script in scripts)
    //             {
    //                 script.enabled = true;
    //             }
    //             character.transform.rotation(0, 0, 0);
    //             character.transform.position(currentx, 1.65, currenty);
    //             Rigidbody rb = character.GetComponent<Rigidbody>();
    //             if (rb != null)
    //             {
    //                 rb.useGravity = false;
    //                 rb.constraints = RigidbodyConstraints.FreezePositionY;
    //             }
    //         }
    //     }
    // }
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
            
            // Reset rotation to IDENTITY (true zero rotation)
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