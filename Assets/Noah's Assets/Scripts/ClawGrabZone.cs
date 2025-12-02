// using UnityEngine;
// using System.Collections.Generic;

// public class ClawGrabZone : MonoBehaviour
// {
//     private List<GameObject> objectsInZone = new List<GameObject>();
//     private List<GameObject> grabbedObjects = new List<GameObject>();
//     public Transform clawParent;
//     private bool isGrabbing = false; // ADD THIS
    
//     void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Character"))
//         {
//             if (!objectsInZone.Contains(other.gameObject))
//             {
//                 objectsInZone.Add(other.gameObject);
//                 Debug.Log("Object entered grab zone: " + other.gameObject.name);
//             }
//         }
//     }
    
//     void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Character"))
//         {
//             objectsInZone.Remove(other.gameObject);
//             Debug.Log("Object left grab zone: " + other.gameObject.name);
//         }
//     }
    
//     void OnTriggerStay(Collider other)
//     {
//         // Only grab objects that stay in zone while grabbing is active
//         if (isGrabbing && other.CompareTag("Character"))
//         {
//             if (!grabbedObjects.Contains(other.gameObject))
//             {
//                 GrabSingleObject(other.gameObject);
//             }
//         }
//     }
    
//     public void GrabObjects()
//     {
//         isGrabbing = true; // Enable grabbing
//         Debug.Log("Attempting to grab " + objectsInZone.Count + " objects");
        
//         // Make a copy of the list to avoid modification during iteration
//         List<GameObject> objectsToGrab = new List<GameObject>(objectsInZone);
        
//         foreach (GameObject obj in objectsToGrab)
//         {
//             if (obj != null && !grabbedObjects.Contains(obj))
//             {
//                 GrabSingleObject(obj);
//             }
//         }
//     }
    
// void GrabSingleObject(GameObject obj)
// {
//     Debug.Log("Grabbing: " + obj.name);
    
//     Rigidbody rb = obj.GetComponent<Rigidbody>();
//     if (rb != null)
//     {
//         // Remove ALL constraints while being carried
//         rb.constraints = RigidbodyConstraints.None;
//         rb.isKinematic = true;
//         rb.useGravity = false;
//     }
    
//     // Disable ALL colliders on object and children
//     Collider[] colliders = obj.GetComponentsInChildren<Collider>();
//     foreach (Collider col in colliders)
//     {
//         col.enabled = false;
//     }
    
//     obj.transform.SetParent(clawParent);
//     grabbedObjects.Add(obj);
// }

// public void ReleaseObjects()
// {
//     isGrabbing = false;
//     Debug.Log("Releasing " + grabbedObjects.Count + " objects");
    
//     foreach (GameObject obj in grabbedObjects)
//     {
//         if (obj != null)
//         {
//             obj.transform.SetParent(null);
            
//             Rigidbody rb = obj.GetComponent<Rigidbody>();
//             if (rb != null)
//             {
//                 rb.isKinematic = false;
//                 rb.useGravity = true;
//                 // Restore constraints for wandering
//                 rb.constraints = RigidbodyConstraints.FreezePositionY | 
//                                  RigidbodyConstraints.FreezeRotation;
//             }
            
            
//             // Re-enable ALL colliders
//             Collider[] colliders = obj.GetComponentsInChildren<Collider>();
//             foreach (Collider col in colliders)
//             {
//                 col.enabled = true;
//             }
//         }
//     }
    
//     grabbedObjects.Clear();
//     objectsInZone.Clear();
// }
// }
using UnityEngine;
using System.Collections.Generic;

public class ClawGrabZone : MonoBehaviour
{
    private List<GameObject> objectsInZone = new List<GameObject>();
    private List<GameObject> grabbedObjects = new List<GameObject>();
    private List<GameObject> droppedObjects = new List<GameObject>(); // Track dropped objects
    public Transform clawParent;
    private bool isGrabbing = false;
    public bool grabbed = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            if (!objectsInZone.Contains(other.gameObject))
            {
                objectsInZone.Add(other.gameObject);
                Debug.Log("Object entered grab zone: " + other.gameObject.name);
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            objectsInZone.Remove(other.gameObject);
            Debug.Log("Object left grab zone: " + other.gameObject.name);
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if (isGrabbing && other.CompareTag("Character"))
        {
            if (!grabbedObjects.Contains(other.gameObject))
            {
                GrabSingleObject(other.gameObject);
            }
        }
    }
    
    public void GrabObjects()
    {
        isGrabbing = true;
        Debug.Log("Attempting to grab " + objectsInZone.Count + " objects");
        
        List<GameObject> objectsToGrab = new List<GameObject>(objectsInZone);
        
        foreach (GameObject obj in objectsToGrab)
        {
            if (obj != null && !grabbedObjects.Contains(obj))
            {
                GrabSingleObject(obj);
                grabbed = true;
            }
        }
    }
    
    void GrabSingleObject(GameObject obj)
    {
        Debug.Log("Grabbing: " + obj.name);
        
        // Disable RandomRoamer script
        RandomRoamer roamer = obj.GetComponent<RandomRoamer>();
        if (roamer != null)
        {
            roamer.enabled = false;
        }
        
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
        
        obj.transform.SetParent(clawParent);
        grabbedObjects.Add(obj);
    }

    public void ReleaseObjects()
    {
        isGrabbing = false;
        Debug.Log("Releasing " + grabbedObjects.Count + " objects");
        
        foreach (GameObject obj in grabbedObjects)
        {
            if (obj != null)
            {
                obj.transform.SetParent(null);
                
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    // Allow free falling
                    rb.constraints = RigidbodyConstraints.None;
                }
                
                // Re-enable ALL colliders
                Collider[] colliders = obj.GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders)
                {
                    col.enabled = true;
                }
                
                // Start coroutine to settle object at Y=1 with zero rotation
                StartCoroutine(SettleObject(obj));
                
                // Add to dropped objects list
                droppedObjects.Add(obj);
            }
        }
        
        grabbedObjects.Clear();
        objectsInZone.Clear();
    }
    
    private System.Collections.IEnumerator SettleObject(GameObject obj)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        
        if (rb != null)
        {
            // Wait until object reaches near Y=1 or velocity is very low
            while (obj.transform.position.y > 1.2f || rb.linearVelocity.magnitude > 0.5f)
            {
                yield return new WaitForFixedUpdate();
            }
            
            // Snap to Y=1 and reset rotation
            Vector3 finalPosition = obj.transform.position;
            finalPosition.y = 1f;
            obj.transform.position = finalPosition;
            obj.transform.rotation = Quaternion.Euler(0, 0, 0);
            
            // Stop all physics movement
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            // Freeze object completely until claw returns
            rb.constraints = RigidbodyConstraints.FreezeAll;
            
            Debug.Log(obj.name + " settled at Y=1, waiting for claw to return");
        }
    }
    
    // Call this method when the claw returns to starting position
    public void OnClawReturnedToStart()
    {
        Debug.Log("Claw returned to start, re-enabling movement for " + droppedObjects.Count + " objects");
        
        foreach (GameObject obj in droppedObjects)
        {
            if (obj != null)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Apply wandering constraints
                    rb.constraints = RigidbodyConstraints.FreezePositionY | 
                                     RigidbodyConstraints.FreezeRotation;
                }
                
                // Re-enable RandomRoamer script
                RandomRoamer roamer = obj.GetComponent<RandomRoamer>();
                if (roamer != null)
                {
                    roamer.enabled = true;
                }
                
                Debug.Log(obj.name + " can now roam again");
            }
        }
        
        droppedObjects.Clear();
    }
}